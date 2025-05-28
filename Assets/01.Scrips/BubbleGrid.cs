using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // 격자의 행(세로) 개수
    public int cols = 6; // 격자의 열(가로) 개수
    public float bubbleSize = 1f; // 각 버블의 크기 (간격을 결정)
    public GameObject bubblePrefab; // 버블 오브젝트 프리팹
    private Bubble[,] grid; // 버블을 저장하는 2D 배열 (격자 공간)

    private void Start()
    {
        grid = new Bubble[rows, cols]; // 배열 크기 초기화
        InitializeGrid(); // 게임 시작 시 격자 초기화
    }

    //격자에 버블을 초기 배치하는 함수
    public void InitializeGrid()
    {
        for (int y = 0; y < rows; y++) //행(세로) 순회
        {
            for (int x = 0; x < cols; x++) // 열(가로) 순회
            {
                // 홀수 행에서는 X 위치를 조금 이동하여 정렬 (육각형 패턴)
                float offsetX = (y % 2 == 0) ? 0f : bubbleSize * 0.5f;

                // X, Y 위치를 계산하여 버블 생성 위치 설정
                Vector2 position = new Vector2(x * bubbleSize + offsetX, -y * bubbleSize);

                // 새로운 버블 프리팹을 생성
                GameObject bubbleObj = Instantiate(bubblePrefab, position, Quaternion.identity);
                bubbleObj.transform.parent = transform; //BubbleGrid 오브젝트를 부모로 설정

                // 생성된 버블을 grid 배열에 저장
                Bubble bubble = bubbleObj.GetComponent<Bubble>();
                grid[y, x] = bubble;

                // 버블의 격자 좌표 정보 저장
                bubble.SetGridPosition(x, y);
            }
        }
    }

    //주어진 좌표에서 가장 가까운 격자 위치를 찾는 함수
    public Vector2 GetClosestGridPosition(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize); // X 위치 계산
        int y = Mathf.RoundToInt(position.y / bubbleSize); // Y 위치 계산

        // 홀수 행일 경우 X 오프셋 추가 (육각형 정렬 유지)
        float offsetX = (y % 2 == 0) ? 0f : bubbleSize * 0.5f;

        return new Vector2(x * bubbleSize + offsetX, y * bubbleSize);
    }

    //새로운 버블을 격자에 배치하는 함수
    public void PlaceBubble(GameObject bubbleObj, Vector2 position)
    {
        //가장 가까운 격자 위치를 찾음
        Vector2 gridPosition = GetClosestGridPosition(position);
        int x = Mathf.RoundToInt(gridPosition.x / bubbleSize);
        int y = Mathf.RoundToInt(gridPosition.y / bubbleSize);

        //격자 내에서 올바른 위치인지 확인 후 배치
        if (y >= 0 && y < rows && x >= 0 && x < cols && grid[x, y] == null)
        {
            bubbleObj.transform.position = gridPosition; //위치 설정
            grid[x, y] = bubbleObj.GetComponent<Bubble>(); //grid 배열에 저장
            grid[x, y].SetGridPosition(x, y); //버블의 격자 좌표 업데이트
        }
    }
}