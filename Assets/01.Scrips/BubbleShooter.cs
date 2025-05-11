using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    public GameObject bubblePrefab; // �߻��� ���� ������
    public Transform firePoint; // �߻� ��ġ
    public float bubbleSpeed = 10f; // ���� �̵� �ӵ�
    public Image nextBubbleImage; // ���� ������ �̹����� ǥ���ϴ� UI
    public Sprite[] bubbleSprites; // ���� �̹��� �迭

    private int nextBubbleIndex; // ������ �߻�� ������ �̹��� �ε���

    void Start()
    {
        GenerateNextBubble(); // ���� ���� �� ���� ���� ����
    }

    void Update()
    {
        RotateTowardsMouse(); // ���콺�� ���� �߻�� ȸ��

        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ�� �� ���� �߻�
        {
            ShootBubble();
        }
    }

    // �߻�븦 ���콺 �������� ȸ���ϴ� �Լ�
    void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f); // �߻�� ȸ��
    }

    // ������ �߻��ϴ� �Լ�
    void ShootBubble()
    {
        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubble = bubbleObj.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;

        bubble.SetDirection(shootDirection); // ���� ����
        bubble.SetBubble(nextBubbleIndex); // ���� ���� �̹��� ����

        GenerateNextBubble(); // ���� ���� ����
    }

    // ���� ������ �̸� �����ϴ� �Լ�
    void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(0, 4); // ���� 4���� �̹����� ����
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI�� ǥ��
    }
}