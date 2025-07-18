using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuitMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        int highScore = ScoreSaveSystem.SaveSystem.LoadHighScore();
        highScoreText.text = ""+highScore;
    }

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
        // 에디터에서는 플레이 모드 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 게임에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}
