using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public static BubbleGrid Instance { get; private set; }

    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public Color gridColor = Color.green; // �׸��� ����
    public bool isGameOver = false;

    [Header("�׸��� ��/��/��/�� ����")]
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
            Destroy(gameObject); // �ߺ� ���� ����
    }

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>(); // ���� �Ŵ��� ����
        InitializeGrid(); // ������ �׸��� �ʱ�ȭ
    }

    private void FixedUpdate()
    {
        RepositionDisconnectedBubbles(); // �� ������ ������� ���� ���� �Ʒ��� �̵�
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
            int actualCols = (y % 2 == 1) ? cols - 1 : cols; // Ȧ�� �� ���̱� ����
            for (int x = 0; x < actualCols; x++)
            {
                Vector2 position = GetGridPosition(x, y);
                Gizmos.DrawWireSphere(position, bubbleSize * 0.4f); // ���� ������ ǥ��
            }
        }
    }

    public int GetHighestBubbleLevel()
    {
        int maxLevel = 1; // �⺻��

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

        return Mathf.Min(maxLevel, 7); // �ִ� 7������ ���
    }


    // ������ �ٴڰ� ����Ǿ� �ִ��� Ȯ��
    public void CheckConnectedBubbles()
    {
        // 1. �켱 ��� ������ ���� ���¸� false�� �ʱ�ȭ
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

        // 2. �ٴ����� �����ϴ� ���� ������ �� (rows - 1)
        for (int x = 0; x < cols; x++)
        {
            if (grid[rows - 1, x] != null)
            {
                Vector2Int pos = new Vector2Int(x, rows - 1);
                queue.Enqueue(pos);
                grid[rows - 1, x].isConnectedToGround = true; // �ٴڿ� �ִ� ������ ������ true
            }
        }

        // 3. BFS Ž���� ���� �ٴڰ� ����� ��� ���� ���� isConnectedToGround�� true�� ����
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
        // BFS�� �湮���� ���� ������ �̹� false �����̹Ƿ� �״�� �Ӵϴ�.
    }

    private List<Vector2Int> GetHexNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        if (y % 2 == 0) // ¦�� ��
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
        else // Ȧ�� ��
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
        //int actualCols = (pos.y % 2 == 1) ? cols - 1 : cols; // Ȧ�� ���̸� cols - 1 ����
        return pos.x >= 0 && pos.x < cols && pos.y >= 0 && pos.y < rows;
    }

    // ������� ���� ���� �Ʒ��� �̵� �� ����� ���� Ž��
    // ������� ���� ���� �Ʒ��� �̵� (�߷� ����)
    public void RepositionDisconnectedBubbles()
    {
        // ���� ���� ��ġ�� �� ���� ���� ������Ʈ
        CheckConnectedBubbles();

        // �ٴڰ� ������� ���� ������ �� ĭ�� ����.
        // (�Ʒ��� ���� �ٴ��̹Ƿ� rows - 2 ���� ����)
        for (int y = rows - 2; y >= 0; y--)
        {
            for (int x = 0; x < cols; x++)
            {
                Bubble bubble = grid[y, x];
                if (bubble != null && !bubble.isConnectedToGround)
                {
                    // ���� ������ ���� �� �� ĭ �Ʒ� ���� �̵�
                    grid[y, x] = null;
                    Vector2Int targetCell = new Vector2Int(x, y + 1);

                    if (IsValidPosition(targetCell) && grid[targetCell.y, targetCell.x] == null)
                    {
                        // �׸��� ��ǥ�� ���� ��ǥ�� ��ȯ�Ͽ� ���� �̵�
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
        for (int ny = y + 1; ny < rows; ny++) // �Ʒ� �������� Ž��
        {
            if (grid[ny, x] == null) // �� ĭ �߰�
            {
                return new Vector2Int(x, ny);
            }
        }

        return new Vector2Int(x, y); // �� ĭ�� ������ ���� ��ġ ����
    }


    // ���� ��ǥ �� �׸��� �ε���
    private Vector2Int WorldToCell(Vector2 worldPos)
    {
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);

        int y = Mathf.Clamp(Mathf.RoundToInt((origin.y - worldPos.y) / verticalSpacing), 0, rows - 1);

        int actualCols = (y % 2 == 1) ? cols - 1 : cols; // Ȧ�� ���̸� cols - 1 ����
        float xOffset = (y % 2 == 0 ? 0f : horizontalSpacing / 2f);

        int x = Mathf.Clamp(Mathf.RoundToInt((worldPos.x - origin.x - xOffset) / horizontalSpacing), 0, actualCols - 1);

        return new Vector2Int(x, y);
    }

    //���� ����� �� ���� ��ġ�� ã�� �Լ�
    // �� ĭ �� ���� ����� ��ġ ���
    public Vector2 FindNearestEmptyGrid(Vector2 pos)
    {
        Vector2 best = Vector2.zero;
        float minDist = float.MaxValue;

        for (int y = 0; y < rows; y++)
        {
            int actualCols = (y % 2 == 1) ? cols - 1 : cols;
            for (int x = 0; x < actualCols; x++) // Ȧ�� ���� ���ŵ� ��ǥ ����
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

    //���� ��ǥ�� ���� ��ǥ�� ��ȯ
    // �׸��� �� ���� ��ǥ
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


    // ���� ���� Ŭ������ Ž�� & ��ġ��
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
            //���� ���� ��ġ�� ������ �������� ���� (placedOrder �� ����)
            cluster.Sort((a, b) => grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder));

            Vector2Int baseCell = cluster[0]; // ���� ���� ��ġ�� ���� ����
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

            TryMerge(baseCell.x, baseCell.y); //���� ���� ����
        }
    }



    public void FinishMergeProcess(Bubble bubble)
    {
        Vector2Int cell = WorldToCell(bubble.transform.position);

        if (bubble.level >= 8) // ���� ��� ���� �� ���� �ִϸ��̼� ����
        {
            bubble.PlayExplosionAnimation();
            RemoveNearbyBubbles(cell);
        }
        else
        {
            bubble.RefreshVisual(); // ���� ���� �� UI ������Ʈ
        }

        TryMerge(cell.x, cell.y); //���� ���� ����
    }
}


    /*private Vector2Int WorldPosToCell(Vector2 worldPos)
    {
        // GetGridPosition(x,y) �� ������
        float gridWidth = cols * bubbleSize;
        float gridHeight = rows * bubbleSize;
        Vector2 centerOffset = new Vector2(-gridWidth / 2 + bubbleSize / 2, gridHeight / 2 - bubbleSize / 2);

        // �뷫���� y �ε���
        int y = Mathf.FloorToInt((centerOffset.y - worldPos.y) / bubbleSize + 0.5f);
        float xOffset = (y % 2 == 0) ? 0f : bubbleSize / 2f;
        int x = Mathf.FloorToInt((worldPos.x - centerOffset.x - xOffset) / bubbleSize + 0.5f);

        return new Vector2Int(x, y);
    }

    
    }*/