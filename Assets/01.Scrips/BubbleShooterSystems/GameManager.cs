using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int score = 0; //게임 점수

    //[한재용]점수 변수 선언
    private int highScore;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI lastScoreText;
    public TextMeshProUGUI lastHghScoreText;

    private GameEndGravityManager gravityManager;

    private void Start()
    {
        //[한재용]최고점수 불러오기
        highScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

        gravityManager = FindObjectOfType<GameEndGravityManager>(); // 참조 설정
        UpdateScoreUI();
    }

    //[한재용]점수 테스트용
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AddScore(10);
        }
    }

    public void AddScore(int points)
    {
        score += points; // 점수 추가

        //[한재용]점수 저장
        ScoreSaveSystem.SaveSystem.SaveScore(score);
        highScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

        UpdateScoreUI();

        Debug.Log("현재 점수: " + score);
    }

    private void UpdateScoreUI()
    {
        //[한재용]점수 UI 업데이트 로직
        currentScoreText.text = ""+score;
        highScoreText.text = ""+highScore;
        lastScoreText.text = ""+score;
        lastHghScoreText.text = ""+highScore;
    }

    public void BubbleMerged(int level)
    {
        int points = level * 10; //합쳐진 버블의 레벨에 따라 점수 증가
        AddScore(points);
    }

    public void BubbleRemoved(Vector2 position)
    {
        AddScore(10); //버블이 사라질 때 기본 점수 추가
    }

    public void GameOver()
    {
        Debug.Log("Game Over!"); // 콘솔 출력

        //[한재용]최종 점수 저장
        ScoreSaveSystem.SaveSystem.SaveScore(score);

        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 게임 일시 정지
        /*if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // 게임 오버 시 중력 정리 실행
        }*/
    }

}