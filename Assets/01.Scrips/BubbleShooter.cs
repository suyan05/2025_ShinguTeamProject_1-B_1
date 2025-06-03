using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    [Header("발사 각도")]
    public float minRotation = -45f; // 최소 회전 각도
    public float maxRotation = 45f;  // 최대 회전 각도

    public GameObject bubblePrefab; // 생성할 버블 프리팹
    public Transform firePoint; // 발사 위치
    public float bubbleSpeed = 10f; // 버블 속도
    public Image nextBubbleImage; // UI에서 다음 버블을 표시할 이미지
    public Sprite[] bubbleSprites; // 사용 가능한 버블 이미지 배열
    private int nextBubbleIndex; // 다음 발사될 버블의 인덱스
    private bool canShoot = true; // 발사 가능 여부

    void Start()
    {
        GenerateNextBubble(); // 게임 시작 시 첫 번째 버블 설정
    }

    void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            ShootBubble();
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        //발사대가 기본적으로 아래를 바라보도록 설정
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        //회전 각도를 최소 ~ 최대 범위로 제한
        angle = Mathf.Clamp(angle, minRotation, maxRotation);

        //발사대 회전 적용
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }

    private void ShootBubble()
    {
        canShoot = false; // 버블을 발사하면 추가 발사 금지

        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        //발사대가 바라보는 방향으로 변환된 y축 기준 발사 방향 설정
        float shootAngle = transform.rotation.eulerAngles.z;
        Vector2 shootDirection = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle));


        bubbleScript.SetDirection(shootDirection);
        bubbleScript.SetBubble(nextBubbleIndex);

        bubbleObj.SetActive(true);

        bubbleScript.SetShooter(this);

        GenerateNextBubble(); //다음 버블 설정
    }

    // 다음 버블을 미리 생성하고 UI에서 표시
    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(1, 5); // 1~4번 버블 중 랜덤 선택
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI에서 이미지 업데이트
    }

    public void EnableShooting()
    {
        canShoot = true;
    }
}