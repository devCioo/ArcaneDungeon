using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float damage = 100f;
    public GameObject explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BombExplosionDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SecretDoor"))
        {
            other.GetComponent<Door>().isRevealed = true;

            if (other.GetComponentInParent<Room>().isClosed == true)
            {
                other.GetComponent<Door>().CloseDoor();
            }
            else
            {
                other.GetComponent<Door>().OpenDoor();
            }

            Vector2Int secretRoomPosition = LevelManager.instance.levelGenerator.secretRoomPosition;
            LevelManager.instance.levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y].GetComponent<Room>().mapRoom.SetActive(true);
        }
        if (other.CompareTag("Player"))
        {
            PlayerHealthController.instance.TakeDamage(2);
        }
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>().TakeDamage(damage);
        }
        if (other.CompareTag("Obstacle"))
        {
            Destroy(other.gameObject);
        }

        Instantiate(explosionEffect, transform.position, Quaternion.Euler(0f, 0f, 0f));
        Destroy(gameObject);
    }

    private IEnumerator BombExplosionDelay()
    {
        for (float i = 0f; i < 3f; i += 0.15f)
        {
            yield return new WaitForSeconds(0.15f);
        }

        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }
}
