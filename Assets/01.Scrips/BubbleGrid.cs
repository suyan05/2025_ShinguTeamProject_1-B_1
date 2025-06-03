using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // ������ �� ����
    public int cols = 6; // ������ �� ����
    public float bubbleSize = 1f; // �� ���� ũ��
    public GameObject bubblePrefab; // ���� ������
    private Bubble[,] grid; // ������ �����ϴ� ���� ����

    void Start()
    {
        grid = new Bubble[rows, cols];
        InitializeGrid(); // ���� �ʱ�ȭ
    }

    // ���ڿ� �ʱ� ���� ��ġ
    private void InitializeGrid()
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

                Bubble bubble = bubbleObj.GetComponent<Bubble>();
                grid[y, x] = bubble;
                bubble.SetGridPosition(x, y); // ���� ��ǥ ����
            }
        }
    }
}