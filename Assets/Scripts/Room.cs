using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour
{
    public bool closeWhenEntered, openWhenEnemiesCleared;
    private bool isActiveRoom;

    public Tilemap tilemap;
    public TileBase doorUp, doorDown, doorLeft, doorRight;
    public TileBase closedDoorUp, closedDoorDown, closedDoorLeft, closedDoorRight;

    public List<GameObject> enemies = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Count > 0 && isActiveRoom && openWhenEnemiesCleared)
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
                if (tilemap.GetTile(new Vector3Int(0, 4, 0)) == closedDoorUp)
                {
                    tilemap.SetTile(new Vector3Int(0, 4, 0), doorUp);
                }
                if (tilemap.GetTile(new Vector3Int(0, -4, 0)) == closedDoorDown)
                {
                    tilemap.SetTile(new Vector3Int(0, -4, 0), doorDown);
                }
                if (tilemap.GetTile(new Vector3Int(-7, 0, 0)) == closedDoorLeft)
                {
                    tilemap.SetTile(new Vector3Int(-7, 0, 0), doorLeft);
                }
                if (tilemap.GetTile(new Vector3Int(7, 0, 0)) == closedDoorRight)
                {
                    tilemap.SetTile(new Vector3Int(7, 0, 0), doorRight);
                }

                closeWhenEntered = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.instance.ChangeTarget(transform);

            if (closeWhenEntered)
            {
                if (tilemap.GetTile(new Vector3Int(0, 4, 0)) == doorUp)
                {
                    tilemap.SetTile(new Vector3Int(0, 4, 0), closedDoorUp);
                }
                if (tilemap.GetTile(new Vector3Int(0, -4, 0)) == doorDown)
                {
                    tilemap.SetTile(new Vector3Int(0, -4, 0), closedDoorDown);
                }
                if (tilemap.GetTile(new Vector3Int(-7, 0, 0)) == doorLeft)
                {
                    tilemap.SetTile(new Vector3Int(-7, 0, 0), closedDoorLeft);
                }
                if (tilemap.GetTile(new Vector3Int(7, 0, 0)) == doorRight)
                {
                    tilemap.SetTile(new Vector3Int(7, 0, 0), closedDoorRight);
                }
            }

            isActiveRoom = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isActiveRoom = false;
        }
    }
}
