using UnityEngine;

public class Bubble : MonoBehaviour
{
    private int gridX, gridY; // ���� ��ġ ����
    private Vector2 direction; // �̵� ����
    private bool isPlaced = false; // ���ڿ� ��ġ�Ǿ����� ����
    public Sprite[] bubbleSprites; // ���� �̹��� �迭
    private int level = 0; // ���� ������ ���

    // ������ ���(�̹���)�� �����ϴ� �Լ�
    public void SetBubble(int level)
    {
        this.level = level;
        GetComponent<SpriteRenderer>().sprite = bubbleSprites[level]; // ������ �̹��� ����
    }

    // ������ �̵� ������ �����ϴ� �Լ�
    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void Update()
    {
        if (!isPlaced) // ���ڿ� ��ġ���� �ʾҴٸ� �̵�
        {
            transform.position += (Vector3)direction * Time.deltaTime * 5f; // ���� �̵�

            // ���� �浹�ϸ� �ݻ�
            if (transform.position.x <= -4.5f || transform.position.x >= 4.5f)
            {
                direction.x = -direction.x; // ���� ����
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPlaced && other.CompareTag("Bubble")) // �ٸ� ����� �浹 ��
        {
            BubbleGrid bubbleGrid = FindObjectOfType<BubbleGrid>(); // ���� ���� �ý��� ã��
            bubbleGrid.PlaceBubble(gameObject, transform.position); // ���ڿ� ��ġ
            isPlaced = true; // ��ġ �Ϸ�
        }
    }

    // ���� ��ġ�� �����ϴ� �Լ�
    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
}