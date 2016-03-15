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
    public GameObject occupant;

	public float moveCost = 1.0f;
	public bool walkable = true;
	public Vector2 myID;
	public GameObject levelHandler;
	public GameObject player;
    int doubleclicked = 1;

    public bool goalTile = false;
	public bool stepTile = false;
    public bool hasEnemy = false;
    public bool hasPlayer = false;
    public bool unReachable = false;
    

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
        //OccupyTile();
	}

	void OnMouseUp()
	{
        if (player.GetComponent<PlayerScript>().isPerformingAttack == false && player.GetComponent<PlayerScript>().myTurn == true)
        {
            // Clicked while moving to cancel movement
            if (player.GetComponent<PlayerScript>().isMoving == true)
            {
                doubleclicked = 0;
                player.GetComponent<PlayerScript>().isMoving = false;
                levelHandler.GetComponent<ReadSpriteScript>().ClearOldPath();
                player.GetComponent<PlayerScript>().currentPath = null;
            }

            // Generate movement path (First click)
            if (player.GetComponent<PlayerScript>().currentPath == null)
            {
                if (doubleclicked > 0)
                    levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y, player, true);
            }

            // Move through generated path (Second click on same tile)
            else if (myID.x == player.GetComponent<PlayerScript>().GetGoalTileX()
                && myID.y == player.GetComponent<PlayerScript>().GetGoalTileY())
            {
                StartCoroutine(player.GetComponent<PlayerScript>().MakeAMove());
            }

            // Generate new movement path (Clicked on another tile)
            else
            {
                levelHandler.GetComponent<ReadSpriteScript>().ClearOldPath();
                levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y, player, true);
            }
            doubleclicked++;
        }
    }

	void FixedUpdate()
	{
        if (unReachable)
        {
            UnreachableColor();
        }
        else
        {
            if (stepTile)
            {
                StepColor();
            }
            if (goalTile)
            {
                GoalColor();
            }
        }
    }

	private void StepColor()
	{
        //GetComponent<SpriteRenderer>().color = Color.red;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

	private void GoalColor()
	{
        GetComponent<SpriteRenderer>().color = Color.cyan;
	}

    private void UnreachableColor()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

    public void ResetColor()
	{
		goalTile = false;
		stepTile = false;
        unReachable = false;
		GetComponent<SpriteRenderer>().color = Color.white;
	}

    public Vector3 GetPos()
    {
        return transform.position;
    }

    public void CharOnTileGetHit(int damageAmount, bool isPlayer)
    {
        if (occupant != null)
        {
            if (isPlayer)
            {
                Debug.Log("myID: " + myID);
                Debug.Log("occutpant: " + occupant.ToString());
                StartCoroutine(occupant.GetComponent<PlayerScript>().GetHit(damageAmount));
                
            }
            else
            {
                StartCoroutine(occupant.GetComponent<EnemyScript>().GetHit(damageAmount));
            }
        }
    }

    public void OccupyTile()
    {
        if (occupant != null && occupant.GetComponents<EnemyScript>().Length != 0)
        {
            occupant.GetComponent<EnemyScript>().tile = this.gameObject;
        }
        if (occupant != null && occupant.GetComponents<PlayerScript>().Length != 0)
        {
            occupant.GetComponent<PlayerScript>().tile = this.gameObject;
        }
    }
}
