using UnityEngine;
using UnityEngine.UI;

public class BubbleShooter : MonoBehaviour
{
    [Header("�߻� ����")]
    public float minRotation = -45f; // �ּ� ȸ�� ����
    public float maxRotation = 45f;  // �ִ� ȸ�� ����

    public GameObject bubblePrefab; // ������ ���� ������
    public Transform firePoint; // �߻� ��ġ
    public float bubbleSpeed = 10f; // ���� �ӵ�
    public Image nextBubbleImage; // UI���� ���� ������ ǥ���� �̹���
    public Sprite[] bubbleSprites; // ��� ������ ���� �̹��� �迭
    private int nextBubbleIndex; // ���� �߻�� ������ �ε���
    private bool canShoot = true; // �߻� ���� ����

    void Start()
    {
        GenerateNextBubble(); // ���� ���� �� ù ��° ���� ����
    }

    void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            ShootBubble();
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        //�߻�밡 �⺻������ �Ʒ��� �ٶ󺸵��� ����
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        //ȸ�� ������ �ּ� ~ �ִ� ������ ����
        angle = Mathf.Clamp(angle, minRotation, maxRotation);

        //�߻�� ȸ�� ����
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

    }

    private void ShootBubble()
    {
        canShoot = false; // ������ �߻��ϸ� �߰� �߻� ����

        GameObject bubbleObj = Instantiate(bubblePrefab, firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        //�߻�밡 �ٶ󺸴� �������� ��ȯ�� y�� ���� �߻� ���� ����
        float shootAngle = transform.rotation.eulerAngles.z;
        Vector2 shootDirection = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle));


        bubbleScript.SetDirection(shootDirection);
        bubbleScript.SetBubble(nextBubbleIndex);

        bubbleObj.SetActive(true);

        bubbleScript.SetShooter(this);

        GenerateNextBubble(); //���� ���� ����
    }

    // ���� ������ �̸� �����ϰ� UI���� ǥ��
    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(1, 5); // 1~4�� ���� �� ���� ����
        nextBubbleImage.sprite = bubbleSprites[nextBubbleIndex]; // UI���� �̹��� ������Ʈ
    }

    public void EnableShooting()
    {
        canShoot = true;
    }
}