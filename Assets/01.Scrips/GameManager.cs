using UnityEngine;

public class GameManager : MonoBehaviour
{
    public BubbleGrid bubbleGrid; // ���� �ý��� ����
    public BubbleShooter bubbleShooter; // ���� �߻� �ý��� ����
    private int score = 0; // ���� ���� ����

    void Start()
    {
        InitializeGame(); // ���� ���� �� ���� �ʱ�ȭ
    }

    // ���ڸ� �ʱ�ȭ�ϴ� �Լ�
    private void InitializeGame()
    {
        bubbleGrid.InitializeGrid();
    }

    // ������ �߰��ϴ� �Լ�
    public void AddScore(int points)
    {
        score += points; // ���� ����
        Debug.Log("���� ����: " + score); // ���� ���� ���
    }

    // Ư�� ������ ���ŵ� �� ȣ��Ǵ� �Լ�
    public void BubbleRemoved(Vector2 position)
    {
        Debug.Log("���� ���ŵ�: " + position);
        AddScore(10); // ���� ���� �� ���� �߰�
    }
}