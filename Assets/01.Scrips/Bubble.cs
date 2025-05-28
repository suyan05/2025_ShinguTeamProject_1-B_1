using UnityEngine;

public class Bubble : MonoBehaviour
{

    public GameObject[] bubbleObjects;

    private int gridX, gridY; // 격자 위치 저장
    private int level = 0; // 현재 버블의 등급

    private Vector2 direction; // 이동 방향

    private bool isPlaced = false; // 격자에 배치되었는지 여부
    
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // 버블의 등급(이미지)을 설정하는 함수
    public void SetBubble(int level)
    {
        this.level = level;

        // 현재 버블 오브젝트를 변경
        foreach (GameObject obj in bubbleObjects)
        {
            obj.SetActive(false); // 모든 버블 오브젝트를 비활성화
        }

        bubbleObjects[level].SetActive(true); // 현재 등급의 버블 활성화

    }

    // 버블의 이동 방향을 설정하는 함수
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
        if (!isPlaced && other.CompareTag("Bubble")) // 다른 버블과 충돌 시
        {
            BubbleGrid bubbleGrid = FindObjectOfType<BubbleGrid>(); // 버블 격자 시스템 찾기
            bubbleGrid.PlaceBubble(gameObject, transform.position); // 격자에 배치
            isPlaced = true; // 배치 완료
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

    // 격자 위치를 설정하는 함수
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
            SetBubble(level); // 등급 업그레이드 시 새로운 오브젝트 표시
        }
        else
        {
            DestroyNearBubbles(); // 최고 등급이면 주변 제거
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