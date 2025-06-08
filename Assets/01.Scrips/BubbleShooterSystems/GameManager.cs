using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int score = 0; //���� ����

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points; // ���� �߰�
        UpdateScoreUI();
        Debug.Log("���� ����: " + score);
    }

    private void UpdateScoreUI()
    {
        //���� UI ������Ʈ ����
    }

    public void BubbleMerged(int level)
    {
        int points = level * 10; //������ ������ ������ ���� ���� ����
        AddScore(points);
    }

    public void BubbleRemoved(Vector2 position)
    {
        AddScore(10); //������ ����� �� �⺻ ���� �߰�
    }
}