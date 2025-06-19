using System.Collections.Generic;
using UnityEngine;

public class ShooterDots : MonoBehaviour
{
    public GameObject dotPrefab;
    public int dotCount = 5;
    public float spacing = 0.2f;

    private List<GameObject> dots = new List<GameObject>();
    private BubbleShooter bubbleShooter;

    void Start()
    {
        bubbleShooter = FindObjectOfType<BubbleShooter>();

        for (int i = 0; i < dotCount; i++)
        {
            GameObject dot = Instantiate(dotPrefab, transform);
            dot.SetActive(true);
            dots.Add(dot);
        }
    }

    void Update()
    {
        Vector2 rawDir = GetMouseDirection();
        Vector2 clampedDir = GetClampedDirection(rawDir);
        Vector2 start = transform.position;

        // 마우스 위치와 거리 계산
        float z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
        float distance = Vector2.Distance(start, mouseWorld);

        // 거리에 따라 spacing 조절 (0.1 ~ 0.3 사이)
        float dynamicSpacing = Mathf.Clamp(distance * 0.05f, 0.1f, 0.3f);

        for (int i = 0; i < dots.Count; i++)
        {
            Vector2 pos = start + clampedDir * dynamicSpacing * (i + 1);
            dots[i].transform.position = pos;

            float scale = Mathf.Lerp(0.05f, 0.15f, (float)i / dotCount);
            dots[i].transform.localScale = Vector3.one * scale;

            SpriteRenderer sr = dots[i].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = Mathf.Lerp(0.3f, 1f, (float)i / dots.Count);
                sr.color = c;
            }
        }
    }

    Vector2 GetMouseDirection()
    {
        float z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, z));
        Vector2 dir = (mouseWorld - transform.position).normalized;
        return dir;
    }

    Vector2 GetClampedDirection(Vector2 originalDir)
    {
        float angle = Mathf.Atan2(originalDir.y, originalDir.x) * Mathf.Rad2Deg;

        if (bubbleShooter != null)
        {
            angle = Mathf.Clamp(angle, bubbleShooter.minAngle, bubbleShooter.maxAngle);
        }

        float rad = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
}