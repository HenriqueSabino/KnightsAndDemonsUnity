using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
    public List<UnityEvent> BossDied;
    public UnityEvent NeedKey;
    public string NextSceneName;
    public string PreviousSceneName;

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

        if (BossLevel)
            StartCoroutine(WaitBossDeath());
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

    public void NextLevel(bool checkKey = false)
    {
        if (checkKey && PlayerPrefs.GetInt("Key") != ((int)KeyStatus.GOT_KEY))
        {
            PlayerPrefs.SetInt("Key", ((int)KeyStatus.USED_KEY));

            NeedKey.Invoke();

            return;
        }

        PlayerPrefs.SetInt("Lives", Player.instance.Lives);
        PlayerPrefs.SetInt("Health", Player.instance.Health);
        PlayerPrefs.SetInt("Arrows", Player.instance.Arrows);
        PlayerPrefs.SetInt("Points", Player.instance.Points);

        SceneManager.LoadScene(NextSceneName);
    }

    public void PreviousLevel(bool checkKey = false)
    {
        if (checkKey && PlayerPrefs.GetInt("Key") != ((int)KeyStatus.GOT_KEY))
        {
            PlayerPrefs.SetInt("Key", ((int)KeyStatus.USED_KEY));

            NeedKey.Invoke();

            return;
        }

        PlayerPrefs.SetInt("Lives", Player.instance.Lives);
        PlayerPrefs.SetInt("Health", Player.instance.Health);
        PlayerPrefs.SetInt("Arrows", Player.instance.Arrows);
        PlayerPrefs.SetInt("Points", Player.instance.Points);
        PlayerPrefs.SetInt("Back", 1);

        SceneManager.LoadScene(PreviousSceneName);
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

    private IEnumerator WaitBossDeath()
    {
        yield return new WaitForEndOfFrame();

        if (Boss != null)
        {
            yield return new WaitUntil(() => Boss == null);


            foreach (var action in BossDied)
            {
                action.Invoke();
            }
        }

        GameObject blockage = GameObject.Find("Level/Blockage");

        if (blockage != null)
        {
            Destroy(blockage);
        }
    }
}
