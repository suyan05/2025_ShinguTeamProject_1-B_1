using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIGravityBreak : MonoBehaviour
{
    [Header("무너짐 연출 설정")]
    public float fallDistanceMin = 150f;       // 최소 낙하 거리
    public float fallDistanceMax = 250f;       // 최대 낙하 거리
    public float horizontalJitter = 50f;       // 좌우 흔들림 정도
    public float maxRotation = 90f;            // 최대 회전 각도
    public float fadeOutDuration = 1.8f;       // 페이드아웃 시간

    private RectTransform rectTransform;
    private Transform regularTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private SpriteRenderer spriteRenderer;
    private Material materialInstance;

    private void Awake()
    {
        // UI 오브젝트라면 RectTransform을 사용
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
        else
        {
            // 일반 게임 오브젝트 처리
            regularTransform = transform;
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // 머티리얼을 복제해서 알파 변경 가능하게 설정
                materialInstance = Instantiate(spriteRenderer.material);
                spriteRenderer.material = materialInstance;
            }
        }

        // UI에만 적용되는 CanvasGroup으로 페이드아웃 처리
        canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform != null && canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // y 기준 위치에 따라 딜레이 차등 적용
    public void BreakWithSortedDelay(float baseY, float delayPer100Y = 0.05f)
    {
        float posY = rectTransform != null ? rectTransform.position.y : regularTransform.position.y;
        float deltaY = baseY - posY;
        float delay = Mathf.Clamp(deltaY / 100f * delayPer100Y, 0f, 2f);
        StartCoroutine(DelayedBreak(delay));
    }

    // 딜레이 후 연출 실행
    private IEnumerator DelayedBreak(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBreakTween();
    }

    // 무너지는 연출 실행
    private void PlayBreakTween()
    {
        // UI와 GameObject의 스케일 차이 보정
        float scale = rectTransform != null ? 0.4f : 0.05f;

        // 이동/회전 값 랜덤 생성
        float xOffset = Random.Range(-horizontalJitter, horizontalJitter) * scale;
        float yOffset = -Random.Range(fallDistanceMin, fallDistanceMax) * scale;
        float rotationZ = Random.Range(-maxRotation, maxRotation);
        Vector3 moveOffset = new Vector3(xOffset, yOffset, 0f);

        // 이동 시간 계산 (거리 / 속도)
        float desiredFallSpeed = 100f * scale;
        float distance = moveOffset.magnitude;
        float duration = distance / desiredFallSpeed;

        if (rectTransform != null)
        {
            // UI 오브젝트 이동 + 회전 + 페이드아웃
            rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(xOffset, yOffset), duration)
                         .SetEase(Ease.OutQuad);
            rectTransform.DORotate(new Vector3(0, 0, rotationZ), duration)
                         .SetEase(Ease.OutSine);
            canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
        }
        else
        {
            // 일반 게임 오브젝트 이동 + 회전
            regularTransform.DOMove(regularTransform.position + moveOffset, duration)
                            .SetEase(Ease.OutQuad);
            regularTransform.DORotate(new Vector3(0, 0, rotationZ), duration)
                            .SetEase(Ease.OutSine);

            // 머티리얼 색상의 알파를 줄여 페이드아웃
            if (materialInstance != null && materialInstance.HasProperty("_Color"))
            {
                Color startColor = materialInstance.color;
                materialInstance.DOColor(new Color(startColor.r, startColor.g, startColor.b, 0f), "_Color", duration)
                                .SetEase(Ease.InQuad);
            }
        }

        // 애니메이션 종료 후 비활성화
        StartCoroutine(DisableAfter(duration));
    }

    // 일정 시간 후 GameObject 비활성화
    private IEnumerator DisableAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
