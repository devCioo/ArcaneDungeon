using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomCenter : MonoBehaviour
{
    public bool openWhenEnemiesCleared;

    public List<GameObject> enemies = new List<GameObject>();

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