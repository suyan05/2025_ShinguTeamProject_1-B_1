using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGravityBreak : MonoBehaviour
{
    private Vector3 velocity;
    private float gravity = -40f;
    private bool isBreaking = false;

    public float fallSpeedMin = -1.5f;
    public float fallSpeedMax = -2.5f;
    public float horizontalJitter = 1.5f;
    public float maxRotationSpeed = 2f;

    private Vector3 rotationVelocity;
    private RectTransform rectTransform;
    private Transform regularTransform;

    private Canvas canvas;
    private float canvasScaleFactor = 1f;
    private float uiVelocityBoost = 200f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
                canvasScaleFactor = canvas.scaleFactor;
        }
        else
        {
            regularTransform = transform;
        }
    }

    // �ܺο��� �����ϴ� �Լ�
    public void BreakWithSortedDelay(float baseY, float delayPer100Y = 0.05f)
    {
        float posY = rectTransform != null ? rectTransform.position.y : regularTransform.position.y;
        float deltaY = baseY - posY; // Y�� �������� delay ŭ
        float delay = Mathf.Clamp(deltaY / 100f * delayPer100Y, 0f, 2f); // 100�ȼ��� 0.05��
        StartCoroutine(DelayedBreak(delay));
    }

    private IEnumerator DelayedBreak(float delay)
    {
        yield return new WaitForSeconds(delay);

        float xOffset = Random.Range(-horizontalJitter, horizontalJitter);
        float ySpeed = Random.Range(fallSpeedMax, fallSpeedMin);

        velocity = new Vector3(xOffset, ySpeed, 0f);
        rotationVelocity = new Vector3(0, 0, Random.Range(-maxRotationSpeed, maxRotationSpeed));
        isBreaking = true;
    }

    private void OnEnable()
    {
        // ���� �߰��� ������ ���� �۵��ϵ��� velocity �ʱ�ȭ ����
        if (isBreaking) return;
    }

    void Update()
    {
        if (!isBreaking) return;

        velocity.y += gravity * Time.deltaTime;

        if (rectTransform != null)
        {
            Vector3 uiVelocity = velocity * uiVelocityBoost * Time.deltaTime;
            rectTransform.localPosition += uiVelocity;
            rectTransform.Rotate(rotationVelocity * Time.deltaTime);
        }
        else
        {
            regularTransform.localPosition += velocity * Time.deltaTime;
            regularTransform.Rotate(rotationVelocity * Time.deltaTime);
        }
    }
}