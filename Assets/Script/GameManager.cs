using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] bubblePrefabs; // Bubble ������ �迭 (8���� ����)
    public int[] activeBubbleIndices = { 0, 1, 2, 3 }; // ���� ������ ���� �ε���
    public Transform spawnPoint; // Bubble �߻� ��ġ

    public Image nextBubblePreview; // ���� Bubble�� ǥ���� UI �̹���
    private int nextBubbleIndex; // ������ ������ Bubble �ε���

    private float Timer = 1.5f;
    private bool isShoot = true;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        // �ʱ� ���� Bubble ����
        nextBubbleIndex = activeBubbleIndices[Random.Range(0, activeBubbleIndices.Length)];
        UpdateNextBubblePreview();
    }

    private void Update()
    {
        if (isShoot)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) // ���콺 Space Ŭ�� ����
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
        // Bubble ����
        GameObject bubble = Instantiate(bubblePrefabs[nextBubbleIndex], spawnPoint.position, Quaternion.identity);

        // ���� Bubble ���� ����
        nextBubbleIndex = activeBubbleIndices[Random.Range(0, activeBubbleIndices.Length)];
        UpdateNextBubblePreview();

        // �߻� ���� ���
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        Vector2 direction = (mousePosition - spawnPoint.position).normalized;
    }

    private void UpdateNextBubblePreview()
    {
        // ���� Bubble �̸����� ������Ʈ
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
