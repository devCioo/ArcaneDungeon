using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class Room : MonoBehaviour
{
    public bool closeWhenEntered;
    public bool isClosed = false;
    [HideInInspector]
    public bool isActiveRoom;

    public RoomType roomType;
    public RoomCenter roomCenter;
    public GameObject doorUp, doorRight, doorDown, doorLeft;
    public Tilemap tilemap;
    public GameObject mapRoom;

    // Start is called before the first frame update
    void Start()
    {
        if (roomCenter.openWhenEnemiesCleared)
        {
            closeWhenEntered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (roomCenter.enemies.Count > 0 && isActiveRoom)
        {
            for (int i = 0; i < roomCenter.enemies.Count; i++)
            {
                if (roomCenter.enemies[i] == null)
                {
                    roomCenter.enemies.RemoveAt(i);
                    i--;
                }
            }

            if (roomCenter.enemies.Count == 0)
            {
                OpenDoors();
                if (roomType == RoomType.BossRoom)
                {
                    foreach (GameObject itemPedestal in roomCenter.itemPedestals)
                    {
                        itemPedestal.SetActive(true);
                    }
                    
                    roomCenter.GetComponentInChildren<LevelExit>().Open();
                }
            }
        }
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
                    doorUp.GetComponent<Door>().CloseDoor();
                }
                if (doorRight != null)
                {
                    doorRight.GetComponent<Door>().CloseDoor();
                }
                if (doorDown != null)
                {
                    doorDown.GetComponent<Door>().CloseDoor();
                }
                if (doorLeft != null)
                {
                    doorLeft.GetComponent<Door>().CloseDoor();
                }

                isClosed = true;
            }

            isActiveRoom = true;
            roomCenter.SpawnEnemies();
            LevelManager.instance.UpdateCurrentRoomPosition(transform);
            LevelManager.instance.UpdateMap();

            if (roomType == RoomType.BossRoom && !BossManager.instance.isBossDefeated)
            {
                UIController.instance.bossHealthBar.SetActive(true);
            }
            if (roomType == RoomType.SecretRoom)
            {
                LevelManager.instance.RevealSecretDoors();
            }
            if (roomType == RoomType.ShopRoom)
            {
                LevelManager.instance.UnlockOtherDoor(LevelManager.instance.levelGenerator.shopRoomPosition);
            }
            if (roomType == RoomType.ItemRoom)
            {
                LevelManager.instance.UnlockOtherDoor(LevelManager.instance.levelGenerator.itemRoomPosition);
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