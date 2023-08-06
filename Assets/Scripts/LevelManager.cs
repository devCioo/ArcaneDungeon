using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float loadTime = 4f;
    public string nextLevel;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

        SceneManager.LoadScene(nextLevel);
    }
}
