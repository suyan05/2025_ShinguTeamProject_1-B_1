using UnityEngine;
using System.Collections.Generic;

public class BubbleShooter : MonoBehaviour
{
    public Transform firePoint; //버블 발사 위치

    public float bubbleSpeed = 10f; //버블 발사 속도
    public float minAngle = -150f, maxAngle = -20f; // 최소/최대 회전 각도

    //06.17 한재용 수정
    public SpriteRenderer nextBubbleVisual;

    public BubbleData[] bubbleDataList; // ScriptableObject 배열
    public int currentUnlockLevel = 1; // 현재 플레이어 해금 단계

    public Transform shoootedBubbleParent;

    private int nextBubbleIndex; //다음 발사될 버블 인덱스

    private bool canShoot = true; //발사 가능 여부

    void Start() => GenerateNextBubble();

    void Update()
    {
        RotateTowardsMouse();

        if (Input.GetMouseButtonDown(0) && canShoot)
            ShootBubble();
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    private void ShootBubble()
    {
        canShoot = false;

        GameObject bubbleObj = Instantiate(
            bubbleDataList[nextBubbleIndex].prefab,
            firePoint.position,
            Quaternion.identity,
            shoootedBubbleParent
        );

        Bubble b = bubbleObj.GetComponent<Bubble>();
        float angle = transform.rotation.eulerAngles.z;
        Vector2 shootDir = new Vector2(-Mathf.Sin(angle * Mathf.Deg2Rad), Mathf.Cos(angle * Mathf.Deg2Rad));

        b.SetDirection(shootDir);
        b.SetSpeed(bubbleSpeed);
        b.SetShooter(this);

        GenerateNextBubble();
    }

    public void EnableShooting() => canShoot = true;

    private void GenerateNextBubble()
    {
        float total = 0f;
        List<float> weights = new List<float>();

        for (int i = 0; i < bubbleDataList.Length; i++)
        {
            float chance = 0f;
            if (bubbleDataList[i].level <= currentUnlockLevel &&
                currentUnlockLevel - 1 < bubbleDataList[i].unlockChances.Length)
            {
                chance = bubbleDataList[i].unlockChances[currentUnlockLevel - 1];
            }
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