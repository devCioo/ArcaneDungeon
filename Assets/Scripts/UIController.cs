using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Globalization;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public int numberOfContainers;
    public int currentHealth;
    public float fadeSpeed;
    private bool fadeToBlack, fadeOutBlack;
    public bool isBigMapActive = false;

    public Image[] hearts;
    public Sprite fullHeart, halfHeart, emptyHeart;
    public TMP_Text coinText, keyText, bombText;
    public TMP_Text damageText, attackSpeedText, moveSpeedText, attackRangeText, shotSpeedText, criticalChanceText;
    public GameObject map, bigMap;
    public GameObject deathScreen, pauseMenu;
    public Image fadeScreen;
    public string mainMenuScene;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        numberOfContainers = PlayerHealthController.instance.maxHealth / 2;
        currentHealth = PlayerHealthController.instance.currentHealth;
        fadeOutBlack = true;
        fadeToBlack = false;

        UpdateStats();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOutBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 0f)
            {
                fadeOutBlack = false;
            }
        }
        if (fadeToBlack)
        {
            fadeScreen.color = new Color(fadeScreen.color.r, fadeScreen.color.g, fadeScreen.color.b, Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime));
            if (fadeScreen.color.a == 1f)
            {
                fadeToBlack = false;
            }
        }
    }

    public void UpdateHealthUI()
    {
        numberOfContainers = Mathf.CeilToInt((float)PlayerHealthController.instance.maxHealth / 2);
        currentHealth = PlayerHealthController.instance.currentHealth;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < numberOfContainers)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }

            if (((i * 2) + 2) <= currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (((i * 2) + 1) <= currentHealth)
            {
                hearts[i].sprite = halfHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    public void UpdateStats()
    {
        damageText.text = PlayerController.instance.stats.damage.ToString(CultureInfo.InvariantCulture);
        attackSpeedText.text = PlayerController.instance.stats.attackSpeed.ToString(CultureInfo.InvariantCulture);
        moveSpeedText.text = PlayerController.instance.stats.moveSpeed.ToString(CultureInfo.InvariantCulture);
        attackRangeText.text = PlayerController.instance.stats.attackRange.ToString(CultureInfo.InvariantCulture);
        shotSpeedText.text = PlayerController.instance.stats.shotSpeed.ToString(CultureInfo.InvariantCulture);
        criticalChanceText.text = $"{PlayerController.instance.stats.criticalChance} %";
    }

    public void StartFadeToBlack()
    {
        fadeToBlack = true;
        fadeOutBlack = false;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Resume()
    {
        LevelManager.instance.PauseOrResume();
    }
}
