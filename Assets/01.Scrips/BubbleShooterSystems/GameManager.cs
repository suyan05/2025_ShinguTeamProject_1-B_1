using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0; //게임 점수
    private GameEndGravityManager gravityManager;

    private void Start()
    {
        gravityManager = FindObjectOfType<GameEndGravityManager>(); // 참조 설정
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points; // 점수 추가
        UpdateScoreUI();
        Debug.Log("현재 점수: " + score);
    }

    private void UpdateScoreUI()
    {
        //점수 UI 업데이트 로직
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
        /*if (gravityManager != null)
        {
            gravityManager.TriggerSortedBreak(); // 게임 오버 시 중력 정리 실행
        }*/
    }

}