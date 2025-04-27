using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] bubblePrefabs; // Bubble 프리팹 배열 (8가지 색상)
    public int[] activeBubbleIndices = { 0, 1, 2, 3 }; // 생성 가능한 색상 인덱스
    public Transform spawnPoint; // Bubble 발사 위치

    public Image nextBubblePreview; // 다음 Bubble을 표시할 UI 이미지
    private int nextBubbleIndex; // 다음에 생성될 Bubble 인덱스

    private float Timer = 1.5f;
    private bool isShoot = true;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // 초기 다음 Bubble 설정
        nextBubbleIndex = activeBubbleIndices[Random.Range(0, activeBubbleIndices.Length)];
        UpdateNextBubblePreview();
    }

    private void Update()
    {
        if (isShoot)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) // 마우스 Space 클릭 감지
            {
                isShoot = false;
                ShootBubble();
            }
        }
        else
        {
            Timers();
        }
    }

    private void ShootBubble()
    {
        // Bubble 생성
        GameObject bubble = Instantiate(bubblePrefabs[nextBubbleIndex], spawnPoint.position, Quaternion.identity);

        // 다음 Bubble 색상 설정
        nextBubbleIndex = activeBubbleIndices[Random.Range(0, activeBubbleIndices.Length)];
        UpdateNextBubblePreview();

        // 발사 방향 계산
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Vector2 direction = (mousePosition - spawnPoint.position).normalized;
    }

    private void UpdateNextBubblePreview()
    {
        // 다음 Bubble 미리보기 업데이트
        Sprite bubbleSprite = bubblePrefabs[nextBubbleIndex].GetComponent<SpriteRenderer>().sprite;
        nextBubblePreview.sprite = bubbleSprite;
    }

    private void Timers()
    {
        if (Timer > 0)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                Timer = 1.5f;
                isShoot = true;
            }
        }
    }
}
