using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

// TriggerGameOverSequence() 호출 해야함!!!!

public class GameEndGravityManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public CanvasGroup gameOverPanelGroup; // Panel_GameOver에 있는 CanvasGroup 연결
    public float fadeDuration = 1.0f;       // 게임오버 패널 페이드 인 시간(초)

    [Header("References")]
    public Transform rootCanvas;            // UI 루트(Canvas)
    public Transform firedObjectsParent;    // 발사 오브젝트 루트
    public Transform characterRoot;         // 캐릭터 루트

    public GameManager gameManager;         // 게임매니저 참조 (인스펙터 연결 또는 자동 할당)

    [Header("Score UI Elements")]
    public TextMeshProUGUI resultScoreText;    // Result 점수 표시용 텍스트
    public TextMeshProUGUI bestScoreText;      // Best 점수 표시용 텍스트

    public CanvasGroup flashPanelGroup;  // 화면 번쩍임용 패널 (흰색 Image + CanvasGroup)
    public float flashDuration = 0.08f;   // 한 번 번쩍이는 시간
    public float flashDelay = 0.1f;      // 번쩍 사이 간격

    private BubbleShooter bubbleShooter;
    private void Awake()
    {
        // GameManager가 인스펙터에 없으면 씬에서 찾아 자동 할당
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    private void Start()
    {
        // UI, 발사체, 캐릭터 모두에 UIGravityBreak 컴포넌트 붙임
        AddGravityBreakToChildren(rootCanvas);
        AddGravityBreakToChildren(firedObjectsParent);
        AddGravityBreakToChildren(characterRoot);
        bubbleShooter = FindObjectOfType<BubbleShooter>();
    }

    // 특정 부모 객체부터 재귀적으로 자식에게 UIGravityBreak 컴포넌트 붙임
    void AddGravityBreakToChildren(Transform parent)
    {
        if (parent == null) return;

        foreach (Transform child in parent)
        {
            if (child.GetComponent<UIGravityBreak>() == null)
            {
                child.gameObject.AddComponent<UIGravityBreak>();
            }
            AddGravityBreakToChildren(child);
        }
    }

    // 게임 오버 상황 시 호출: 오브젝트 무너짐 + 게임오버 패널 페이드인 + 점수 애니메이션
    public void TriggerSortedBreak()
    {
        SoundManager.Instance.PlayGameOver();
        List<UIGravityBreak> breakers = new List<UIGravityBreak>();

        if (rootCanvas != null)
            breakers.AddRange(rootCanvas.GetComponentsInChildren<UIGravityBreak>(true));

        if (firedObjectsParent != null)
            breakers.AddRange(firedObjectsParent.GetComponentsInChildren<UIGravityBreak>(true));

        if (characterRoot != null)
            breakers.AddRange(characterRoot.GetComponentsInChildren<UIGravityBreak>(true));

        if (breakers.Count == 0) return;

        float baseY = GetTopMostY(breakers.ToArray());

        // 무너짐 연출 시작
        foreach (var breaker in breakers)
        {
            breaker.enabled = true;
            breaker.BreakWithSortedDelay(baseY);
        }

        // 게임오버 패널 페이드인 및 점수 애니메이션 실행
        StartCoroutine(FadeInGameOverPanelAndAnimateScore());
    }

    // 최상단 Y 좌표 계산
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
                Transform t = b.transform;
                if (t.position.y > maxY)
                    maxY = t.position.y;
            }
        }
        return maxY;
    }

    // 게임오버 패널 페이드인 + 점수 애니메이션 동시 처리 코루틴
    private IEnumerator FadeInGameOverPanelAndAnimateScore()
    {
        if (gameOverPanelGroup == null)
            yield break;

        // 페이드 인 초기화
        gameOverPanelGroup.alpha = 0f;
        gameOverPanelGroup.gameObject.SetActive(true);

        // DOTween으로 페이드 인
        Tween fadeTween = gameOverPanelGroup.DOFade(1f, fadeDuration)
            .SetEase(Ease.InOutQuad);

        // 점수 애니메이션 시작 — 점수 올리는 연출
        // 실제 게임매니저 점수 받아서 호출
        AnimateScore(gameManager.Score, gameManager.HighScore);

        // 페이드 인 완료 대기
        yield return fadeTween.WaitForCompletion();
    }

    // 점수 애니메이션: 점수가 0에서부터 목표값까지 증가하는 효과
    private void AnimateScore(int finalScore, int bestScore)
    {
        if (resultScoreText == null || bestScoreText == null)
        {
            return;
        }

        // 0부터 finalScore까지 1초 동안 Tween으로 애니메이션
        DOTween.To(() => 0, x =>
        {
            resultScoreText.text = x.ToString("N0");  // 천 단위 콤마 표시
        }, finalScore, 1f);

        // Best Score도 즉시 표시하거나 따로 애니메이션 할 수 있음
        bestScoreText.text = bestScore.ToString("N0");
    }


    public void TriggerGameOverSequence()
    {
        bubbleShooter.canShoot = false;
        // 게임 오버 시퀀스: 번쩍 효과 후 UI 무너짐 + 점수 표시
        StartCoroutine(GameOverFlashThenBreak());
    }

    private IEnumerator GameOverFlashThenBreak()
    {
        if (flashPanelGroup == null)
        {
            Debug.LogWarning("Flash Panel이 연결되어 있지 않음.");
            TriggerSortedBreak(); // 안전 fallback
            yield break;
        }

        flashPanelGroup.alpha = 0f;
        flashPanelGroup.gameObject.SetActive(true);

        for (int i = 0; i < 2; i++)
        {
            flashPanelGroup.DOFade(0.3f, flashDuration).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(flashDuration);
            flashPanelGroup.DOFade(0f, flashDuration).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(flashDelay);
        }

        flashPanelGroup.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.3f);
        // 연출이 끝난 뒤 무너짐 시작
        TriggerSortedBreak();
    }
}
