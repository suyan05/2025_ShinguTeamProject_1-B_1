using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScoreSaveSystem;

//ScoreSaveSystem의 SaveSystem에서 최고점수 최종점수 가져와서 출력

public class GameOverScoreUI : MonoBehaviour
{
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    public TextMeshProUGUI gameOverCurrentScoreText;
    public TextMeshProUGUI gameOverHighScoreText;

    public void ShowGameOverScore()
    {
        //점수 불러와서 표시
        int high = SaveSystem.LoadHighScore();
        int last = SaveSystem.LoadLastScore();

        currentScoreText.text = last.ToString("N0");
        highScoreText.text = high.ToString("N0"); ;

        gameOverCurrentScoreText.text = last.ToString("N0");
        gameOverHighScoreText.text = high.ToString("N0"); ;
    }
}
