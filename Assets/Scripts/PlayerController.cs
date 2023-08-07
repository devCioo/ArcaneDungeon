using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float moveSpeed;
    private Vector2 moveInput;
    public float attackSpeed;
    private float shotDelay;
    [HideInInspector]
    public bool canMove = true, canShoot = true;

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
        shotDelay = 1 / attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !LevelManager.instance.isPaused)
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();

            rb.velocity = moveInput * moveSpeed;

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
}
