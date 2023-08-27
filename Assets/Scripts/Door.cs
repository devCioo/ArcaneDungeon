using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isSecretDoor, isShopOrItemDoor;
    public bool isRevealed, isLocked;

    public BoxCollider2D doorCollider;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (!isSecretDoor)
        {
            isRevealed = true;
        }
        if (isShopOrItemDoor && LevelManager.instance.levelGenerator.shopAndItemRoomRequireKeys)
        {
            isLocked = true;
            anim.Play("Door_Lock");
            doorCollider.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenDoor()
    {
        if (isRevealed && !isLocked)
        {
            anim.Play("Door_Open");
            doorCollider.enabled = false;
        }
    }

    public void CloseDoor()
    {
        if (isRevealed)
        {
            anim.Play("Door_Close");
            doorCollider.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isLocked && !gameObject.GetComponentInParent<Room>().isClosed)
            {
                if (LevelManager.instance.currentKeys > 0)
                {
                    isLocked = false;
                    OpenDoor();
                    LevelManager.instance.UseKey();
                }
            }
        }
    }
}
