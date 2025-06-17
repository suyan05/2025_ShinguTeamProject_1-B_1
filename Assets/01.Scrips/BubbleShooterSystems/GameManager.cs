using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using static ScoreSaveSystem;

public class GameManager : MonoBehaviour
{
    public int Score { get; private set; }      //캡슐화
    public int HighScore { get; private set; }

    [Header("UI")]
    [SerializeField] TextMeshProUGUI currentScoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    //[SerializeField] TextMeshProUGUI lastScoreText;
    //[SerializeField] TextMeshProUGUI lastHighScoreText;

    private GameEndGravityManager gravityManager;

    private void Start()
    {
        //[한재용]최고점수 불러오기
        HighScore = ScoreSaveSystem.SaveSystem.LoadHighScore();

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
        SaveSystem.SaveScore(HighScore);

        UnityEditor.EditorApplication.isPlaying = false; // 에디터에서 게임 일시 정지
        /*if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // 게임 오버 시 중력 정리 실행
        }*/
    }

}