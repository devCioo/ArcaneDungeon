using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    public int levelSize;
    private Direction direction;
    private bool canCreate;

    private int?[,] grid = new int?[13, 13];
    private Vector2Int gridPoint, oldPoint;

    public GameObject[,] rooms = new GameObject[13, 13];
    public Vector2Int secretRoomPosition;

    public RoomOutlines outlines;
    public GameObject bossRoomDoor, shopRoomDoor, itemRoomDoor, secretRoomDoor;
    public RoomCenter startRoomCenter, bossRoomCenter;
    public RoomCenter[] roomCenters;

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        CreateOutlines();
        SetRoomTypes();
        CreateCenters();
        ReplaceDoorsVariants();
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

    public void GenerateGrid()
    {
        grid[6, 6] = (int)RoomType.StartingRoom;
        gridPoint = new Vector2Int(6, 6);

        GenerateStandardRooms();
        GenerateSecretRoom();
        GenerateBossRoom();
        GenerateShop();
        GenerateItemRoom();
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

            grid[gridPoint.x, gridPoint.y] = (int)RoomType.NormalRoom;
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
        grid[selectedPoint.x, selectedPoint.y] = (int)RoomType.SecretRoom;
        secretRoomPosition = selectedPoint;
        Debug.Log($"Created secret room at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void GenerateBossRoom()
    {
        List<Vector2Int> matchingPoints = GetBossRoomMatchingPoints();
        Vector2Int selectedPoint = FindFurthestPointFromStart(matchingPoints);
        grid[selectedPoint.x, selectedPoint.y] = (int)RoomType.BossRoom;
        Debug.Log($"Created boss room at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void GenerateShop()
    {
        List<Vector2Int> matchingPoints = GetShopRoomMatchingPoints();
        int selected = Random.Range(0, matchingPoints.Count);
        Debug.Log($"Selected point has index {selected}");
        Vector2Int selectedPoint = matchingPoints[selected];
        grid[selectedPoint.x, selectedPoint.y] = (int)RoomType.ShopRoom;
        Debug.Log($"Created shop at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void GenerateItemRoom()
    {
        List<Vector2Int> matchingPoints = GetItemRoomMatchingPoints();
        int selected = Random.Range(0, matchingPoints.Count);
        Debug.Log($"Selected point has index {selected}");
        Vector2Int selectedPoint = matchingPoints[selected];
        grid[selectedPoint.x, selectedPoint.y] = (int)RoomType.ItemRoom;
        Debug.Log($"Created item room at [{selectedPoint.x},{selectedPoint.y}]");
    }

    public void CreateOutlines()
    {
        bool isRoomUp = false, isRoomRight = false, isRoomDown = false, isRoomLeft = false;
        int directionCount = 0;
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

                    directionCount += (isRoomUp ? 1 : 0) + (isRoomRight ? 1 : 0) + (isRoomDown ? 1 : 0) + (isRoomLeft ? 1 : 0);

                    if (grid[gridPoint.x, gridPoint.y] == 2)
                    {
                        directionCount = 0;
                    }

                    switch (directionCount)
                    {
                        case 0:
                            rooms[i, j] = Instantiate(outlines.noDoor, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            break;
                        case 1:
                            if (isRoomUp)
                            {
                                rooms[i, j] = Instantiate(outlines.singleUp, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight)
                            {
                                rooms[i, j] = Instantiate(outlines.singleRight, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomDown)
                            {
                                rooms[i, j] = Instantiate(outlines.singleDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomLeft)
                            {
                                rooms[i, j] = Instantiate(outlines.singleLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;

                        case 2:
                            if (isRoomUp && isRoomRight)
                            {
                                rooms[i, j] = Instantiate(outlines.doubleUpRight, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight && isRoomDown)
                            {
                                rooms[i, j] = Instantiate(outlines.doubleRightDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomDown && isRoomLeft)
                            {
                                rooms[i, j] = Instantiate(outlines.doubleDownLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomLeft && isRoomUp)
                            {
                                rooms[i, j] = Instantiate(outlines.doubleLeftUp, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomUp && isRoomDown)
                            {
                                rooms[i, j] = Instantiate(outlines.doubleUpDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight && isRoomLeft)
                            {
                                rooms[i, j] = Instantiate(outlines.doubleRightLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;

                        case 3:
                            if (isRoomUp && isRoomRight && isRoomDown)
                            {
                                rooms[i, j] = Instantiate(outlines.tripleUpRightDown, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomRight && isRoomDown && isRoomLeft)
                            {
                                rooms[i, j] = Instantiate(outlines.tripleRightDownLeft, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomDown && isRoomLeft && isRoomUp)
                            {
                                rooms[i, j] = Instantiate(outlines.tripleDownLeftUp, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            if (isRoomLeft && isRoomUp && isRoomRight)
                            {
                                rooms[i, j] = Instantiate(outlines.tripleLeftUpRight, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;

                        case 4:
                            if (isRoomUp && isRoomRight && isRoomDown && isRoomLeft)
                            {
                                rooms[i, j] = Instantiate(outlines.quadruple, roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            }
                            break;
                    }
                }

                roomPosition += new Vector2(xOffset, 0f);
            }

            roomPosition = new Vector2(-101.5f, roomPosition.y - yOffset);
        }
    }

    public void SetRoomTypes()
    {
        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (rooms[i, j] != null)
                {
                    int? roomType = grid[i, j];
                    rooms[i, j].GetComponent<Room>().roomType = (RoomType)roomType;
                }
            }
        }
    }

    public void CreateCenters()
    {
        float xOffset = 17f, yOffset = 11f;
        Vector2 roomPosition = new Vector2(-101.5f, 66.5f);
        int selectedCenter;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);

                if (rooms[i ,j] is not null)
                {
                    if (grid[gridPoint.x, gridPoint.y] == (int)RoomType.StartingRoom)
                    {
                        Instantiate(startRoomCenter, roomPosition, Quaternion.Euler(0f, 0f, 0f)).room = rooms[i, j].GetComponent<Room>();
                    }
                    else if (grid[gridPoint.x, gridPoint.y] == (int)RoomType.BossRoom)
                    {
                        Instantiate(bossRoomCenter, roomPosition, Quaternion.Euler(0f, 0f, 0f)).room = rooms[i, j].GetComponent<Room>();
                    }
                    else
                    {
                        selectedCenter = Random.Range(0, roomCenters.Length);
                        Instantiate(roomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f)).room = rooms[i, j].GetComponent<Room>();
                    }
                }

                roomPosition += new Vector2(xOffset, 0f);
            }

            roomPosition = new Vector2(-101.5f, roomPosition.y - yOffset);
        }
    }

    public void ReplaceDoorsVariants()
    {
        Vector2Int secretRoomPosition = new Vector2Int(0, 0);

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);
                if (rooms[i, j] is not null)
                {
                    if (grid[gridPoint.x, gridPoint.y] == (int)RoomType.SecretRoom)
                    {
                        secretRoomPosition = gridPoint;
                    }
                    if (grid[gridPoint.x, gridPoint.y] == (int)RoomType.BossRoom)
                    {
                        SwapDoors(bossRoomDoor);
                    }
                    if (grid[gridPoint.x, gridPoint.y] == (int)RoomType.ShopRoom)
                    {
                        SwapDoors(shopRoomDoor);
                    }
                    if (grid[gridPoint.x, gridPoint.y] == (int)RoomType.ItemRoom)
                    {
                        SwapDoors(itemRoomDoor);
                    }
                }
            }
        }

        gridPoint = secretRoomPosition;
        SwapSecretDoors(secretRoomDoor);
    }

    public void SwapDoors(GameObject door)
    {
        GameObject room = rooms[gridPoint.x, gridPoint.y];

        if (room.GetComponent<Room>().doorUp != null)
        {
            Destroy(room.GetComponent<Room>().doorUp);
            GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(0f, 3.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            newDoor.transform.SetParent(room.transform.Find("Doors"));
            room.GetComponent<Room>().doorUp = newDoor;

            GameObject adjacentRoom = rooms[gridPoint.x - 1, gridPoint.y];
            Destroy(adjacentRoom.GetComponent<Room>().doorDown);
            GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(0f, -3.5f, 0f), Quaternion.Euler(0f, 0f, -180f));
            newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
            adjacentRoom.GetComponent<Room>().doorDown = newDoor2;
        }
        if (room.GetComponent<Room>().doorRight != null)
        {
            Destroy(room.GetComponent<Room>().doorRight);
            GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
            newDoor.transform.SetParent(room.transform.Find("Doors"));
            room.GetComponent<Room>().doorRight = newDoor;

            GameObject adjacentRoom = rooms[gridPoint.x, gridPoint.y + 1];
            Destroy(adjacentRoom.GetComponent<Room>().doorLeft);
            GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(-6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -270f));
            newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
            adjacentRoom.GetComponent<Room>().doorLeft = newDoor2;
        }
        if (room.GetComponent<Room>().doorDown != null)
        {
            Destroy(room.GetComponent<Room>().doorDown);
            GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(0f, -3.5f, 0f), Quaternion.Euler(0f, 0f, -180f));
            newDoor.transform.SetParent(room.transform.Find("Doors"));
            room.GetComponent<Room>().doorDown = newDoor;

            GameObject adjacentRoom = rooms[gridPoint.x + 1, gridPoint.y];
            Destroy(adjacentRoom.GetComponent<Room>().doorUp);
            GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(0, 3.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
            adjacentRoom.GetComponent<Room>().doorUp = newDoor2;
        }
        if (room.GetComponent<Room>().doorLeft != null)
        {
            Destroy(room.GetComponent<Room>().doorLeft);
            GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(-6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -270f));
            newDoor.transform.SetParent(room.transform.Find("Doors"));
            room.GetComponent<Room>().doorLeft = newDoor;

            GameObject adjacentRoom = rooms[gridPoint.x, gridPoint.y - 1];
            Destroy(adjacentRoom.GetComponent<Room>().doorRight);
            GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
            newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
            adjacentRoom.GetComponent<Room>().doorRight = newDoor;
        }
    }

    public void SwapSecretDoors(GameObject door)
    {
        GameObject room = rooms[gridPoint.x, gridPoint.y];

        if (gridPoint.x > 0)
        {
            if (rooms[gridPoint.x - 1, gridPoint.y] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, 4, 0), null);
                GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(0f, 4f, 0f), Quaternion.Euler(0f, 0f, 0f));
                newDoor.GetComponent<SecretDoors>().areRevealed = true;
                newDoor.GetComponent<BoxCollider2D>().enabled = false;
                newDoor.GetComponent<Animator>().Play("Doors_Open");
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorUp = newDoor;

                GameObject adjacentRoom = rooms[gridPoint.x - 1, gridPoint.y];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, -4, 0), null);
                GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(0f, -4f, 0f), Quaternion.Euler(0f, 0f, -180f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorDown = newDoor2;
            }
        }
        if (gridPoint.y < 12)
        {
            if (rooms[gridPoint.x, gridPoint.y + 1] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(7, 0, 0), null);
                GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(7f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
                newDoor.GetComponent<SecretDoors>().areRevealed = true;
                newDoor.GetComponent<BoxCollider2D>().enabled = false;
                newDoor.GetComponent<Animator>().Play("Doors_Open");
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorRight = newDoor;

                GameObject adjacentRoom = rooms[gridPoint.x, gridPoint.y + 1];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(-7, 0, 0), null);
                GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(-7f, 0f, 0f), Quaternion.Euler(0f, 0f, -270f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorLeft = newDoor2;
            }
        }
        if (gridPoint.x < 12)
        {
            if (rooms[gridPoint.x + 1, gridPoint.y] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, -4, 0), null);
                GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(0f, -4f, 0f), Quaternion.Euler(0f, 0f, -180f));
                newDoor.GetComponent<SecretDoors>().areRevealed = true;
                newDoor.GetComponent<BoxCollider2D>().enabled = false;
                newDoor.GetComponent<Animator>().Play("Doors_Open");
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorDown = newDoor;

                GameObject adjacentRoom = rooms[gridPoint.x + 1, gridPoint.y];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, 4, 0), null);
                GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(0f, 4f, 0f), Quaternion.Euler(0f, 0f, 0f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorUp = newDoor2;
            }
        }
        if (gridPoint.y > 0)
        {
            if (rooms[gridPoint.x, gridPoint.y - 1] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(-7, 0, 0), null);
                GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(-7f, 0f, 0f), Quaternion.Euler(0f, 0f, -270f));
                newDoor.GetComponent<SecretDoors>().areRevealed = true;
                newDoor.GetComponent<BoxCollider2D>().enabled = false;
                newDoor.GetComponent<Animator>().Play("Doors_Open");
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorLeft = newDoor;

                GameObject adjacentRoom = rooms[gridPoint.x, gridPoint.y - 1];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(7, 0, 0), null);
                GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(7f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorRight = newDoor2;
            }
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
                            if (grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.SecretRoom)
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
                            if (grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.SecretRoom)
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
                            if (grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.SecretRoom)
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
                            if (grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.SecretRoom)
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
                            if (grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.StartingRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x - 1, gridPoint.y] != (int)RoomType.SecretRoom)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.StartingRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y + 1] != (int)RoomType.SecretRoom)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.StartingRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x + 1, gridPoint.y] != (int)RoomType.SecretRoom)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.StartingRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y - 1] != (int)RoomType.SecretRoom)
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
                            if (grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.ShopRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x - 1, gridPoint.y] != (int)RoomType.SecretRoom)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.ShopRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y + 1] != (int)RoomType.SecretRoom)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] is not null)
                        {
                            if (grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.ShopRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x + 1, gridPoint.y] != (int)RoomType.SecretRoom)
                            {
                                adjacentRooms++;
                            }
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] is not null)
                        {
                            if (grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.ShopRoom)
                            {
                                continue;
                            }
                            else if (grid[gridPoint.x, gridPoint.y - 1] != (int)RoomType.SecretRoom)
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

public enum Direction 
{ 
    up,
    right,
    down,
    left 
}

public enum RoomType
{
    StartingRoom,
    NormalRoom,
    SecretRoom,
    BossRoom,
    ShopRoom,
    ItemRoom
}

[System.Serializable]
public class RoomOutlines
{
    public GameObject noDoor, singleUp, singleRight, singleDown, singleLeft,
        doubleUpRight, doubleRightDown, doubleDownLeft, doubleLeftUp, doubleUpDown, doubleRightLeft,
        tripleUpRightDown, tripleRightDownLeft, tripleDownLeftUp, tripleLeftUpRight, quadruple;
}