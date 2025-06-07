using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitMenu : MonoBehaviour
{
    public GameObject pausePanel;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = pausePanel.activeSelf;
            pausePanel.SetActive(!isActive);
        }
    }

    public void ExitGame()
    {
        // �����Ϳ����� �÷��� ��� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // ����� ���ӿ����� ���ø����̼� ����
        Application.Quit();
#endif
    }
}
