using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BubbleGrid bubbleGrid; // 격자 시스템 참조
    public BubbleShooter bubbleShooter; // 버블 발사 시스템 참조
    private int score = 0; // 현재 게임 점수

    void Start()
    {
        InitializeGame(); // 게임 시작 시 격자 초기화
    }

    // 격자를 초기화하는 함수
    private void InitializeGame()
    {
        bubbleGrid.InitializeGrid();
    }

    // 점수를 추가하는 함수
    public void AddScore(int points)
    {
        score += points; // 점수 증가
        Debug.Log("현재 점수: " + score); // 현재 점수 출력
    }

    // 특정 버블이 제거될 때 호출되는 함수
    public void BubbleRemoved(Vector2 position)
    {
        Debug.Log("버블 제거됨: " + position);
        AddScore(10); // 버블 제거 시 점수 추가
    }
}