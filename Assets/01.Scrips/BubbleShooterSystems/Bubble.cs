using DG.Tweening;
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
    public bool isConnectedToGround = false; //바닥과 연결 여부 확인용 변수

    [HideInInspector] public int placedOrder; // 배치 순서 (병합 기준)

    public GameObject mergeAnimationImage;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] levelSprites; // 레벨별 스프라이트 배열

    void Start()
    {
        animator = GetComponent<Animator>();
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

    public void PlayMergeAnimation()
    {
        mergeAnimationImage.SetActive(true); // 애니메이션 이미지 활성화
        spriteRenderer.DOFade(0.5f, 0.3f); // 불투명도 낮춤

        DOVirtual.DelayedCall(0.6f, () => EndMergeAnimation()); // 일정 시간 후 애니메이션 종료
    }

    private void EndMergeAnimation()
    {
        mergeAnimationImage.SetActive(false); // 애니메이션 이미지 비활성화
        spriteRenderer.DOFade(1f, 0.3f); // 불투명도 복구

        level++; //애니메이션이 끝난 후 레벨 증가!
        FindObjectOfType<BubbleGrid>().FinishMergeProcess(this);
    }


    public void PlayExplosionAnimation()
    {
        if (animator != null)
        {
            // 애니메이션 트리거를 사용하여 폭발 애니메이션 실행
            animator.SetTrigger("Explode");

            // 애니메이션 클립의 길이에 맞춰 오브젝트 제거 (여기서는 1초 후로 설정)
            Destroy(gameObject, 1f);
        }
        else
        {
            Debug.LogWarning("Animator 컴포넌트를 찾을 수 없습니다.");
            Destroy(gameObject, 1f);
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

    // 병합 후 레벨 증가 및 이미지 변경
    public void MergeBubble()
    {
        level++;
        RefreshVisual(); // 병합 후 새로운 이미지 적용
    }
}