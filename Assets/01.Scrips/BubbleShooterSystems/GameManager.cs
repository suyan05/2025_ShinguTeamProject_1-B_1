using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score = 0; //���� ����

    //[�����]���� ���� ����
    private int highScore;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI lastHghScoreText;

    private GameEndGravityManager gravityManager;

    private void Start()
    {
        //[�����]�ְ����� �ҷ�����
        highScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

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

    public void AddScore(int points)
    {
        score += points; // ���� �߰�

        //[�����]���� ����
        ScoreSaveSystem.SaveSystem.SaveScore(score);
        highScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

        UpdateScoreUI();

        Debug.Log("���� ����: " + score);
    }

    private void UpdateScoreUI()
    {
        //[�����]���� UI ������Ʈ ����
        currentScoreText.text = ""+score;
        highScoreText.text = ""+highScore;
        lastScoreText.text = ""+score;
        lastHghScoreText.text = ""+highScore;
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
        ScoreSaveSystem.SaveSystem.SaveScore(score);

        UnityEditor.EditorApplication.isPlaying = false; // �����Ϳ��� ���� �Ͻ� ����
        /*if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // ���� ���� �� �߷� ���� ����
        }*/
    }

}