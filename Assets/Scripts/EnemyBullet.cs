using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float shotSpeed;
    public int damage;

    public bool aimAtPlayer;
    public bool isPiercing;
    public bool isSplitting;
    public GameObject smallBullet;
    public int splitBulletsCount;
    public float splitAngle;

    private Vector3 direction = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {
        if (aimAtPlayer)
        {
            direction = PlayerController.instance.transform.position - transform.position;
            direction.Normalize();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * shotSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerHealthController.instance.TakeDamage(damage);

            if (!isPiercing)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnDestroy()
    {
        if (isSplitting)
        {
            for (int i = 0; i < splitBulletsCount; i++)
            {
                float angle = i * splitAngle - (splitBulletsCount - 1) * splitAngle / 2;
                Vector3 splitDirection = Quaternion.Euler(0f, 0f, angle) * direction;

                Instantiate(smallBullet, transform.position, Quaternion.identity).GetComponent<EnemyBullet>().direction = -splitDirection.normalized;
            }
        }
    }
}
