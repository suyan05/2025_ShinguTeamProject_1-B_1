using UnityEngine;

public class Bubble : MonoBehaviour
{
    private GameManager gameManager; //게임 점수 관리 참조
    private BubbleGrid bubbleGrid;   //버블 격자 시스템 참조
    private BubbleShooter bubbleShooter; //슈터 참조 추가

    private Rigidbody2D rb;

    private Vector2 direction;       //버블 이동 방향
    private float speed = 5f;        //버블 이동 속도
    private bool isPlaced = false;   //격자에 배치 여부
    
    public int level;                //버블 등급

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        bubbleGrid = FindObjectOfType<BubbleGrid>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; //방향 설정 및 정규화
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed; //속도 설정
    }

    public void SetShooter(BubbleShooter shooter)
    {
        bubbleShooter = shooter; //슈터 참조 설정
    }

    private void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * speed; //이동 로직
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            direction = Vector2.Reflect(direction, normal);
            GetComponent<Rigidbody2D>().velocity = direction * speed; //속도 유지
        }
        else if (collision.gameObject.CompareTag("Bubble"))
        {
            Bubble otherBubble = collision.gameObject.GetComponent<Bubble>();

            if (otherBubble != null && otherBubble.level == level)
            {
                level++; // 같은 레벨일 경우 합치기
                gameManager.BubbleMerged(level); // 점수 증가
                Destroy(otherBubble.gameObject); // 합쳐진 버블 제거
            }
        }

        if (!isPlaced && collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Bubble"))
        {
            isPlaced = true;
            Vector2 nearestGridPosition = bubbleGrid.FindNearestEmptyGrid(transform.position);
            transform.position = nearestGridPosition;
            bubbleGrid.PlaceBubble(this);

            rb.velocity = Vector2.zero;
            rb.isKinematic = true; // 물리적 영향을 받지 않도록 설정

            bubbleShooter.EnableShooting();
        }
    }
}