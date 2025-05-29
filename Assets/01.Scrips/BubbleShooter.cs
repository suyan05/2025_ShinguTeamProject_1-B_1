using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    public GameObject bubblePrefab; // 생성할 버블 프리팹
    public Transform firePoint; // 발사 위치
    public float bubbleSpeed = 10f; // 버블 속도
    public Image nextBubbleImage; // UI에서 다음 버블을 표시할 이미지
    public Sprite[] bubbleSprites; // 사용 가능한 버블 이미지 배열

    private int nextBubbleIndex; // 다음에 발사될 버블의 인덱스

    private void Start()
    {
        GenerateNextBubble(); // 게임 시작 시 다음 버블 설정
    }

    private void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0))
        {
            ShootBubble(); // 클릭할 때 즉시 생성 후 발사
        }
    }

    // 발사대를 마우스 방향으로 회전하지만, 일정 각도만 허용
    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle - 90f, -30f, 30f); // 회전 각도 제한
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // 클릭할 때 새로운 버블을 즉시 생성하여 발사
    private void ShootBubble()
    {
        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity); //버블 즉시 생성
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;

        bubbleScript.SetDirection(shootDirection);
        bubbleScript.SetBubble(nextBubbleIndex); // 발사될 버블 이미지 설정

        GenerateNextBubble(); // 다음 버블 설정
    }

    // 다음 버블을 미리 설정하고 UI에서 표시
    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(1, 5); // 1~4번 버블 중 랜덤 선택
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI에서 이미지 업데이트
    }
}