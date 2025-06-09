using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGravityBreak : MonoBehaviour
{
    private Vector3 velocity;
    private float gravity = -700f;
    private bool isBreaking = false;

    public float fallSpeedMin = -300f;
    public float fallSpeedMax = -400f;
    public float horizontalJitter = 30f;
    public float maxRotationSpeed = 30f;

    private Vector3 rotationVelocity;
    private RectTransform rectTransform;
    private Transform regularTransform;

    private void Awake()
    {
        // UI용 RectTransform 우선 시도
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            // 없으면 일반 Transform 사용
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

    void Update()
    {
        if (!isBreaking) return;

        velocity.y += gravity * Time.deltaTime;

        if (rectTransform != null)
        {
            rectTransform.localPosition += velocity * Time.deltaTime;
            rectTransform.Rotate(rotationVelocity * Time.deltaTime);
        }
        else
        {
            regularTransform.localPosition += velocity * Time.deltaTime;
            regularTransform.Rotate(rotationVelocity * Time.deltaTime);
        }
    }
}
