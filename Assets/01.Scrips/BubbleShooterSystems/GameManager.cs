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

    public int Score { get; private set; }      //ĸ��ȭ
    public int HighScore { get; private set; }

    // ���� ���� ����
    public bool gameOver = false;

    // �ִ� ���� (�� ���̸� �ʰ��ϸ� ���� ����)
    public float maxHeight = 10f;

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

        // ���� ���� ������ �ƴϸ� ����ؼ� ���� ���� ���¸� Ȯ��
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
            return score;  // levelScores ��ųʸ��� ���� ������ ��ȯ
        return 10; // �⺻��
    }

    public void BubbleRemoved(Vector2 position)
    {
        AddScore(10); //������ ����� �� �⺻ ���� �߰�
    }

    //���� ���� üũ �Լ�
    public bool CheckGameOver()
    {
        foreach (Bubble bubble in FindObjectsOfType<Bubble>()) // ��� ���� �˻�
        {
            if (bubble.transform.position.y >= maxHeight) // Ư�� ���� �ʰ� ���� Ȯ��
                return true;
        }
        return false;
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