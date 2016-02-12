using UnityEngine;
using System.Collections;

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
	}
	
	//void Update () {
	//	spr.sprite = mySprite[(int)myTileType];
	//}
}
