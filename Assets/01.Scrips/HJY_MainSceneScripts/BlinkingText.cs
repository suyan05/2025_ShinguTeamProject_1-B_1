using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    public TextMeshProUGUI pressText;
    public float fadeSpeed = 2f; // 페이드 속도 조절 (클수록 빨라짐)

    private float alpha = 1f;
    private bool fadingOut = true;

    void Update()
    {
        if (fadingOut)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            if (alpha <= 0f)
            {
                alpha = 0f;
                fadingOut = false;
            }
        }
        else
        {
            alpha += fadeSpeed * Time.deltaTime;
            if (alpha >= 1f)
            {
                alpha = 1f;
                fadingOut = true;
            }
        }

        Color c = pressText.color;
        c.a = alpha;
        pressText.color = c;
    }

}
