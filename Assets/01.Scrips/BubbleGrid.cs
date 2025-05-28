using UnityEngine;

public class BubbleGrid : MonoBehaviour
{
    public int rows = 10; // ������ ��(����) ����
    public int cols = 6; // ������ ��(����) ����
    public float bubbleSize = 1f; // �� ������ ũ�� (������ ����)
    public GameObject bubblePrefab; // ���� ������Ʈ ������
    private Bubble[,] grid; // ������ �����ϴ� 2D �迭 (���� ����)

    private void Start()
    {
        grid = new Bubble[rows, cols]; // �迭 ũ�� �ʱ�ȭ
        InitializeGrid(); // ���� ���� �� ���� �ʱ�ȭ
    }

    //���ڿ� ������ �ʱ� ��ġ�ϴ� �Լ�
    public void InitializeGrid()
    {
        for (int y = 0; y < rows; y++) //��(����) ��ȸ
        {
            for (int x = 0; x < cols; x++) // ��(����) ��ȸ
            {
                // Ȧ�� �࿡���� X ��ġ�� ���� �̵��Ͽ� ���� (������ ����)
                float offsetX = (y % 2 == 0) ? 0f : bubbleSize * 0.5f;

                // X, Y ��ġ�� ����Ͽ� ���� ���� ��ġ ����
                Vector2 position = new Vector2(x * bubbleSize + offsetX, -y * bubbleSize);

                // ���ο� ���� �������� ����
                GameObject bubbleObj = Instantiate(bubblePrefab, position, Quaternion.identity);
                bubbleObj.transform.parent = transform; //BubbleGrid ������Ʈ�� �θ�� ����

                // ������ ������ grid �迭�� ����
                Bubble bubble = bubbleObj.GetComponent<Bubble>();
                grid[y, x] = bubble;

                // ������ ���� ��ǥ ���� ����
                bubble.SetGridPosition(x, y);
            }
        }
    }

    //�־��� ��ǥ���� ���� ����� ���� ��ġ�� ã�� �Լ�
    public Vector2 GetClosestGridPosition(Vector2 position)
    {
        int x = Mathf.RoundToInt(position.x / bubbleSize); // X ��ġ ���
        int y = Mathf.RoundToInt(position.y / bubbleSize); // Y ��ġ ���

        // Ȧ�� ���� ��� X ������ �߰� (������ ���� ����)
        float offsetX = (y % 2 == 0) ? 0f : bubbleSize * 0.5f;

        return new Vector2(x * bubbleSize + offsetX, y * bubbleSize);
    }

    //���ο� ������ ���ڿ� ��ġ�ϴ� �Լ�
    public void PlaceBubble(GameObject bubbleObj, Vector2 position)
    {
        //���� ����� ���� ��ġ�� ã��
        Vector2 gridPosition = GetClosestGridPosition(position);
        int x = Mathf.RoundToInt(gridPosition.x / bubbleSize);
        int y = Mathf.RoundToInt(gridPosition.y / bubbleSize);

        //���� ������ �ùٸ� ��ġ���� Ȯ�� �� ��ġ
        if (y >= 0 && y < rows && x >= 0 && x < cols && grid[x, y] == null)
        {
            bubbleObj.transform.position = gridPosition; //��ġ ����
            grid[x, y] = bubbleObj.GetComponent<Bubble>(); //grid �迭�� ����
            grid[x, y].SetGridPosition(x, y); //������ ���� ��ǥ ������Ʈ
        }
    }
}