using System.Collections.Generic;
using UnityEngine;

public class RoomCenter : MonoBehaviour
{
    public bool openWhenEnemiesCleared;

    public List<GameObject> enemies;
    public bool hasItemPedestals;
    public List<GameObject> itemPedestals;

    private void Awake()
    {
        if (itemPedestals.Count > 0)
        {
            hasItemPedestals = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetActive(true);
        }
    }
}