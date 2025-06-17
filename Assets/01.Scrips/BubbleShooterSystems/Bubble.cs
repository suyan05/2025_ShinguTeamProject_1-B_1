using UnityEngine;

public class Bubble : MonoBehaviour
{
    public int level = 1; // ������ ���� ����
    public int placedOrder; // ���ڿ� ��ġ�� ���� (���� ���� �Ǵܿ�)

    private Vector2 direction;
    private float speed;
    private BubbleShooter shooter;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite[] levelSprites; // ������ �ð� ���ҽ�

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir) => direction = dir.normalized;
    public void SetSpeed(float s) => speed = s;
    public void SetShooter(BubbleShooter s) => shooter = s;
    public void StopMovement() => speed = 0f;

    public void RefreshVisual()
    {
        if (spriteRenderer != null && level - 1 >= 0 && level - 1 < levelSprites.Length)
            spriteRenderer.sprite = levelSprites[level - 1];
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopMovement();
        if (shooter != null) shooter.EnableShooting();

        // ���� �ý��ۿ� ���� ���
        BubbleGrid grid = FindObjectOfType<BubbleGrid>();
        if (grid != null) grid.PlaceBubble(this);
    }
}