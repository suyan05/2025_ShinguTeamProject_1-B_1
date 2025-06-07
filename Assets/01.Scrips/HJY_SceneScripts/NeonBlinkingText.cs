using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NeonBlinkingText : MonoBehaviour
{
    public Image image;
    public float fadeSpeed = 2f;               // �ε巯�� ���̵� �ӵ�
    public float flickerChance = 0.1f;         // �� ������ flicker �ߵ� Ȯ��
    public float flickerDuration = 0.05f;      // ��½�ϴ� �ð�

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
        // �����ϰ� ���ڱ� flicker �߻� (�׿� ����Ʀ ����)
        if (!isFlickering && Random.value < flickerChance * Time.deltaTime)
        {
            StartCoroutine(FlickerOnce());
        }

        // ���̵� ���� �� �ε巴�� ���󰡱�
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        Color c = originalColor;
        c.a = currentAlpha;
        image.color = c;

        // Ÿ�� ���İ� ���� ���������� ���� ��ȯ
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

        // ���ڱ� Ȯ ��ų� Ȯ �����ٰ� �ٷ� ����
        float originalTarget = targetAlpha;

        targetAlpha = Random.value > 0.5f ? 1f : 0.6f;
        yield return new WaitForSeconds(flickerDuration);

        targetAlpha = originalTarget;
        isFlickering = false;
    }
}
