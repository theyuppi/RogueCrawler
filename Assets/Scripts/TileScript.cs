using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public class TileScript : MonoBehaviour
{

	SpriteRenderer spr;
	public Sprite[] mySprite;
	public enum TileTypes
	{
        Wall,
		Floor,

        Innan,
		Hole,
		Spike,
		Chest,
		LDoor,
		RDoor,
		UDoor,
		DDoor,
		FloorEnd
	}
	public TileTypes myTileType = TileTypes.Floor;
	public GameObject chestPrefab;
	public GameObject spikePrefab;
	public GameObject holePrefab;
    public GameObject doorLprefab;
	public GameObject doorRprefab;
    public GameObject doorUprefab;
	public GameObject doorDprefab;
    public GameObject roofPrefab;
    public GameObject occupant;
	public GameObject endPortalPrefab;

	public string owner;
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
    public bool isDoor = false;
    

	void Awake()
	{
        //mySprite = Resources.LoadAll<Sprite>("tileSet");
        //mySprite = Resources.LoadAll<Sprite>("tilesSquashed");
        mySprite = Resources.LoadAll<Sprite>("bigtiles");
        myTileType = TileTypes.Floor;
    }

	void Start()
	{
		spr = GetComponent<SpriteRenderer>();
		//spr.sprite = mySprite[(int)myTileType];

		if (myTileType == TileTypes.Floor)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
		}
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
			//tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
			moveCost = 5.0f;
		}
		else if (myTileType == TileTypes.Wall)
		{
			spr.sprite = mySprite[(int)TileTypes.Wall];
			walkable = false;
            GameObject tileAddon = Instantiate(roofPrefab, transform.position, transform.rotation) as GameObject;
            tileAddon.transform.parent = transform;
            var locPos = tileAddon.transform.localPosition;
            locPos.y = 60f;
            tileAddon.transform.localPosition = locPos;
            tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 9;
        }
		else if (myTileType == TileTypes.Hole)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			GameObject tileAddon = Instantiate(holePrefab, transform.position, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
			moveCost = 30.0f;
		}
        else if (myTileType == TileTypes.LDoor)
        {
            spr.sprite = mySprite[(int)TileTypes.Floor];
			var pos = transform.position;
			pos.x -= 49;
			pos.y += 33;
            GameObject tileAddon = Instantiate(doorLprefab, pos, transform.rotation) as GameObject;
            tileAddon.transform.parent = transform;
            walkable = false;
            isDoor = true;
        }
        else if (myTileType == TileTypes.RDoor)
        {
            spr.sprite = mySprite[(int)TileTypes.Floor];
			var pos = transform.position;
			pos.x += 49;
			pos.y += 33;
			GameObject tileAddon = Instantiate(doorRprefab, pos, transform.rotation) as GameObject;
            tileAddon.transform.parent = transform;
            walkable = false;
            isDoor = true;
        }
		else if (myTileType == TileTypes.UDoor)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			var pos = transform.position;
			pos.y += 22;
			GameObject tileAddon = Instantiate(doorUprefab, pos, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			walkable = false;
			isDoor = true;
		}
		else if (myTileType == TileTypes.DDoor)
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			var pos = transform.position;
			pos.y += 24;
			GameObject tileAddon = Instantiate(doorDprefab, pos, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			walkable = false;
			isDoor = true;
		}
        //OccupyTile();


		else if (myTileType == TileTypes.FloorEnd)  //EndTile
		{
			spr.sprite = mySprite[(int)TileTypes.Floor];
			GameObject tileAddon = Instantiate(endPortalPrefab, transform.position, transform.rotation) as GameObject;
			tileAddon.transform.parent = transform;
			tileAddon.GetComponent<SpriteRenderer>().sortingOrder = 1;
		}

    }
	
	void OnMouseUp()
	{
		//if (player.GetComponent<PlayerScript>().isPerformingAttack == false && player.GetComponent<PlayerScript>().myTurn == true)
		//{
		//	// Clicked while moving to cancel movement
		//	if (player.GetComponent<PlayerScript>().isMoving == true)
		//	{
		//		doubleclicked = 0;
		//		player.GetComponent<PlayerScript>().isMoving = false;
		//		levelHandler.GetComponent<ReadSpriteScript>().ClearOldPath();
		//		player.GetComponent<PlayerScript>().currentPath = null;
		//	}

		//	// Generate movement path (First click)
		//	if (player.GetComponent<PlayerScript>().currentPath == null)
		//	{
		//		if (doubleclicked > 0)
		//			levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y, player, true);
		//	}

		//	// Move through generated path (Second click on same tile)
		//	else if (myID.x == player.GetComponent<PlayerScript>().GetGoalTileX()
		//		&& myID.y == player.GetComponent<PlayerScript>().GetGoalTileY())
		//	{
		//		StartCoroutine(player.GetComponent<PlayerScript>().MakeAMove());
		//	}

		//	// Generate new movement path (Clicked on another tile)
		//	else if (player.GetComponent<PlayerScript>().currentPath != null)
		//	{
		//		levelHandler.GetComponent<ReadSpriteScript>().ClearOldPath();
		//		levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y, player, true);
		//	}
		//	doubleclicked++;
		//}
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
                //Debug.Log("myID: " + myID);
                //Debug.Log("occupant: " + occupant.ToString());
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

    //void OnDestroy()
    //{
    //    if (occupant != null)
    //    {
    //        occupant.GetComponent<EnemyScript>().Destroy();
    //    }
    //}

	public void GotClicked()
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
			else if (player.GetComponent<PlayerScript>().currentPath != null)
			{
				levelHandler.GetComponent<ReadSpriteScript>().ClearOldPath();
				levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo((int)myID.x, (int)myID.y, player, true);
			}
			doubleclicked++;
		}
	}
}
