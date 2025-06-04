using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // 격자의 행 개수
    public int cols = 6; // 격자의 열 개수
    public float bubbleSize = 1f; // 각 버블 크기
    public GameObject bubblePrefab; // 버블 프리팹
    public Bubble bubble;
    public Bubble[,] grid; // 버블을 저장하는 격자 공간

    private void Start()
    {
        grid = new Bubble[rows, cols];
        bubble = GetComponent<Bubble>(); // Bubble 컴포넌트 가져오기
        InitializeGrid(); // 격자 초기화
    }

    // 격자에 초기 버블 배치
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
                bubble.SetGridPosition(x, y); // 격자 좌표 저장
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
                if (grid[y, x] == null) // 빈 공간인지 확인
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
    private Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(x * bubbleSize, y * bubbleSize);
    }

    //버블을 지정된 위치에 배치
    public void PlaceBubble(Bubble bubble, Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize);
        int y = Mathf.RoundToInt(position.y / bubbleSize);
        grid[y, x] = bubble;
    }

}