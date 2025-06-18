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

    [Header("�׸��� ��/��/��/�� ����")]
    public float horizontalSpacing = 1.0f; // ���� ����
    public float verticalSpacing = 1.2f; // ���� ����


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
        new(0, 1),  new(-1, 0), new(-1, -1)
    };

        Vector2Int[] oddOffsets = {
        new(0, -1), new(1, 0), new(1, 1),
        new(0, 1),  new(-1, 1), new(-1, 0)
    };

        var offsets = (y % 2 == 0) ? evenOffsets : oddOffsets;

        foreach (var offset in offsets)
        {
            int nx = x + offset.x;
            int ny = y + offset.y;
            if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                neighbors.Add(new(nx, ny));
        }

        return neighbors;
    }

    public void RemoveNearbyBubbles(Vector2Int basePosition)
{
    List<Vector2Int> bubblesToRemove = new List<Vector2Int>();

    for (int y = -2; y <= 2; y++)
    {
        for (int x = -2; x <= 2; x++)
        {
            Vector2Int pos = new Vector2Int(basePosition.x + x, basePosition.y + y);
            if (IsValidPosition(pos) && grid[pos.y, pos.x] != null)
            {
                bubblesToRemove.Add(pos);
            }
        }
    }

    int totalPoints = 0;
    foreach (Vector2Int pos in bubblesToRemove)
    {
        totalPoints += grid[pos.y, pos.x].level * 10; // ���ŵ� ������ ���� �߰�
        Destroy(grid[pos.y, pos.x].gameObject);
        grid[pos.y, pos.x] = null;
    }

    FindObjectOfType<GameManager>().AddScore(totalPoints);
}

private bool IsValidPosition(Vector2Int pos)
{
    return pos.x >= 0 && pos.x < cols && pos.y >= 0 && pos.y < rows;
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
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);

        int y = Mathf.Clamp(
            Mathf.RoundToInt((origin.y - worldPos.y) / verticalSpacing),
            0, rows - 1);

        float xOffset = (y % 2 == 0 ? 0f : horizontalSpacing / 2f);
        int x = Mathf.Clamp(
            Mathf.RoundToInt((worldPos.x - origin.x - xOffset) / horizontalSpacing),
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
        float gridW = cols * horizontalSpacing;
        float gridH = rows * verticalSpacing;
        Vector2 origin = new Vector2(-gridW / 2 + horizontalSpacing / 2, gridH / 2 - verticalSpacing / 2);

        float xOff = (y % 2 == 0 ? 0f : horizontalSpacing / 2f);
        return new Vector2(origin.x + x * horizontalSpacing + xOff, origin.y - y * verticalSpacing);
    }



    public void PlaceBubble(Bubble b)
    {
        if (isGameOver) return;

        // 1) ��ġ ���� (�ֺ� �� ĭ Ȯ��)
        Vector2 snap = FindNearestEmptyGrid(b.transform.position);

        // ���� ������ �ִ��� Ȯ��
        Vector2Int cell = WorldToCell(snap);
        if (grid[cell.y, cell.x] != null)
        {
            snap = FindLowestAvailableCell(cell.x, cell.y); // ���� ���� �� ���� ã��
        }

        // 2) ���� ��ġ ����
        b.transform.position = snap;
        cell = WorldToCell(snap);

        // 3) ���� ��� (���� ���� ��ġ ����)
        b.placedOrder = placeCounter++;
        grid[cell.y, cell.x] = b;

        // 4) ���ӿ��� üũ
        if (b.transform.position.y >= maxHeight)
        {
            isGameOver = true;
            gameManager.GameOver();
            return;
        }

        // 5) �ֺ� ���� ���� Ŭ������ ��ġ��
        TryMerge(cell.x, cell.y);

        // 6) ��ģ �� ���� ����� ����߸��� (���� ���� ��ġ ����)
        RepositionDisconnectedBubbles();
    }


    // ���� ���� Ŭ������ Ž�� & ��ġ��
    private void TryMerge(int sx, int sy)
    {
        Bubble start = grid[sy, sx];
        if (start == null) return;

        int lvl = start.level;
        bool[,] visited = new bool[rows, cols];
        Queue<Vector2Int> queue = new();
        List<Vector2Int> cluster = new();

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
            cluster.Sort((a, b) => grid[a.y, a.x].placedOrder.CompareTo(grid[b.y, b.x].placedOrder));

            Vector2Int baseCell = cluster[0];
            Bubble baseBubble = grid[baseCell.y, baseCell.x];
            baseBubble.level++;
            baseBubble.RefreshVisual();
            gameManager.AddScore(lvl * 10);

            for (int i = 1; i < cluster.Count; i++)
            {
                Vector2Int pos = cluster[i];
                Destroy(grid[pos.y, pos.x].gameObject);
                grid[pos.y, pos.x] = null;
                gameManager.AddScore(lvl * 10);
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