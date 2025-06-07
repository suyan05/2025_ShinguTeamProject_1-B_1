using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    private Bubble[,] grid;

    void Start()
    {
        grid = new Bubble[rows, cols]; //격자 배열 초기화
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
                if (grid[y, x] == null) //빈 공간인지 확인
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

    private Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(x * bubbleSize, y * bubbleSize);
    }

    public void PlaceBubble(Bubble bubble, Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize);
        int y = Mathf.RoundToInt(position.y / bubbleSize);
        grid[y, x] = bubble; //버블을 격자에 배치
    }
}