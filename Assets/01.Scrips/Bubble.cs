using UnityEngine;

public class Bubble : MonoBehaviour
{
    private BubbleShooter bubbleShooter; // **BubbleShooter 참조 추가**
    private int gridX, gridY;
    private bool isPlaced = false;
    private Vector2 direction;

    public void SetBubble(int level)
    {
        // 버블 이미지 설정 로직
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

    public void SetShooter(BubbleShooter shooter) // **BubbleShooter 참조 설정**
    {
        bubbleShooter = shooter;
    }

    public void SetGridPosition(int x, int y)
    {
        gridX = x;
        gridY = y;
        isPlaced = true;
        bubbleShooter.EnableShooting(); // **격자 배치 완료 후 발사 가능**
    }

    void Update()
    {
        if (!isPlaced)
        {
            transform.position += (Vector3)direction * Time.deltaTime * 5f;
        }
    }
}