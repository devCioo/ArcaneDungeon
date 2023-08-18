using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public bool closeWhenEntered;
    [HideInInspector]
    public bool isActiveRoom;

    public RoomType roomType;
    public GameObject doorUp, doorRight, doorDown, doorLeft;
    public Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenDoors()
    {
        if (doorUp != null)
        {
            doorUp.GetComponent<Animator>().Play("Doors_Open");
            doorUp.GetComponent<Collider2D>().enabled = false;
        }
        if (doorRight != null)
        {
            doorRight.GetComponent<Animator>().Play("Doors_Open");
            doorRight.GetComponent<Collider2D>().enabled = false;
        }
        if (doorDown != null)
        {
            doorDown.GetComponent<Animator>().Play("Doors_Open");
            doorDown.GetComponent<Collider2D>().enabled = false;
        }
        if (doorLeft != null)
        {
            doorLeft.GetComponent<Animator>().Play("Doors_Open");
            doorLeft.GetComponent<Collider2D>().enabled = false;
        }

        closeWhenEntered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.instance.ChangeTarget(transform);

            if (closeWhenEntered)
            {
                if (doorUp != null)
                {
                    doorUp.GetComponent<Animator>().Play("Doors_Close");
                    doorUp.GetComponent<Collider2D>().enabled = true;
                }
                if (doorRight != null)
                {
                    doorRight.GetComponent<Animator>().Play("Doors_Close");
                    doorRight.GetComponent<Collider2D>().enabled = true;
                }
                if (doorDown != null)
                {
                    doorDown.GetComponent<Animator>().Play("Doors_Close");
                    doorDown.GetComponent<Collider2D>().enabled = true;
                }
                if (doorLeft != null)
                {
                    doorLeft.GetComponent<Animator>().Play("Doors_Close");
                    doorLeft.GetComponent<Collider2D>().enabled = true;
                }
            }

            isActiveRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isActiveRoom = false;
        }
    }
}