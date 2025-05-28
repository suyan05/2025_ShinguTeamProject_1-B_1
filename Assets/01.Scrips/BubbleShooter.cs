using UnityEngine;
using System.Collections.Generic;

public class BubbleShooter : MonoBehaviour
{
    public List<GameObject> bubblePool; // 미리 생성된 버블 오브젝트 리스트
    public Transform firePoint; // 발사 위치
    public float bubbleSpeed = 10f;

    private GameObject nextBubble; // 다음에 발사될 버블 오브젝트

    private void Start()
    {
        GenerateBubblePool();
        SelectNextBubble();
    }

    private void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0))
        {
            ShootBubble();
        }
    }

    //마우스 방향으로 발사대 회전
    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    //미리 생성된 버블들을 리스트에 저장
    private void GenerateBubblePool()
    {
        bubblePool = new List<GameObject>();
        for (int i = 0; i < 10; i++) // 10개의 버블 미리 생성
        {
            GameObject bubble = Instantiate(Resources.Load<GameObject>("BubblePrefab")); // 버블 프리팹 로드
            bubble.SetActive(false); // 비활성화하여 보이지 않도록 설정
            bubblePool.Add(bubble);
        }
    }

    //다음에 발사될 버블을 선택
    public void SelectNextBubble()
    {
        nextBubble = bubblePool[Random.Range(0, bubblePool.Count)];
    }

    //버블 발사 기능
    private void ShootBubble()
    {
        nextBubble.transform.position = firePoint.position;
        nextBubble.SetActive(true); // 활성화하여 보이도록 설정
        Bubble bubbleScript = nextBubble.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;

        bubbleScript.SetDirection(shootDirection); // 방향 설정
        SelectNextBubble(); // 다음 버블 준비
    }
}