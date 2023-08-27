using System.Collections;
using System.Collections.Generic;
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
    public bool isPaused;
    public GameObject bomb;

    public int currentCoins, currentKeys, currentBombs;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        oldRoom = new Vector2Int(6, 6);
        currentRoom = new Vector2Int(6, 6);
        currentRoomX = 0.5f;
        currentRoomY = 0.5f;

        levelGenerator.rooms[currentRoom.x, currentRoom.y].GetComponent<Room>().mapRoom.SetActive(true);

        UIController.instance.coinText.text = currentCoins.ToString();
        UIController.instance.keyText.text = currentBombs.ToString();
        UIController.instance.bombText.text = currentBombs.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseOrResume();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentBombs > 0)
            {
                Instantiate(bomb, PlayerController.instance.transform.position, Quaternion.Euler(0f, 0f, 0f));
                UseBomb();
            }
        }
    }

    public IEnumerator EndLevel()
    {
        PlayerController.instance.canMove = false;
        UIController.instance.StartFadeToBlack();

        yield return new WaitForSeconds(loadTime);

        SceneManager.LoadScene(nextLevel);
    }

    public void PauseOrResume()
    {
        if (!isPaused)
        {
            UIController.instance.pauseMenu.SetActive(true);
            isPaused = true;
            Time.timeScale = 0f;
        }
        else
        {
            UIController.instance.pauseMenu.SetActive(false);
            isPaused = false;
            Time.timeScale = 1f;
        }
    }

    public void RevealSecretDoors()
    {
        Vector2Int secretRoomPosition = levelGenerator.secretRoomPosition;
        if (secretRoomPosition.x > 0)
        {
            if (levelGenerator.rooms[secretRoomPosition.x - 1, secretRoomPosition.y] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x - 1, secretRoomPosition.y].GetComponent<Room>().doorDown;
                door.GetComponent<Door>().isRevealed = true;
                door.GetComponent<Door>().OpenDoor();
            }
        }
        if (secretRoomPosition.y < 12)
        {
            if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y + 1] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y + 1].GetComponent<Room>().doorLeft;
                door.GetComponent<Door>().isRevealed = true;
                door.GetComponent<Door>().OpenDoor();
            }
        }
        if (secretRoomPosition.x < 12)
        {
            if (levelGenerator.rooms[secretRoomPosition.x + 1, secretRoomPosition.y] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x + 1, secretRoomPosition.y].GetComponent<Room>().doorUp;
                door.GetComponent<Door>().isRevealed = true;
                door.GetComponent<Door>().OpenDoor();
            }
        }
        if (secretRoomPosition.y > 0)
        {
            if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y - 1] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y - 1].GetComponent<Room>().doorRight;
                door.GetComponent<Door>().isRevealed = true;
                door.GetComponent<Door>().OpenDoor();
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

    public void GetCoin(int coinValue)
    {
        currentCoins += coinValue;

        UIController.instance.coinText.text = currentCoins.ToString();
    }

    public void SpendCoin(int coinValue)
    {
        currentCoins -= coinValue;

        if (currentCoins < 0)
        {
            currentCoins = 0;
        }

        UIController.instance.coinText.text = currentCoins.ToString();
    }

    public void GetKey()
    {
        currentKeys++;

        UIController.instance.keyText.text = currentKeys.ToString();
    }

    public void UseKey()
    {
        currentKeys--;

        if (currentKeys < 0)
        {
            currentKeys = 0;
        }

        UIController.instance.keyText.text = currentKeys.ToString();
    }

    public void GetBomb()
    {
        currentBombs++;

        UIController.instance.bombText.text = currentBombs.ToString();
    }

    public void UseBomb()
    {
        currentBombs--;

        if (currentBombs < 0)
        {
            currentBombs = 0;
        }

        UIController.instance.bombText.text = currentBombs.ToString();
    }
}
