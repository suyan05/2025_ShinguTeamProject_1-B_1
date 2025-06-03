using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // 격자의 행 개수
    public int cols = 6; // 격자의 열 개수
    public float bubbleSize = 1f; // 각 버블 크기
    public GameObject bubblePrefab; // 버블 프리팹
    private Bubble[,] grid; // 버블을 저장하는 격자 공간

    void Start()
    {
        grid = new Bubble[rows, cols];
        InitializeGrid(); // 격자 초기화
    }

    // 격자에 초기 버블 배치
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
                bubble.SetGridPosition(x, y); // 격자 좌표 저장
            }
        }
    }
}