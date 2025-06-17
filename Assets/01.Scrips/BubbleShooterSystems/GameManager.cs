using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static ScoreSaveSystem;

public class GameManager : MonoBehaviour
{
    public int Score { get; private set; }      //ĸ��ȭ
    public int HighScore { get; private set; }

    [Header("UI")]
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    //[SerializeField] TextMeshProUGUI lastScoreText;
    //[SerializeField] TextMeshProUGUI lastHighScoreText;

    private GameEndGravityManager gravityManager;

    private void Start()
    {
        //[�����]�ְ����� �ҷ�����
        HighScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

        gravityManager = FindObjectOfType<GameEndGravityManager>(); // ���� ����
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


    public void BubbleMerged(int level)
    {
        int points = level * 10; //������ ������ ������ ���� ���� ����
        AddScore(points);
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

        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� �Ͻ� ����
        /*if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // ���� ���� �� �߷� ���� ����
        }*/
    }

}