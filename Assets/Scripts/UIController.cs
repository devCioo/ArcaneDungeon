using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public int numberOfContainers;
    public int currentHealth;
    public float fadeSpeed;
    private bool fadeToBlack, fadeOutBlack;

    public Image[] hearts;
    public Sprite fullHeart, halfHeart, emptyHeart;
    public GameObject deathScreen;
    public Image fadeScreen;

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

    public void StartFadeToBlack()
    {
        fadeToBlack = true;
        fadeOutBlack = false;
    }
}
