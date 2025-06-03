using UnityEngine;

public class Bubble : MonoBehaviour
{
    private BubbleShooter bubbleShooter; // BubbleShooter ���� �߰�
    private BubbleGrid bubbleGrid; // ���� �ý��� ����

    public float speed = 5f;
    private int gridX, gridY; // ���� ��ġ
    private Vector2 direction; // ���� �̵� ����
    private bool isPlaced = false; // ���ڿ� ��ġ ���� Ȯ��

    private int level; // ���� ���
    public GameObject[] bubbleObjects; // ������ �̹��� �迭
    private Animator animator; // �ִϸ��̼� ����

    private void Start()
    {
        animator = GetComponent<Animator>(); // �ִϸ����� ������Ʈ ��������
        gameObject.SetActive(true);
    }

    public void SetBubble(int level)
    {
        this.level = level; // ���� ���� ��� ����

        // ��� ���� �̹����� ����
        foreach (GameObject obj in bubbleObjects)
        {
            obj.SetActive(false);
        }

        // �ش� ����� �̹��� Ȱ��ȭ
        if (level >= 0 && level < bubbleObjects.Length)
        {
            bubbleObjects[level].SetActive(true);
        }
        else
        {
            Debug.LogError("�߸��� ���� ����: " + level);
        }

        // �ִϸ��̼� ȿ�� ���� (�ε巯�� ��ȯ)
        if (animator != null)
        {
            animator.SetTrigger("ChangeBubble");
        }
    }

    public void SetGrid(BubbleGrid grid)
    {
        bubbleGrid = grid; //���� �ý��� ���� ����
    }


    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; //���� ����
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed; //�ӵ� ����
    }

    public void SetShooter(BubbleShooter shooter)
    {
        bubbleShooter = shooter;
    }

    private void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * speed; //y�� ���� �̵� ����

        }

    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        isPlaced = true;
        bubbleShooter.EnableShooting(); // ������ ���ڿ� ��ġ�Ǹ� �߻� ���� ���� ����
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) //���� �浹 ����
        {
            Vector2 normal = collision.contacts[0].normal; // �浹 ǥ���� ���� ����
            direction = Vector2.Reflect(direction, normal).normalized; // �Ի簢�� �������� �ݻ簢 ���

            GetComponent<Rigidbody2D>().velocity = direction * speed; // �� �ݻ� �������� �̵�
        }

        if (collision.gameObject.CompareTag("Bubble")|| collision.gameObject.CompareTag("Ground")) //�ٴ� �Ǵ� �ٸ� ���� ����
        {
            if (!isPlaced)
            {
                isPlaced = true;
                direction = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Debug.Log("���� ����");
                //���� ����� �� ���ڸ� ã�� ��ġ
                Vector2 nearestGridPosition = bubbleGrid.FindNearestEmptyGrid(transform.position);
                transform.position = nearestGridPosition;
                bubbleGrid.PlaceBubble(this, nearestGridPosition);
                Debug.Log("���� ���� �ƴ�");
            }
        }
    }
}