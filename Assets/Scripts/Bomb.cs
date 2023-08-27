using System.Collections;
using System.Collections.Generic;
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
            if (other.GetComponentInParent<Room>().isClosed == true)
            {
                other.GetComponent<Animator>().Play("Door_Close");
                other.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                other.GetComponent<Animator>().Play("Door_Open");
                other.GetComponent<BoxCollider2D>().enabled = false;
            }

            //other.GetComponent<SecretDoors>().areRevealed = true;

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
