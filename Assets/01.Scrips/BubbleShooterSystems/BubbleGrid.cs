using System.Collections.Generic;
using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10;
    public int cols = 6;
    public float bubbleSize = 1f;
    public float maxHeight = 7f; //게임 오버 높이 기준
    public bool isGameOver = false;


    private Bubble[,] grid;
    private GameManager gameManager;

    void Start()
    {
        grid = new Bubble[rows, cols]; //격자 배열 초기화
        gameManager = FindObjectOfType<GameManager>(); //게임 매니저 참조
    }

    // 버블이 바닥과 연결되어 있는지 확인
    private HashSet<Vector2Int> CheckConnectedBubbles()
    {
        HashSet<Vector2Int> connectedBubbles = new HashSet<Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // 첫 번째 행의 모든 버블을 시작점으로 추가
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

    // 주어진 위치의 이웃 좌표 반환
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

    // 연결이 끊긴 버블 아래로 이동
    public void RepositionDisconnectedBubbles()
    {
        HashSet<Vector2Int> connected = CheckConnectedBubbles();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (grid[y, x] != null && !connected.Contains(new Vector2Int(x, y)))
                {
                    grid[y, x] = null; // 연결이 끊긴 버블 제거

                    // 가장 아래로 이동
                    int newY = rows - 1;
                    while (newY > y && grid[newY, x] != null)
                        newY--;

                    if (newY > y)
                    {
                        grid[newY, x] = new Bubble(); // 빈 공간에 새로운 위치 지정
                    }
                }
            }
        }
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
        if (isGameOver) return;

        Vector2 nearestGridPos = FindNearestEmptyGrid(bubble.transform.position);
        bubble.transform.position = nearestGridPos;

        if (CheckGameOver()) // 높이 초과 여부 확인
        {
            isGameOver = true;
            gameManager.GameOver(); // 게임 오버 실행
        }
    }


    //게임 오버 체크 함수
    public bool CheckGameOver()
    {
        foreach (Bubble bubble in FindObjectsOfType<Bubble>()) // 모든 버블 검사
        {
            if (bubble.transform.position.y >= maxHeight) // 특정 높이 초과 여부 확인
                return true;
        }
        return false;
    }


}