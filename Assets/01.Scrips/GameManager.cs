using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BubbleGrid bubbleGrid; // ���� ���� �ý���
    public BubbleShooter bubbleShooter; // ���� �߻� �ý���
    private int score = 0; // ���� ����

    private void Start()
    {
        InitializeGame();
    }

    // ������ �ʱ�ȭ�ϴ� �Լ�
    private void InitializeGame()
    {
        bubbleGrid.InitializeGrid(); // ���� �ý��� �ʱ�ȭ
        //bubbleShooter.SelectNextBubble(); // ù ��° ���� ����
    }

    // ������ ������Ʈ�ϴ� �Լ�
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("���� ����: " + score);
    }

    // Ư�� ������ ����� �� ȣ��Ǵ� �Լ�
    public void BubbleRemoved(Vector2 position)
    {
        Debug.Log("���� ���ŵ�: " + position);
        AddScore(10); // ���� ���� �� ���� �߰�
    }
}