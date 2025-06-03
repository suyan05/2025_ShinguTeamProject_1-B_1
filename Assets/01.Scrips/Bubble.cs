using UnityEngine;

public class Bubble : MonoBehaviour
{
    private BubbleShooter bubbleShooter; // BubbleShooter 참조 추가
    private BubbleGrid bubbleGrid; // 격자 시스템 참조

    public float speed = 5f;
    private int gridX, gridY; // 격자 위치
    private Vector2 direction; // 버블 이동 방향
    private bool isPlaced = false; // 격자에 배치 여부 확인

    private int level; // 버블 등급
    public GameObject[] bubbleObjects; // 버블의 이미지 배열
    private Animator animator; // 애니메이션 관리

    private void Start()
    {
        animator = GetComponent<Animator>(); // 애니메이터 컴포넌트 가져오기
        gameObject.SetActive(true);
    }

    public void SetBubble(int level)
    {
        this.level = level; // 현재 버블 등급 설정

        // 모든 버블 이미지를 숨김
        foreach (GameObject obj in bubbleObjects)
        {
            obj.SetActive(false);
        }

        // 해당 등급의 이미지 활성화
        if (level >= 0 && level < bubbleObjects.Length)
        {
            bubbleObjects[level].SetActive(true);
        }
        else
        {
            Debug.LogError("잘못된 버블 레벨: " + level);
        }

        // 애니메이션 효과 적용 (부드러운 전환)
        if (animator != null)
        {
            animator.SetTrigger("ChangeBubble");
        }
    }

    public void SetGrid(BubbleGrid grid)
    {
        bubbleGrid = grid; //격자 시스템 참조 저장
    }


    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; //방향 설정
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed; //속도 설정
    }

    public void SetShooter(BubbleShooter shooter)
    {
        bubbleShooter = shooter;
    }

    private void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * speed; //y축 방향 이동 유지

        }

    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        isPlaced = true;
        bubbleShooter.EnableShooting(); // 버블이 격자에 배치되면 발사 가능 상태 변경
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) //벽과 충돌 감지
        {
            Vector2 normal = collision.contacts[0].normal; // 충돌 표면의 법선 벡터
            direction = Vector2.Reflect(direction, normal).normalized; // 입사각을 기준으로 반사각 계산

            GetComponent<Rigidbody2D>().velocity = direction * speed; // 새 반사 방향으로 이동
        }

        if (collision.gameObject.CompareTag("Bubble")|| collision.gameObject.CompareTag("Ground")) //바닦 또는 다른 버블 감지
        {
            if (!isPlaced)
            {
                isPlaced = true;
                direction = Vector2.zero;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                Debug.Log("격자 오류");
                //가장 가까운 빈 격자를 찾고 배치
                Vector2 nearestGridPosition = bubbleGrid.FindNearestEmptyGrid(transform.position);
                transform.position = nearestGridPosition;
                bubbleGrid.PlaceBubble(this, nearestGridPosition);
                Debug.Log("격자 오류 아님");
            }
        }
    }
}