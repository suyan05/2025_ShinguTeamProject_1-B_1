using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    public GameObject[] bubblePrefabs;  //버블 프리팹 배열
    public Transform firePoint;         //버블 발사 위치
    public float bubbleSpeed = 10f;     //버블 발사 속도
    private int nextBubbleIndex;        //다음 발사될 버블 인덱스
    private bool canShoot = true;       //발사 가능 여부

    public float minAngle = -45f;       //최소 회전 각도 (외부 수정 가능)
    public float maxAngle = 45f;        //최대 회전 각도 (외부 수정 가능)

    void Start()
    {
        GenerateNextBubble(); //첫 번째 버블 생성
    }

    void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            ShootBubble(); //발사 가능할 때만 버블 발사
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //회전 각도 제한 적용
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    private void ShootBubble()
    {
        canShoot = false; //발사 후 일시적으로 추가 발사 금지

        GameObject bubbleObj = Instantiate(bubblePrefabs[nextBubbleIndex], firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        float shootAngle = transform.rotation.eulerAngles.z;
        Vector2 shootDirection = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle));

        bubbleScript.SetDirection(shootDirection.normalized);
        bubbleScript.SetSpeed(bubbleSpeed);
        bubbleScript.SetShooter(this); //버블에 슈터 참조 전달

        GenerateNextBubble(); //다음 버블 설정
    }

    public void EnableShooting()
    {
        canShoot = true; //격자 배치 후 다시 발사 가능
    }

    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(0, bubblePrefabs.Length); //랜덤한 프리팹 선택
    }
}