using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public int PlayerLives;

    public int health;

    public int heathmax;

    public Image HealthBar;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Update() 
    {
        AttLife();
    }

    public void PlayerDeath() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AttLife()
    {
        if(Player.instance.Health > 0)
            HealthBar.transform.localScale = new Vector3(Player.instance.Health,1,0);
        else
            HealthBar.transform.localScale = new Vector3(0,1,0);
    }
}
