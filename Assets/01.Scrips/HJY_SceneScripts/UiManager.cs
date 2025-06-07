using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UiManager : MonoBehaviour
{
    public void LoadInGameScene()
    {
        SceneManager.LoadScene("Test_InGame");  
    }

    public void LoadMainScene()
    {
        SceneManager.LoadScene("Test_Main");
    }
}
