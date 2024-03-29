using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float shotSpeed;
    public float damage;

    public Rigidbody2D rb;
    public GameObject impactEffect;

    // Start is called before the first frame update
    void Start()
    {
        damage = PlayerController.instance.stats.damage;
        shotSpeed = PlayerController.instance.stats.shotSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = transform.right * shotSpeed * 10;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.CompareTag("Boss"))
        {
            other.GetComponent<BossController>().TakeDamage(damage);
            Destroy(gameObject);
        }
        if (other.CompareTag("Breakable"))
        {
            other.GetComponent<Breakables>().TakeDamage(1);
            Destroy(gameObject);
        }
        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Instantiate(impactEffect, transform.position, transform.rotation);
    }
}
