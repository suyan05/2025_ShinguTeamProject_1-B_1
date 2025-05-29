using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    public GameObject bubblePrefab; // ������ ���� ������
    public Transform firePoint; // �߻� ��ġ
    public float bubbleSpeed = 10f; // ���� �ӵ�
    public Image nextBubbleImage; // UI���� ���� ������ ǥ���� �̹���
    public Sprite[] bubbleSprites; // ��� ������ ���� �̹��� �迭

    private int nextBubbleIndex; // ������ �߻�� ������ �ε���

    private void Start()
    {
        GenerateNextBubble(); // ���� ���� �� ���� ���� ����
    }

    private void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0))
        {
            ShootBubble(); // Ŭ���� �� ��� ���� �� �߻�
        }
    }

    // �߻�븦 ���콺 �������� ȸ��������, ���� ������ ���
    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle = Mathf.Clamp(angle - 90f, -30f, 30f); // ȸ�� ���� ����
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Ŭ���� �� ���ο� ������ ��� �����Ͽ� �߻�
    private void ShootBubble()
    {
        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity); //���� ��� ����
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;

        bubbleScript.SetDirection(shootDirection);
        bubbleScript.SetBubble(nextBubbleIndex); // �߻�� ���� �̹��� ����

        GenerateNextBubble(); // ���� ���� ����
    }

    // ���� ������ �̸� �����ϰ� UI���� ǥ��
    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(1, 5); // 1~4�� ���� �� ���� ����
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI���� �̹��� ������Ʈ
    }
}