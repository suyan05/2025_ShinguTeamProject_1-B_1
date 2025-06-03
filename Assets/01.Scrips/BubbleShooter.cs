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
    private bool canShoot = true; // 발사 가능 여부를 확인하는 플래그

    void Start()
    {
        GenerateNextBubble();
    }

    void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0) && canShoot) // canShoot이 true일 때만 발사 가능
        {
            ShootBubble();
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle - 90f, -30f, 30f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootBubble()
    {
        canShoot = false; // 버블을 발사하면 canShoot을 false로 변경하여 추가 발사 금지

        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;
        bubbleScript.SetDirection(shootDirection);
        bubbleScript.SetBubble(nextBubbleIndex);
        bubbleScript.SetShooter(this); // Bubble에서 BubbleShooter 참조 저장

        GenerateNextBubble();
    }

    // 다음 버블을 미리 설정하고 UI에서 표시
    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(1, 5); // 1~4번 버블 중 랜덤 선택
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI에서 이미지 업데이트
    }

    public void EnableShooting() // 버블이 격자에 배치되면 발사 가능 상태 변경
    {
        canShoot = true;
    }
}