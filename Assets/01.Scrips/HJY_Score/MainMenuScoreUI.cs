using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScoreSaveSystem;

//ScoreSaveSystem의 SaveSystem에서 최고점수 가져와서 출력

public class MainMenuScoreUI : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    public void ShowMainMenuScore()
    {
        //점수 불러와서 표시
        int highScore = SaveSystem.LoadHighScore();

        highScoreText.text = highScore.ToString("N0");
    }
}
