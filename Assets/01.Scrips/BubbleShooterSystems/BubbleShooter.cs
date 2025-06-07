using UnityEngine;

public class BubbleShooter : MonoBehaviour
{
    public GameObject[] bubblePrefabs; //���� ������ �迭
    public Transform firePoint;        //���� �߻� ��ġ
    public float bubbleSpeed = 10f;    //���� �߻� �ӵ�
    private int nextBubbleIndex;       //���� �߻�� ���� �ε���

    void Start()
    {
        GenerateNextBubble(); //ù ��° ���� ����
    }

    void Update()
    {
        RotateTowardsMouse(); //���콺�� ���� �߻�� ȸ��
        if (Input.GetMouseButtonDown(0))
        {
            ShootBubble(); //���� �߻�
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - transform.position).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle); //�߻�� ȸ�� ����
    }

    private void ShootBubble()
    {
        //�������� �ν��Ͻ�ȭ�Ͽ� ���� ����
        GameObject bubbleObj = Instantiate(bubblePrefabs[nextBubbleIndex], firePoint.position, Quaternion.identity);
        Bubble bubbleScript = bubbleObj.GetComponent<Bubble>();

        //�߻���� ȸ�� ������ ���� �߻� ���� ����
        float shootAngle = transform.rotation.eulerAngles.z;
        Vector2 shootDirection = new Vector2(-Mathf.Sin(Mathf.Deg2Rad * shootAngle), Mathf.Cos(Mathf.Deg2Rad * shootAngle));

        bubbleScript.SetDirection(shootDirection.normalized); //�߻� ���� ����
        bubbleScript.SetSpeed(bubbleSpeed); //�ӵ� ����
        GenerateNextBubble(); //���� ���� ����
    }

    private void GenerateNextBubble()
    {
        nextBubbleIndex = Random.Range(0, bubblePrefabs.Length); //������ ������ ����
    }
}