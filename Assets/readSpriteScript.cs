using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class readSpriteScript : MonoBehaviour
{
	SpriteRenderer sr;
	Sprite spr;

	public GameObject selectedUnit;
	public GameObject tilePrefab;

	private int gridSizeX = 50;
	private int gridSizeY = 50;
	private int tileSizeX = 80;
	private int tileSizeY = 80;

	public GameObject[,] myTileArray;
	Node[,] graph;

	void Start()
	{
		selectedUnit.GetComponent<playerMovementScript>().map = this;
		myTileArray = new GameObject[gridSizeX, gridSizeY];
		CreateRoom("maproom" + 1);
		GeneratePathfindingGraph();
	}

	void Update()
	{}

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
					GameObject tile = Instantiate(tilePrefab, new Vector2(i * 0.8f, (sizeX -j) * -0.8f), transform.rotation) as GameObject;

					if (tileType.Equals("C4AA6C"))
					{
						tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Floor;
						//Debug.Log("Floor");
					}
					else if (tileType.Equals("584A33"))
					{
						tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Wall;
						//Debug.Log("Wall");
					}
					else if (tileType.Equals("FF0000"))
					{
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
		return new Vector3(0.4f + y * 0.8f, -1.2f + (-23.2f + x * 0.8f), 0);
	}

	public bool UnitCanEnterTile(int x, int y)
	{
		// We could test the unit's walk/hover/fly type against various
		// terrain flags here to see if they are allowed to enter the tile.
		//Debug.Log(myTileArray[x, y].GetComponent<TileScript>().walkable);
		//Debug.Log("X = " + x + " Y = " + y);
		return myTileArray[x, y].GetComponent<TileScript>().walkable;
	}



	public void GeneratePathTo(int x, int y)
	{
		// Clear out our unit's old path.
		selectedUnit.GetComponent<playerMovementScript>().currentPath = null;

		if (UnitCanEnterTile(x, y) == false)
		{
			// We probably clicked on a mountain or something, so just quit out.
			return;
		}

		Dictionary<Node, float> dist = new Dictionary<Node, float>();
		Dictionary<Node, Node> prev = new Dictionary<Node, Node>();

		// Setup the "Q" -- the list of nodes we haven't checked yet.
		List<Node> unvisited = new List<Node>();
		Node source = graph[
							selectedUnit.GetComponent<playerMovementScript>().tileX,
							selectedUnit.GetComponent<playerMovementScript>().tileY
							];

		Node target = graph[
							x,
							y
							];

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
				break;	// Exit the while loop!
			}

			unvisited.Remove(u);

			foreach (Node v in u.neighbours)
			{
				float alt = dist[u] + u.DistanceTo(v);
				//float alt = dist[u] + CostToEnterTile(u.x, u.y, v.x, v.y);
				if (alt < dist[v])
				{
					dist[v] = alt;
					prev[v] = u;
				}
			}
		}

		// If we get there, the either we found the shortest route
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

		selectedUnit.GetComponent<playerMovementScript>().currentPath = currentPath;
	}

	public float CostToEnterTile(int sourceX, int sourceY, int targetX, int targetY)
	{
		if (UnitCanEnterTile(targetX, targetY) == false)
		{
			return Mathf.Infinity;
		}
		float cost = myTileArray[targetX, targetY].GetComponent<TileScript>().moveCost;
		return cost;
	}
}
