using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string nextLevel;
    public bool statisticsVisible;
    public GameObject statisticsMenu;
    public TMP_Text attemptsStarted, attemptsWon, enemiesDefeated, itemsPickedUp, coinsCollected, keysCollected, bombsCollected, potsDestroyed, coinsSpent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        StatsManager.instance.gameStats.attemptsStarted++;
        StatsManager.instance.SaveStats();
        SceneManager.LoadScene(nextLevel);
    }

    public void ShowHideStatistics()
    {
        StatsManager.instance.LoadStats();
        attemptsStarted.text = StatsManager.instance.gameStats.attemptsStarted.ToString();
        attemptsWon.text = StatsManager.instance.gameStats.attemptsWon.ToString();
        enemiesDefeated.text = StatsManager.instance.gameStats.enemiesDefeated.ToString();
        itemsPickedUp.text = StatsManager.instance.gameStats.itemsPickedUp.ToString();
        coinsCollected.text = StatsManager.instance.gameStats.coinsCollected.ToString();
        keysCollected.text = StatsManager.instance.gameStats.keysCollected.ToString();
        bombsCollected.text = StatsManager.instance.gameStats.bombsCollected.ToString();
        potsDestroyed.text = StatsManager.instance.gameStats.potsDestroyed.ToString();
        coinsSpent.text = StatsManager.instance.gameStats.coinsSpent.ToString();
        statisticsMenu.SetActive(!statisticsMenu.activeSelf);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
