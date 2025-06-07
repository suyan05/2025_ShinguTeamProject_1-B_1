using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    public GameObject[] bubblePrefabs; //버블 프리팹 배열
    public Transform firePoint;        //버블 발사 위치
    public float bubbleSpeed = 10f;    //버블 발사 속도
    private int nextBubbleIndex;       //다음 발사될 버블 인덱스

    void Start()
    {
        GenerateNextBubble(); //첫 번째 버블 생성
    }

    void Update()
    {
        RotateTowardsMouse(); //마우스를 따라 발사대 회전
        if (Input.GetMouseButtonDown(0))
        {
            ShootBubble(); //버블 발사
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle); //발사대 회전 적용
    }

    private void ShootBubble()
    {
        //프리팹을 인스턴스화하여 버블 생성
        GameObject bubbleObj = Instantiate(bubblePrefabs[nextBubbleIndex], firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        //발사대의 회전 각도에 따라 발사 방향 설정
        float shootAngle = transform.rotation.eulerAngles.z;
        Vector2 shootDirection = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle));

        bubbleScript.SetDirection(shootDirection.normalized); //발사 방향 설정
        bubbleScript.SetSpeed(bubbleSpeed); //속도 적용
        GenerateNextBubble(); //다음 버블 설정
    }

    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(0, bubblePrefabs.Length); //랜덤한 프리팹 선택
    }
}