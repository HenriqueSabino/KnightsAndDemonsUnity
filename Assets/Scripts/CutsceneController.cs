using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public int Scene;
    public Image Scene1;
    public Image Scene2;

    void Update()
    {
        Scenes1();
    }
    // Start is called before the first frame update
    public void Scenes1()
    {
        if (Scene == 0)
        {
            Scene1.enabled = true;
            Scene2.enabled = false;
        }
        if (Scene == 1)
        {
            Scene1.enabled = false;
            Scene2.enabled = true;
        }
        else if (Scene == 2)
        {
            PlayerPrefs.DeleteKey("Lives");
            PlayerPrefs.DeleteKey("Health");
            PlayerPrefs.DeleteKey("Arrows");
            PlayerPrefs.DeleteKey("Points");
            PlayerPrefs.DeleteKey("Wings");
            PlayerPrefs.SetInt("Key", ((int)KeyStatus.NO_KEY));

            SceneManager.LoadScene("Level1_1");
        }
    }

    public void NextScene()
    {
        Scene++;
    }
}
