using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{
    public float waitForAnyKey = 2f;

    public GameObject anyKeyText;
    public string mainMenuScene;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(WaitForAnyKey());
    }

    // Update is called once per frame
    void Update()
    {
        if (anyKeyText.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                SceneManager.LoadScene(mainMenuScene);
            }
        }
    }

    public IEnumerator WaitForAnyKey()
    {
        yield return new WaitForSeconds(2f);

        anyKeyText.SetActive(true);
    }
}
