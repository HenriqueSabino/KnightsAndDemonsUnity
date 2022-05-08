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

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        UpdatePlayerHealth(Player.instance.Health);
        UpdatePlayerArrows(Player.instance.Arrows);
    }

    public void PlayerDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
}
