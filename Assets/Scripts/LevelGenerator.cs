using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    public int levelSize;
    public FloorDepth floorDepth;
    public bool shopAndItemRoomRequireKeys;
    private Direction direction;
    private bool canCreate;

    private int?[,] grid = new int?[13, 13];
    private Vector2Int gridPoint, oldPoint;

    public GameObject[,] rooms = new GameObject[13, 13];
    public Vector2Int secretRoomPosition, shopRoomPosition, itemRoomPosition;

    public RoomOutlines outlines;
    public GameObject bossRoomDoor, shopRoomDoor, itemRoomDoor, secretRoomDoor;
    public GameObject mapRoom;
    public Sprite normalMapRoom, secretMapRoom, bossMapRoom, shopMapRoom, itemMapRoom;
    public RoomCenter[] startingRoomCenters, normalRoomCenters, bossRoomCenters, shopRoomCenters, itemRoomCenters, secretRoomCenters;


    private void Awake()
    {
        GenerateGrid();
        CreateOutlines();
        CreateCenters();
        GenerateMapLayout();
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
        gridPoint = new Vector2Int(6, 6);
        grid[gridPoint.x, gridPoint.y] = (int)RoomType.StartingRoom;

        GenerateNormalRooms();
        GenerateSpecialRoom(RoomType.BossRoom);
        GenerateSpecialRoom(RoomType.ShopRoom);
        GenerateSpecialRoom(RoomType.ItemRoom);
        GenerateSpecialRoom(RoomType.SecretRoom);
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

    public void GenerateNormalRooms()
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

    public void GenerateSpecialRoom(RoomType roomType)
    {
        Debug.Log($"Generating {roomType}...");
        List<Vector2Int> matchingPoints = new List<Vector2Int>();
        Vector2Int selectedPoint = new Vector2Int();

        if (roomType == RoomType.SecretRoom)
        {
            matchingPoints = GetRoomMatchingPoints(roomType, 4);
            if (matchingPoints.Count == 0)
            {
                matchingPoints = GetRoomMatchingPoints(roomType, 3);
                if (matchingPoints.Count == 0)
                {
                    matchingPoints = GetRoomMatchingPoints(roomType, 2);
                    if (matchingPoints.Count == 0)
                    {
                        matchingPoints = GetRoomMatchingPoints(roomType, 1);
                    }
                }
            }
        }
        else
        {
            matchingPoints = GetRoomMatchingPoints(roomType, 1);
            Debug.Log($"Points count: {matchingPoints.Count}");
        }

        if (roomType == RoomType.BossRoom)
        {
            selectedPoint = FindFurthestPointFromStart(matchingPoints);
        }
        else
        {
            Debug.Log($"Points count: {matchingPoints.Count}");
            int selected = Random.Range(0, matchingPoints.Count);
            Debug.Log($"Selected: {selected}");
            selectedPoint = matchingPoints[selected];
            if (roomType == RoomType.SecretRoom)
            {
                secretRoomPosition = selectedPoint;
            }
        }

        grid[selectedPoint.x, selectedPoint.y] = (int)roomType;
        Debug.Log($"Created {roomType} at ({selectedPoint.x},{selectedPoint.y})");
    }

    public List<Vector2Int> GetRoomMatchingPoints(RoomType roomType, int matchValue)
    {
        Debug.Log($"Looking for points with {matchValue} adjacent rooms to create a {roomType}");
        List<Vector2Int> points = new List<Vector2Int>();
        int adjacentRooms = 0;

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                gridPoint = new Vector2Int(i, j);

                if (grid[gridPoint.x, gridPoint.y] == null)
                {
                    adjacentRooms = 0;

                    if (gridPoint.x > 0)
                    {
                        if (grid[gridPoint.x - 1, gridPoint.y] != null)
                        {
                            switch (roomType)
                            {
                                case RoomType.BossRoom:
                                    adjacentRooms++;
                                    break;

                                case RoomType.ShopRoom:
                                    if (grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.StartingRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.ItemRoom:
                                    if (grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.ShopRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.SecretRoom:
                                    if (grid[gridPoint.x - 1, gridPoint.y] == (int)RoomType.BossRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;
                            }
                        }
                    }
                    if (gridPoint.y < 12)
                    {
                        if (grid[gridPoint.x, gridPoint.y + 1] != null)
                        {
                            switch (roomType)
                            {
                                case RoomType.BossRoom:
                                    adjacentRooms++;
                                    break;

                                case RoomType.ShopRoom:
                                    if (grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.StartingRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.ItemRoom:
                                    if (grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.ShopRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.SecretRoom:
                                    if (grid[gridPoint.x, gridPoint.y + 1] == (int)RoomType.BossRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;
                            }
                        }
                    }
                    if (gridPoint.x < 12)
                    {
                        if (grid[gridPoint.x + 1, gridPoint.y] != null)
                        {
                            switch (roomType)
                            {
                                case RoomType.BossRoom:
                                    adjacentRooms++;
                                    break;

                                case RoomType.ShopRoom:
                                    if (grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.StartingRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.ItemRoom:
                                    if (grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.BossRoom || grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.ShopRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.SecretRoom:
                                    if (grid[gridPoint.x + 1, gridPoint.y] == (int)RoomType.BossRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;
                            }
                        }
                    }
                    if (gridPoint.y > 0)
                    {
                        if (grid[gridPoint.x, gridPoint.y - 1] != null)
                        {
                            switch (roomType)
                            {
                                case RoomType.BossRoom:
                                    adjacentRooms++;
                                    break;

                                case RoomType.ShopRoom:
                                    if (grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.StartingRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.ItemRoom:
                                    if (grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.BossRoom || grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.ShopRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;

                                case RoomType.SecretRoom:
                                    if (grid[gridPoint.x, gridPoint.y - 1] == (int)RoomType.BossRoom)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        adjacentRooms++;
                                    }
                                    break;
                            }
                        }
                    }

                    if (adjacentRooms == matchValue)
                    {
                        points.Add(gridPoint);
                        Debug.Log($"Added point ({gridPoint.x},{gridPoint.y}) to the list");
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

                    rooms[i, j].GetComponent<Room>().roomType = (RoomType)grid[i, j];
                }

                roomPosition += new Vector2(xOffset, 0f);
            }

            roomPosition = new Vector2(-101.5f, roomPosition.y - yOffset);
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
                if (rooms[i, j] is not null)
                {
                    RoomCenter roomCenter;

                    switch (rooms[i, j].GetComponent<Room>().roomType)
                    {
                        case RoomType.StartingRoom:
                            selectedCenter = Random.Range(0, startingRoomCenters.Length);
                            roomCenter = Instantiate(startingRoomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            rooms[i, j].GetComponent<Room>().roomCenter = roomCenter;
                            break;

                        case RoomType.SecretRoom:
                            secretRoomPosition = new Vector2Int(i, j);
                            selectedCenter = Random.Range(0, secretRoomCenters.Length);
                            roomCenter = Instantiate(secretRoomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            rooms[i, j].GetComponent<Room>().roomCenter = roomCenter;
                            break;

                        case RoomType.BossRoom:
                            selectedCenter = Random.Range(0, bossRoomCenters.Length);
                            roomCenter = Instantiate(bossRoomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            rooms[i, j].GetComponent<Room>().roomCenter = roomCenter;
                            SwapDoors(new Vector2Int(i, j), bossRoomDoor);
                            SpawnBoss(roomCenter);
                            break;

                        case RoomType.ShopRoom:
                            shopRoomPosition = new Vector2Int(i, j);
                            selectedCenter = Random.Range(0, shopRoomCenters.Length);
                            roomCenter = Instantiate(shopRoomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            rooms[i, j].GetComponent<Room>().roomCenter = roomCenter;
                            SwapDoors(new Vector2Int(i, j), shopRoomDoor);
                            break;

                        case RoomType.ItemRoom:
                            itemRoomPosition = new Vector2Int(i, j);
                            selectedCenter = Random.Range(0, itemRoomCenters.Length);
                            roomCenter = Instantiate(itemRoomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            rooms[i, j].GetComponent<Room>().roomCenter = roomCenter;
                            SwapDoors(new Vector2Int(i, j), itemRoomDoor);
                            break;

                        case RoomType.NormalRoom:
                            selectedCenter = Random.Range(0, normalRoomCenters.Length);
                            roomCenter = Instantiate(normalRoomCenters[selectedCenter], roomPosition, Quaternion.Euler(0f, 0f, 0f));
                            rooms[i, j].GetComponent<Room>().roomCenter = roomCenter;
                            break;

                        default:
                            roomCenter = null;
                            break;
                    }

                    if (roomCenter.hasItemPedestals)
                    {
                        int selectedItem;
                        GameObject item = new GameObject();

                        foreach (GameObject itemPedestal in roomCenter.itemPedestals)
                        {
                            switch (rooms[i, j].GetComponent<Room>().roomType)
                            {
                                case RoomType.SecretRoom:
                                    selectedItem = Random.Range(0, ItemManager.instance.secretRoomItems.Count);
                                    item = Instantiate(ItemManager.instance.secretRoomItems[selectedItem], roomCenter.transform.position, Quaternion.Euler(0f, 0f, 0f));
                                    ItemManager.instance.secretRoomItems.RemoveAt(selectedItem);
                                    itemPedestal.GetComponent<ItemPedestal>().item = item;
                                    break;

                                case RoomType.BossRoom:
                                    selectedItem = Random.Range(0, ItemManager.instance.bossRoomItems.Count);
                                    item = Instantiate(ItemManager.instance.bossRoomItems[selectedItem], roomCenter.transform.position, Quaternion.Euler(0f, 0f, 0f));
                                    ItemManager.instance.bossRoomItems.RemoveAt(selectedItem);
                                    itemPedestal.GetComponent<ItemPedestal>().item = item;
                                    item.transform.SetParent(itemPedestal.transform);
                                    itemPedestal.gameObject.SetActive(false);
                                    break;

                                case RoomType.ShopRoom:
                                    selectedItem = Random.Range(0, ItemManager.instance.shopRoomItems.Count);
                                    item = Instantiate(ItemManager.instance.shopRoomItems[selectedItem], roomCenter.transform.position, Quaternion.Euler(0f, 0f, 0f));
                                    ItemManager.instance.shopRoomItems.RemoveAt(selectedItem);
                                    itemPedestal.GetComponent<ShopItem>().item = item;
                                    break;

                                case RoomType.ItemRoom:
                                    selectedItem = Random.Range(0, ItemManager.instance.itemRoomItems.Count);
                                    item = Instantiate(ItemManager.instance.itemRoomItems[selectedItem], roomCenter.transform.position, Quaternion.Euler(0f, 0f, 0f));
                                    ItemManager.instance.itemRoomItems.RemoveAt(selectedItem);
                                    itemPedestal.GetComponent<ItemPedestal>().item = item;
                                    break;
                            }

                        }
                    }
                }

                roomPosition += new Vector2(xOffset, 0f);
            }

            roomPosition = new Vector2(-101.5f, roomPosition.y - yOffset);
        }

        SwapSecretDoors(secretRoomPosition);
    }

    public void SwapDoors(Vector2Int roomGridPosition, GameObject door)
    {
        GameObject room = rooms[roomGridPosition.x, roomGridPosition.y];

        if (room.GetComponent<Room>().doorUp != null)
        {
            Destroy(room.GetComponent<Room>().doorUp);
            GameObject newDoor = Instantiate(door, room.transform.position + new Vector3(0f, 3.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
            newDoor.transform.SetParent(room.transform.Find("Doors"));
            room.GetComponent<Room>().doorUp = newDoor;

            GameObject adjacentRoom = rooms[roomGridPosition.x - 1, roomGridPosition.y];
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

            GameObject adjacentRoom = rooms[roomGridPosition.x, roomGridPosition.y + 1];
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

            GameObject adjacentRoom = rooms[roomGridPosition.x + 1, roomGridPosition.y];
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

            GameObject adjacentRoom = rooms[roomGridPosition.x, roomGridPosition.y - 1];
            Destroy(adjacentRoom.GetComponent<Room>().doorRight);
            GameObject newDoor2 = Instantiate(door, adjacentRoom.transform.position + new Vector3(6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
            newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
            adjacentRoom.GetComponent<Room>().doorRight = newDoor2;
        }
    }

    public void SwapSecretDoors(Vector2Int roomGridPosition)
    {
        GameObject room = rooms[roomGridPosition.x, roomGridPosition.y];

        if (roomGridPosition.x > 0)
        {
            if (rooms[roomGridPosition.x - 1, roomGridPosition.y] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, 4, 0), null);
                GameObject newDoor = Instantiate(secretRoomDoor, room.transform.position + new Vector3(0f, 3.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
                newDoor.GetComponent<Door>().isRevealed = true;
                newDoor.GetComponent<Door>().OpenDoor();
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorUp = newDoor;

                GameObject adjacentRoom = rooms[roomGridPosition.x - 1, roomGridPosition.y];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, -4, 0), null);
                GameObject newDoor2 = Instantiate(secretRoomDoor, adjacentRoom.transform.position + new Vector3(0f, -3.5f, 0f), Quaternion.Euler(0f, 0f, -180f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorDown = newDoor2;
            }
        }
        if (roomGridPosition.y < 12)
        {
            if (rooms[roomGridPosition.x, roomGridPosition.y + 1] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(7, 0, 0), null);
                GameObject newDoor = Instantiate(secretRoomDoor, room.transform.position + new Vector3(6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
                newDoor.GetComponent<Door>().isRevealed = true;
                newDoor.GetComponent<Door>().OpenDoor();
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorRight = newDoor;

                GameObject adjacentRoom = rooms[roomGridPosition.x, roomGridPosition.y + 1];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(-7, 0, 0), null);
                GameObject newDoor2 = Instantiate(secretRoomDoor, adjacentRoom.transform.position + new Vector3(-6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -270f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorLeft = newDoor2;
            }
        }
        if (roomGridPosition.x < 12)
        {
            if (rooms[roomGridPosition.x + 1, roomGridPosition.y] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, -4, 0), null);
                GameObject newDoor = Instantiate(secretRoomDoor, room.transform.position + new Vector3(0f, -3.5f, 0f), Quaternion.Euler(0f, 0f, -180f));
                newDoor.GetComponent<Door>().isRevealed = true;
                newDoor.GetComponent<Door>().OpenDoor();
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorDown = newDoor;

                GameObject adjacentRoom = rooms[roomGridPosition.x + 1, roomGridPosition.y];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(0, 4, 0), null);
                GameObject newDoor2 = Instantiate(secretRoomDoor, adjacentRoom.transform.position + new Vector3(0f, 3.5f, 0f), Quaternion.Euler(0f, 0f, 0f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorUp = newDoor2;
            }
        }
        if (roomGridPosition.y > 0)
        {
            if (rooms[roomGridPosition.x, roomGridPosition.y - 1] != null)
            {
                room.GetComponent<Room>().tilemap.SetTile(new Vector3Int(-7, 0, 0), null);
                GameObject newDoor = Instantiate(secretRoomDoor, room.transform.position + new Vector3(-6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -270f));
                newDoor.GetComponent<Door>().isRevealed = true;
                newDoor.GetComponent<Door>().OpenDoor();
                newDoor.transform.SetParent(room.transform.Find("Doors"));
                room.GetComponent<Room>().doorLeft = newDoor;

                GameObject adjacentRoom = rooms[roomGridPosition.x, roomGridPosition.y - 1];
                adjacentRoom.GetComponent<Room>().tilemap.SetTile(new Vector3Int(7, 0, 0), null);
                GameObject newDoor2 = Instantiate(secretRoomDoor, adjacentRoom.transform.position + new Vector3(6.5f, 0f, 0f), Quaternion.Euler(0f, 0f, -90f));
                newDoor2.transform.SetParent(adjacentRoom.transform.Find("Doors"));
                adjacentRoom.GetComponent<Room>().doorRight = newDoor2;
            }
        }
    }

    public void SpawnBoss(RoomCenter roomCenter)
    {
        int selectedBoss = 0;
        switch (floorDepth)
        {
            case FloorDepth.Underground_1:
            case FloorDepth.Underground_2:
                selectedBoss = Random.Range(0, BossManager.instance.undergroundBosses.Count);
                BossManager.instance.currentBoss = Instantiate(BossManager.instance.undergroundBosses[selectedBoss], roomCenter.transform.position, Quaternion.Euler(0f, 0f, 0f));
                BossManager.instance.undergroundBosses.RemoveAt(selectedBoss);
                break;
            case FloorDepth.Sewers_1:
            case FloorDepth.Sewers_2:
                selectedBoss = Random.Range(0, BossManager.instance.sewersBosses.Count);
                BossManager.instance.currentBoss = Instantiate(BossManager.instance.sewersBosses[selectedBoss], roomCenter.transform.position, Quaternion.Euler(0f, 0f, 0f));
                BossManager.instance.sewersBosses.RemoveAt(selectedBoss);
                break;
        }

        roomCenter.enemies.Add(BossManager.instance.currentBoss);
        BossManager.instance.currentBoss.SetActive(false);
        UIController.instance.bossHealthBarImage.sprite = BossManager.instance.currentBoss.GetComponent<BossController>().bossHealthBarSprite;
        BossManager.instance.isBossDefeated = false;
        BossManager.instance.currentBoss.transform.SetParent(roomCenter.transform.Find("Enemies"));
    }

    public void GenerateMapLayout()
    {
        Vector2 position = new Vector2(-5.5f, 6.5f);

        for (int i = 0; i < 13; i++)
        {
            for (int j = 0; j < 13; j++)
            {
                if (grid[i, j] != null)
                {
                    GameObject mapRoomObject = Instantiate(mapRoom, position, Quaternion.Euler(0f, 0f, 0f));

                    switch (grid[i, j])
                    {
                        case 0:
                        case 1:
                            break;

                        case 2:
                            mapRoomObject.GetComponent<SpriteRenderer>().sprite = secretMapRoom;
                            break;

                        case 3:
                            mapRoomObject.GetComponent<SpriteRenderer>().sprite = bossMapRoom;
                            break;

                        case 4:
                            mapRoomObject.GetComponent<SpriteRenderer>().sprite = shopMapRoom;
                            break;

                        case 5:
                            mapRoomObject.GetComponent<SpriteRenderer>().sprite = itemMapRoom;
                            break;
                    }

                    rooms[i, j].GetComponent<Room>().mapRoom = mapRoomObject;
                    rooms[i, j].GetComponent<Room>().mapRoom.GetComponent<SpriteRenderer>().color = new Color(0.2f, 0.2f, 0.2f);
                    rooms[i, j].GetComponent<Room>().mapRoom.SetActive(false);
                }

                position += new Vector2(1f, 0f);
            }

            position = new Vector2(-5.5f, position.y - 1f);
        }

        Debug.Log("Starting room: " + rooms[6, 6]);
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

public enum FloorDepth
{
    Underground_1,
    Underground_2,
    Sewers_1,
    Sewers_2
}

[System.Serializable]
public class RoomOutlines
{
    public GameObject noDoor, singleUp, singleRight, singleDown, singleLeft,
        doubleUpRight, doubleRightDown, doubleDownLeft, doubleLeftUp, doubleUpDown, doubleRightLeft,
        tripleUpRightDown, tripleRightDownLeft, tripleDownLeftUp, tripleLeftUpRight, quadruple;
}