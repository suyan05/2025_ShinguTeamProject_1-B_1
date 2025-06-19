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
    public bool isConnectedToGround = false; //바닥과 연결 여부 확인용 변수

    [HideInInspector] public int placedOrder; // 배치 순서 (병합 기준)

    [Header("Animations")]
    public GameObject mergeAnimationImage;
    public GameObject explosionAnimationImage;
    public GameObject smallExplosionImage;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] levelSprites; // 레벨별 스프라이트 배열

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        bubbleGrid = FindObjectOfType<BubbleGrid>();
        bubbleShooter = FindObjectOfType<BubbleShooter>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        RefreshVisual(); // 시작 시 스프라이트 갱신

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
        Debug.Log("ExplosionAnimation 시전");

        if (explosionAnimationImage != null)
        {
            explosionAnimationImage.SetActive(true);
            spriteRenderer.DOFade(0.5f, 0.3f);

            DOVirtual.DelayedCall(1f, () =>
            {
                FindObjectOfType<BubbleShooter>().isMerging = false;
                onComplete?.Invoke(); //삭제는 여기서 호출되도록!
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
        Debug.Log("SmallAnimation 시전");

        if (smallExplosionImage != null)
        {
            smallExplosionImage.SetActive(true);
            spriteRenderer.DOFade(0.3f, 0.2f);

            DOVirtual.DelayedCall(0.8f, () =>
            {
                smallExplosionImage.SetActive(false);
                spriteRenderer.DOFade(1f, 0.2f);
                Destroy(gameObject); // 작게 터진 후 삭제
            });
        }
        else
        {
            Destroy(gameObject, 1f); // fallback 처리
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

        // 고정 처리: 바닥 또는 다른 버블과 접촉 시 그리드에 배치
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