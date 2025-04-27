using UnityEngine;
using UnityEngine.UI;

public class Bubble : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) // 좌우 벽에 충돌
        {
            Vector2 normal = collision.contacts[0].normal;
            rb.velocity = Vector2.Reflect(rb.velocity, normal); // 반사 처리
        }
        else if (collision.gameObject.CompareTag("Ceiling") || collision.gameObject.CompareTag("Bubble")) // 천장 또는 다른 Bubble에 충돌
        {
            rb.velocity = Vector2.zero; // 멈춤 처리
            rb.bodyType = RigidbodyType2D.Static; // 정적 상태로 변경
        }
    }
}
