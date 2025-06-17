using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public Color gridColor = Color.green; // 그리드 색상
    public float maxHeight = 7f; //게임 오버 높이 기준
    public bool isGameOver = false;


    private Bubble[,] grid;
    private GameManager gameManager;
    private int placeCounter = 0;

    void Start()
    {
        grid = new Bubble[rows, cols]; //격자 배열 초기화
        gameManager = FindObjectOfType<GameManager>(); //게임 매니저 참조
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2 position = GetGridPosition(x, y);
                Gizmos.DrawWireSphere(position, bubbleSize * 0.4f); // 작은 원으로 표시
            }
        }
    }


    // 버블이 바닥과 연결되어 있는지 확인
    private HashSet<Vector2Int> CheckConnectedBubbles()
    {
        HashSet<Vector2Int> connectedBubbles = new HashSet<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 첫 번째 행의 모든 버블을 시작점으로 추가
        for (int x = 0; x < cols; x++)
        {
            if (grid[0, x] != null)
                queue.Enqueue(new Vector2Int(x, 0));
        }

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (!connectedBubbles.Contains(current))
            {
                connectedBubbles.Add(current);

                foreach (Vector2Int neighbor in GetNeighbors(current.x, current.y))
                {
                    if (grid[neighbor.y, neighbor.x] != null && !connectedBubbles.Contains(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
        }

        return connectedBubbles;
    }

    // 주어진 위치의 이웃 좌표 반환
    // 상하좌우 이웃 반환
    private List<Vector2Int> GetNeighbors(int x, int y)
    {
        var ret = new List<Vector2Int>();
        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };
        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i], ny = y + dy[i];
            if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                ret.Add(new Vector2Int(nx, ny));
        }
        return ret;
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
                if (grid[y, x] != null && !connected.Contains(new Vector2Int(x, y)))
                {
                    Bubble fallingBubble = grid[y, x];
                    grid[y, x] = null;

                    Vector2Int targetCell = FindLowestAvailableCell(x, y);

                    // 빈 칸이 있다면 이동
                    if (targetCell.y >= 0)
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
        float gridW = cols * bubbleSize;
        float gridH = rows * bubbleSize;
        Vector2 origin = new Vector2(-gridW / 2 + bubbleSize / 2, gridH / 2 - bubbleSize / 2);

        int y = Mathf.Clamp(
            Mathf.RoundToInt((origin.y - worldPos.y) / bubbleSize),
            0, rows - 1);
        float xOffset = (y % 2 == 0 ? 0f : bubbleSize / 2f);
        int x = Mathf.Clamp(
            Mathf.RoundToInt((worldPos.x - origin.x - xOffset) / bubbleSize),
            0, cols - 1);

        return new Vector2Int(x, y);
    }


    //가장 가까운 빈 격자 위치를 찾는 함수
    // 빈 칸 중 가장 가까운 위치 계산
    public Vector2 FindNearestEmptyGrid(Vector2 pos)
    {
        Vector2 best = Vector2.zero;
        float minDist = float.MaxValue;

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < cols; x++)
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
        return best;
    }


    //격자 좌표를 월드 좌표로 변환
    // 그리드 → 월드 좌표
    public Vector2 GetGridPosition(int x, int y)
    {
        float gridW = cols * bubbleSize;
        float gridH = rows * bubbleSize;
        Vector2 origin = new Vector2(-gridW / 2 + bubbleSize / 2, gridH / 2 - bubbleSize / 2);
        float xOff = (y % 2 == 0 ? 0f : bubbleSize / 2f);
        return new Vector2(origin.x + x * bubbleSize + xOff, origin.y - y * bubbleSize);
    }


    // 버블 배치
    public void PlaceBubble(Bubble b)
    {
        if (isGameOver) return;

        // 1) 위치 스냅
        Vector2 snap = FindNearestEmptyGrid(b.transform.position);
        b.transform.position = snap;

        // 2) 그리드 등록
        Vector2Int cell = WorldToCell(snap);
        b.placedOrder = placeCounter++;
        grid[cell.y, cell.x] = b;

        // 3) 게임오버 체크 (버블 기준)
        if (b.transform.position.y >= maxHeight)
        {
            isGameOver = true;
            gameManager.GameOver();
            return;
        }

        // 4) 주변 같은 레벨 클러스터 합치기
        TryMerge(cell.x, cell.y);

        // 5) 합친 뒤 끊긴 버블들 떨어뜨리기
        RepositionDisconnectedBubbles();
    }

    // 같은 레벨 클러스터 탐색 & 합치기
    private void TryMerge(int sx, int sy)
    {
        Bubble start = grid[sy, sx];
        if (start == null) return;

        int lvl = start.level;
        bool[,] visited = new bool[rows, cols];
        var queue = new Queue<Vector2Int>();
        var cluster = new List<Vector2Int>();

        queue.Enqueue(new Vector2Int(sx, sy));
        visited[sy, sx] = true;

        // BFS로 같은 레벨 수집
        while (queue.Count > 0)
        {
            var cur = queue.Dequeue();
            cluster.Add(cur);

            foreach (var n in GetNeighbors(cur.x, cur.y))
            {
                if (!visited[n.y, n.x] &&
                    grid[n.y, n.x] != null &&
                    grid[n.y, n.x].level == lvl)
                {
                    visited[n.y, n.x] = true;
                    queue.Enqueue(n);
                }
            }
        }

        // 3개 이상 모이면 합치기
        if (cluster.Count >= 3)
        {
            // 1) 배치 순서 기준 정렬
            cluster.Sort((a, b) =>
                grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder)
            );

            // 2) 기준 버블 업그레이드
            Vector2Int baseCell = cluster[0];
            Bubble baseB = grid[baseCell.y, baseCell.x];
            baseB.level++;
            baseB.RefreshVisual();
            gameManager.AddScore(lvl * 10);

            // 3) 나머지 클러스터 모두 제거
            for (int i = 1; i < cluster.Count; i++)
            {
                var c = cluster[i];
                Destroy(grid[c.y, c.x].gameObject);
                grid[c.y, c.x] = null;
                gameManager.AddScore(lvl * 10);
            }

            // 4) 기준 버블 기준 2칸 반경 내 모든 버블 제거
            for (int dy = -2; dy <= 2; dy++)
                for (int dx = -2; dx <= 2; dx++)
                {
                    int nx = baseCell.x + dx;
                    int ny = baseCell.y + dy;
                    if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                    {
                        Bubble victim = grid[ny, nx];
                        if (victim != null && !(nx == baseCell.x && ny == baseCell.y))
                        {
                            Destroy(victim.gameObject);
                            grid[ny, nx] = null;
                            gameManager.AddScore(victim.level * 10);
                        }
                    }
                }
        }
    }


    private Vector2Int WorldPosToCell(Vector2 worldPos)
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
    }


}