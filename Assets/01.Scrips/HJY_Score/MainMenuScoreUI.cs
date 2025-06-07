using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static ScoreSaveSystem;

//ScoreSaveSystem�� SaveSystem���� �ְ����� �����ͼ� ���

public class MainMenuScoreUI : MonoBehaviour
{
    public TextMeshProUGUI highScoreText;

    public void ShowMainMenuScore()
    {
        //���� �ҷ��ͼ� ǥ��
        int highScore = SaveSystem.LoadHighScore();

        highScoreText.text = highScore.ToString("N0");
    }
}
