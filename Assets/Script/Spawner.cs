using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float minAngle = -45f;
    public float maxAngle = 45f;

    private Camera mainCamera;

    void Start()
    {
        // 메인 카메라 참조
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirVec = mousePos - (Vector2)transform.position;

        transform.up = dirVec.normalized;
    }
}
