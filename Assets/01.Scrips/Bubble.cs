using UnityEngine;

public class Bubble : MonoBehaviour
{

    public GameObject[] bubbleObjects;

    private int gridX, gridY; // ���� ��ġ ����
    private int level = 0; // ���� ������ ���

    private Vector2 direction; // �̵� ����

    private bool isPlaced = false; // ���ڿ� ��ġ�Ǿ����� ����
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // ������ ���(�̹���)�� �����ϴ� �Լ�
    public void SetBubble(int level)
    {
        this.level = level;

        // ���� ���� ������Ʈ�� ����
        foreach (GameObject obj in bubbleObjects)
        {
            obj.SetActive(false); // ��� ���� ������Ʈ�� ��Ȱ��ȭ
        }

        bubbleObjects[level].SetActive(true); // ���� ����� ���� Ȱ��ȭ

    }

    // ������ �̵� ������ �����ϴ� �Լ�
    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * 5f;

            if (transform.position.x <= -4.5f || transform.position.x >= 4.5f)
            {
                direction.x = -direction.x;
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

        if (other.CompareTag("Bubble"))
        {
            Bubble otherBubble = other.GetComponent<Bubble>();

            if (otherBubble.level == this.level)
            {
                MergeBubble();
            }
        }
    }

    // ���� ��ġ�� �����ϴ� �Լ�
    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    private void MergeBubble()
    {
        animator.SetTrigger("Merge");
        Invoke(nameof(UpgradeBubble), 0.3f);

    }

    private void UpgradeBubble()
    {
        if (level < bubbleObjects.Length - 1)
        {
            level++;
            SetBubble(level); // ��� ���׷��̵� �� ���ο� ������Ʈ ǥ��
        }
        else
        {
            DestroyNearBubbles(); // �ְ� ����̸� �ֺ� ����
        }
    }

    private void DestroyNearBubbles()
    {
        Collider2D[] nearbyBubbles = Physics2D.OverlapCircleAll(transform.position, 2f);
        foreach (Collider2D bubble in nearbyBubbles)
        {
            if (bubble.CompareTag("Bubble"))
            {
                Destroy(bubble.gameObject);
                FindObjectOfType<GameManager>().BubbleRemoved(bubble.transform.position);
            }
        }

    }

}