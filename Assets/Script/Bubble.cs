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
        if (collision.gameObject.CompareTag("Wall")) // �¿� ���� �浹
        {
            Vector2 normal = collision.contacts[0].normal;
            rb.velocity = Vector2.Reflect(rb.velocity, normal); // �ݻ� ó��
        }
        else if (collision.gameObject.CompareTag("Ceiling") || collision.gameObject.CompareTag("Bubble")) // õ�� �Ǵ� �ٸ� Bubble�� �浹
        {
            rb.velocity = Vector2.zero; // ���� ó��
            rb.bodyType = RigidbodyType2D.Static; // ���� ���·� ����
        }
    }
}
