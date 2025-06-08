using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndGravityManager : MonoBehaviour
{
    // UI 루트(Canvas)
    public Transform rootCanvas;

    // 발사한 오브젝트들 루트 (예: 발사체 부모)
    public Transform firedObjectsParent;

    // 캐릭터 루트 (캐릭터가 여러 명이면 부모로, 한 명이면 직접)
    public Transform characterRoot;

    void Start()
    {
        // UI, 발사체, 캐릭터 모두에 UIGravityBreak 붙임
        AddGravityBreakToChildren(rootCanvas);
        AddGravityBreakToChildren(firedObjectsParent);
        AddGravityBreakToChildren(characterRoot);
    }

    void AddGravityBreakToChildren(Transform parent)
    {
        if (parent == null) return;  // null 체크

        foreach (Transform child in parent)
        {
            if (child.GetComponent<UIGravityBreak>() == null)
            {
                child.gameObject.AddComponent<UIGravityBreak>();
            }

            // 재귀적으로 하위도 검사
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
                // RectTransform 없으면 일반 Transform 사용
                Transform t = b.transform;
                if (t.position.y > maxY)
                    maxY = t.position.y;
            }
        }
        return maxY;
    }
}
