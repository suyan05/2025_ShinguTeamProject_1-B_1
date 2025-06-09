using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public float maxHeight = 7f; //���� ���� ���� ����
    public bool isGameOver = false;


    private Bubble[,] grid;
    private GameManager gameManager;

    void Start()
    {
        grid = new Bubble[rows, cols]; //���� �迭 �ʱ�ȭ
        gameManager = FindObjectOfType<GameManager>(); //���� �Ŵ��� ����
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
    private List<Vector2Int> GetNeighbors(int x, int y)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        int[] dx = { -1, 1, 0, 0 };
        int[] dy = { 0, 0, -1, 1 };

        for (int i = 0; i < dx.Length; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            if (nx >= 0 && nx < cols && ny >= 0 && ny < rows)
                neighbors.Add(new Vector2Int(nx, ny));
        }

        return neighbors;
    }

    // ������ ���� ���� �Ʒ��� �̵�
    public void RepositionDisconnectedBubbles()
    {
        HashSet<Vector2Int> connected = CheckConnectedBubbles();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != null && !connected.Contains(new Vector2Int(x, y)))
                {
                    grid[y, x] = null; // ������ ���� ���� ����

                    // ���� �Ʒ��� �̵�
                    int newY = rows - 1;
                    while (newY > y && grid[newY, x] != null)
                        newY--;

                    if (newY > y)
                    {
                        grid[newY, x] = new Bubble(); // �� ������ ���ο� ��ġ ����
                    }
                }
            }
        }
    }


    //���� ����� �� ���� ��ġ�� ã�� �Լ�
    public Vector2 FindNearestEmptyGrid(Vector2 position)
    {
        Vector2 nearestGridPos = Vector2.zero;
        float minDistance = float.MaxValue;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] == null) //�� �������� Ȯ��
                {
                    Vector2 gridPos = GetGridPosition(x, y);
                    float distance = Vector2.Distance(position, gridPos);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestGridPos = gridPos;
                    }
                }
            }
        }

        return nearestGridPos;
    }

    //���� ��ǥ�� ���� ��ǥ�� ��ȯ
    public Vector2 GetGridPosition(int x, int y)
    {
        float gridWidth = cols * bubbleSize;
        float gridHeight = rows * bubbleSize;
        Vector2 centerOffset = new Vector2(-gridWidth / 2f + bubbleSize / 2f, gridHeight / 2f - bubbleSize / 2f);

        // Ȧ�� ���� ��� x ��ġ�� ���ݾ� �̵�
        float xOffset = (y % 2 == 0) ? 0f : bubbleSize / 2f;

        return new Vector2(centerOffset.x + x * bubbleSize + xOffset, centerOffset.y - y * bubbleSize);
    }

    //������ ������ ��ġ�� ��ġ
    public void PlaceBubble(Bubble bubble)
    {
        if (isGameOver) return;

        Vector2 nearestGridPos = FindNearestEmptyGrid(bubble.transform.position);
        bubble.transform.position = nearestGridPos;

        if (CheckGameOver()) // ���� �ʰ� ���� Ȯ��
        {
            isGameOver = true;
            gameManager.GameOver(); // ���� ���� ����
        }
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