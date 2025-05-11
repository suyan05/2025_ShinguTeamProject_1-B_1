using UnityEngine;

public class Bubble : MonoBehaviour
{
    private int gridX, gridY; // 격자 위치 저장
    private Vector2 direction; // 이동 방향
    private bool isPlaced = false; // 격자에 배치되었는지 여부
    public Sprite[] bubbleSprites; // 버블 이미지 배열
    private int level = 0; // 현재 버블의 등급

    // 버블의 등급(이미지)을 설정하는 함수
    public void SetBubble(int level)
    {
        this.level = level;
        GetComponent<SpriteRenderer>().sprite = bubbleSprites[level]; // 버블의 이미지 변경
    }

    // 버블의 이동 방향을 설정하는 함수
    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    void Update()
    {
        if (!isPlaced) // 격자에 배치되지 않았다면 이동
        {
            transform.position += (Vector3)direction * Time.deltaTime * 5f; // 버블 이동

            // 벽에 충돌하면 반사
            if (transform.position.x <= -4.5f || transform.position.x >= 4.5f)
            {
                direction.x = -direction.x; // 방향 반전
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
    }

    // 격자 위치를 설정하는 함수
    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
    }
}