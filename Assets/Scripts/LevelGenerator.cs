using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public int levelSize;
    public Color startRoomColor, standardRoomColor, secretRoomColor, bossRoomColor, itemRoomColor, shopRoomColor;
    public enum Direction { up, right, down, left }
    public Direction direction;
    private bool canCreate;

    public GameObject roomBase;
    private int?[,] grid = new int?[13, 13];
    private Vector2Int gridPoint, oldPoint;

    public RoomOutlines outlines;
    public RoomCenter startRoomCenter, bossRoomCenter;
    public RoomCenter[] roomCenters;

    // Start is called before the first frame update
    void Start()
    {
        grid[6, 6] = 0;
        gridPoint = new Vector2Int(6, 6);

        GenerateStandardRooms();
        GenerateSecretRoom();
        GenerateBossRoom();
        GenerateShop();
        GenerateItemRoom();

        CreateLevel();
        CreateRooms();
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
#endif
    }

    public void MoveGridPoint()
    {
        oldPoint = gridPoint;

        direction = (Direction)Random.Range(0, 4);
        switch (direction)
        {
            case Direction.up:
                gridPoint += new Vector2Int(-1, 0);
                break;

            case Direction.right:
                gridPoint += new Vector2Int(0, 1);
                break;

            case Direction.down:
                gridPoint += new Vector2Int(1, 0);
                break;

            case Direction.left:
                gridPoint += new Vector2Int(0, -1);
                break;
        }
        if (gridPoint.x < 0 || gridPoint.x > 12 || gridPoint.y < 0 || gridPoint.y > 12)
        {
            gridPoint = oldPoint;
        }
        Debug.Log("Moved " + direction);
    }

    public void GenerateStandardRooms()
    {
        for (int i = 0; i < levelSize; i++)
        {
            canCreate = false;

            while (!canCreate)
            {
                MoveGridPoint();

                if (grid[gridPoint.x, gridPoint.y] is not null)
                {
                    continue;
                }

                if (gridPoint.x > 0 && gridPoint.y > 0)
                {
                    if (grid[gridPoint.x - 1, gridPoint.y] is not null && grid[gridPoint.x, gridPoint.y - 1] is not null)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y - 1] is not null)
                        {
                            gridPoint = oldPoint;
                            continue;
                        }
                    }
                }
                if (gridPoint.x > 0 && gridPoint.y < 12)
                {
                    if (grid[gridPoint.x - 1, gridPoint.y] is not null && grid[gridPoint.x, gridPoint.y + 1] is not null)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y + 1] is not null)
                        {
                            gridPoint = oldPoint;
                            continue;
                        }
                    }
                }
                if (gridPoint.x < 12 && gridPoint.y > 0)
                {
                    if (grid[gridPoint.x + 1, gridPoint.y] is not null && grid[gridPoint.x, gridPoint.y - 1] is not null)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y - 1] is not null)
                        {
                            gridPoint = oldPoint;
                            continue;
                        }
                    }
                }
                if (gridPoint.x < 12 && gridPoint.y < 12)
                {
                    if (grid[gridPoint.x + 1, gridPoint.y] is not null && grid[gridPoint.x, gridPoint.y + 1] is not null)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y + 1] is not null)
                        {
                            gridPoint = oldPoint;
                            continue;
                        }
                    }
                }

                canCreate = true;
            }

            grid[gridPoint.x, gridPoint.y] = 1;
            Debug.Log($"Created room at [{gridPoint.x},{gridPoint.y}]");
        }
    }

    public void GenerateSecretRoom()
    {
        List<Vector2Int> matchingPoints = GetSecretRoomMatchingPoints(4);
        if (matchingPoints.Count == 0)
        {
            matchingPoints = GetSecretRoomMatchingPoints(3);
            if (matchingPoints.Count == 0)
            {
                matchingPoints = GetSecretRoomMatchingPoints(2);
                if (matchingPoints.Count == 0)
                {
                    matchingPoints = GetSecretRoomMatchingPoints(1);
                }
            }
        }

        int selected = Random.Range(0, matchingPoints.Count);
        Debug.Log($"Selected point has index {selected}");
        Vector2Int selectedPoint = matchingPoints[selected];
        grid[selectedPoint.x, selectedPoint.y] = 2;
        Debug.Log($"Created secret room at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void GenerateBossRoom()
    {
        List<Vector2Int> matchingPoints = GetBossRoomMatchingPoints();
        Vector2Int selectedPoint = FindFurthestPointFromStart(matchingPoints);
        grid[selectedPoint.x, selectedPoint.y] = 3;
        Debug.Log($"Created boss room at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void GenerateShop()
    {
        List<Vector2Int> matchingPoints = GetShopRoomMatchingPoints();
        int selected = Random.Range(0, matchingPoints.Count);
        Debug.Log($"Selected point has index {selected}");
        Vector2Int selectedPoint = matchingPoints[selected];
        grid[selectedPoint.x, selectedPoint.y] = 4;
        Debug.Log($"Created shop at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void GenerateItemRoom()
    {
        List<Vector2Int> matchingPoints = GetItemRoomMatchingPoints();
        int selected = Random.Range(0, matchingPoints.Count);
        Debug.Log($"Selected point has index {selected}");
        Vector2Int selectedPoint = matchingPoints[selected];
        grid[selectedPoint.x, selectedPoint.y] = 5;
        Debug.Log($"Created item room at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void CreateLevel()
    {
        float xOffset = 17f, yOffset = 11f;
        Vector2 roomPosition = new Vector2(-101.5f, 66.5f);

        for (int i = 0; i < 13; i++)
        {
            for ( int j = 0; j < 13; j++)
            {  
                switch (grid[i, j])
                {
                    case null:
                        break;

                    case 0:
                        Instantiate(roomBase, roomPosition, Quaternion.Euler(0f, 0f, 0f)).GetComponent<SpriteRenderer>().color = startRoomColor;
                        break;

                    case 1:
                        Instantiate(roomBase, roomPosition, Quaternion.Euler(0f, 0f, 0f)).GetComponent<SpriteRenderer>().color = standardRoomColor;
                        break;

                    case 2:
                        Instantiate(roomBase, roomPosition, Quaternion.Euler(0f, 0f, 0f)).GetComponent<SpriteRenderer>().color = secretRoomColor;
                        break;

                    case 3:
                        Instantiate(roomBase, roomPosition, Quaternion.Euler(0f, 0f, 0f)).GetComponent<SpriteRenderer>().color = bossRoomColor;
                        break;

                    case 4:
                        Instantiate(roomBase, roomPosition, Quaternion.Euler(0f, 0f, 0f)).GetComponent<SpriteRenderer>().color = shopRoomColor;
                        break;

                    case 5:
                        Instantiate(roomBase, roomPosition, Quaternion.Euler(0f, 0f, 0f)).GetComponent<SpriteRenderer>().color = itemRoomColor;
                        break;
                }

                roomPosition += new Vector2(xOffset, 0f);
            }
            roomPosition = new Vector2(-101.5f, roomPosition.y - yOffset);
        }
    }

    public void CreateRooms()
    {
        bool isRoomUp = false, isRoomRight = false, isRoomDown = false, isRoomLeft = false;
        int directionCount = 0, selectedCenter = 0;
        float xOffset = 17f, yOffset = 11f;
        Vector2 roomPosition = new Vector2(-101.5f, 66.5f);

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                isRoomUp = isRoomRight = isRoomDown = isRoomLeft = false;
                directionCount = 0;
                gridPoint = new Vector2Int(i, j);
                if (grid[gridPoint.x, gridPoint.y] is not null)
                {
                    if (gridPoint.x > 0)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y] is not null && grid[gridPoint.x - 1, gridPoint.y] != 2)
                        {
                            isRoomUp = true;
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null && grid[gridPoint.x, gridPoint.y + 1] != 2)
                        {
                            isRoomRight = true;
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null && grid[gridPoint.x + 1, gridPoint.y] != 2)
                        {
                            isRoomDown = true;
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null && grid[gridPoint.x, gridPoint.y - 1] != 2)
                        {
                            isRoomLeft = true;
                        }
                    }

                    if (isRoomUp)
                    {
                        directionCount++;
                    }
                    if (isRoomRight)
                    {
                        directionCount++;
                    }
                    if (isRoomDown)
                    {
                        directionCount++;
                    }
                    if (isRoomLeft)
                    {
                        directionCount++;
                    }

                    if (grid[gridPoint.x, gridPoint.y] == 2)
                    {
                        directionCount = 0;
                    }

                    GameObject outline = new GameObject();
                    switch (directionCount)
                    {
                        case 0:
                            outline = Instantiate(outlines.noDoor, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            break;
                        case 1:
                            if (isRoomUp)
                            {
                                outline = Instantiate(outlines.singleUp, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight)
                            {
                                outline = Instantiate(outlines.singleRight, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomDown)
                            {
                                outline = Instantiate(outlines.singleDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomLeft)
                            {
                                outline = Instantiate(outlines.singleLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;

                        case 2:
                            if (isRoomUp && isRoomRight)
                            {
                                outline = Instantiate(outlines.doubleUpRight, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight && isRoomDown)
                            {
                                outline = Instantiate(outlines.doubleRightDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomDown && isRoomLeft)
                            {
                                outline = Instantiate(outlines.doubleDownLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomLeft && isRoomUp)
                            {
                                outline = Instantiate(outlines.doubleLeftUp, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomUp && isRoomDown)
                            {
                                outline = Instantiate(outlines.doubleUpDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight && isRoomLeft)
                            {
                                outline = Instantiate(outlines.doubleRightLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;

                        case 3:
                            if (isRoomUp && isRoomRight && isRoomDown)
                            {
                                outline = Instantiate(outlines.tripleUpRightDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight && isRoomDown && isRoomLeft)
                            {
                                outline = Instantiate(outlines.tripleRightDownLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomDown && isRoomLeft && isRoomUp)
                            {
                                outline = Instantiate(outlines.tripleDownLeftUp, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomLeft && isRoomUp && isRoomRight)
                            {
                                outline = Instantiate(outlines.tripleLeftUpRight, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;

                        case 4:
                            if (isRoomUp && isRoomRight && isRoomDown && isRoomLeft)
                            {
                                outline = Instantiate(outlines.quadruple, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;
                    }

                    if (grid[gridPoint.x, gridPoint.y] == 0)
                    {
                        Instantiate(startRoomCenter, roomPosition, Quaternion.Euler(0f, 0f, 0f)).room = outline.GetComponent<Room>();
                    }
                    else if (grid[gridPoint.x, gridPoint.y] == 3)
                    {
                        Instantiate(bossRoomCenter, roomPosition, Quaternion.Euler(0f, 0f, 0f)).room = outline.GetComponent<Room>();
                    }
                    else
                    {
                        selectedCenter = Random.Range(0, roomCenters.Length);
                        Instantiate(roomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f)).room = outline.GetComponent<Room>();
                    }
                }

                roomPosition += new Vector2(xOffset, 0f);
            }

            roomPosition = new Vector2(-101.5f, roomPosition.y - yOffset);
        }
    }

    public List<Vector2Int> GetSecretRoomMatchingPoints(int matchValue)
    {
        Debug.Log($"Looking for points with {matchValue} adjacent room/s...");
        List<Vector2Int> points = new List<Vector2Int>();
        int adjacentRooms = 0;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);
                if (grid[gridPoint.x, gridPoint.y] is null)
                {
                    adjacentRooms = 0;
                    if (gridPoint.x > 0)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y] is not null)
                        {
                            adjacentRooms++;
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null)
                        {
                            adjacentRooms++;
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null)
                        {
                            adjacentRooms++;
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null)
                        {
                            adjacentRooms++;
                        }
                    }
                    if (adjacentRooms == matchValue)
                    {
                        points.Add(gridPoint);
                        Debug.Log($"Added point [{gridPoint.x}, {gridPoint.y}] to the list");
                    }
                }
            }
        }

        return points;
    }

    public List<Vector2Int> GetBossRoomMatchingPoints()
    {
        Debug.Log("Looking for points to spawn a boss room...");
        List<Vector2Int> points = new List<Vector2Int>();
        int adjacentRooms = 0;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);
                if (grid[gridPoint.x, gridPoint.y] is null)
                {
                    adjacentRooms = 0;
                    if (gridPoint.x > 0)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x - 1, gridPoint.y] == 2)
                            {
                                continue;
                            }
                            else
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y + 1] == 2)
                            {
                                continue;
                            }
                            else
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x + 1, gridPoint.y] == 2)
                            {
                                continue;
                            }
                            else
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y - 1] == 2)
                            {
                                continue;
                            }
                            else
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (adjacentRooms == 1)
                    {
                        points.Add(gridPoint);
                        Debug.Log($"Added point [{gridPoint.x}, {gridPoint.y}] to the list");
                    }
                }
            }
        }

        return points;
    }

    public List<Vector2Int> GetShopRoomMatchingPoints()
    {
        Debug.Log("Looking for points to spawn a shop...");
        List<Vector2Int> points = new List<Vector2Int>();
        int adjacentRooms = 0;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);
                if (grid[gridPoint.x, gridPoint.y] is null)
                {
                    adjacentRooms = 0;
                    if (gridPoint.x > 0)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x - 1, gridPoint.y] == 3 || grid[gridPoint.x - 1, gridPoint.y] == 0)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x - 1, gridPoint.y] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y + 1] == 3 || grid[gridPoint.x, gridPoint.y + 1] == 0)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y + 1] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x + 1, gridPoint.y] == 3 || grid[gridPoint.x + 1, gridPoint.y] == 0)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x + 1, gridPoint.y] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y - 1] == 3 || grid[gridPoint.x, gridPoint.y - 1] == 0)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y - 1] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (adjacentRooms == 1)
                    {
                        points.Add(gridPoint);
                        Debug.Log($"Added point [{gridPoint.x}, {gridPoint.y}] to the list");
                    }
                }
            }
        }

        return points;
    }

    public List<Vector2Int> GetItemRoomMatchingPoints()
    {
        Debug.Log("Looking for points to spawn an item room...");
        List<Vector2Int> points = new List<Vector2Int>();
        int adjacentRooms = 0;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);
                if (grid[gridPoint.x, gridPoint.y] is null)
                {
                    adjacentRooms = 0;
                    if (gridPoint.x > 0)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x - 1, gridPoint.y] == 3 || grid[gridPoint.x - 1, gridPoint.y] == 4)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x - 1, gridPoint.y] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y + 1] == 3 || grid[gridPoint.x, gridPoint.y + 1] == 4)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y + 1] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x + 1, gridPoint.y] == 3 || grid[gridPoint.x + 1, gridPoint.y] == 4)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x + 1, gridPoint.y] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y - 1] == 3 || grid[gridPoint.x, gridPoint.y - 1] == 4)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y - 1] != 2)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (adjacentRooms == 1)
                    {
                        points.Add(gridPoint);
                        Debug.Log($"Added point [{gridPoint.x}, {gridPoint.y}] to the list");
                    }
                }
            }
        }

        return points;
    }

    public Vector2Int FindFurthestPointFromStart(List<Vector2Int> points)
    {
        Vector2Int furthestPoint = new Vector2Int(6, 6);
        float distance = 0f, pointDistance = 0f;

        foreach (Vector2Int point in points)
        {
            pointDistance = Vector2Int.Distance(point, new Vector2Int(6, 6));
            Debug.Log($"Distance between start (6, 6) and point ({point.x},{point.y}) is {pointDistance}");
            if (pointDistance > distance)
            {
                distance = pointDistance;
                furthestPoint = point;
                Debug.Log($"Furthest point is now point ({furthestPoint.x},{furthestPoint.y})");
            }
            else if (pointDistance == distance)
            {
                Debug.Log($"Point ({point.x},{point.y} has the same distance as current furthest point ({furthestPoint.x},{furthestPoint.y})");
                if (Random.Range(0, 2) == 1)
                {
                    Debug.Log("Changing to new point");
                    furthestPoint = point;
                }
            }
        }

        return furthestPoint;
    }
}

[System.Serializable]
public class RoomOutlines
{
    public GameObject noDoor, singleUp, singleRight, singleDown, singleLeft,
        doubleUpRight, doubleRightDown, doubleDownLeft, doubleLeftUp, doubleUpDown, doubleRightLeft,
        tripleUpRightDown, tripleRightDownLeft, tripleDownLeftUp, tripleLeftUpRight, quadruple;
}