using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelGenerator : MonoBehaviour
{
    public int levelSize;
    public float xOffset = 17f, yOffset = 11f;
    public Color startRoomColor, secretRoomColor, bossRoomColor, treasureRoomColor, shopRoomColor;
    public enum Direction { up, right, down, left }
    public Direction direction;
    private bool canCreate;

    public GameObject roomBase;
    public Transform generatorPoint, oldPoint;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(roomBase, generatorPoint.position, generatorPoint.rotation).GetComponent<SpriteRenderer>().color = startRoomColor;
        GenerateNormalRooms();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void MoveGenerationPoint()
    {
        oldPoint.position = generatorPoint.position;
        switch (direction)
        {
            case Direction.up:
                generatorPoint.position += new Vector3(0f, yOffset, 0f);
                break;

            case Direction.right:
                generatorPoint.position += new Vector3(xOffset, 0f, 0f);
                break;

            case Direction.down:
                generatorPoint.position += new Vector3(0f, -yOffset, 0f);
                break;

            case Direction.left:
                generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
                break;
        }
    }

    public void GenerateNormalRooms()
    {
        for (int i = 0; i < levelSize; i++)
        {
            canCreate = false;

            while (!canCreate)
            {
                direction = (Direction)Random.Range(0, 4);
                MoveGenerationPoint();

                if (Physics2D.OverlapCircle(generatorPoint.position, .2f, layerMask))
                {
                    continue;
                }

                if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x, generatorPoint.position.y + yOffset), .2f, layerMask) &&
                    Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x - xOffset, generatorPoint.position.y), .2f, layerMask))
                {
                    if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x - xOffset, generatorPoint.position.y + yOffset), .2f, layerMask))
                    {
                        generatorPoint.position = oldPoint.position;
                        continue;
                    }
                }
                if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x, generatorPoint.position.y + yOffset), .2f, layerMask) &&
                    Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x + xOffset, generatorPoint.position.y), .2f, layerMask))
                {
                    if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x + xOffset, generatorPoint.position.y + yOffset), .2f, layerMask))
                    {
                        generatorPoint.position = oldPoint.position;
                        continue;
                    }
                }
                if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x, generatorPoint.position.y - yOffset), .2f, layerMask) &&
                    Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x - xOffset, generatorPoint.position.y), .2f, layerMask))
                {
                    if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x - xOffset, generatorPoint.position.y - yOffset), .2f, layerMask))
                    {
                        generatorPoint.position = oldPoint.position;
                        continue;
                    }
                }
                if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x, generatorPoint.position.y - yOffset), .2f, layerMask) &&
                    Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x + xOffset, generatorPoint.position.y), .2f, layerMask))
                {
                    if (Physics2D.OverlapCircle(new Vector2(generatorPoint.position.x + xOffset, generatorPoint.position.y - yOffset), .2f, layerMask))
                    {
                        generatorPoint.position = oldPoint.position;
                        continue;
                    }
                }

                canCreate = true;
            }

            Instantiate(roomBase, generatorPoint.position, generatorPoint.rotation);
        }
    }
}
