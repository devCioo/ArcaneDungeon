using System.Collections;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public static PlayerHealthController instance;

    public int maxHealth;
    public int currentHealth;
    private float invincibilityDuration = 1.5f;
    private float invincibilityDeltaTime = 0.15f;
    private bool isInvincible;

    public Animator anim;
    public SpriteRenderer sr, hurtSr;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = PlayerController.instance.GetComponent<Animator>();
        sr = PlayerController.instance.transform.Find("Body").GetComponent<SpriteRenderer>();
        hurtSr = PlayerController.instance.transform.Find("Hurt Body").GetComponent<SpriteRenderer>();
        UIController.instance.UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        if (!isInvincible)
        {
            currentHealth -= amount;
            UIController.instance.UpdateHealthUI();
            if (currentHealth <= 0)
            {
                PlayerController.instance.gameObject.SetActive(false);
                UIController.instance.deathScreen.SetActive(true);
            }
            else
            {
                StartCoroutine(BecomeTemporarilyInvincible());
            }
        }
    }

    private IEnumerator BecomeTemporarilyInvincible()
    {
        isInvincible = true;

        sr.enabled = false;
        hurtSr.enabled = true;
        yield return new WaitForSeconds(0.15f);

        sr.enabled = true;
        hurtSr.enabled = false;

        for (float i = 0f; i < (invincibilityDuration - 0.15f); i += invincibilityDeltaTime)
        {
            if (sr.color.a == 1)
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.2f);
            }
            else
            {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            }

            yield return new WaitForSeconds(invincibilityDeltaTime);
        }

        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        isInvincible = false;
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UIController.instance.UpdateHealthUI();
    }
}
