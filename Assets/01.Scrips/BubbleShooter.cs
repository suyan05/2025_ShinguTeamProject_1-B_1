using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    public GameObject bubblePrefab; // 발사할 버블 프리팹
    public Transform firePoint; // 발사 위치
    public float bubbleSpeed = 10f; // 버블 이동 속도
    public Image nextBubbleImage; // 다음 버블의 이미지를 표시하는 UI
    public Sprite[] bubbleSprites; // 버블 이미지 배열

    private int nextBubbleIndex; // 다음에 발사될 버블의 이미지 인덱스

    void Start()
    {
        GenerateNextBubble(); // 게임 시작 시 다음 버블 설정
    }

    void Update()
    {
        RotateTowardsMouse(); // 마우스를 향해 발사대 회전

        if (Input.GetMouseButtonDown(0)) // 마우스 클릭 시 버블 발사
        {
            ShootBubble();
        }
    }

    // 발사대를 마우스 방향으로 회전하는 함수
    void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // 발사대 회전
    }

    // 버블을 발사하는 함수
    void ShootBubble()
    {
        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubble = bubbleObj.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;

        bubble.SetDirection(shootDirection); // 방향 설정
        bubble.SetBubble(nextBubbleIndex); // 다음 버블 이미지 적용

        GenerateNextBubble(); // 다음 버블 설정
    }

    // 다음 버블을 미리 설정하는 함수
    void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(0, 4); // 하위 4개의 이미지를 선택
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI에 표시
    }
}