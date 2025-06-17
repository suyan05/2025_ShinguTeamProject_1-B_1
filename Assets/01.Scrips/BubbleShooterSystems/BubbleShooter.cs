using UnityEngine;
using System.Collections.Generic;

public class BubbleShooter : MonoBehaviour
{

    public Transform firePoint;         //버블 발사 위치
    public float bubbleSpeed = 10f;     //버블 발사 속도
    private int nextBubbleIndex;        //다음 발사될 버블 인덱스
    private bool canShoot = true;       //발사 가능 여부

    public float minAngle = -150;       //최소 회전 각도
    public float maxAngle = -20;        //최대 회전 각도

    //06.17 한재용 수정
    public SpriteRenderer nextBubbleVisual;
    public BubbleData[] bubbleDataList; // ScriptableObject 배열
    public int currentUnlockLevel = 1; // 현재 플레이어 해금 단계

    public Transform shoootedBubbleParent; // 버블이 생성될 부모 오브젝트

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

        //발사한 버블 자식으로 넣기
        GameObject bubbleObj = Instantiate(bubbleDataList[nextBubbleIndex].prefab,firePoint.position,Quaternion.identity,shoootedBubbleParent);

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
        canShoot = true;
    }

    private void GenerateNextBubble()
    {
        float total = 0f;
        List<float> weights = new List<float>();

        for (int i = 0; i < bubbleDataList.Length; i++)
        {
            float chance = 0f;
            if (bubbleDataList[i].level <= currentUnlockLevel && currentUnlockLevel - 1 < bubbleDataList[i].unlockChances.Length)
                chance = bubbleDataList[i].unlockChances[currentUnlockLevel - 1];

            weights.Add(chance);
            total += chance;
        }

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        for (int i = 0; i < weights.Count; i++)
        {
            cumulative += weights[i];
            if (rand <= cumulative)
            {
                nextBubbleIndex = i;
                nextBubbleVisual.sprite = bubbleDataList[i].prefab.GetComponent<SpriteRenderer>().sprite;
                return;
            }
        }

        // fallback
        nextBubbleIndex = 0;
        nextBubbleVisual.sprite = bubbleDataList[0].prefab.GetComponent<SpriteRenderer>().sprite;
    }
}
