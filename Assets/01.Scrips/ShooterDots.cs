using System.Collections.Generic;
using UnityEngine;

public class ShooterDots : MonoBehaviour
{
    public GameObject dotPrefab;
    public int dotCount = 5;
    public float spacing = 0.2f;

    private List<GameObject> dots = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < dotCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dot.SetActive(true);
            dots.Add(dot);
        }
    }

    void Update()
    {
        Vector2 direction = GetMouseDirection();
        Vector2 start = transform.position;

        for (int i = 0; i < dots.Count; i++)
        {
            Vector2 pos = start + direction * spacing * (i + 1);
            dots[i].transform.position = pos;

            float scale = Mathf.Lerp(0.05f, 0.15f, (float)i / dotCount);
            dots[i].transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = dots[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = Mathf.Lerp(0.3f, 1f, (float)i / dots.Count); // 첫 점은 진하게, 마지막 점은 희미하게
                sr.color = c;
            }
        }
    }

    Vector2 GetMouseDirection()
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mouseWorld - transform.position).normalized;
        return dir;
    }


}
