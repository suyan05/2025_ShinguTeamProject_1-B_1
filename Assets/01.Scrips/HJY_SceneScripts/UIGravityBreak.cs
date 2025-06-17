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

    // 외부에서 실행하는 함수
    public void BreakWithSortedDelay(float baseY, float delayPer100Y = 0.05f)
    {
        float posY = rectTransform != null ? rectTransform.position.y : regularTransform.position.y;
        float deltaY = baseY - posY; // Y가 낮을수록 delay 큼
        float delay = Mathf.Clamp(deltaY / 100f * delayPer100Y, 0f, 2f); // 100픽셀당 0.05초
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
        // 연출 중간에 켜졌을 때도 작동하도록 velocity 초기화 방지
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