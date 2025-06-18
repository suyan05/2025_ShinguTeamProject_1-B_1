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

    public int Score { get; private set; }      //ĸ��ȭ
    public int HighScore { get; private set; }

    [Header("UI")]
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    //[SerializeField] TextMeshProUGUI lastScoreText;
    //[SerializeField] TextMeshProUGUI lastHighScoreText;

    private GameEndGravityManager gravityManager;
    private BubbleShooter bubbleShooter; // BubbleShooter ����

    private int maxBubbleLevel = 1; // �ְ� ���յ� ���� ����

    private void Start()
    {
        //[�����]�ְ����� �ҷ�����
        HighScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

        gravityManager = FindObjectOfType<GameEndGravityManager>(); // ���� ����
        bubbleShooter = FindObjectOfType<BubbleShooter>(); // BubbleShooter ã��

        UpdateScoreUI();
    }

    //[�����]���� �׽�Ʈ��
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

    // ���� ������ �� �ֺ� ���� ����
    if (level >= 7) // ���� ���� ����
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
        AddScore(10); //������ ����� �� �⺻ ���� �߰�
    }

    public void GameOver()
    {
        Debug.Log("Game Over!"); // �ܼ� ���

        //[�����]���� ���� ����
        SaveSystem.SaveScore(HighScore);

        if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // ���� ���� �� �߷� ���� ����
        }
    }

}