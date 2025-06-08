using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    private Bubble[,] grid;

    void Start()
    {
        grid = new Bubble[rows, cols]; // ���� �迭 �ʱ�ȭ
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
                if (grid[y, x] == null) // **�� �������� Ȯ��**
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
        Vector2 nearestGridPos = FindNearestEmptyGrid(bubble.transform.position);
        bubble.transform.position = nearestGridPos; //���� ��ġ�� ��ġ
    }
}