using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public int PlayerDeaths;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void PlayerDeath() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
