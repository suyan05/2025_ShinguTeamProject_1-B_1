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
    private bool canShoot = true; // �߻� ���� ���θ� Ȯ���ϴ� �÷���

    void Start()
    {
        GenerateNextBubble();
    }

    void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0) && canShoot) // canShoot�� true�� ���� �߻� ����
        {
            ShootBubble();
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle = Mathf.Clamp(angle - 90f, -30f, 30f);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootBubble()
    {
        canShoot = false; // ������ �߻��ϸ� canShoot�� false�� �����Ͽ� �߰� �߻� ����

        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mousePos - firePoint.position).normalized;
        bubbleScript.SetDirection(shootDirection);
        bubbleScript.SetBubble(nextBubbleIndex);
        bubbleScript.SetShooter(this); // Bubble���� BubbleShooter ���� ����

        GenerateNextBubble();
    }

    // ���� ������ �̸� �����ϰ� UI���� ǥ��
    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(1, 5); // 1~4�� ���� �� ���� ����
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI���� �̹��� ������Ʈ
    }

    public void EnableShooting() // ������ ���ڿ� ��ġ�Ǹ� �߻� ���� ���� ����
    {
        canShoot = true;
    }
}