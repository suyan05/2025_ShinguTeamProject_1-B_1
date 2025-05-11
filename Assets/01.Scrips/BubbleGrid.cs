using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // 격자의 행 개수
    public int cols = 6; // 격자의 열 개수
    public float bubbleSize = 1f; // 각 버블의 크기
    public GameObject bubblePrefab; // 버블 프리팹
    private Bubble[,] grid; // 버블을 저장할 2D 배열

    void Start()
    {
        grid = new Bubble[rows, cols]; // 격자 공간 초기화
        InitializeGrid(); // 초기 버블 배치
    }

    // 격자에 버블을 생성하는 함수
    void InitializeGrid()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2 position = new Vector2(x * bubbleSize, -y * bubbleSize); // 버블 위치 설정
                GameObject bubbleObj = Instantiate(bubblePrefab, position, Quaternion.identity);
                bubbleObj.transform.parent = transform; // 부모 설정
                Bubble bubble = bubbleObj.GetComponent<Bubble>();
                grid[y, x] = bubble; // 격자 배열에 저장
                bubble.SetGridPosition(x, y); // 각 버블의 위치 설정
            }
        }
    }

    // 주어진 좌표에서 가장 가까운 격자 위치를 반환하는 함수
    public Vector2 GetClosestGridPosition(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize);
        int y = Mathf.RoundToInt(position.y / bubbleSize);
        return new Vector2(x * bubbleSize, y * bubbleSize); // 격자 좌표 반환
    }

    // 버블을 격자에 배치하는 함수
    public void PlaceBubble(GameObject bubbleObj, Vector2 position)
    {
        Vector2 gridPosition = GetClosestGridPosition(position);
        int x = Mathf.RoundToInt(gridPosition.x / bubbleSize);
        int y = Mathf.RoundToInt(gridPosition.y / bubbleSize);

        if (y >= 0 && y < rows && x >= 0 && x < cols && grid[y, x] == null) // 격자에 빈 공간이 있는지 확인
        {
            bubbleObj.transform.position = gridPosition; // 버블 위치 조정
            grid[y, x] = bubbleObj.GetComponent<Bubble>(); // 격자 배열에 저장
            grid[y, x].SetGridPosition(x, y); // 버블의 격자 위치 설정
        }
    }
}