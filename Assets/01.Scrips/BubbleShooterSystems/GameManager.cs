using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static ScoreSaveSystem;

public class GameManager : MonoBehaviour
{
    public Dictionary<int, int> levelScores = new Dictionary<int, int>
    {
        {1, 20}, {2, 40}, {3, 80}, {4, 140}, {5, 220}, {6, 330}, {7, 480}, {8, 1000}
    };

    public int Score { get; private set; }      //캡슐화
    public int HighScore { get; private set; }

    // 게임 오버 여부
    public bool gameOver = false;

    // 최대 높이 (이 높이를 초과하면 게임 오버)
    public float maxHeight = 10f;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    //[SerializeField] TextMeshProUGUI lastScoreText;
    //[SerializeField] TextMeshProUGUI lastHighScoreText;

    private GameEndGravityManager gravityManager;
    private BubbleShooter bubbleShooter; // BubbleShooter 참조

    private int maxBubbleLevel = 1; // 최고 병합된 버블 레벨

    private void Start()
    {
        //[한재용]최고점수 불러오기
        HighScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

        gravityManager = FindObjectOfType<GameEndGravityManager>(); // 참조 설정
        bubbleShooter = FindObjectOfType<BubbleShooter>(); // BubbleShooter 찾기

        UpdateScoreUI();
    }

    //[한재용]점수 테스트용
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddScore(10);
        }

        // 아직 게임 오버가 아니면 계속해서 게임 오버 상태를 확인
        if (!gameOver && CheckGameOver())
        {
            GameOver();
        }

    }

    public void AddScore(int amount)
    {
        Score += amount;
        if (Score > HighScore)
        {
            HighScore = Score;
            SaveSystem.SaveScore(HighScore);
        }
        UpdateScoreUI();
    }


    private void UpdateScoreUI()
    {
        if (!currentScoreText) return;
        currentScoreText.text = Score.ToString();
        highScoreText.text = HighScore.ToString();
    }

    public int GetMaxBubbleLevel()
    {
        return maxBubbleLevel;
    }

    public int GetScoreForLevel(int level)
    {
        if (levelScores.TryGetValue(level, out int score))
            return score;  // levelScores 딕셔너리에 값이 있으면 반환
        return 10; // 기본값
    }

    public void BubbleRemoved(Vector2 position)
    {
        AddScore(10); //버블이 사라질 때 기본 점수 추가
    }

    //게임 오버 체크 함수
    public bool CheckGameOver()
    {
        foreach (Bubble bubble in FindObjectsOfType<Bubble>()) // 모든 버블 검사
        {
            if (bubble.transform.position.y >= maxHeight) // 특정 높이 초과 여부 확인
                return true;
        }
        return false;
    }

    public void GameOver()
    {
        Debug.Log("Game Over!"); // 콘솔 출력

        //[한재용]최종 점수 저장
        SaveSystem.SaveScore(HighScore);

        if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // 게임 오버 시 중력 정리 실행
        }
    }
}