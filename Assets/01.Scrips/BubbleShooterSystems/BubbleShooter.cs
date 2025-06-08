using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    public GameObject[] bubblePrefabs;  //���� ������ �迭
    public Transform firePoint;         //���� �߻� ��ġ
    public float bubbleSpeed = 10f;     //���� �߻� �ӵ�
    private int nextBubbleIndex;        //���� �߻�� ���� �ε���
    private bool canShoot = true;       //�߻� ���� ����

    public float minAngle = -45f;       //�ּ� ȸ�� ���� (�ܺ� ���� ����)
    public float maxAngle = 45f;        //�ִ� ȸ�� ���� (�ܺ� ���� ����)

    void Start()
    {
        GenerateNextBubble(); //ù ��° ���� ����
    }

    void Update()
    {
        RotateTowardsMouse();
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            ShootBubble(); //�߻� ������ ���� ���� �߻�
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //ȸ�� ���� ���� ����
        angle = Mathf.Clamp(angle, minAngle, maxAngle);
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
    }

    private void ShootBubble()
    {
        canShoot = false; //�߻� �� �Ͻ������� �߰� �߻� ����

        GameObject bubbleObj = Instantiate(bubblePrefabs[nextBubbleIndex], firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        float shootAngle = transform.rotation.eulerAngles.z;
        Vector2 shootDirection = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle));

        bubbleScript.SetDirection(shootDirection.normalized);
        bubbleScript.SetSpeed(bubbleSpeed);
        bubbleScript.SetShooter(this); //���� ���� ���� ����

        GenerateNextBubble(); //���� ���� ����
    }

    public void EnableShooting()
    {
        canShoot = true; //���� ��ġ �� �ٽ� �߻� ����
    }

    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(0, bubblePrefabs.Length); //������ ������ ����
    }
}