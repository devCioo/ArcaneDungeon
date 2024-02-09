using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    private Vector3 moveDirection;
    public float health;

    public bool shouldJump;

    public SpriteRenderer sr, hurtSr;
    public Rigidbody2D rb;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (shouldJump)
        {
            StartCoroutine(JumpAndWait());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.gameObject.activeSelf)
        {
            moveDirection = PlayerController.instance.transform.position - transform.position;
            moveDirection.Normalize();
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        if (!shouldJump)
        {
            rb.velocity = moveDirection * moveSpeed;
            anim.SetFloat("velocity", moveDirection.sqrMagnitude);
        }

        anim.SetFloat("posX", moveDirection.x);
        anim.SetFloat("posY", moveDirection.y);
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            StatsManager.instance.gameStats.enemiesDefeated++;
            StatsManager.instance.SaveStats();
            Destroy(gameObject);
        }
        StartCoroutine(GetHurt());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(1);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(1);
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

    private IEnumerator JumpAndWait()
    {
        anim.SetTrigger("jump");

        for (float i = 0f; i < 1.5f; i += 0.1f)
        {
            if (i == 0.2f)
            {
                rb.AddForce(moveDirection * 5f, ForceMode2D.Impulse);
            }
            if (i == 0.6f)
            {
                rb.velocity = Vector2.zero;
            }
            yield return new WaitForSeconds(0.1f);
        }

        StartCoroutine(JumpAndWait());
    }
}
