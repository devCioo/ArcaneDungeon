using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomCenter : MonoBehaviour
{
    public bool openWhenEnemiesCleared;

    public List<GameObject> enemies = new List<GameObject>();

    public Room room;

    // Start is called before the first frame update
    void Start()
    {
        if (openWhenEnemiesCleared)
        {
            room.closeWhenEntered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Count > 0 && room.isActiveRoom && openWhenEnemiesCleared)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] == null)
                {
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            if (enemies.Count == 0)
            {
                room.OpenDoors();
            }
        }
    }
}