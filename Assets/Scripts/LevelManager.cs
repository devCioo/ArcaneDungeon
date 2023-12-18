using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelGenerator levelGenerator;

    public float loadTime = 4f;
    public Vector2Int oldRoom, currentRoom;
    public float currentRoomX, currentRoomY;
    public string nextLevel;


    public int currentCoins, currentKeys, currentBombs;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CharacterTracker.instance.LoadValues();

        Time.timeScale = 1f;
        oldRoom = new Vector2Int(6, 6);
        currentRoom = new Vector2Int(6, 6);
        currentRoomX = 0.5f;
        currentRoomY = 0.5f;

        levelGenerator.rooms[currentRoom.x, currentRoom.y].GetComponent<Room>().mapRoom.SetActive(true);

        UIController.instance.UpdateStatistics(PlayerController.instance.stats, PlayerController.instance.stats);
        UIController.instance.coinText.text = currentCoins > 9 ? currentCoins.ToString() : $"0{currentCoins.ToString()}";
        UIController.instance.keyText.text = currentKeys > 9 ? currentKeys.ToString() : $"0{currentKeys.ToString()}";
        UIController.instance.bombText.text = currentBombs > 9 ? currentBombs.ToString() : $"0{currentBombs.ToString()}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator EndLevel()
    {
        PlayerController.instance.canMove = false;
        UIController.instance.StartFadeToBlack();

        yield return new WaitForSeconds(loadTime);

        CharacterTracker.instance.SaveValues();
        SceneManager.LoadScene(nextLevel);
    }

    public void RevealSecretDoors()
    {
        Vector2Int secretRoomPosition = levelGenerator.secretRoomPosition;

        if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y].GetComponent<Room>().doorUp != null)
        {
            GameObject door = levelGenerator.rooms[secretRoomPosition.x - 1, secretRoomPosition.y].GetComponent<Room>().doorDown;
            door.GetComponent<Door>().isRevealed = true;
            door.GetComponent<Door>().OpenDoor();
        }
        
        if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y].GetComponent<Room>().doorRight != null)
        {
            GameObject door = levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y + 1].GetComponent<Room>().doorLeft;
            door.GetComponent<Door>().isRevealed = true;
            door.GetComponent<Door>().OpenDoor();
        }
        if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y].GetComponent<Room>().doorDown != null)
        {
            GameObject door = levelGenerator.rooms[secretRoomPosition.x + 1, secretRoomPosition.y].GetComponent<Room>().doorUp;
            door.GetComponent<Door>().isRevealed = true;
            door.GetComponent<Door>().OpenDoor();
        }
        if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y].GetComponent<Room>().doorLeft != null)
        {
            GameObject door = levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y - 1].GetComponent<Room>().doorRight;
            door.GetComponent<Door>().isRevealed = true;
            door.GetComponent<Door>().OpenDoor();
        }
    }

    public void UnlockOtherDoor(Vector2Int roomPosition)
    {
        GameObject room = levelGenerator.rooms[roomPosition.x, roomPosition.y];

        if (room.GetComponent<Room>().doorUp != null)
        {
            GameObject door = room.GetComponent<Room>().doorUp;

            if (door.GetComponent<Door>().isShopOrItemDoor)
            {
                GameObject otherDoor = levelGenerator.rooms[roomPosition.x - 1, roomPosition.y].GetComponent<Room>().doorDown;
                otherDoor.GetComponent<Door>().isLocked = false;
                otherDoor.GetComponent<Door>().OpenDoor();
            }
        }
        if (room.GetComponent<Room>().doorRight != null)
        {
            GameObject door = room.GetComponent<Room>().doorRight;

            if (door.GetComponent<Door>().isShopOrItemDoor)
            {
                GameObject otherDoor = levelGenerator.rooms[roomPosition.x, roomPosition.y + 1].GetComponent<Room>().doorLeft;
                otherDoor.GetComponent<Door>().isLocked = false;
                otherDoor.GetComponent<Door>().OpenDoor();
            }
        }
        if (room.GetComponent<Room>().doorDown != null)
        {
            GameObject door = room.GetComponent<Room>().doorDown;

            if (door.GetComponent<Door>().isShopOrItemDoor)
            {
                GameObject otherDoor = levelGenerator.rooms[roomPosition.x + 1, roomPosition.y].GetComponent<Room>().doorUp;
                otherDoor.GetComponent<Door>().isLocked = false;
                otherDoor.GetComponent<Door>().OpenDoor();
            }
        }
        if (room.GetComponent<Room>().doorLeft != null)
        {
            GameObject door = room.GetComponent<Room>().doorLeft;

            if (door.GetComponent<Door>().isShopOrItemDoor)
            {
                GameObject otherDoor = levelGenerator.rooms[roomPosition.x, roomPosition.y - 1].GetComponent<Room>().doorRight;
                otherDoor.GetComponent<Door>().isLocked = false;
                otherDoor.GetComponent<Door>().OpenDoor();
            }
        }
    }

    public void UpdateCurrentRoomPosition(Transform transform)
    {
        oldRoom = currentRoom;

        if (transform.position.y > currentRoomY)
        {
            currentRoom = new Vector2Int(currentRoom.x - 1, currentRoom.y);
        }
        if (transform.position.x > currentRoomX)
        {
            currentRoom = new Vector2Int(currentRoom.x, currentRoom.y + 1);
        }
        if (transform.position.y < currentRoomY)
        {
            currentRoom = new Vector2Int(currentRoom.x + 1, currentRoom.y);
        }
        if (transform.position.x < currentRoomX)
        {
            currentRoom = new Vector2Int(currentRoom.x, currentRoom.y - 1);
        }

        currentRoomX = transform.position.x;
        currentRoomY = transform.position.y;
    }

    public void UpdateMap()
    {
        levelGenerator.rooms[oldRoom.x, oldRoom.y].GetComponent<Room>().mapRoom.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
        levelGenerator.rooms[currentRoom.x, currentRoom.y].GetComponent<Room>().mapRoom.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);

        if (currentRoom.x > 0)
        {
            if (levelGenerator.rooms[currentRoom.x - 1, currentRoom.y] != null && levelGenerator.rooms[currentRoom.x - 1, currentRoom.y].GetComponent<Room>().roomType != RoomType.SecretRoom)
            {
                levelGenerator.rooms[currentRoom.x - 1, currentRoom.y].GetComponent<Room>().mapRoom.SetActive(true);
            }
        }
        if (currentRoom.y < 12)
        {
            if (levelGenerator.rooms[currentRoom.x, currentRoom.y + 1] != null && levelGenerator.rooms[currentRoom.x, currentRoom.y + 1].GetComponent<Room>().roomType != RoomType.SecretRoom)
            {
                levelGenerator.rooms[currentRoom.x, currentRoom.y + 1].GetComponent<Room>().mapRoom.SetActive(true);
            }
        }
        if (currentRoom.x < 12)
        {
            if (levelGenerator.rooms[currentRoom.x + 1, currentRoom.y] != null && levelGenerator.rooms[currentRoom.x + 1, currentRoom.y].GetComponent<Room>().roomType != RoomType.SecretRoom)
            {
                levelGenerator.rooms[currentRoom.x + 1, currentRoom.y].GetComponent<Room>().mapRoom.SetActive(true);
            }
        }
        if (currentRoom.y > 0)
        {
            if (levelGenerator.rooms[currentRoom.x, currentRoom.y - 1] != null && levelGenerator.rooms[currentRoom.x, currentRoom.y - 1].GetComponent<Room>().roomType != RoomType.SecretRoom)
            {
                levelGenerator.rooms[currentRoom.x, currentRoom.y - 1].GetComponent<Room>().mapRoom.SetActive(true);
            }
        }
    }

    public void GetCoins(int coinValue)
    {
        currentCoins += coinValue;

        UIController.instance.coinText.text = currentCoins > 9 ? currentCoins.ToString() : $"0{currentCoins.ToString()}";
    }

    public void SpendCoins(int coinValue)
    {
        currentCoins -= coinValue;

        if (currentCoins < 0)
        {
            currentCoins = 0;
        }

        UIController.instance.coinText.text = currentCoins > 9 ? currentCoins.ToString() : $"0{currentCoins.ToString()}";
    }

    public void GetKey()
    {
        currentKeys++;

        UIController.instance.keyText.text = currentKeys > 9 ? currentKeys.ToString() : $"0{currentKeys.ToString()}";
    }

    public void UseKey()
    {
        currentKeys--;

        if (currentKeys < 0)
        {
            currentKeys = 0;
        }

        UIController.instance.keyText.text = currentKeys > 9 ? currentKeys.ToString() : $"0{currentKeys.ToString()}";
    }

    public void GetBomb()
    {
        currentBombs++;

        UIController.instance.bombText.text = currentBombs > 9 ? currentBombs.ToString() : $"0{currentBombs.ToString()}";
    }

    public void UseBomb()
    {
        currentBombs--;

        if (currentBombs < 0)
        {
            currentBombs = 0;
        }

        UIController.instance.bombText.text = currentBombs > 9 ? currentBombs.ToString() : $"0{currentBombs.ToString()}";
    }
}
