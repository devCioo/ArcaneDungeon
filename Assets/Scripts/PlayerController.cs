using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public Statistics stats;
    private float shotDelay;
    private Vector2 moveInput, attackInput;
    [HideInInspector]
    public bool canMove = true, canShoot = true;

    public Joystick movementJoystick, attackJoystick;
    public Rigidbody2D rb;
    public GameObject bulletToFire, bomb;
    public Transform upPos, downPos, leftPos, rightPos;
    public Animator anim;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        shotDelay = 1 / stats.attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !GameManager.instance.isPaused)
        {
            moveInput = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
            moveInput.Normalize();

            rb.velocity = moveInput * stats.moveSpeed * 5;

            anim.SetFloat("movePosX", moveInput.x);
            anim.SetFloat("movePosY", moveInput.y);
            anim.SetFloat("velocity", moveInput.sqrMagnitude);
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetFloat("movePosX", 0f);
            anim.SetFloat("movePosY", 0f);
            anim.SetFloat("velocity", moveInput.sqrMagnitude);
        }

        if (canShoot && !GameManager.instance.isPaused)
        {
            attackInput = new Vector2(attackJoystick.Horizontal, attackJoystick.Vertical);
            if (attackInput != Vector2.zero)
            {
                float angle = Mathf.Atan2(attackInput.y, attackInput.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                if (attackInput.y > 0)
                {
                    Instantiate(bulletToFire, upPos.position, rotation);
                }
                else if (attackInput.x > 0)
                {
                    Instantiate(bulletToFire, rightPos.position, rotation);
                }
                else if (attackInput.y < 0)
                {
                    Instantiate(bulletToFire, downPos.position, rotation);
                }
                else if (attackInput.x < 0)
                {
                    Instantiate(bulletToFire, leftPos.position, rotation);
                }

                StartCoroutine(DisableShootingTemporarily());
                anim.SetFloat("shootPosX", attackInput.x);
                anim.SetFloat("shootPosY", attackInput.y);
                anim.SetTrigger("shot");
            }
        }
    }

    public void PlaceBomb()
    {
        Instantiate(bomb, transform.position, transform.rotation);
        LevelManager.instance.UseBomb();
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
            PlayerHealthController.instance.currentHealth += item.healthUpgradeValue;
            UIController.instance.UpdateHealthUI();
        }
    }

    public IEnumerator CollectItem(GameObject item)
    {
        anim.SetTrigger("lifting");
        StatsManager.instance.gameStats.itemsPickedUp++;
        StatsManager.instance.SaveStats();
        AddStats(item.GetComponent<Item>());

        for (float i = 0f; i < 0.6f; i += 0.01f)
        {
            item.transform.position = transform.position + new Vector3(0f, 1.3f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(item);
    }

    public IEnumerator BuyItem(GameObject item, ShopItem shopItem)
    {
        anim.SetTrigger("lifting");
        AddStats(item.GetComponent<Item>());
        shopItem.GetComponent<CircleCollider2D>().enabled = false;

        for (float i = 0f; i < 0.6f; i += 0.01f)
        {
            item.transform.position = transform.position + new Vector3(0f, 1.3f, 0f);
            yield return new WaitForSeconds(0.01f);
        }

        Destroy(item);
        Destroy(shopItem);
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
