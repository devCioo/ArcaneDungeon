using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Globalization;
using JetBrains.Annotations;

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
    public TMP_Text damageUpdateText, attackSpeedUpdateText, moveSpeedUpdateText, attackRangeUpdateText, shotSpeedUpdateText, criticalChanceUpdateText;
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

        UpdateStatistics(PlayerController.instance.stats, PlayerController.instance.stats);
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

    public void UpdateStatistics(Statistics oldStats, Statistics newStats)
    {
        damageText.text = newStats.damage.ToString("0.00", CultureInfo.InvariantCulture);
        attackSpeedText.text = newStats.attackSpeed.ToString("0.00", CultureInfo.InvariantCulture);
        moveSpeedText.text = newStats.moveSpeed.ToString("0.00", CultureInfo.InvariantCulture);
        attackRangeText.text = newStats.attackRange.ToString("0.00", CultureInfo.InvariantCulture);
        shotSpeedText.text = newStats.shotSpeed.ToString("0.00", CultureInfo.InvariantCulture);
        criticalChanceText.text = $"{newStats.criticalChance.ToString("0.00", CultureInfo.InvariantCulture)} %";

        DisplayDifference(damageUpdateText, oldStats.damage, newStats.damage);
        DisplayDifference(attackSpeedUpdateText, oldStats.attackSpeed, newStats.attackSpeed);
        DisplayDifference(moveSpeedUpdateText, oldStats.moveSpeed, newStats.moveSpeed);
        DisplayDifference(attackRangeUpdateText, oldStats.attackRange, newStats.attackRange);
        DisplayDifference(shotSpeedUpdateText, oldStats.shotSpeed, newStats.shotSpeed);
        DisplayDifference(criticalChanceUpdateText, oldStats.criticalChance, newStats.criticalChance);
    }

    public void DisplayDifference(TMP_Text textBox, float oldStat, float newStat)
    {
        if (oldStat != newStat)
        {
            if (oldStat < newStat)
            {
                textBox.text = "+" + (newStat - oldStat).ToString("0.00", CultureInfo.InvariantCulture);
                textBox.color = new Color(0f, 1f, 0f, 0f);
                StartCoroutine(ShowStatDifference(textBox));
            }
            if (oldStat > newStat)
            {
                textBox.text = "-" + (newStat - oldStat).ToString("0.00", CultureInfo.InvariantCulture);
                textBox.color = new Color(1f, 0f, 0f, 0f);
                StartCoroutine(ShowStatDifference(textBox));
            }
        }
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

    private IEnumerator ShowStatDifference(TMP_Text textBox)
    {
        for (float i = 0f; i < 0.5f; i += 0.01f)
        {
            textBox.color += new Color(0f, 0f, 0f, 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
        for (float i = 0f; i < 2f; i += 0.1f)
        {
            yield return new WaitForSeconds(0.1f);
        }
        for (float i = 0f; i < 0.5f; i += 0.01f)
        {
            textBox.color += new Color(0f, 0f, 0f, -0.02f);
            yield return new WaitForSeconds(0.01f);
        }
    }
}
