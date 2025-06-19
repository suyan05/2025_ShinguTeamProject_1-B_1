using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public static BubbleGrid Instance { get; private set; }

    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public Color gridColor = Color.green;
    public bool isGameOver = false;

    [Header("Grid Spacing")]
    public float horizontalSpacing = 1.0f;
    public float verticalSpacing = 1.2f;

    private Bubble[,] grid;
    private GameManager gameManager;
    private List<Vector2Int> pendingMergeRemovals = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        InitializeGrid();
    }

    private void FixedUpdate()
    {
        RepositionDisconnectedBubbles();
    }

    private void InitializeGrid()
    {
        grid = new Bubble[rows, cols];
        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols;
            for (int x = 0; x < actualCols; x++) grid[y, x] = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;
        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols;
            for (int x = 0; x < actualCols; x++)
            {
                Vector2 pos = GetGridPosition(x, y);
                Gizmos.DrawWireSphere(pos, bubbleSize * 0.4f);
            }
        }
    }

    public Vector2 GetGridPosition(int x, int y)
    {
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);
        float offsetX = (y % 2 == 0) ? 0f : horizontalSpacing / 2f;
        return new Vector2(origin.x + x * horizontalSpacing + offsetX, origin.y - y * verticalSpacing);
    }

    private Vector2Int WorldToCell(Vector2 worldPos)
    {
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);

        int y = Mathf.Clamp(Mathf.RoundToInt((origin.y - worldPos.y) / verticalSpacing), 0, rows - 1);
        int actualCols = (y % 2 == 1) ? cols - 1 : cols;
        float offsetX = (y % 2 == 0) ? 0f : horizontalSpacing / 2f;
        int x = Mathf.Clamp(Mathf.RoundToInt((worldPos.x - origin.x - offsetX) / horizontalSpacing), 0, actualCols - 1);

        return new Vector2Int(x, y);
    }

    public Vector2 FindNearestEmptyGrid(Vector2 pos)
    {
        Vector2 best = Vector2.zero;
        float minDist = float.MaxValue;
        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols;
            for (int x = 0; x < actualCols; x++)
            {
                if (grid[y, x] == null)
                {
                    Vector2 world = GetGridPosition(x, y);
                    float dist = Vector2.Distance(pos, world);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        best = world;
                    }
                }
            }
        }
        return best;
    }

    public void PlaceBubble(Bubble b)
    {
        if (isGameOver) return;

        Vector2Int cell = WorldToCell(b.transform.position);
        int actualCols = (cell.y % 2 == 1) ? cols - 1 : cols;
        if (cell.x >= actualCols) cell.x = actualCols - 1;

        if (grid[cell.y, cell.x] != null)
            cell = FindLowestAvailableCell(cell.x, cell.y);

        b.transform.position = GetGridPosition(cell.x, cell.y);
        grid[cell.y, cell.x] = b;

        FindObjectOfType<BubbleShooter>().UpdateCurrentUnlockLevel();

        // 머지 1차 시도
        bool merged = TryMerge(cell.x, cell.y);

        if (!merged)
        {
            // 머지가 안 됐더라도 주변 버블 변화로 인해 병합 가능성이 있는 경우 재시도
            foreach (var neighbor in GetHexNeighbors(cell.x, cell.y))
            {
                if (TryMerge(neighbor.x, neighbor.y))
                {
                    merged = true;
                    break; // 한번이라도 병합되면 종료
                }
            }
        }

        if (!merged) SoundManager.Instance.PlayAttach();

    }

    private Vector2Int FindLowestAvailableCell(int x, int y)
    {
        for (int ny = y + 1; ny < rows; ny++)
            if (grid[ny, x] == null) return new Vector2Int(x, ny);
        return new Vector2Int(x, y);
    }

    private bool TryMerge(int sx, int sy)
    {
        Bubble start = grid[sy, sx];
        if (start == null) return false;

        int level = start.level;
        Queue<Vector2Int> queue = new();
        List<Vector2Int> cluster = new();
        bool[,] visited = new bool[rows, cols];

        queue.Enqueue(new Vector2Int(sx, sy));
        visited[sy, sx] = true;

        // 1. BFS로 병합 가능한 클러스터 탐색
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            cluster.Add(current);

            foreach (var neighbor in GetHexNeighbors(current.x, current.y))
            {
                if (!visited[neighbor.y, neighbor.x] &&
                    grid[neighbor.y, neighbor.x] != null &&
                    grid[neighbor.y, neighbor.x].level == level)
                {
                    visited[neighbor.y, neighbor.x] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        // 2. 3개 이상일 때만 병합 진행
        if (cluster.Count >= 3)
        {
            FindObjectOfType<BubbleShooter>().isMerging = true;

            cluster.Sort((a, b) => grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder));
            Vector2Int baseCell = cluster[0];
            Bubble baseBubble = grid[baseCell.y, baseCell.x];

            int baseLevel = baseBubble.level;
            int baseScore = gameManager.GetScoreForLevel(baseLevel);
            int bonus = (cluster.Count - 3) * (baseScore / 2);
            int totalScore = baseScore + bonus;

            gameManager.AddScore(totalScore);

            foreach (var pos in cluster)
            {
                Bubble bubble = grid[pos.y, pos.x];
                if (bubble == null) continue;

                if (bubble.level >= 8)
                {
                    SoundManager.Instance.PlayExplosion();
                    bubble.PlayExplosionAnimation();
                    grid[pos.y, pos.x] = null;

                    if (pos == baseCell)
                    {
                        bubble.PlayExplosionAnimation();
                        RemoveNearbyBubbles(baseCell);
                    }
                    else
                    {
                        bubble.PlaySmallExplosion(); // ← 작은 폭발 효과
                    }

                    grid[pos.y, pos.x] = null;
                }
                else
                {
                    SoundManager.Instance.PlayMerge();
                    bubble.PlayMergeAnimation();
                    if (pos != baseCell)
                        pendingMergeRemovals.Add(pos);
                }
            }
            FindObjectOfType<BubbleShooter>().isMerging = false;
            return true;
        }

        return false;
    }

    public void FinishMergeProcess(Bubble bubble)
    {
        Vector2Int cell = WorldToCell(bubble.transform.position);

        // 중심 버블 기준 좌표 저장
        Vector2Int baseCell = cell;

        if (bubble.level >= 8)
        {
            bubble.PlayExplosionAnimation();         // 중심 버블은 큰 폭발
            RemoveNearbyBubbles(baseCell);
            grid[baseCell.y, baseCell.x] = null;
        }
        else
        {
            bubble.RefreshVisual();
        }

        // 병합된 나머지 버블들 제거
        foreach (var pos in pendingMergeRemovals)
        {
            if (grid[pos.y, pos.x] != null)
            {
                Bubble target = grid[pos.y, pos.x];

                if (target.level >= 8)
                {
                    target.PlaySmallExplosion();     // 나머지는 작은 폭발
                }
                else
                {
                    Destroy(target.gameObject);      // 기존처럼 제거
                }

                grid[pos.y, pos.x] = null;
            }
        }

        pendingMergeRemovals.Clear();

        // 병합된 버블로 인해 추가 병합 가능성 재확인
        TryMerge(cell.x, cell.y);

        FindObjectOfType<BubbleShooter>().isMerging = false;
    }

    public List<Vector2Int> GetHexNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new();
        Vector2Int[] offsets = (y % 2 == 0)
            ? new Vector2Int[] { new(-1, -1), new(0, -1), new(1, 0), new(0, 1), new(-1, 1), new(-1, 0) }
            : new Vector2Int[] { new(0, -1), new(1, -1), new(1, 0), new(1, 1), new(0, 1), new(-1, 0) };

        foreach (var offset in offsets)
        {
            Vector2Int pos = new(x + offset.x, y + offset.y);
            if (IsValidPosition(pos)) neighbors.Add(pos);
        }
        return neighbors;
    }

    public int GetHighestBubbleLevel()
    {
        int maxLevel = 1; // 기본값

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != null)
                {
                    maxLevel = Mathf.Max(maxLevel, grid[y, x].level);
                }
            }
        }

        return Mathf.Min(maxLevel, 7); // 최대 7까지만 허용
    }


    public void CheckConnectedBubbles()
    {
        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
                if (grid[y, x] != null)
                    grid[y, x].isConnectedToGround = false;

        Queue<Vector2Int> queue = new();
        HashSet<Vector2Int> visited = new();

        for (int x = 0; x < cols; x++)
        {
            if (grid[rows - 1, x] != null)
            {
                Vector2Int pos = new(x, rows - 1);
                queue.Enqueue(pos);
                grid[pos.y, pos.x].isConnectedToGround = true;
            }
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (visited.Contains(current)) continue;

            visited.Add(current);

            foreach (var neighbor in GetHexNeighbors(current.x, current.y))
            {
                if (grid[neighbor.y, neighbor.x] != null && !visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    grid[neighbor.y, neighbor.x].isConnectedToGround = true;
                }
            }
        }
    }

    public void RepositionDisconnectedBubbles()
    {
        CheckConnectedBubbles();

        for (int y = rows - 2; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                Bubble bubble = grid[y, x];
                if (bubble != null && !bubble.isConnectedToGround)
                {
                    grid[y, x] = null; // 현재 위치에서 제거
                    Vector2Int down = new(x, y + 1); // 아래 한 칸

                    if (IsValidPosition(down) && grid[down.y, down.x] == null)
                    {
                        bubble.transform.position = GetGridPosition(down.x, down.y); // 위치 이동
                        grid[down.y, down.x] = bubble; // 격자 갱신
                    }
                }
            }
        }
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < cols && pos.y >= 0 && pos.y < rows;
    }

    private void RemoveNearbyBubbles(Vector2Int center)
    {
        for (int y = -2; y <= 2; y++)
        {
            for (int x = -2; x <= 2; x++)
            {
                Vector2Int pos = new(center.x + x, center.y + y);

                if (IsValidPosition(pos) && grid[pos.y, pos.x] != null)
                {
                    Bubble bubble = grid[pos.y, pos.x];
                    grid[pos.y, pos.x] = null;

                    // 작은 폭발 이펙트 실행 → 애니메이션 후 자동 제거
                    bubble.PlaySmallExplosion();
                }
            }
        }
    }
}