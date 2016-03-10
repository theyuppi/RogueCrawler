using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadSpriteScript : MonoBehaviour
{
    SpriteRenderer sr;
    Sprite spr;

    public GameObject selectedUnit;
    public GameObject tilePrefab;
    public GameObject targetPrefab;
    public EnemyHandler eHandler;
    public PlayerHandler pHandler;

    private int gridSizeX = 50;
    private int gridSizeY = 50;
    private int tileSizeX = 80;
    private int tileSizeY = 80;

    public GameObject[,] myTileArray;
    Node[,] graph;

    void Start()
    {
        selectedUnit.GetComponent<PlayerScript>().map = this;
        myTileArray = new GameObject[gridSizeX, gridSizeY];
        CreateRoom("maproom" + 1 + "new");
        GeneratePathfindingGraph();
    }

    void Update()
    { }

    void CreateRoom(string levelName)
    {
        spr = Resources.Load<Sprite>(levelName);

        float sizeY = spr.texture.width; //Rows
        float sizeX = spr.texture.width; //Columns

        for (float j = sizeY; j > 0; j--) //Rows
        {
            for (float i = 0; i < sizeX; i++) //Columns
            {
                Color pixelCol = new Color(spr.texture.GetPixel((int)i, (int)j).r, spr.texture.GetPixel((int)i, (int)j).g, spr.texture.GetPixel((int)i, (int)j).b, spr.texture.GetPixel((int)i, (int)j).a);
                string tileType = ColorToHex(pixelCol);
                if (!tileType.Equals("FFFFFF"))
                {
                    //GameObject tile = Instantiate(tilePrefab, new Vector2(i * 0.8f, (sizeX - j) * -0.8f), transform.rotation) as GameObject;
                    GameObject tile = Instantiate(tilePrefab, new Vector2(i * 0.8f, j * 0.8f), transform.rotation) as GameObject;

                    if (tileType.Equals("C4AA6C"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Floor;
                        tile.tag = "Floor";
                        //Debug.Log("Floor");
                    }
                    else if (tileType.Equals("584A33"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Wall;
                        //Debug.Log("Wall");
                    }
                    else if (tileType.Equals("FF0000"))
                    {
                        //eHandler.SpawnEnemy(EnemyHandler.enemies.axeSkeleton, new Vector2(i * 0.8f, j * 0.8f));
                        tile.GetComponent<TileScript>().occupant = eHandler.SpawnEnemy(EnemyHandler.enemies.axeSkeleton, new Vector2(i * 0.8f, j * 0.8f), (int)i, (int)j);
                        tile.GetComponent<TileScript>().walkable = false;
                        tile.GetComponent<TileScript>().hasEnemy = true;
                        //Debug.Log("Enemy");
                    }
                    else if (tileType.Equals("FFFF00"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Chest;
                        //Debug.Log("Treasure");
                    }
                    else if (tileType.Equals("505050"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Spike;
                        //Debug.Log("Spikes");
                    }
                    else if (tileType.Equals("000000"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Hole;
                        //Debug.Log("Hole");
                    }
                    myTileArray[(int)j, (int)i] = tile;
                    tile.GetComponent<TileScript>().myID = new Vector2(j, i);
                    tile.GetComponent<TileScript>().levelHandler = this.gameObject;
                    eHandler.GetComponent<EnemyHandler>().levelhandler = this.gameObject;
                    tile.GetComponent<TileScript>().player = selectedUnit;
                    //tile.GetComponent<TileScript>().transform.parent = transform;
                }
            }
        }
        Application.Quit();
    }

    string ColorToHex(Color32 color)
    {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    void GeneratePathfindingGraph()
    {
        // Initialize the array
        graph = new Node[gridSizeX, gridSizeY];

        // Initialize a Node for each spot in the array
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }

        // Now that all the nodes exist, calculate their neighbours
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                if (x > 0)
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                if (x < gridSizeX - 1)
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                if (y > 0)
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                if (y < gridSizeY - 1)
                    graph[x, y].neighbours.Add(graph[x, y + 1]);
            }
        }
    }

    public Vector3 TileCoordToWorldCoord(int x, int y)
    {
        return new Vector3(y * 0.8f, (x * 0.8f), 0);
    }

    public bool UnitCanEnterTile(int x, int y)
    {
        // We could test the unit's walk/hover/fly type against various
        // terrain flags here to see if they are allowed to enter the tile.
        //Debug.Log(myTileArray[x, y].GetComponent<TileScript>().walkable);
        //Debug.Log("X = " + x + " Y = " + y);
        if (myTileArray[x, y] == null)
        {
            return false;
        }
        //if (myTileArray[x, y].GetComponent<TileScript>().walkable == false)
        //{
        //    if (myTileArray[x, y].GetComponent<TileScript>().hasEnemy == true)
        //    {
        //        return true;
        //    }
        //}

        return myTileArray[x, y].GetComponent<TileScript>().walkable;
    }

    public void GeneratePathTo(int x, int y, GameObject pathRequester, bool isPlayer)
    {
        // Clear out our unit's old path.
        //selectedUnit.GetComponent<PlayerScript>().currentPath = null;
        if (isPlayer)
        {
            pathRequester.GetComponent<PlayerScript>().currentPath = null;
        }
        else
        {
            pathRequester.GetComponent<EnemyScript>().currentPath = null;
        }

        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

        // Setup the "Q" -- the list of nodes we haven't checked yet.
        List<Node> unvisited = new List<Node>();
        Node source;
        if (isPlayer)
        {
            source = graph[pathRequester.GetComponent<PlayerScript>().tileX, pathRequester.GetComponent<PlayerScript>().tileY];
        }
        else
        {
            source = graph[pathRequester.GetComponent<EnemyScript>().tileX, pathRequester.GetComponent<EnemyScript>().tileY];
        }

        Node target = graph[x, y];

        dist[source] = 0;
        prev[source] = null;

        // Initialize everything to have INFINITY distance, since
        // we don't know any better right now. Also, it's possible
        // that some nodes CAN'T be reached from the source,
        // which would make INFINITY a reasonable value
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            unvisited.Add(v);
        }

        while (unvisited.Count > 0)
        {
            // "u" is going to be the unvisited node with the smallest distance.
            Node u = null;

            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;  // Exit the while loop!
            }

            unvisited.Remove(u);

            foreach (Node v in u.neighbours)
            {
                float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        // If we get there, then either we found the shortest route
        // to our target, or there is no route at ALL to our target.

        if (prev[target] == null)
        {
            // No route between our target and the source
            return;
        }

        List<Node> currentPath = new List<Node>();

        Node curry = target;

        // Step through the "prev" chain and add it to our path
        while (curry != null)
        {
            currentPath.Add(curry);
            curry = prev[curry];
        }

        // Right now, currentPath describes a route from out target to our source
        // So we need to invert it!
        currentPath.Reverse();


        //Draw new path
        for (int i = 1; i < currentPath.Count; i++)
        {
            myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().StepTile = true;
            if (i == currentPath.Count - 1)
            {
                if (myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().walkable == true)
                {
                    myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().GoalTile = true;
                }
                else
                {
                    myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().StepTile = false;
                    if (myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().hasEnemy == true)
                    {
                        targetPrefab.transform.position = myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().GetPos();
                        targetPrefab.GetComponent<SpriteRenderer>().enabled = true;
                    }
                }
            }
        }
        //for (int i = 0; i < currentPath.Count; i++)
        //{
        //    Debug.Log("Node " + i + ": X = " + currentPath[i].x + ", Y = " + currentPath[i].y);
        //}
        //Set new path

        if (isPlayer)
        {
            pathRequester.GetComponent<PlayerScript>().currentPath = currentPath;
        }
        else
        {
            pathRequester.GetComponent<EnemyScript>().currentPath = currentPath;
        }
    }

    public void ClearOldPath()
    {
        if (selectedUnit.GetComponent<PlayerScript>().currentPath != null)
        {
            //Clear old path
            for (int i = 0; i < selectedUnit.GetComponent<PlayerScript>().currentPath.Count; i++)
            {
                myTileArray[selectedUnit.GetComponent<PlayerScript>().currentPath[i].x,
                    selectedUnit.GetComponent<PlayerScript>().currentPath[i].y].GetComponent<TileScript>().ResetColor();
            }
            targetPrefab.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
    {
        if (UnitCanEnterTile(targetX, targetY) == false)
        {
            return 30;
        }
        float cost = myTileArray[targetX, targetY].GetComponent<TileScript>().moveCost;

        return cost;
    }
}
