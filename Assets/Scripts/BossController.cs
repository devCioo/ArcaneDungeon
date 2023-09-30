using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public Vector2 moveDirection;
    public bool canShoot = true;
    public float maxHealth, currentHealth;

    public BossAction[] actions;
    public BossSequence[] sequences;
    public int currentSequence;
    public int currentAction;

    public SpriteRenderer sr, hurtSr;
    public Rigidbody2D rb;
    public Animator anim;

    public Sprite bossHealthBarSprite;

    // Start is called before the first frame update
    void Start()
    {
        actions = sequences[currentSequence].actions;
        currentHealth = maxHealth;
        UIController.instance.UpdateBossHealthUI(currentHealth, maxHealth);
        StartCoroutine(CountActionLength());
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.gameObject.activeSelf)
        {
            moveDirection = PlayerController.instance.transform.position - transform.position;
            moveDirection.Normalize();

            if (actions[currentAction].shouldMove)
            {
                rb.velocity = moveDirection * actions[currentAction].moveSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }

            if (actions[currentAction].shouldShoot)
            {
                if (canShoot)
                {
                    StartCoroutine(ShootBullet());
                }
            }

            anim.SetFloat("posX", moveDirection.x);
            anim.SetFloat("posY", moveDirection.y);
        }
        else
        {
            rb.velocity = Vector2.zero;
            StopAllCoroutines();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UIController.instance.UpdateBossHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
            UIController.instance.bossHealthBar.SetActive(false);
            BossManager.instance.isBossDefeated = true;
        }
        else
        {
            if (currentHealth <= sequences[currentSequence].endSequenceHealth && currentSequence < sequences.Length - 1)
            {
                currentSequence++;
                currentAction = 0;
                actions = sequences[currentSequence].actions;
            }
        }
        StartCoroutine(GetHurt());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(2);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(2);
        }
    }

    private IEnumerator GetHurt()
    {
        sr.enabled = false;
        hurtSr.enabled = true;
        yield return new WaitForSeconds(0.1f);
        sr.enabled = true;
        hurtSr.enabled = false;
    }

    public IEnumerator CountActionLength()
    {
        for (float i = 0f; i < actions[currentAction].actionLength; i += 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
        }

        currentAction++;
        if (currentAction >= actions.Length)
        {
            currentAction = 0;
        }

        StartCoroutine(CountActionLength());
    }

    public IEnumerator ShootBullet()
    {
        canShoot = false;

        for (float i = 0f; i < actions[currentAction].shotDelay; i += 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
        }

        if (actions[currentAction].shouldShoot)
        {
            Instantiate(actions[currentAction].bullet, actions[currentAction].firePoint.position, actions[currentAction].firePoint.rotation);
        }

        canShoot = true;
    }
}

[System.Serializable]
public class BossAction
{
    [Header("Action")]
    public float actionLength;
    public bool shouldMove;
    public bool shouldShoot;
    public float moveSpeed, shotDelay;
    public Transform firePoint;
    public GameObject bullet;

    public bool spawnEnemiesOnDeath;
    public GameObject enemy;
}

[System.Serializable]
public class BossSequence
{
    [Header("Sequence")]
    public BossAction[] actions;

    public float endSequenceHealth;
}
