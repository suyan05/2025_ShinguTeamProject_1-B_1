using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public Color gridColor = Color.green; // �׸��� ����
    public float maxHeight = 7f; //���� ���� ���� ����
    public bool isGameOver = false;


    private Bubble[,] grid;
    private GameManager gameManager;
    private int placeCounter = 0;

    void Start()
    {
        grid = new Bubble[rows, cols]; //���� �迭 �ʱ�ȭ
        gameManager = FindObjectOfType<GameManager>(); //���� �Ŵ��� ����
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gridColor;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2 position = GetGridPosition(x, y);
                Gizmos.DrawWireSphere(position, bubbleSize * 0.4f); // ���� ������ ǥ��
            }
        }
    }


    // ������ �ٴڰ� ����Ǿ� �ִ��� Ȯ��
    private HashSet<Vector2Int> CheckConnectedBubbles()
    {
        HashSet<Vector2Int> connectedBubbles = new HashSet<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // ù ��° ���� ��� ������ ���������� �߰�
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

    // �־��� ��ġ�� �̿� ��ǥ ��ȯ
    // �����¿� �̿� ��ȯ
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


    // ������� ���� ���� �Ʒ��� �̵� �� ����� ���� Ž��
    // ������� ���� ���� �Ʒ��� �̵� (�߷� ����)
    public void RepositionDisconnectedBubbles()
    {
        var connected = CheckConnectedBubbles();

        for (int y = rows - 1; y >= 0; y--) // �Ʒ��ʺ��� Ž��
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != null && !connected.Contains(new Vector2Int(x, y)))
                {
                    Bubble fallingBubble = grid[y, x];
                    grid[y, x] = null;

                    Vector2Int targetCell = FindLowestAvailableCell(x, y);

                    // �� ĭ�� �ִٸ� �̵�
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


    //���� ����� �� ���� ��ġ�� ã�� �Լ�
    // �� ĭ �� ���� ����� ��ġ ���
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


    //���� ��ǥ�� ���� ��ǥ�� ��ȯ
    // �׸��� �� ���� ��ǥ
    public Vector2 GetGridPosition(int x, int y)
    {
        float gridW = cols * bubbleSize;
        float gridH = rows * bubbleSize;
        Vector2 origin = new Vector2(-gridW / 2 + bubbleSize / 2, gridH / 2 - bubbleSize / 2);
        float xOff = (y % 2 == 0 ? 0f : bubbleSize / 2f);
        return new Vector2(origin.x + x * bubbleSize + xOff, origin.y - y * bubbleSize);
    }


    // ���� ��ġ
    public void PlaceBubble(Bubble b)
    {
        if (isGameOver) return;

        // 1) ��ġ ����
        Vector2 snap = FindNearestEmptyGrid(b.transform.position);
        b.transform.position = snap;

        // 2) �׸��� ���
        Vector2Int cell = WorldToCell(snap);
        b.placedOrder = placeCounter++;
        grid[cell.y, cell.x] = b;

        // 3) ���ӿ��� üũ (���� ����)
        if (b.transform.position.y >= maxHeight)
        {
            isGameOver = true;
            gameManager.GameOver();
            return;
        }

        // 4) �ֺ� ���� ���� Ŭ������ ��ġ��
        TryMerge(cell.x, cell.y);

        // 5) ��ģ �� ���� ����� ����߸���
        RepositionDisconnectedBubbles();
    }

    // ���� ���� Ŭ������ Ž�� & ��ġ��
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

        // BFS�� ���� ���� ����
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

        // 3�� �̻� ���̸� ��ġ��
        if (cluster.Count >= 3)
        {
            // 1) ��ġ ���� ���� ����
            cluster.Sort((a, b) =>
                grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder)
            );

            // 2) ���� ���� ���׷��̵�
            Vector2Int baseCell = cluster[0];
            Bubble baseB = grid[baseCell.y, baseCell.x];
            baseB.level++;
            baseB.RefreshVisual();
            gameManager.AddScore(lvl * 10);

            // 3) ������ Ŭ������ ��� ����
            for (int i = 1; i < cluster.Count; i++)
            {
                var c = cluster[i];
                Destroy(grid[c.y, c.x].gameObject);
                grid[c.y, c.x] = null;
                gameManager.AddScore(lvl * 10);
            }

            // 4) ���� ���� ���� 2ĭ �ݰ� �� ��� ���� ����
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

    //���� ���� üũ �Լ�
    public bool CheckGameOver()
    {
        foreach (Bubble bubble in FindObjectsOfType<Bubble>()) // ��� ���� �˻�
        {
            if (bubble.transform.position.y >= maxHeight) // Ư�� ���� �ʰ� ���� Ȯ��
                return true;
        }
        return false;
    }


}