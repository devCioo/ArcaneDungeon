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
    public GameObject mapRoom;

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
            if (doorUp.GetComponent<SecretDoors>() != null)
            {
                if (doorUp.GetComponent<SecretDoors>().areRevealed == true)
                {
                    doorUp.GetComponent<Animator>().Play("Doors_Open");
                    doorUp.GetComponent<Collider2D>().enabled = false;
                }
            }
            else
            {
                doorUp.GetComponent<Animator>().Play("Doors_Open");
                doorUp.GetComponent<Collider2D>().enabled = false;
            }
        }
        if (doorRight != null)
        {
            if (doorRight.GetComponent<SecretDoors>() != null)
            {
                if (doorRight.GetComponent<SecretDoors>().areRevealed == true)
                {
                    doorRight.GetComponent<Animator>().Play("Doors_Open");
                    doorRight.GetComponent<Collider2D>().enabled = false;
                }
            }
            else
            {
                doorRight.GetComponent<Animator>().Play("Doors_Open");
                doorRight.GetComponent<Collider2D>().enabled = false;
            }
        }
        if (doorDown != null)
        {
            if (doorDown.GetComponent<SecretDoors>() != null)
            {
                if (doorDown.GetComponent<SecretDoors>().areRevealed == true)
                {
                    doorDown.GetComponent<Animator>().Play("Doors_Open");
                    doorDown.GetComponent<Collider2D>().enabled = false;
                }
            }
            else
            {
                doorDown.GetComponent<Animator>().Play("Doors_Open");
                doorDown.GetComponent<Collider2D>().enabled = false;
            }
        }
        if (doorLeft != null)
        {
            if (doorLeft.GetComponent<SecretDoors>() != null)
            {
                if (doorLeft.GetComponent<SecretDoors>().areRevealed == true)
                {
                    doorLeft.GetComponent<Animator>().Play("Doors_Open");
                    doorLeft.GetComponent<Collider2D>().enabled = false;
                }
            }
            else
            {
                doorLeft.GetComponent<Animator>().Play("Doors_Open");
                doorLeft.GetComponent<Collider2D>().enabled = false;
            }
        }

        closeWhenEntered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.instance.ChangeTarget(transform);
            MapController.instance.ChangeTarget(mapRoom.transform);

            if (closeWhenEntered)
            {
                if (doorUp != null)
                {
                    if (doorUp.GetComponent<SecretDoors>() != null)
                    {
                        Debug.Log("Doors up are secret doors");
                        if (doorUp.GetComponent<SecretDoors>().areRevealed == true)
                        {
                            doorUp.GetComponent<Animator>().Play("Doors_Close");
                            doorUp.GetComponent<Collider2D>().enabled = true;
                        }
                    }
                    else
                    {
                        doorUp.GetComponent<Animator>().Play("Doors_Close");
                        doorUp.GetComponent<Collider2D>().enabled = true;
                    }
                }
                if (doorRight != null)
                {
                    if (doorRight.GetComponent<SecretDoors>() != null)
                    {
                        Debug.Log("Doors right are secret doors");
                        if (doorRight.GetComponent<SecretDoors>().areRevealed == true)
                        {
                            doorRight.GetComponent<Animator>().Play("Doors_Close");
                            doorRight.GetComponent<Collider2D>().enabled = true;
                        }
                    }
                    else
                    {
                        doorRight.GetComponent<Animator>().Play("Doors_Close");
                        doorRight.GetComponent<Collider2D>().enabled = true;
                    }
                }
                if (doorDown != null)
                {
                    if (doorDown.GetComponent<SecretDoors>() != null)
                    {
                        Debug.Log("Doors down are secret doors");
                        if (doorDown.GetComponent<SecretDoors>().areRevealed == true)
                        {
                            doorDown.GetComponent<Animator>().Play("Doors_Close");
                            doorDown.GetComponent<Collider2D>().enabled = true;
                        }
                    }
                    else
                    {
                        doorDown.GetComponent<Animator>().Play("Doors_Close");
                        doorDown.GetComponent<Collider2D>().enabled = true;
                    }
                }
                if (doorLeft != null)
                {
                    if (doorLeft.GetComponent<SecretDoors>() != null)
                    {
                        if (doorLeft.GetComponent<SecretDoors>().areRevealed == true)
                        {
                            Debug.Log("Doors left are secret doors");
                            doorLeft.GetComponent<Animator>().Play("Doors_Close");
                            doorLeft.GetComponent<Collider2D>().enabled = true;
                        }
                    }
                    else
                    {
                        doorLeft.GetComponent<Animator>().Play("Doors_Close");
                        doorLeft.GetComponent<Collider2D>().enabled = true;
                    }
                }
            }

            isActiveRoom = true;
            LevelManager.instance.UpdateCurrentRoomPosition(transform);
            LevelManager.instance.UpdateMap();

            if (roomType == RoomType.SecretRoom)
            {
                LevelManager.instance.RevealSecretDoors();
            }
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