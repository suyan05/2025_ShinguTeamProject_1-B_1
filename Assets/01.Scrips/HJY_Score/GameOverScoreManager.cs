using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScoreSaveSystem;

//���� ���� �� OnGameOver ����. ��� currentScore�� finalScore�� �����ϰ� ScoreSaveSystem�� SaveSystem �Լ��� ����

public class GameOverScoreManager : MonoBehaviour
{
    
    public GameManager gamemanager;  // currentScore ����Ǵ� ��ũ��Ʈ ����

    public void OnGameOver()
    {
        //int finalScore = gamemanager.currentScore;
        //SaveSystem.SaveScore(finalScore);
    }
    
}
