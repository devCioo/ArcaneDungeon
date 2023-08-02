using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed;
    private Vector3 moveDirection;
    public float health;

    public Rigidbody2D rb;
    public SpriteRenderer sr;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.isVisible && PlayerController.instance.gameObject.activeSelf)
        {
            moveDirection = PlayerController.instance.transform.position - transform.position;
            moveDirection.Normalize();
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        rb.velocity = moveDirection * moveSpeed;

        anim.SetFloat("posX", moveDirection.x);
        anim.SetFloat("posY", moveDirection.y);
        anim.SetFloat("velocity", moveDirection.sqrMagnitude);
    }

    public void TakeDamage(float amount)
    {
        anim.SetTrigger("damaged");
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
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
}
