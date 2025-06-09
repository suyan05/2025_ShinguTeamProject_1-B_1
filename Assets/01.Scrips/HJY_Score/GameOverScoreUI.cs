using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScoreSaveSystem;

//ScoreSaveSystem�� SaveSystem���� �ְ����� �������� �����ͼ� ���

public class GameOverScoreUI : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    public TextMeshProUGUI gameOverCurrentScoreText;
    public TextMeshProUGUI gameOverHighScoreText;

    public void ShowGameOverScore()
    {
        //���� �ҷ��ͼ� ǥ��
        int high = SaveSystem.LoadHighScore();
        int last = SaveSystem.LoadLastScore();

        currentScoreText.text = last.ToString("N0");
        highScoreText.text = high.ToString("N0"); ;

        gameOverCurrentScoreText.text = last.ToString("N0");
        gameOverHighScoreText.text = high.ToString("N0"); ;
    }
}
