using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public bool closeWhenEntered;
    public bool isClosed = false;
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
            doorUp.GetComponent<Door>().OpenDoor();
        }
        if (doorRight != null)
        {
            doorRight.GetComponent<Door>().OpenDoor();
        }
        if (doorDown != null)
        {
            doorDown.GetComponent<Door>().OpenDoor();
        }
        if (doorLeft != null)
        {
            doorLeft.GetComponent<Door>().OpenDoor();
        }

        isClosed = false;
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
                    doorUp.GetComponent<Door>().OpenDoor();
                }
                if (doorRight != null)
                {
                    doorRight.GetComponent<Door>().OpenDoor();
                }
                if (doorDown != null)
                {
                    doorDown.GetComponent<Door>().OpenDoor();
                }
                if (doorLeft != null)
                {
                    doorLeft.GetComponent<Door>().OpenDoor();
                }

                isClosed = true;
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