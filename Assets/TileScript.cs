using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileScript : MonoBehaviour {

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

    void Awake()
	{
		mySprite = Resources.LoadAll<Sprite>("tileSet");
	}

	void Start () {
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
<<<<<<< HEAD
		else if (myTileType == TileTypes.Wall)
		{
			walkable = false;
		}
	}
=======
        else if (myTileType == TileTypes.Hole)
        {
            spr.sprite = mySprite[(int)TileTypes.Floor];
            GameObject tileAddon = Instantiate(holePrefab, transform.position, transform.rotation) as GameObject;
            tileAddon.transform.parent = transform;
            tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }
    }
>>>>>>> origin/dev
	
	void OnMouseUp()
	{
		//Debug.Log("YOU CLICKED ME! " + myID);
		levelHandler.GetComponent<readSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y);
	}
}
