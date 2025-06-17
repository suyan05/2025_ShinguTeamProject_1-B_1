using System.Collections;
using UnityEngine;
using DG.Tweening;

public class UIGravityBreak : MonoBehaviour
{
    [Header("������ ���� ����")]
    public float fallDistanceMin = 150f;       // �ּ� ���� �Ÿ�
    public float fallDistanceMax = 250f;       // �ִ� ���� �Ÿ�
    public float horizontalJitter = 50f;       // �¿� ��鸲 ����
    public float maxRotation = 90f;            // �ִ� ȸ�� ����
    public float fadeOutDuration = 1.8f;       // ���̵�ƿ� �ð�

    private RectTransform rectTransform;
    private Transform regularTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private SpriteRenderer spriteRenderer;
    private Material materialInstance;

    private void Awake()
    {
        // UI ������Ʈ��� RectTransform�� ���
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            canvas = GetComponentInParent<Canvas>();
        }
        else
        {
            // �Ϲ� ���� ������Ʈ ó��
            regularTransform = transform;
            spriteRenderer = GetComponent<SpriteRenderer>();

            if (spriteRenderer != null)
            {
                // ��Ƽ������ �����ؼ� ���� ���� �����ϰ� ����
                materialInstance = Instantiate(spriteRenderer.material);
                spriteRenderer.material = materialInstance;
            }
        }

        // UI���� ����Ǵ� CanvasGroup���� ���̵�ƿ� ó��
        canvasGroup = GetComponent<CanvasGroup>();
        if (rectTransform != null && canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // y ���� ��ġ�� ���� ������ ���� ����
    public void BreakWithSortedDelay(float baseY, float delayPer100Y = 0.05f)
    {
        float posY = rectTransform != null ? rectTransform.position.y : regularTransform.position.y;
        float deltaY = baseY - posY;
        float delay = Mathf.Clamp(deltaY / 100f * delayPer100Y, 0f, 2f);
        StartCoroutine(DelayedBreak(delay));
    }

    // ������ �� ���� ����
    private IEnumerator DelayedBreak(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayBreakTween();
    }

    // �������� ���� ����
    private void PlayBreakTween()
    {
        // UI�� GameObject�� ������ ���� ����
        float scale = rectTransform != null ? 0.4f : 0.05f;

        // �̵�/ȸ�� �� ���� ����
        float xOffset = Random.Range(-horizontalJitter, horizontalJitter) * scale;
        float yOffset = -Random.Range(fallDistanceMin, fallDistanceMax) * scale;
        float rotationZ = Random.Range(-maxRotation, maxRotation);
        Vector3 moveOffset = new Vector3(xOffset, yOffset, 0f);

        // �̵� �ð� ��� (�Ÿ� / �ӵ�)
        float desiredFallSpeed = 100f * scale;
        float distance = moveOffset.magnitude;
        float duration = distance / desiredFallSpeed;

        if (rectTransform != null)
        {
            // UI ������Ʈ �̵� + ȸ�� + ���̵�ƿ�
            rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(xOffset, yOffset), duration)
                         .SetEase(Ease.OutQuad);
            rectTransform.DORotate(new Vector3(0, 0, rotationZ), duration)
                         .SetEase(Ease.OutSine);
            canvasGroup.DOFade(0f, duration).SetEase(Ease.InQuad);
        }
        else
        {
            // �Ϲ� ���� ������Ʈ �̵� + ȸ��
            regularTransform.DOMove(regularTransform.position + moveOffset, duration)
                            .SetEase(Ease.OutQuad);
            regularTransform.DORotate(new Vector3(0, 0, rotationZ), duration)
                            .SetEase(Ease.OutSine);

            // ��Ƽ���� ������ ���ĸ� �ٿ� ���̵�ƿ�
            if (materialInstance != null && materialInstance.HasProperty("_Color"))
            {
                Color startColor = materialInstance.color;
                materialInstance.DOColor(new Color(startColor.r, startColor.g, startColor.b, 0f), "_Color", duration)
                                .SetEase(Ease.InQuad);
            }
        }

        // �ִϸ��̼� ���� �� ��Ȱ��ȭ
        StartCoroutine(DisableAfter(duration));
    }

    // ���� �ð� �� GameObject ��Ȱ��ȭ
    private IEnumerator DisableAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
