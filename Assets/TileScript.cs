using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileScript : MonoBehaviour
{

	SpriteRenderer spr;
	public Sprite[] mySprite;
	public enum TileTypes
	{
		Floor,
		Wall,
		Hole,
		Spike,
		Chest,
		HDoor,
		VDoor
	}
	public TileTypes myTileType = TileTypes.Floor;
	public GameObject chestPrefab;
	public GameObject spikePrefab;
	public GameObject holePrefab;

	public float moveCost = 1.0f;
	public bool walkable = true;
	public Vector2 myID;
	public GameObject levelHandler;
	public GameObject player;

	public bool GoalTile = false;
	public bool StepTile = false;

	void Awake()
	{
		mySprite = Resources.LoadAll<Sprite>("tileSet");
	}

	void Start()
	{
		spr = GetComponent<SpriteRenderer>();
		spr.sprite = mySprite[(int)myTileType];


		if (myTileType == TileTypes.Chest)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			GameObject tileAddon = Instantiate(chestPrefab, transform.position, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}
		else if (myTileType == TileTypes.Spike)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			GameObject tileAddon = Instantiate(spikePrefab, transform.position, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}
		else if (myTileType == TileTypes.Wall)
		{
			walkable = false;
		}
		else if (myTileType == TileTypes.Hole)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			GameObject tileAddon = Instantiate(holePrefab, transform.position, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}
	}

	void OnMouseUp()
	{
		//Debug.Log("YOU CLICKED ME! " + myID);
		if (player.GetComponent<playerMovementScript>().currentPath == null)
		{
			levelHandler.GetComponent<readSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y);
		}

		else if (myID.x == player.GetComponent<playerMovementScript>().GetGoalTileX()
			&& myID.y == player.GetComponent<playerMovementScript>().GetGoalTileY())
		{
			//player.GetComponent<playerMovementScript>().MakeAMove();
			StartCoroutine(player.GetComponent<playerMovementScript>().MakeAMove());
		}
		else
		{
			levelHandler.GetComponent<readSpriteScript>().ClearOldPath();
			levelHandler.GetComponent<readSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y);
		}
	}

	void FixedUpdate()
	{
		if (StepTile)
		{
			StepColor();
		}
		if (GoalTile)
		{
			GoalColor();
		}
	}

	private void StepColor()
	{
		GetComponent<SpriteRenderer>().color = Color.red;
	}

	private void GoalColor()
	{
		GetComponent<SpriteRenderer>().color = Color.green;
	}

	public void ResetColor()
	{
		GoalTile = false;
		StepTile = false;
		GetComponent<SpriteRenderer>().color = Color.white;
	}
}
