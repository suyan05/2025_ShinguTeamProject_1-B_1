using UnityEngine;

public class Bubble : MonoBehaviour
{
    private GameManager gameManager; //���� ���� ���� ����
    private BubbleGrid bubbleGrid;   //���� ���� �ý��� ����
    private Vector2 direction;       //���� �̵� ����
    private float speed = 5f;        //���� �̵� �ӵ�
    private bool isPlaced = false;   //���ڿ� ��ġ ����
    public int level;                //���� ���

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        bubbleGrid = FindObjectOfType<BubbleGrid>();
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; //���� ���� �� ����ȭ
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed; //�ӵ� ����
    }

    void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * speed; //�̵� ����
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal; //�浹 ǥ���� ���� ����
            direction = Vector2.Reflect(direction, normal); //�Ի簢�� �������� �ݻ簢 ����
            GetComponent<Rigidbody2D>().velocity = direction * speed; //�ӵ� ����
        }
        else if (collision.gameObject.CompareTag("Bubble"))
        {
            Bubble otherBubble = collision.gameObject.GetComponent<Bubble>();

            if (otherBubble != null && otherBubble.level == level)
            {
                level++; // ���� ������ ��� ��ġ��
                gameManager.BubbleMerged(level); // ���� ����
                Destroy(otherBubble.gameObject); // ������ ���� ����
            }
        }

        if (!isPlaced && collision.gameObject.CompareTag("Ground"))
        {
            isPlaced = true; //���ڿ� ��ġ��

            //���� ����� �� ���� ��ġ ã��
            Vector2 nearestGridPosition = bubbleGrid.FindNearestEmptyGrid(transform.position);
            transform.position = nearestGridPosition;
            bubbleGrid.PlaceBubble(this, nearestGridPosition);

            //�̵� ���� (�ӵ� 0, ���� ����)
            direction = Vector2.zero;
            speed = 0f;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero; //���������ε� ���ߵ��� ����

            gameManager.BubbleRemoved(transform.position);
        }
    }
}