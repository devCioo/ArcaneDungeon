using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakables : MonoBehaviour
{
    private int health;
    public float dropChance;
    public Drop[] possibleDrops;

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

            float chance = Random.Range(0f, 100f);

            if (chance <= dropChance)
            {
                float itemChance = Random.Range(0f, 100f);
                float roll = 0f;
                foreach (Drop drop in possibleDrops)
                {
                    roll += drop.dropChance;
                    if (roll >= itemChance)
                    {
                        Instantiate(drop.item, transform.position, transform.rotation);
                        break;
                    }
                }
            }
        }
        else
        {
            sr.sprite = sprites[sprites.Length - health];
        }
    }
}

[System.Serializable]
public class Drop
{
    public GameObject item;
    public float dropChance;
}
