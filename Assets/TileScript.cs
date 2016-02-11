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
	
	void Awake()
	{
		mySprite = Resources.LoadAll<Sprite>("tileSet");
	}

	void Start () {
		spr = GetComponent<SpriteRenderer>();
		spr.sprite = mySprite[(int)myTileType];
		
	}
	
	void Update () {
		spr.sprite = mySprite[(int)myTileType];
	}
}
