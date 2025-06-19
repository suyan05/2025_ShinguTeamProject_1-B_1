using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public static BubbleGrid Instance { get; private set; }

    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public Color gridColor = Color.green; // 그리드 색상
    public bool isGameOver = false;

    [Header("그리드 상/하/좌/우 간격")]
    public float horizontalSpacing = 1.0f;
    public float verticalSpacing = 1.2f;

    private Bubble[,] grid;
    private GameManager gameManager;
    //private int placeCounter = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // 중복 생성 방지
    }

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
            int actualCols = (y % 2 == 1) ? cols - 1 : cols;
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
    public void CheckConnectedBubbles()
    {
        // 1. 우선 모든 버블의 연결 상태를 false로 초기화
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != null)
                    grid[y, x].isConnectedToGround = false;
            }
        }

        HashSet<Vector2Int> connected = new HashSet<Vector2Int>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 2. 바닥으로 간주하는 행은 마지막 행 (rows - 1)
        for (int x = 0; x < cols; x++)
        {
            if (grid[rows - 1, x] != null)
            {
                Vector2Int pos = new Vector2Int(x, rows - 1);
                queue.Enqueue(pos);
                grid[rows - 1, x].isConnectedToGround = true; // 바닥에 있는 버블은 무조건 true
            }
        }

        // 3. BFS 탐색을 통해 바닥과 연결된 모든 버블에 대해 isConnectedToGround를 true로 설정
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (!connected.Contains(current))
            {
                connected.Add(current);

                foreach (Vector2Int neighbor in GetHexNeighbors(current.x, current.y))
                {
                    if (grid[neighbor.y, neighbor.x] != null && !connected.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        grid[neighbor.y, neighbor.x].isConnectedToGround = true;
                    }
                }
            }
        }
        // BFS로 방문하지 않은 버블은 이미 false 상태이므로 그대로 둡니다.
    }

    private List<Vector2Int> GetHexNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (y % 2 == 0) // 짝수 행
        {
            Vector2Int[] evenOffsets = {
            new Vector2Int(-1, -1), // NW
            new Vector2Int(0, -1),  // NE
            new Vector2Int(1, 0),   // E
            new Vector2Int(0, 1),   // SE
            new Vector2Int(-1, 1),  // SW
            new Vector2Int(-1, 0)   // W
            };

            foreach (Vector2Int offset in evenOffsets)
            {
                Vector2Int neighborPos = new Vector2Int(x + offset.x, y + offset.y);
                if (IsValidPosition(neighborPos))
                {
                    neighbors.Add(neighborPos);
                }
            }
        }
        else // 홀수 행
        {
            Vector2Int[] oddOffsets = {
            new Vector2Int(0, -1),  // NW
            new Vector2Int(1, -1),  // NE
            new Vector2Int(1, 0),   // E
            new Vector2Int(1, 1),   // SE
            new Vector2Int(0, 1),   // SW
            new Vector2Int(-1, 0)   // W
            };

            foreach (Vector2Int offset in oddOffsets)
            {
                Vector2Int neighborPos = new Vector2Int(x + offset.x, y + offset.y);
                if (IsValidPosition(neighborPos))
                {
                    neighbors.Add(neighborPos);
                }
            }
        }

        return neighbors;
    }


    private void RemoveNearbyBubbles(Vector2Int basePosition)
    {
        List<Vector2Int> bubblesToRemove = new();

        for (int y = -2; y <= 2; y++)
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
        //int actualCols = (pos.y % 2 == 1) ? cols - 1 : cols; // 홀수 줄이면 cols - 1 적용
        return pos.x >= 0 && pos.x < cols && pos.y >= 0 && pos.y < rows;
    }

    // 연결되지 않은 버블 아래로 이동 및 연결된 버블 탐색
    // 연결되지 않은 버블 아래로 이동 (중력 적용)
    public void RepositionDisconnectedBubbles()
    {
        // 먼저 현재 터치한 후 연결 여부 업데이트
        CheckConnectedBubbles();

        // 바닥과 연결되지 않은 버블을 한 칸씩 내림.
        // (아래쪽 행은 바닥이므로 rows - 2 부터 진행)
        for (int y = rows - 2; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                Bubble bubble = grid[y, x];
                if (bubble != null && !bubble.isConnectedToGround)
                {
                    // 현재 셀에서 삭제 후 한 칸 아래 셀로 이동
                    grid[y, x] = null;
                    Vector2Int targetCell = new Vector2Int(x, y + 1);

                    if (IsValidPosition(targetCell) && grid[targetCell.y, targetCell.x] == null)
                    {
                        // 그리드 좌표를 월드 좌표로 변환하여 버블 이동
                        Vector2 targetPos = GetGridPosition(targetCell.x, targetCell.y);
                        bubble.transform.position = targetPos;
                        grid[targetCell.y, targetCell.x] = bubble;
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

        Vector2Int cell = WorldToCell(b.transform.position);
        int actualCols = (cell.y % 2 == 1) ? cols - 1 : cols;
        if (cell.x >= actualCols) cell.x = actualCols - 1;

        if (grid[cell.y, cell.x] != null)
            cell = FindLowestAvailableCell(cell.x, cell.y);

        b.transform.position = GetGridPosition(cell.x, cell.y);
        grid[cell.y, cell.x] = b;

        FindObjectOfType<BubbleShooter>().UpdateCurrentUnlockLevel();
        TryMerge(cell.x, cell.y);
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

        if (cluster.Count >= 3)
        {
            //가장 먼저 배치된 버블을 기준으로 정렬 (placedOrder 값 기준)
            cluster.Sort((a, b) => grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder));

            Vector2Int baseCell = cluster[0]; // 가장 먼저 배치된 버블 선택
            Bubble baseBubble = grid[baseCell.y, baseCell.x];

            foreach (var pos in cluster)
            {
                Bubble bubble = grid[pos.y, pos.x];
                bubble.PlayMergeAnimation();
            }


            if (baseBubble.level >= 7)
            {
                baseBubble.PlayExplosionAnimation();
                RemoveNearbyBubbles(baseCell);
            }
            else
            {
                baseBubble.RefreshVisual();
            }

            for (int i = 1; i < cluster.Count; i++)
            {
                Vector2Int pos = cluster[i];
                Destroy(grid[pos.y, pos.x].gameObject);
                grid[pos.y, pos.x] = null;
            }

            TryMerge(baseCell.x, baseCell.y); //연속 병합 실행
        }
    }



    public void FinishMergeProcess(Bubble bubble)
    {
        Vector2Int cell = WorldToCell(bubble.transform.position);

        if (bubble.level >= 8) // 최종 등급 도달 시 폭발 애니메이션 실행
        {
            bubble.PlayExplosionAnimation();
            RemoveNearbyBubbles(cell);
        }
        else
        {
            bubble.RefreshVisual(); // 레벨 증가 후 UI 업데이트
        }

        TryMerge(cell.x, cell.y); //연속 병합 실행
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

    
    }*/