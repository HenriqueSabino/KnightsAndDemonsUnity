using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public int PlayerLives;

    public RectTransform HealthBar;
    public TMP_Text Health;
    public TMP_Text Lives;
    public TMP_Text Arrows;
    public bool BossLevel;
    public GameObject Boss;
    public string NextSceneName;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        UpdatePlayerHealth(Player.instance.Health);
        UpdatePlayerArrows(Player.instance.Arrows);
        UpdatePlayerLives(Player.instance.Lives);
    }

    public void PlayerDeath(int Lives)
    {
        PlayerLives = Lives;
        PlayerLives--;
        PlayerPrefs.SetInt("Lives", PlayerLives);
        if (PlayerLives >= 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene("Game Over");
    }

    public void NextLevel()
    {
        PlayerPrefs.SetInt("Lives", Player.instance.Lives);
        PlayerPrefs.SetInt("Health", Player.instance.Health);
        PlayerPrefs.SetInt("Arrows", Player.instance.Arrows);

        SceneManager.LoadScene(NextSceneName);
    }

    public void UpdatePlayerHealth(int health)
    {
        Vector2 anchorMax = HealthBar.anchorMax;
        anchorMax.x = health / 100f;
        HealthBar.anchorMax = anchorMax;

        Health.text = $"{health}%";
    }

    public void UpdatePlayerArrows(int arrows)
    {
        Arrows.text = $"x {arrows.ToString("D2")}";
    }

    public void UpdatePlayerLives(int lives)
    {
        Lives.text = $"x {lives.ToString("D2")}";
    }
}
