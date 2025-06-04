using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // ������ �� ����
    public int cols = 6; // ������ �� ����
    public float bubbleSize = 1f; // �� ���� ũ��
    public GameObject bubblePrefab; // ���� ������
    public Bubble bubble;
    public Bubble[,] grid; // ������ �����ϴ� ���� ����

    private void Start()
    {
        grid = new Bubble[rows, cols];
        bubble = GetComponent<Bubble>(); // Bubble ������Ʈ ��������
        InitializeGrid(); // ���� �ʱ�ȭ
    }

    // ���ڿ� �ʱ� ���� ��ġ
    public void InitializeGrid()
    {
        float gridWidth = cols * bubbleSize;
        float gridHeight = rows * bubbleSize;
        Vector2 startPosition = new Vector2(-gridWidth / 2f + bubbleSize / 2f, -gridHeight / 2f + bubbleSize / 2f);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                float offsetX = (y % 2 == 0) ? 0f : bubbleSize * 0.5f;
                Vector2 position = new Vector2(startPosition.x + x * bubbleSize + offsetX, startPosition.y + y * bubbleSize);

                GameObject bubbleObj = Instantiate(bubblePrefab, position, Quaternion.identity);
                bubbleObj.transform.parent = transform;

                grid[y, x] = bubble;
                bubble.SetGridPosition(x, y); // ���� ��ǥ ����
            }
        }
    }

    public Vector2 FindNearestEmptyGrid(Vector2 position)
    {
        Vector2 nearestGridPos = Vector2.zero;
        float minDistance = float.MaxValue;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] == null) // �� �������� Ȯ��
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
    private Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(x * bubbleSize, y * bubbleSize);
    }

    //������ ������ ��ġ�� ��ġ
    public void PlaceBubble(Bubble bubble, Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize);
        int y = Mathf.RoundToInt(position.y / bubbleSize);
        grid[y, x] = bubble;
    }

}