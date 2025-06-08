using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndGravityManager : MonoBehaviour
{
    public CanvasGroup gameOverPanelGroup; // Panel_GameOver�� �ִ� CanvasGroup�� �����ϼ���.
    public float fadeDuration = 1.0f;       // ���̵� �ο� �ɸ��� �ð� (��)

    // UI ��Ʈ(Canvas)
    public Transform rootCanvas;

    // �߻��� ������Ʈ�� ��Ʈ (��: �߻�ü �θ�)
    public Transform firedObjectsParent;

    // ĳ���� ��Ʈ (ĳ���Ͱ� ���� ���̸� �θ��, �� ���̸� ����)
    public Transform characterRoot;

    void Start()
    {
        // UI, �߻�ü, ĳ���� ��ο� UIGravityBreak ����
        AddGravityBreakToChildren(rootCanvas);
        AddGravityBreakToChildren(firedObjectsParent);
        AddGravityBreakToChildren(characterRoot);
    }

    void AddGravityBreakToChildren(Transform parent)
    {
        if (parent == null) return;  // null üũ

        foreach (Transform child in parent)
        {
            if (child.GetComponent<UIGravityBreak>() == null)
            {
                child.gameObject.AddComponent<UIGravityBreak>();
            }

            // ��������� ������ �˻�
            AddGravityBreakToChildren(child);
        }
    }

    public void TriggerSortedBreak()
    {
        List<UIGravityBreak> breakers = new List<UIGravityBreak>();

        if (rootCanvas != null)
            breakers.AddRange(rootCanvas.GetComponentsInChildren<UIGravityBreak>());

        if (firedObjectsParent != null)
            breakers.AddRange(firedObjectsParent.GetComponentsInChildren<UIGravityBreak>());

        if (characterRoot != null)
            breakers.AddRange(characterRoot.GetComponentsInChildren<UIGravityBreak>());

        if (breakers.Count == 0) return;

        float baseY = GetTopMostY(breakers.ToArray());

        foreach (var breaker in breakers)
        {
            breaker.BreakWithSortedDelay(baseY);
        }

        // �г� ���̵� �� ����
        StartCoroutine(FadeInGameOverPanel());
    }

    private float GetTopMostY(UIGravityBreak[] breakers)
    {
        float maxY = float.MinValue;
        foreach (var b in breakers)
        {
            RectTransform rt = b.GetComponent<RectTransform>();
            if (rt != null)
            {
                if (rt.position.y > maxY)
                    maxY = rt.position.y;
            }
            else
            {
                // RectTransform ������ �Ϲ� Transform ���
                Transform t = b.transform;
                if (t.position.y > maxY)
                    maxY = t.position.y;
            }
        }
        return maxY;
    }

    private IEnumerator FadeInGameOverPanel()
    {
        if (gameOverPanelGroup == null) yield break;

        gameOverPanelGroup.alpha = 0f;
        gameOverPanelGroup.gameObject.SetActive(true); // �г��� ��Ȱ��ȭ�Ǿ� �ִٸ� Ȱ��ȭ

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            gameOverPanelGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }

        gameOverPanelGroup.alpha = 1f; // ������ ���̰� ����
    }
}
