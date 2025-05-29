using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BubbleGrid bubbleGrid; // 버블 격자 시스템
    public BubbleShooter bubbleShooter; // 버블 발사 시스템
    private int score = 0; // 점수 관리

    private void Start()
    {
        InitializeGame();
    }

    // 게임을 초기화하는 함수
    private void InitializeGame()
    {
        bubbleGrid.InitializeGrid(); // 격자 시스템 초기화
        //bubbleShooter.SelectNextBubble(); // 첫 번째 버블 설정
    }

    // 점수를 업데이트하는 함수
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("현재 점수: " + score);
    }

    // 특정 버블이 사라질 때 호출되는 함수
    public void BubbleRemoved(Vector2 position)
    {
        Debug.Log("버블 제거됨: " + position);
        AddScore(10); // 버블 제거 시 점수 추가
    }
}