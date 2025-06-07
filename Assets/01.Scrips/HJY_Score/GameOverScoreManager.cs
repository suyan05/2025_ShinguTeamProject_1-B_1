using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScoreSaveSystem;

//게임 오버 시 OnGameOver 실행. 즉시 currentScore을 finalScore에 저장하고 ScoreSaveSystem의 SaveSystem 함수로 보냄

public class GameOverScoreManager : MonoBehaviour
{
    
    public GameManager gamemanager;  // currentScore 저장되는 스크립트 참조

    public void OnGameOver()
    {
        //int finalScore = gamemanager.currentScore;
        //SaveSystem.SaveScore(finalScore);
    }
    
}
