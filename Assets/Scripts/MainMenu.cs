using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        PlayerPrefs.DeleteKey("Lives");
        PlayerPrefs.DeleteKey("Health");
        PlayerPrefs.DeleteKey("Arrows");
        SceneManager.LoadScene("Level1_1");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
