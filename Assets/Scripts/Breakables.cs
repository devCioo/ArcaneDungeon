using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakables : MonoBehaviour
{
    private int health;

    public SpriteRenderer sr;
    public Sprite[] sprites;
    public GameObject[] destroyed;

    // Start is called before the first frame update
    void Start()
    {
        health = sprites.Length;
        sr.sprite = sprites[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Destroy(gameObject);
            Instantiate(destroyed[Random.Range(0, destroyed.Length)], transform.position, transform.rotation);
        }
        else
        {
            sr.sprite = sprites[sprites.Length - health];
        }
    }
}