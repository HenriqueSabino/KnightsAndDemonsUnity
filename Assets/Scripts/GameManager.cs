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
    public RectTransform WingsBar;
    public TMP_Text Health;
    public TMP_Text Lives;
    public TMP_Text Arrows;
    public TMP_Text Points;
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
        PlayerPrefs.SetInt("Points", Player.instance.Points);

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

    public void UpdatePlayerPoints(int points)
    {
        Points.text = $"Points: {points.ToString("D3")}";
    }

    public void UpdatePlayerLives(int lives)
    {
        Lives.text = $"x {lives.ToString("D2")}";
    }

    public void UpdateWingsCooldown(float cooldown)
    {
        Vector2 anchorMax = WingsBar.anchorMax;
        anchorMax.x = cooldown;
        WingsBar.anchorMax = anchorMax;
    }
}
