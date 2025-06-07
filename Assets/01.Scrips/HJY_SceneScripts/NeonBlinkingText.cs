using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeonBlinkingText : MonoBehaviour
{
    public Image image;
    public float fadeSpeed = 2f;               // 부드러운 페이드 속도
    public float flickerChance = 0.1f;         // 매 프레임 flicker 발동 확률
    public float flickerDuration = 0.05f;      // 번쩍하는 시간

    private float targetAlpha = 1f;
    private float currentAlpha = 1f;
    private bool isFlickering = false;
    private Color originalColor;

    void Start()
    {
        originalColor = image.color;
    }

    void Update()
    {
        // 랜덤하게 갑자기 flicker 발생 (네온 전기튐 느낌)
        if (!isFlickering && Random.value < flickerChance * Time.deltaTime)
        {
            StartCoroutine(FlickerOnce());
        }

        // 페이드 알파 값 부드럽게 따라가기
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        Color c = originalColor;
        c.a = currentAlpha;
        image.color = c;

        // 타겟 알파가 끝에 도달했으면 방향 전환
        if (!isFlickering)
        {
            if (Mathf.Approximately(currentAlpha, targetAlpha))
            {
                targetAlpha = (targetAlpha == 1f) ? 0.6f : 1f;
            }
        }
    }

    private IEnumerator FlickerOnce()
    {
        isFlickering = true;

        // 갑자기 확 밝거나 확 꺼졌다가 바로 복귀
        float originalTarget = targetAlpha;

        targetAlpha = Random.value > 0.5f ? 1f : 0.6f;
        yield return new WaitForSeconds(flickerDuration);

        targetAlpha = originalTarget;
        isFlickering = false;
    }
}
