using UnityEngine;
using System.Collections.Generic;

public class BubbleShooter : MonoBehaviour
{
    public List<GameObject> bubblePool; // �̸� ������ ���� ������Ʈ ����Ʈ
    public Transform firePoint; // �߻� ��ġ
    public float bubbleSpeed = 10f;

    private GameObject nextBubble; // ������ �߻�� ���� ������Ʈ

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

    //���콺 �������� �߻�� ȸ��
    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    //�̸� ������ ������� ����Ʈ�� ����
    private void GenerateBubblePool()
    {
        bubblePool = new List<GameObject>();
        for (int i = 0; i < 10; i++) // 10���� ���� �̸� ����
        {
            GameObject bubble = Instantiate(Resources.Load<GameObject>("BubblePrefab")); // ���� ������ �ε�
            bubble.SetActive(false); // ��Ȱ��ȭ�Ͽ� ������ �ʵ��� ����
            bubblePool.Add(bubble);
        }
    }

    //������ �߻�� ������ ����
    public void SelectNextBubble()
    {
        nextBubble = bubblePool[Random.Range(0, bubblePool.Count)];
    }

    //���� �߻� ���
    private void ShootBubble()
    {
        nextBubble.transform.position = firePoint.position;
        nextBubble.SetActive(true); // Ȱ��ȭ�Ͽ� ���̵��� ����
        Bubble bubbleScript = nextBubble.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;

        bubbleScript.SetDirection(shootDirection); // ���� ����
        SelectNextBubble(); // ���� ���� �غ�
    }
}