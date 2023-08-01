using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    private Vector2 moveInput;
    public float attackSpeed;
    private float shotDelay;
    private bool canShoot = true;

    public Rigidbody2D rb;
    public GameObject bulletToFire;
    public Transform upPos, downPos, leftPos, rightPos;

    // Start is called before the first frame update
    void Start()
    {
        shotDelay = 1 / attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        rb.velocity = moveInput * moveSpeed;

        if (canShoot)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Instantiate(bulletToFire, upPos.position, Quaternion.Euler(0f, 0f, 90f));
                StartCoroutine(DisableShootingTemporarily());
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Instantiate(bulletToFire, downPos.position, Quaternion.Euler(0f, 0f, -90f));
                StartCoroutine(DisableShootingTemporarily());
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Instantiate(bulletToFire, leftPos.position, Quaternion.Euler(0f, 0f, 180f));
                StartCoroutine(DisableShootingTemporarily());
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Instantiate(bulletToFire, rightPos.position, rightPos.rotation);
                StartCoroutine(DisableShootingTemporarily());
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                if (canShoot)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        Instantiate(bulletToFire, upPos.position, Quaternion.Euler(0f, 0f, 90f));
                        StartCoroutine(DisableShootingTemporarily());
                    }
                    if (Input.GetKey(KeyCode.DownArrow))
                    {
                        Instantiate(bulletToFire, downPos.position, Quaternion.Euler(0f, 0f, -90f));
                        StartCoroutine(DisableShootingTemporarily());
                    }
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        Instantiate(bulletToFire, leftPos.position, Quaternion.Euler(0f, 0f, 180f));
                        StartCoroutine(DisableShootingTemporarily());
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        Instantiate(bulletToFire, rightPos.position, rightPos.rotation);
                        StartCoroutine(DisableShootingTemporarily());
                    }
                }
            }
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
