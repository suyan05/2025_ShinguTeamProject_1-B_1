using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private GameManager gameManager;
    private BubbleGrid bubbleGrid;
    private BubbleShooter bubbleShooter;
    private Rigidbody2D rb;

    private Vector2 direction;
    private float speed = 5f;
    private bool isPlaced = false;

    public int level;
    [HideInInspector] public int placedOrder; // ��ġ ���� (���� ����)

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] levelSprites; // ������ ��������Ʈ �迭

    void Start()
    {
        animator = GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        bubbleGrid = FindObjectOfType<BubbleGrid>();
        bubbleShooter = FindObjectOfType<BubbleShooter>();
        rb = GetComponent<Rigidbody2D>();

        RefreshVisual(); // ���� �� ��������Ʈ ����
    }

    void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)(direction * Time.deltaTime * speed);
        }
    }

    public void PlayMergeAnimation()
    {
        if (animator != null)
            animator.Play("Merge_Animation"); // ���� �ִϸ��̼� Ʈ����
    }

    public void PlayExplosionAnimation()
    {
        if (animator != null)
            animator.Play("Explosion_Animation");
    }

    public void SetDirection(Vector2 dir) => direction = dir.normalized;
    public void SetSpeed(float newSpeed) => speed = newSpeed;
    public void SetShooter(BubbleShooter shooter) => bubbleShooter = shooter;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            direction = Vector2.Reflect(direction, normal);
            rb.velocity = direction * speed;
        }

        // ���� ó��: �ٴ� �Ǵ� �ٸ� ����� ���� �� �׸��忡 ��ġ
        if (!isPlaced && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Bubble")))
        {
            isPlaced = true;

            Vector2 snapped = bubbleGrid.FindNearestEmptyGrid(transform.position);
            transform.position = snapped;
            bubbleGrid.PlaceBubble(this);

            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            bubbleShooter?.EnableShooting();
        }
    }

    public void RefreshVisual()
    {
        if (spriteRenderer != null && level - 1 >= 0 && level - 1 < levelSprites.Length)
        {
            spriteRenderer.sprite = levelSprites[level - 1];
        }
    }
}