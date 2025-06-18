using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public Color gridColor = Color.green; // 그리드 색상
    public float maxHeight = 7f; // 게임 오버 높이 기준
    public bool isGameOver = false;

    [Header("그리드 상/하/좌/우 간격")]
    public float horizontalSpacing = 1.0f;
    public float verticalSpacing = 1.2f;

    private Bubble[,] grid;
    private GameManager gameManager;
    private int placeCounter = 0;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // 게임 매니저 참조
        InitializeGrid(); // 수정된 그리드 초기화
    }

    private void FixedUpdate()
    {
        RepositionDisconnectedBubbles(); // 매 프레임 연결되지 않은 버블 아래로 이동
    }

    private void InitializeGrid()
    {
        grid = new Bubble[rows, cols];

        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols; // 홀수 줄이면 cols - 1
            for (int x = 0; x < actualCols; x++)
            {
                grid[y, x] = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols; // 홀수 줄 줄이기 적용
            for (int x = 0; x < actualCols; x++)
            {
                Vector2 position = GetGridPosition(x, y);
                Gizmos.DrawWireSphere(position, bubbleSize * 0.4f); // 작은 원으로 표시
            }
        }
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


    // 버블이 바닥과 연결되어 있는지 확인
    public HashSet<Vector2Int> CheckConnectedBubbles()
    {
        HashSet<Vector2Int> connectedBubbles = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 바닥에 있는 모든 버블을 시작점으로 추가
        for (int x = 0; x < cols; x++)
        {
            if (grid[0, x] != null) //바닥에 존재하는 버블만 큐에 추가
                queue.Enqueue(new Vector2Int(x, 0));
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (!connectedBubbles.Contains(current))
            {
                connectedBubbles.Add(current);

                foreach (Vector2Int neighbor in GetHexNeighbors(current.x, current.y))
                {
                    if (grid[neighbor.y, neighbor.x] != null && !connectedBubbles.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
        }

        return connectedBubbles;
    }

    private List<Vector2Int> GetHexNeighbors(int x, int y)
    {
        var neighbors = new List<Vector2Int>();

        Vector2Int[] evenOffsets = {
        new(0, -1), new(1, -1), new(1, 0),
        new(0, 1), new(-1, 0), new(-1, -1)
    };

        Vector2Int[] oddOffsets = {
        new(0, -1), new(1, 0), new(1, 1),
        new(0, 1), new(-1, 1), new(-1, 0)
    };

        var offsets = (y % 2 == 0) ? evenOffsets : oddOffsets;
        int actualCols = (y % 2 == 1) ? cols - 1 : cols; // 홀수 줄의 제한 적용

        foreach (var offset in offsets)
        {
            Vector2Int pos = new(x + offset.x, y + offset.y);
            if (IsValidPosition(pos)) // 유효한 위치인지 검사하여 정확한 탐색 가능
            {
                neighbors.Add(pos);
            }
        }

        return neighbors;
    }

    private void RemoveNearbyBubbles(Vector2Int basePosition)
    {
        List<Vector2Int> bubblesToRemove = new List<Vector2Int>();

        for (int y = -2; y <= 2; y++) // 2칸 범위 검사
        {
            for (int x = -2; x <= 2; x++)
            {
                Vector2Int pos = new(basePosition.x + x, basePosition.y + y);
                if (IsValidPosition(pos) && grid[pos.y, pos.x] != null)
                {
                    bubblesToRemove.Add(pos);
                }
            }
        }

        foreach (Vector2Int pos in bubblesToRemove)
        {
            Destroy(grid[pos.y, pos.x].gameObject);
            grid[pos.y, pos.x] = null;
        }
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        int actualCols = (pos.y % 2 == 1) ? cols - 1 : cols; // 홀수 줄이면 cols - 1 적용
        return pos.x >= 0 && pos.x < actualCols && pos.y >= 0 && pos.y < rows;
    }

    // 연결되지 않은 버블 아래로 이동 및 연결된 버블 탐색
    // 연결되지 않은 버블 아래로 이동 (중력 적용)
    public void RepositionDisconnectedBubbles()
    {
        var connected = CheckConnectedBubbles();

        for (int y = rows - 1; y >= 0; y--) // 아래쪽부터 탐색
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != null && !connected.Contains(new Vector2Int(x, y))) //연결되지 않은 경우만 처리
                {
                    Bubble fallingBubble = grid[y, x];
                    grid[y, x] = null;

                    Vector2Int targetCell = FindLowestAvailableCell(x, y);
                    if (IsValidPosition(targetCell) && targetCell.y >= 0) //빈 칸이 유효한지 확인 후 이동
                    {
                        Vector2 targetPosition = GetGridPosition(targetCell.x, targetCell.y);
                        fallingBubble.transform.position = targetPosition;
                        grid[targetCell.y, targetCell.x] = fallingBubble;
                    }
                }
            }
        }
    }

    private Vector2Int FindLowestAvailableCell(int x, int y)
    {
        for (int ny = y + 1; ny < rows; ny++) // 아래 방향으로 탐색
        {
            if (grid[ny, x] == null) // 빈 칸 발견
            {
                return new Vector2Int(x, ny);
            }
        }

        return new Vector2Int(x, y); // 빈 칸이 없으면 원래 위치 유지
    }


    // 월드 좌표 → 그리드 인덱스
    private Vector2Int WorldToCell(Vector2 worldPos)
    {
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);

        int y = Mathf.Clamp(Mathf.RoundToInt((origin.y - worldPos.y) / verticalSpacing), 0, rows - 1);

        int actualCols = (y % 2 == 1) ? cols - 1 : cols; // 홀수 줄이면 cols - 1 적용
        float xOffset = (y % 2 == 0 ? 0f : horizontalSpacing / 2f);

        int x = Mathf.Clamp(Mathf.RoundToInt((worldPos.x - origin.x - xOffset) / horizontalSpacing), 0, actualCols - 1);

        return new Vector2Int(x, y);
    }

    //가장 가까운 빈 격자 위치를 찾는 함수
    // 빈 칸 중 가장 가까운 위치 계산
    public Vector2 FindNearestEmptyGrid(Vector2 pos)
    {
        Vector2 best = Vector2.zero;
        float minDist = float.MaxValue;

        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols;
            for (int x = 0; x < actualCols; x++) // 홀수 줄의 제거된 좌표 제외
            {
                if (grid[y, x] == null)
                {
                    Vector2 world = GetGridPosition(x, y);
                    float d = Vector2.Distance(pos, world);
                    if (d < minDist)
                    {
                        minDist = d;
                        best = world;
                    }
                }
            }
        }
        return best;
    }

    //격자 좌표를 월드 좌표로 변환
    // 그리드 → 월드 좌표
    public Vector2 GetGridPosition(int x, int y)
    {
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);

        float xOff = (y % 2 == 0 ? 0f : horizontalSpacing / 2f);
        return new Vector2(origin.x + x * horizontalSpacing + xOff, origin.y - y * verticalSpacing);
    }

    public void PlaceBubble(Bubble b)
    {
        if (isGameOver) return;

        Vector2 snap = FindNearestEmptyGrid(b.transform.position);
        Vector2Int cell = WorldToCell(snap);

        if (grid[cell.y, cell.x] != null)
            snap = FindLowestAvailableCell(cell.x, cell.y);

        b.transform.position = snap;
        cell = WorldToCell(snap);

        b.placedOrder = placeCounter++;
        grid[cell.y, cell.x] = b;

        // 버블 배치 후 최고 레벨 갱신
        FindObjectOfType<BubbleShooter>().UpdateCurrentUnlockLevel();

        if (b.transform.position.y >= maxHeight)
        {
            isGameOver = true;
            gameManager.GameOver();
            return;
        }

        TryMerge(cell.x, cell.y);
        RepositionDisconnectedBubbles();
    }

    // 같은 레벨 클러스터 탐색 & 합치기
    private void TryMerge(int sx, int sy)
    {
        Bubble start = grid[sy, sx];
        if (start == null) return;

        int lvl = start.level;
        Queue<Vector2Int> queue = new();
        List<Vector2Int> cluster = new();
        bool[,] visited = new bool[rows, cols];

        queue.Enqueue(new Vector2Int(sx, sy));
        visited[sy, sx] = true;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            cluster.Add(current);

            foreach (var neighbor in GetHexNeighbors(current.x, current.y))
            {
                if (!visited[neighbor.y, neighbor.x] &&
                    grid[neighbor.y, neighbor.x] != null &&
                    grid[neighbor.y, neighbor.x].level == lvl)
                {
                    visited[neighbor.y, neighbor.x] = true;
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (cluster.Count >= 3) // 3개 이상일 때 병합
        {
            cluster.Sort((a, b) => grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder));

            Vector2Int baseCell = cluster[0];
            Bubble baseBubble = grid[baseCell.y, baseCell.x];

            int baseScore = gameManager.levelScores.TryGetValue(lvl, out int basePoints) ? basePoints : lvl * 10;
            int additionalCount = cluster.Count - 3;
            int additionalScore = additionalCount * (baseScore / 2);

            gameManager.AddScore(baseScore + additionalScore);

            foreach (var pos in cluster)
            {
                Bubble bubble = grid[pos.y, pos.x];
                bubble.PlayMergeAnimation();
            }

            baseBubble.level++;
            baseBubble.RefreshVisual();

            // 최종 등급 도달 시 폭발 애니메이션 실행 및 주변 제거
            if (baseBubble.level >= 8) // 최종 등급 체크
            {
                baseBubble.PlayExplosionAnimation();
                RemoveNearbyBubbles(baseCell);
            }

            for (int i = 1; i < cluster.Count; i++)
            {
                Vector2Int pos = cluster[i];
                Destroy(grid[pos.y, pos.x].gameObject);
                grid[pos.y, pos.x] = null;
            }

            TryMerge(baseCell.x, baseCell.y);
        }
    }

    /*private Vector2Int WorldPosToCell(Vector2 worldPos)
    {
        // GetGridPosition(x,y) 의 역연산
        float gridWidth = cols * bubbleSize;
        float gridHeight = rows * bubbleSize;
        Vector2 centerOffset = new Vector2(-gridWidth / 2 + bubbleSize / 2, gridHeight / 2 - bubbleSize / 2);

        // 대략적인 y 인덱스
        int y = Mathf.FloorToInt((centerOffset.y - worldPos.y) / bubbleSize + 0.5f);
        float xOffset = (y % 2 == 0) ? 0f : bubbleSize / 2f;
        int x = Mathf.FloorToInt((worldPos.x - centerOffset.x - xOffset) / bubbleSize + 0.5f);

        return new Vector2Int(x, y);
    }

    //게임 오버 체크 함수
    public bool CheckGameOver()
    {
        foreach (Bubble bubble in FindObjectsOfType<Bubble>()) // 모든 버블 검사
        {
            if (bubble.transform.position.y >= maxHeight) // 특정 높이 초과 여부 확인
                return true;
        }
        return false;
    }*/
}