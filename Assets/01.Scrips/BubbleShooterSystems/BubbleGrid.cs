using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    private Bubble[,] grid;

    void Start()
    {
        grid = new Bubble[rows, cols]; // 격자 배열 초기화
    }

    //가장 가까운 빈 격자 위치를 찾는 함수
    public Vector2 FindNearestEmptyGrid(Vector2 position)
    {
        Vector2 nearestGridPos = Vector2.zero;
        float minDistance = float.MaxValue;

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] == null) // **빈 공간인지 확인**
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

    //격자 좌표를 월드 좌표로 변환
    public Vector2 GetGridPosition(int x, int y)
    {
        float gridWidth = cols * bubbleSize;
        float gridHeight = rows * bubbleSize;
        Vector2 centerOffset = new Vector2(-gridWidth / 2f + bubbleSize / 2f, gridHeight / 2f - bubbleSize / 2f);

        // 홀수 행의 경우 x 위치를 절반씩 이동
        float xOffset = (y % 2 == 0) ? 0f : bubbleSize / 2f;

        return new Vector2(centerOffset.x + x * bubbleSize + xOffset, centerOffset.y - y * bubbleSize);
    }

    //버블을 지정된 위치에 배치
    public void PlaceBubble(Bubble bubble)
    {
        Vector2 nearestGridPos = FindNearestEmptyGrid(bubble.transform.position);
        bubble.transform.position = nearestGridPos; //격자 위치에 배치
    }
}