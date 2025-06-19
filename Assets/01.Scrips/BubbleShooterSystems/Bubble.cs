using DG.Tweening;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private GameManager gameManager;
    private BubbleGrid bubbleGrid;
    private BubbleShooter bubbleShooter;
    private Rigidbody2D rb;

    private Vector2 direction;
    private float speed = 5f;
    private bool isPlaced = false;

    public int level;
    public bool isConnectedToGround = false; //�ٴڰ� ���� ���� Ȯ�ο� ����

    [HideInInspector] public int placedOrder; // ��ġ ���� (���� ����)

    [Header("Animations")]
    public GameObject mergeAnimationImage;
    public GameObject explosionAnimationImage;
    public GameObject smallExplosionImage;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] levelSprites; // ������ ��������Ʈ �迭

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        bubbleGrid = FindObjectOfType<BubbleGrid>();
        bubbleShooter = FindObjectOfType<BubbleShooter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        RefreshVisual(); // ���� �� ��������Ʈ ����

        placedOrder = Time.frameCount;
    }

    void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)(direction * Time.deltaTime * speed);
        }
    }

    public void PlayMergeAnimation(bool shouldLevelUp = true)
    {
        mergeAnimationImage.SetActive(true);
        spriteRenderer.DOFade(0.5f, 0.2f);

        DOVirtual.DelayedCall(0.6f, () =>
        {
            mergeAnimationImage.SetActive(false);
            spriteRenderer.DOFade(1f, 0.2f);

            if (shouldLevelUp)
                level++;

            BubbleGrid.Instance.FinishMergeProcess(this);
        });
    }

    public void PlayExplosionAnimation(System.Action onComplete = null)
    {
        Debug.Log("ExplosionAnimation ����");

        if (explosionAnimationImage != null)
        {
            explosionAnimationImage.SetActive(true);
            spriteRenderer.DOFade(0.5f, 0.3f);

            DOVirtual.DelayedCall(1f, () =>
            {
                FindObjectOfType<BubbleShooter>().isMerging = false;
                onComplete?.Invoke(); //������ ���⼭ ȣ��ǵ���!
                Destroy(gameObject);
            });
        }
        else
        {
            Destroy(gameObject, 1f); // fallback
        }
    }

    public void PlaySmallExplosion()
    {
        Debug.Log("SmallAnimation ����");

        if (smallExplosionImage != null)
        {
            smallExplosionImage.SetActive(true);
            spriteRenderer.DOFade(0.3f, 0.2f);

            DOVirtual.DelayedCall(0.8f, () =>
            {
                smallExplosionImage.SetActive(false);
                spriteRenderer.DOFade(1f, 0.2f);
                Destroy(gameObject); // �۰� ���� �� ����
            });
        }
        else
        {
            Destroy(gameObject, 1f); // fallback ó��
        }
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