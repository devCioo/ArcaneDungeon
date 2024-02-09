using UnityEngine;

public class CharacterTracker : MonoBehaviour
{
    public static CharacterTracker instance;

    public int currentHealth, maxHealth, currentCoins, currentKeys, currentBombs;
    public Statistics statistics;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadValues();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadValues()
    {
        PlayerHealthController.instance.maxHealth = maxHealth;
        PlayerHealthController.instance.currentHealth = currentHealth;
        LevelManager.instance.currentCoins = currentCoins;
        LevelManager.instance.currentKeys = currentKeys;
        LevelManager.instance.currentBombs = currentBombs;
        PlayerController.instance.stats = statistics;
    }

    public void SaveValues()
    {
        maxHealth = PlayerHealthController.instance.maxHealth;
        currentHealth = PlayerHealthController.instance.currentHealth;
        currentCoins = LevelManager.instance.currentCoins;
        currentKeys = LevelManager.instance.currentKeys;
        currentBombs = LevelManager.instance.currentBombs;
        statistics = PlayerController.instance.stats;
    }
}
