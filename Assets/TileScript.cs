﻿using UnityEngine;
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
    int doubleclicked = 1;

    public bool GoalTile = false;
	public bool StepTile = false;
    public bool hasEnemy = false;
    

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
        // Clicked while moving to cancel movement
        Debug.Log("Clicked on tile");
        if (player.GetComponent<playerMovementScript>().isMoving == true)
        {
            doubleclicked = 0;
            player.GetComponent<playerMovementScript>().isMoving = false;
            levelHandler.GetComponent<readSpriteScript>().ClearOldPath();
            player.GetComponent<playerMovementScript>().currentPath = null;
        }

        // Generate movement path (First click)
        if (player.GetComponent<playerMovementScript>().currentPath == null)
		{
            if (doubleclicked > 0)
			    levelHandler.GetComponent<readSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y);
		}

        // Move through generated path (Second click on same tile)
		else if (myID.x == player.GetComponent<playerMovementScript>().GetGoalTileX()
			&& myID.y == player.GetComponent<playerMovementScript>().GetGoalTileY())
		{
            StartCoroutine(player.GetComponent<playerMovementScript>().MakeAMove());
		}

        // Generate new movement path (Clicked on another tile)
        else
        {
			levelHandler.GetComponent<readSpriteScript>().ClearOldPath();
			levelHandler.GetComponent<readSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y);
		}
        doubleclicked++;
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
        //GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<SpriteRenderer>().color = Color.green;
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
