using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public LevelGenerator levelGenerator;

    public float loadTime = 4f;
    public string nextLevel;
    public bool isPaused;

    public int currentCoins, currentKeys, currentBombs;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

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
                door.GetComponent<SecretDoors>().areRevealed = true;
                door.GetComponent<BoxCollider2D>().enabled = false;
                door.GetComponent<Animator>().Play("Doors_Open");
            }
        }
        if (secretRoomPosition.y < 12)
        {
            if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y + 1] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y + 1].GetComponent<Room>().doorLeft;
                door.GetComponent<SecretDoors>().areRevealed = true;
                door.GetComponent<BoxCollider2D>().enabled = false;
                door.GetComponent<Animator>().Play("Doors_Open");
            }
        }
        if (secretRoomPosition.x < 12)
        {
            if (levelGenerator.rooms[secretRoomPosition.x + 1, secretRoomPosition.y] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x + 1, secretRoomPosition.y].GetComponent<Room>().doorUp;
                door.GetComponent<SecretDoors>().areRevealed = true;
                door.GetComponent<BoxCollider2D>().enabled = false;
                door.GetComponent<Animator>().Play("Doors_Open");
            }
        }
        if (secretRoomPosition.y > 0)
        {
            if (levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y - 1] != null)
            {
                GameObject door = levelGenerator.rooms[secretRoomPosition.x, secretRoomPosition.y - 1].GetComponent<Room>().doorRight;
                door.GetComponent<SecretDoors>().areRevealed = true;
                door.GetComponent<BoxCollider2D>().enabled = false;
                door.GetComponent<Animator>().Play("Doors_Open");
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
