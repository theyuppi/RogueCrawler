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
    public CameraScript cScript;

	public int gridSizeX = 40;
	public int gridSizeY = 40;
	private int tileSizeX = 100;
	private int tileSizeY = 100;
    private float mplX;
    private float mplY;

    public GameObject[,] myTileArray;
	Node[,] graph;
    
	float completeCost = 0;
	public bool unReachable = false;
    public List<GameObject> roomList = new List<GameObject>();

	////test
	//private System.Threading.Thread myThread = null;
	public string[,] roomNode = new string[12, 5];


	void Start()
	{
        mplX = (float)tileSizeX / 100;
        mplY = (float)tileSizeY / 100;
        selectedUnit.GetComponent<PlayerScript>().map = this;
		myTileArray = new GameObject[gridSizeX, gridSizeY];
		#region RoomNodeAlignment
		roomNode[0, 0] = "bigmap1_00";
		roomNode[0, 1] = "bigmap1_02";
		roomNode[0, 2] = null;
		roomNode[0, 3] = null;
		roomNode[0, 4] = null;

		roomNode[1, 0] = "bigmap1_01";
		roomNode[1, 1] = "bigmap1_04";
		roomNode[1, 2] = null;
		roomNode[1, 3] = null;
		roomNode[1, 4] = "bigmap1_02";

		roomNode[2, 0] = "bigmap1_02";
		roomNode[2, 1] = null;
		roomNode[2, 2] = "bigmap1_00";
		roomNode[2, 3] = "bigmap1_01";
		roomNode[2, 4] = "bigmap1_03";

		roomNode[3, 0] = "bigmap1_03";
		roomNode[3, 1] = "bigmap1_05";
		roomNode[3, 2] = null;
		roomNode[3, 3] = "bigmap1_02";
		roomNode[3, 4] = null;

		roomNode[4, 0] = "bigmap1_04";
		roomNode[4, 1] = "bigmap1_07";
		roomNode[4, 2] = "bigmap1_01";
		roomNode[4, 3] = null;
		roomNode[4, 4] = null;

		roomNode[5, 0] = "bigmap1_05";
		roomNode[5, 1] = null;
		roomNode[5, 2] = "bigmap1_03";
		roomNode[5, 3] = null;
		roomNode[5, 4] = "bigmap1_06";

		roomNode[6, 0] = "bigmap1_06";
		roomNode[6, 1] = "bigmap1_10";
		roomNode[6, 2] = null;
		roomNode[6, 3] = "bigmap1_05";
		roomNode[6, 4] = null;

		roomNode[7, 0] = "bigmap1_07";
		roomNode[7, 1] = null;
		roomNode[7, 2] = "bigmap1_04";
		roomNode[7, 3] = null;
		roomNode[7, 4] = "bigmap1_08";

		roomNode[8, 0] = "bigmap1_08";
		roomNode[8, 1] = "bigmap1_11";
		roomNode[8, 2] = null;
		roomNode[8, 3] = "bigmap1_07";
		roomNode[8, 4] = "bigmap1_09";

		roomNode[9, 0] = "bigmap1_09";
		roomNode[9, 1] = null;
		roomNode[9, 2] = null;
		roomNode[9, 3] = "bigmap1_08";
		roomNode[9, 4] = "bigmap1_10";

		roomNode[10, 0] = "bigmap1_10";
		roomNode[10, 1] = null;
		roomNode[10, 2] = "bigmap1_06";
		roomNode[10, 3] = "bigmap1_09";
		roomNode[10, 4] = null;

		roomNode[11, 0] = "bigmap1_11";
		roomNode[11, 1] = null;
		roomNode[11, 2] = "bigmap1_08";
		roomNode[11, 3] = null;
		roomNode[11, 4] = null;
		#endregion
        //CreateRoom("maproom" + 1 + "newer");
        MakeRoom(0 , 0,  "bigmap1_00");
        //MakeRoom(40, 80, "bigmap1_11");
        //MakeRoom(40, 40, "bigmap1_10");
		//MakeRoom(40, 0, "bigmap1_09");
		//MakeRoom(80, 120, "bigmap1_08");
		//MakeRoom(80, 80, "bigmap1_07");
		//MakeRoom(80, 0, "bigmap1_06");
		//MakeRoom(120, 120, "bigmap1_05");
		//MakeRoom(120, 80, "bigmap1_04");
		//MakeRoom(120, 40, "bigmap1_03");
		//MakeRoom(120, 0, "bigmap1_02");
		//MakeRoom(160, 40, "bigmap1_01");
		GeneratePathfindingGraph();
		//myThread = new System.Threading.Thread(delegate() { GeneratePathfindingGraph(); });
		//myThread.Start();

	

		//for (int a = 0; a < roomNode.GetLength(0); a++)
		//{
		//	for (int b = 0; b < roomNode.GetLength(1); b++)
		//	{
		//		Debug.Log("A is: " + a + " - B is: " + b + " || " + roomNode[a,b]);
		//	}
		//}

	}

	void Update()
	{ }

    public void MakeRoom(int x, int y, string levelName)
    {
        spr = Resources.Load<Sprite>(levelName);
        float sizeY = spr.texture.height; //Rows
        float sizeX = spr.texture.width; //Columns

        if (roomList.Count != 0)
        {
            for (int i = 0; i < eHandler.enemyList.Count; i++)
            {
                Destroy(eHandler.enemyList[i]);
            }
            eHandler.enemyList.Clear();
            Destroy(roomList[0]);
            roomList.Clear();
        }
            
        GameObject Room = new GameObject();
        Room.name = "Room";
        roomList.Add(Room);

        for (float j = sizeY-1; j >= 0; j--) //Rows
        {
            for (float i = 0; i < sizeX; i++) //Columns
            {
                Color pixelCol = new Color(spr.texture.GetPixel((int)i, (int)j).r, spr.texture.GetPixel((int)i, (int)j).g, spr.texture.GetPixel((int)i, (int)j).b, spr.texture.GetPixel((int)i, (int)j).a);
                string tileType = ColorToHex(pixelCol);
                if (!tileType.Equals("FFFFFF"))
                {
                    //GameObject tile = Instantiate(tilePrefab, new Vector2((i + y) * mplX, (j + x) * mplY), transform.rotation) as GameObject;
                    GameObject tile = Instantiate(tilePrefab, new Vector2((i) * mplX, (j) * mplY), transform.rotation) as GameObject;

                    if (tileType.Equals("C4AA6C"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Floor;
                        tile.tag = "Floor";
                        //Debug.Log("Floor");
                    }
                    else if (tileType.Equals("584A33"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Wall;
                        tile.tag = "Wall";
                        //Debug.Log("Wall");
                    }
                    else if (tileType.Equals("FF0000"))
                    {
                        tile.GetComponent<TileScript>().occupant = eHandler.SpawnEnemy(EnemyHandler.enemies.axeSkeleton, new Vector2((i + y) * mplX, (j + x) * mplY), (int)i + y, (int)j + x);
                        tile.GetComponent<TileScript>().walkable = false;
                        tile.GetComponent<TileScript>().hasEnemy = true;
                        tile.tag = "Floor";
                        //Debug.Log("Enemy");
                    }
                    else if (tileType.Equals("FFFF00"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Chest;
                        tile.tag = "Chest";
                        //Debug.Log("Treasure");
                    }
                    else if (tileType.Equals("505050"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Spike;
                        tile.tag = "Spike";
                        //Debug.Log("Spikes");
                    }
                    else if (tileType.Equals("000000"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Hole;
                        tile.tag = "Hole";
                        //Debug.Log("Hole");
                    }
                    else if (tileType.Equals("8D4800"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.HDoor;
                        tile.tag = "HDoor";

						for (int a = 0; a < roomNode.GetLength(0); a++)
						{
							if (roomNode[a,0] == levelName)
							{
								tile.GetComponent<TileScript>().owner = roomNode[a, 0];	
							}	
						}
                        //Debug.Log("HDoor");
                    }
                    else if (tileType.Equals("492601"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.VDoor;
                        tile.tag = "VDoor";

						for (int a = 0; a < roomNode.GetLength(0); a++)
						{
							if (roomNode[a, 0] == levelName)
							{
								tile.GetComponent<TileScript>().owner = roomNode[a, 0];
							}
						}
                        //Debug.Log("VDoor");
                    }
                    myTileArray[(int)i + y, (int)j + x] = tile;
                    tile.GetComponent<TileScript>().myID = new Vector2(i + y, j + x);
                    tile.GetComponent<TileScript>().levelHandler = this.gameObject;
                    eHandler.GetComponent<EnemyHandler>().levelHandler = this.gameObject;
                    tile.GetComponent<TileScript>().player = selectedUnit;
                    //tile.GetComponent<TileScript>().transform.parent = transform;
                    tile.transform.parent = Room.transform;
                }
            }
        }
        cScript.MergeList();
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
		//myThread.Join();
	}

	public Vector3 TileCoordToWorldCoord(int x, int y)
	{
		return new Vector3(x * mplX, (y * mplY), 0);
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
		completeCost = 0;
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
        //foreach (Node v in graph)
        //{
        //	if (v != source)
        //	{
        //		dist[v] = Mathf.Infinity;
        //		prev[v] = null;
        //	}

        //	unvisited.Add(v);
        //}
        foreach (Node v in graph)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }

            switch (isPlayer)
            {
                case true:
                    if (x < (pathRequester.GetComponent<PlayerScript>().tileX - 12)
                     || x > (pathRequester.GetComponent<PlayerScript>().tileX + 12)
                     || y < (pathRequester.GetComponent<PlayerScript>().tileY - 12)
                     || y > (pathRequester.GetComponent<PlayerScript>().tileY + 12)
                )
                    {
                    }
                    else
                    {
                        unvisited.Add(v);
                    }
                    break;
                case false:
                    unvisited.Add(v);
                    break;
                default:
                    break;
            }
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
			//Debug.Log("cost: " + completeCost);
			foreach (Node v in u.neighbours)
			{
				float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);

				if (alt < dist[v])
				{
					dist[v] = alt;
					prev[v] = u;
				}
			}
			if (completeCost > 40)
			{
				unReachable = true;
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

		// Stop at first obstruction
		List<Node> returnPath = new List<Node>();
		for (int i = 0; i < currentPath.Count; i++)
		{
			returnPath.Add(currentPath[i]);
			if (i != 0 && myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().walkable == false)
			{
				break;
			}
		}

		currentPath = returnPath;

		//Draw new path
		if (isPlayer)
		{
			for (int i = 1; i < currentPath.Count; i++)
			{

				int currActPts = pathRequester.GetComponent<PlayerScript>().currActPts;


				if (i > currActPts)
				{
					if (myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().walkable == true)
					{
						myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().unReachable = true;
						if (i != 1)
						{
							myTileArray[currentPath[i - 1].x, currentPath[i - 1].y].GetComponent<TileScript>().goalTile = true;
						}
					}
				}

				myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().stepTile = true;
				if (i == currentPath.Count - 1)
				{
					if (myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().walkable == true)
					{
						myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().goalTile = true;
					}
					else
					{
						myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().stepTile = false;
						if (i != 1)
						{
							myTileArray[currentPath[i - 1].x, currentPath[i - 1].y].GetComponent<TileScript>().goalTile = true;
						}
						if (myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().hasEnemy == true)
						{
							targetPrefab.transform.position = myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().GetPos();
							targetPrefab.GetComponent<SpriteRenderer>().enabled = true;
						}
					}
				}
			}
		}


		if (isPlayer)
		{
			pathRequester.GetComponent<PlayerScript>().currentPath = currentPath;
		}
		else
		{
			pathRequester.GetComponent<EnemyScript>().currentPath = currentPath;
		}
	}

	public void GeneratePathTo2(int x, int y, GameObject pathRequester, bool isPlayer)
	{
		//myThread = new System.Threading.Thread(delegate() { GeneratePathTo2(x, y, pathRequester, isPlayer); });
		//myThread.Start();
	}
	

	public void ClearOldPath()
	{
		if (selectedUnit.GetComponent<PlayerScript>().currentPath != null)
		{
			//Clear old path
			for (int i = 0; i < selectedUnit.GetComponent<PlayerScript>().currentPath.Count; i++)
			{
                if (myTileArray[selectedUnit.GetComponent<PlayerScript>().currentPath[i].x,
                    selectedUnit.GetComponent<PlayerScript>().currentPath[i].y] != null)
                {
                    myTileArray[selectedUnit.GetComponent<PlayerScript>().currentPath[i].x,
                        selectedUnit.GetComponent<PlayerScript>().currentPath[i].y].GetComponent<TileScript>().ResetColor();
                }
				
			}
			targetPrefab.GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
	{
		if (UnitCanEnterTile(targetX, targetY) == false)
		{
			completeCost += 10;
			return 10;
		}
		float cost = myTileArray[targetX, targetY].GetComponent<TileScript>().moveCost;

		completeCost += cost;

		return cost;
	}

    //public void ClearCertainPath(List<Node> currentPath)
    //{
    //    if (currentPath != null)
    //    {
    //        for (int i = 0; i < currentPath.Count; i++)
    //        {
    //            myTileArray[currentPath[i].x, currentPath[i].y].GetComponent<TileScript>().ResetColor();
    //        }
    //    }
    //}

    
}
