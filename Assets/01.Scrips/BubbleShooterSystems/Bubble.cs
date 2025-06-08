using UnityEngine;

public class Bubble : MonoBehaviour
{
    private GameManager gameManager; //���� ���� ���� ����
    private BubbleGrid bubbleGrid;   //���� ���� �ý��� ����
    private BubbleShooter bubbleShooter; //���� ���� �߰�

    private Rigidbody2D rb;

    private Vector2 direction;       //���� �̵� ����
    private float speed = 5f;        //���� �̵� �ӵ�
    private bool isPlaced = false;   //���ڿ� ��ġ ����
    
    public int level;                //���� ���

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        bubbleGrid = FindObjectOfType<BubbleGrid>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; //���� ���� �� ����ȭ
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed; //�ӵ� ����
    }

    public void SetShooter(BubbleShooter shooter)
    {
        bubbleShooter = shooter; //���� ���� ����
    }

    private void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * speed; //�̵� ����
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            direction = Vector2.Reflect(direction, normal);
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

        if (!isPlaced && collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Bubble"))
        {
            isPlaced = true;
            Vector2 nearestGridPosition = bubbleGrid.FindNearestEmptyGrid(transform.position);
            transform.position = nearestGridPosition;
            bubbleGrid.PlaceBubble(this);

            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // ������ ������ ���� �ʵ��� ����

            bubbleShooter.EnableShooting();
        }
    }
}