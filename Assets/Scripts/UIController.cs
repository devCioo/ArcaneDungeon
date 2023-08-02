using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public int numberOfContainers;
    public int currentHealth;
    public Image[] hearts;
    public Sprite fullHeart, halfHeart, emptyHeart;
    public GameObject deathScreen;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        numberOfContainers = PlayerHealthController.instance.maxHealth / 2;
        currentHealth = PlayerHealthController.instance.currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
