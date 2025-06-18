using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static ScoreSaveSystem;

public class GameManager : MonoBehaviour
{
    private Dictionary<int, int> levelScores = new Dictionary<int, int>
    {
        {1, 5}, {2, 15}, {3, 30}, {4, 50}, {5, 75}, {6, 100}, {7, 150}
    };

    public int Score { get; private set; }      //캡슐화
    public int HighScore { get; private set; }

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


    public void BubbleMerged(int level, Vector2Int basePosition)
{
    int points = level * 10;
    AddScore(points);

    if (level > maxBubbleLevel)
    {
        maxBubbleLevel = level;

        if (bubbleShooter != null)
        {
            bubbleShooter.UpdateCurrentUnlockLevel(maxBubbleLevel);
        }
    }

    // 최종 레벨일 때 주변 버블 제거
    if (level >= 7) // 최종 레벨 기준
    {
        FindObjectOfType<BubbleGrid>().RemoveNearbyBubbles(basePosition);
    }
}


    public int GetMaxBubbleLevel()
    {
        return maxBubbleLevel;
    }



    public void BubbleRemoved(Vector2 position)
    {
        AddScore(10); //버블이 사라질 때 기본 점수 추가
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