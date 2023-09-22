using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Statistics stats;
    private float shotDelay;
    private Vector2 moveInput;
    [HideInInspector]
    public bool canMove = true, canShoot = true;

    public List<GameObject> items = new List<GameObject>();

    public Rigidbody2D rb;
    public GameObject bulletToFire;
    public Transform upPos, downPos, leftPos, rightPos;
    public Animator anim;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        shotDelay = 1 / stats.attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !LevelManager.instance.isPaused)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            rb.velocity = moveInput * stats.moveSpeed * 5;

            anim.SetFloat("posY", moveInput.y);
            anim.SetFloat("posX", moveInput.x);

            if (canShoot)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    anim.SetTrigger("attackBack");
                    Instantiate(bulletToFire, upPos.position, Quaternion.Euler(0f, 0f, 90f));
                    StartCoroutine(DisableShootingTemporarily());
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    anim.SetTrigger("attackFront");
                    Instantiate(bulletToFire, downPos.position, Quaternion.Euler(0f, 0f, -90f));
                    StartCoroutine(DisableShootingTemporarily());
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    anim.SetTrigger("attackLeft");
                    Instantiate(bulletToFire, leftPos.position, Quaternion.Euler(0f, 0f, 180f));
                    StartCoroutine(DisableShootingTemporarily());
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    anim.SetTrigger("attackRight");
                    Instantiate(bulletToFire, rightPos.position, upPos.rotation);
                    StartCoroutine(DisableShootingTemporarily());
                }

                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
                {
                    if (canShoot)
                    {
                        if (Input.GetKey(KeyCode.UpArrow))
                        {
                            anim.SetTrigger("attackBack");
                            Instantiate(bulletToFire, upPos.position, Quaternion.Euler(0f, 0f, 90f));
                            StartCoroutine(DisableShootingTemporarily());
                        }
                        if (Input.GetKey(KeyCode.DownArrow))
                        {
                            anim.SetTrigger("attackFront");
                            Instantiate(bulletToFire, downPos.position, Quaternion.Euler(0f, 0f, -90f));
                            StartCoroutine(DisableShootingTemporarily());
                        }
                        if (Input.GetKey(KeyCode.LeftArrow))
                        {
                            anim.SetTrigger("attackLeft");
                            Instantiate(bulletToFire, leftPos.position, Quaternion.Euler(0f, 0f, 180f));
                            StartCoroutine(DisableShootingTemporarily());
                        }
                        if (Input.GetKey(KeyCode.RightArrow))
                        {
                            anim.SetTrigger("attackRight");
                            Instantiate(bulletToFire, rightPos.position, upPos.rotation);
                            StartCoroutine(DisableShootingTemporarily());
                        }
                    }
                }
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetFloat("posY", 0f);
            anim.SetFloat("posX", 0f);
        }
    }

    private IEnumerator DisableShootingTemporarily()
    {
        canShoot = false;

        for (float i = 0f; i < shotDelay; i += Time.deltaTime)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }

        canShoot = true;
    }

    public void AddStats(Item item)
    {
        Statistics oldStats = new Statistics(stats);

        stats.damage += item.stats.damage;
        stats.attackSpeed += item.stats.attackSpeed;
        stats.moveSpeed += item.stats.moveSpeed;
        stats.attackRange += item.stats.attackRange;
        stats.shotSpeed += item.stats.shotSpeed;
        stats.criticalChance += item.stats.criticalChance;

        UIController.instance.UpdateStatistics(oldStats, stats);

        if (item.isHealthUpgrade)
        {
            PlayerHealthController.instance.maxHealth += item.healthUpgradeValue;
            UIController.instance.UpdateHealthUI();
        }
    }

    public IEnumerator PositionOverPlayer(GameObject item)
    {
        anim.SetTrigger("lifting");
        items.Add(item);
        AddStats(item.GetComponent<Item>());

        for (float i = 0f; i < 0.6f; i += 0.01f)
        {
            item.transform.position = PlayerController.instance.transform.position + new Vector3(0f, 1.3f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        item.SetActive(false);
    }
}

[System.Serializable]
public class Statistics
{
    public float damage, attackSpeed, moveSpeed, attackRange, shotSpeed, criticalChance;

    public Statistics(Statistics other)
    {
        damage = other.damage;
        attackSpeed = other.attackSpeed;
        moveSpeed = other.moveSpeed;
        attackRange = other.attackRange;
        shotSpeed = other.shotSpeed;
        criticalChance = other.criticalChance;
    }
}
