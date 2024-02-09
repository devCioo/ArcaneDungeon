using System.IO;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager instance;
    private static string filePath;
    public GameStats gameStats;

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "gameStats.json");
        instance = this;
        LoadStats();
    }

    public void SaveStats()
    {
        string json = JsonUtility.ToJson(gameStats);
        File.WriteAllText(filePath, json);
    }

    public void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameStats = JsonUtility.FromJson<GameStats>(json);
        }
        else
        {
            gameStats = new GameStats();
        }
    }
}

[System.Serializable]
public class GameStats
{
    public int attemptsStarted = 0;
    public int attemptsWon = 0;
    public int enemiesDefeated = 0;
    public int itemsPickedUp = 0;
    public int coinsCollected = 0;
    public int keysCollected = 0;
    public int bombsCollected = 0;
    public int potsDestroyed = 0;
    public int coinsSpent = 0;
}
