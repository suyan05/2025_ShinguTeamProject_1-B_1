using UnityEngine;

public class Bubble : MonoBehaviour
{
    private BubbleShooter bubbleShooter; // **BubbleShooter ���� �߰�**
    private int gridX, gridY;
    private bool isPlaced = false;
    private Vector2 direction;

    public void SetBubble(int level)
    {
        // ���� �̹��� ���� ����
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    public void SetShooter(BubbleShooter shooter) // **BubbleShooter ���� ����**
    {
        bubbleShooter = shooter;
    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        isPlaced = true;
        bubbleShooter.EnableShooting(); // **���� ��ġ �Ϸ� �� �߻� ����**
    }

    void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * 5f;
        }
    }
}