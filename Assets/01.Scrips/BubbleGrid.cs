using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // ������ �� ����
    public int cols = 6; // ������ �� ����
    public float bubbleSize = 1f; // �� ������ ũ��
    public GameObject bubblePrefab; // ���� ������
    private Bubble[,] grid; // ������ ������ 2D �迭

    void Start()
    {
        grid = new Bubble[rows, cols]; // ���� ���� �ʱ�ȭ
        InitializeGrid(); // �ʱ� ���� ��ġ
    }

    // ���ڿ� ������ �����ϴ� �Լ�
    void InitializeGrid()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                Vector2 position = new Vector2(x * bubbleSize, -y * bubbleSize); // ���� ��ġ ����
                GameObject bubbleObj = Instantiate(bubblePrefab, position, Quaternion.identity);
                bubbleObj.transform.parent = transform; // �θ� ����
                Bubble bubble = bubbleObj.GetComponent<Bubble>();
                grid[y, x] = bubble; // ���� �迭�� ����
                bubble.SetGridPosition(x, y); // �� ������ ��ġ ����
            }
        }
    }

    // �־��� ��ǥ���� ���� ����� ���� ��ġ�� ��ȯ�ϴ� �Լ�
    public Vector2 GetClosestGridPosition(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize);
        int y = Mathf.RoundToInt(position.y / bubbleSize);
        return new Vector2(x * bubbleSize, y * bubbleSize); // ���� ��ǥ ��ȯ
    }

    // ������ ���ڿ� ��ġ�ϴ� �Լ�
    public void PlaceBubble(GameObject bubbleObj, Vector2 position)
    {
        Vector2 gridPosition = GetClosestGridPosition(position);
        int x = Mathf.RoundToInt(gridPosition.x / bubbleSize);
        int y = Mathf.RoundToInt(gridPosition.y / bubbleSize);

        if (y >= 0 && y < rows && x >= 0 && x < cols && grid[y, x] == null) // ���ڿ� �� ������ �ִ��� Ȯ��
        {
            bubbleObj.transform.position = gridPosition; // ���� ��ġ ����
            grid[y, x] = bubbleObj.GetComponent<Bubble>(); // ���� �迭�� ����
            grid[y, x].SetGridPosition(x, y); // ������ ���� ��ġ ����
        }
    }
}