using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public int currentImage;
    public List<Image> images;
    public Button next;
    public string NextScene;
    public bool showOnAwake;
    public string saveOnPlayerPrefs = "";

    void Awake()
    {
        foreach (var image in images)
        {
            image.enabled = false;
        }
        next.gameObject.SetActive(false);

        if (showOnAwake && ((saveOnPlayerPrefs == "") || (saveOnPlayerPrefs != "" && !PlayerPrefs.HasKey(saveOnPlayerPrefs))))
        {
            ShowCutscene();
        }
    }

    public void ShowCutscene()
    {
        currentImage = 0;
        Time.timeScale = 0;
        images[currentImage].enabled = true;
        next.gameObject.SetActive(true);
    }

    public void NextImage()
    {
        currentImage++;
        ChangeImage();
    }

    // Start is called before the first frame update
    public void ChangeImage()
    {
        if (currentImage < images.Count)
        {
            images[currentImage - 1].enabled = false;
            images[currentImage].enabled = true;
        }
        else
        {
            if (NextScene == "Level1_1")
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.SetInt("Key", ((int)KeyStatus.NO_KEY));
            }

            if (saveOnPlayerPrefs != "")
            {
                PlayerPrefs.SetInt(saveOnPlayerPrefs, 1);
            }

            Time.timeScale = 1;
            if (NextScene != "")
                SceneManager.LoadScene(NextScene);
            else
            {
                foreach (var image in images)
                {
                    image.enabled = false;
                }
                next.gameObject.SetActive(false);
            }
        }
    }
}
