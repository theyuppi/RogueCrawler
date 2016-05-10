using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class PlayerScript : MonoBehaviour
{
	private Rigidbody2D rBody;
	private Coroutine playerMovement;
	private SpriteRenderer sRender;
	private Animator animaThor;
	public TileScript tScript;
	public GameObject tile;

	public const float stepDuration = 0.2f;
	public const float stepAttackDuration = 0.15f;
	public const float stepDoorDuration = 0.5f;
	public const float stepDoorDistance = 140;
	public List<Node> currentPath = null;
	public ReadSpriteScript map;
	public int tileX = 0;
	public int tileY = 0;
	public int tileXmoved = 0;
	public int tileYmoved = 0;
	public bool isMoving = false;
	public bool isPerformingAttack = false;
	public bool lastNotWalkable = false;
	public bool myTurn = false;
	public Text healthText;

	//Stats
	public int xp = 0;
	private int health = 100;
	private int maxHealth = 100;
	public int attackPower = 0;
	public int currActPts = 0;
	private int maxActPts = 100;
	public int skillPointsRemaining = 0;
	public int skillPointsPerLevel = 2;
	private int base_vitality = 5;
	private int base_strength = 5;
	private int base_defence = 5;
	private int base_speed = 5;
	private int base_agility = 5;

	private int vitality = 0;
	private int strength = 0;
	private int defence = 0;
	private int speed = 0;
	private int agility = 0;
	private int recoveryRate = 0;
	private int combatStartAP = 0; //Actionpoints at start of combat

	public InventoryScript inventory;
	public List<Text> statsLabels;

	//Gear
	Item mainhand_Slot = null;
	Item offhand_Slot = null;
	Item helm_Slot = null;
	Item armor_Slot = null;
	Item leggings_Slot = null;
	int bonusStr = 0;
	int bonusDef = 0;
	int bonusVit = 0;

	//States
	public bool stunned = false;

	public int charLVL = 1;
	public List<int> xpLevels = new List<int>{
		10, 20, 30, 40, 50
	};

	public float myOffsetX = 0;
	public float myOffsetY = 20f;

	private enum direction
	{
		Up,
		Down,
		Left,
		Right
	}

	private direction myDirection = direction.Right;

	void Start()
	{
		rBody = GetComponent<Rigidbody2D>();
		sRender = GetComponent<SpriteRenderer>();
		animaThor = GetComponent<Animator>();
		animaThor.SetInteger("State", 0);
		healthText = GetComponentInChildren<Text>();
		healthText.text = health.ToString();

		StringBuilder str = new StringBuilder();
		str.Append(health.ToString());
		str.Append(" / ");
		str.Append(maxHealth.ToString());
		statsLabels[9].text = str.ToString();

		ReceiveActPts();
	}

	private void FixedUpdate()
	{
		//Level up
		if (xp >= xpLevels[charLVL - 1])
		{
			xp = 0;
			charLVL++;
		}
		UpdateStats();
	}

	public void UpdateStats()
	{
		bonusStr = 0;
		bonusDef = 0;
		bonusVit = 0;
		mainhand_Slot = null;
		offhand_Slot = null;
		helm_Slot = null;
		armor_Slot = null;
		leggings_Slot = null;

		foreach (Transform child in inventory.eqSlots[1].transform)
		{
			if (child.CompareTag("Item"))
			{
				mainhand_Slot = child.GetComponent<ItemDataScript>().item;
			}
		}

		foreach (Transform child in inventory.eqSlots[2].transform)
		{
			if (child.CompareTag("Item"))
			{
				offhand_Slot = child.GetComponent<ItemDataScript>().item;
			}
		}

		foreach (Transform child in inventory.eqSlots[0].transform)
		{
			if (child.CompareTag("Item"))
			{
				helm_Slot = child.GetComponent<ItemDataScript>().item;
			}
		}

		foreach (Transform child in inventory.eqSlots[3].transform)
		{
			if (child.CompareTag("Item"))
			{
				armor_Slot = child.GetComponent<ItemDataScript>().item;
			}
		}

		foreach (Transform child in inventory.eqSlots[4].transform)
		{
			if (child.CompareTag("Item"))
			{
				leggings_Slot = child.GetComponent<ItemDataScript>().item;
			}
		}

		if (mainhand_Slot != null)
		{
			bonusStr += mainhand_Slot.stats.power;
			bonusDef += mainhand_Slot.stats.defence;
			bonusVit += mainhand_Slot.stats.vitality;
		}
		if (offhand_Slot != null)
		{
			bonusStr += offhand_Slot.stats.power;
			bonusDef += offhand_Slot.stats.defence;
			bonusVit += offhand_Slot.stats.vitality;
		}
		if (helm_Slot != null)
		{
			bonusStr += helm_Slot.stats.power;
			bonusDef += helm_Slot.stats.defence;
			bonusVit += helm_Slot.stats.vitality;
		}
		if (armor_Slot != null)
		{
			bonusStr += armor_Slot.stats.power;
			bonusDef += armor_Slot.stats.defence;
			bonusVit += armor_Slot.stats.vitality;
		}
		if (leggings_Slot != null)
		{
			bonusStr += leggings_Slot.stats.power;
			bonusDef += leggings_Slot.stats.defence;
			bonusVit += leggings_Slot.stats.vitality;
		}

		strength = base_strength + bonusStr;
		defence = base_defence + bonusDef;
		vitality = base_vitality + bonusVit;
		maxHealth = 100 + vitality;

		statsLabels[1].text = charLVL.ToString();
		statsLabels[2].text = xp.ToString();
		statsLabels[3].text = maxActPts.ToString();
		statsLabels[4].text = vitality.ToString();
		statsLabels[5].text = strength.ToString();
		statsLabels[6].text = agility.ToString();
		statsLabels[7].text = speed.ToString();
		statsLabels[8].text = defence.ToString();
		statsLabels[10].text = attackPower.ToString();

		StringBuilder str = new StringBuilder();
		str.Append(health.ToString());
		str.Append(" / ");
		str.Append(maxHealth.ToString());
		statsLabels[9].text = str.ToString();

		attackPower = strength;  //Magic formula(TM)
	}

	#region ManualMove
	private IEnumerator Move(Vector2 direction)
	{
		Vector2 startPosition = transform.position;
		Vector2 destinationPosition = new Vector2(transform.position.x, transform.position.y) + (direction * 0.8f);
		float t = 0.0f;

		while (t < 1.0f)
		{
			transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
			t += Time.deltaTime / stepDuration;
			yield return new WaitForEndOfFrame();
		}

		transform.position = destinationPosition;

		playerMovement = null;
	}
	#endregion

	public IEnumerator MakeAMove()
	{

		int currNode = 0;
		isMoving = true;
		while (isMoving == true && currentPath != null && currNode < currentPath.Count - 1)
		{
			if (stunned)
			{
				yield return new WaitForSeconds(0.5f);
				stunned = false;
			}
			if (currActPts > 0)
			{
				Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
					new Vector3(0, 0, -1f);
				Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
					new Vector3(0, 0, -1f);

				//Debug.DrawLine(start, end, Color.red);

				currNode++;


				StartCoroutine(MoveNextTile(currNode));

				if (currentPath == null || currNode == currentPath.Count - 1)
				{
					isMoving = false;
				}
				yield return new WaitForSeconds(stepDuration);
			}
			else
			{
				break;
			}
		}

		map.ClearOldPath();
		currentPath = null;
	}

	public IEnumerator MoveNextTile(int currNode)
	{
		if (currentPath == null)
			yield return new WaitForEndOfFrame();

		// Move us to the next tile in the sequence

		tileX = currentPath[currNode].x;
		tileY = currentPath[currNode].y;

		Vector2 startPosition = transform.position;

		Vector2 destinationPosition = map.TileCoordToWorldCoord(tileX, tileY);
		destinationPosition.y += myOffsetY;
		Vector2 dir = destinationPosition - startPosition;
		dir.Normalize();
		switch ((int)dir.x)
		{
			case 1:
				sRender.flipX = true;
				break;

			case -1:
				sRender.flipX = false;
				break;

			default:
				break;
		}

		// Attacking enemy or walking in to wall
		if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().walkable == false)
		{
			if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().hasEnemy == true)
			{
				Vector2 roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
				if (roundDir == Vector2.up)
				{
					myDirection = direction.Up;
				}
				else if (roundDir == Vector2.down)
				{
					myDirection = direction.Down;
				}
				else if (roundDir == Vector2.left)
				{
					myDirection = direction.Left;
				}
				else if (roundDir == Vector2.right)
				{
					myDirection = direction.Right;
				}
				if (isPerformingAttack == false)
				{
					if (currActPts >= 2)
					{
						PerformAttack(myDirection);
						currActPts -= 2;
					}
				}
			}
			// Walking in to door
			if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().isDoor == true)
			{
				yield return new WaitForSeconds(0.05f);

				if (tileX < 2)
				{
					for (int a = 0; a < map.roomNode.GetLength(0); a++)
					{
						if (map.roomNode[a, 0] == map.myTileArray[tileX, tileY].GetComponent<TileScript>().owner)
						{
							//Debug.Log("Creating room: " + map.roomNode[a, 3]);
							StartCoroutine(FadeInOut(a, 3)); //West
						}
					}
				}
				else if (tileX > 36)
				{
					for (int a = 0; a < map.roomNode.GetLength(0); a++)
					{
						if (map.roomNode[a, 0] == map.myTileArray[tileX, tileY].GetComponent<TileScript>().owner)
						{
							//Debug.Log("Creating room: " + map.roomNode[a, 4]);
							StartCoroutine(FadeInOut(a, 4)); //East
						}
					}
				}
				else if (tileY < 2)
				{
					for (int a = 0; a < map.roomNode.GetLength(0); a++)
					{
						if (map.roomNode[a, 0] == map.myTileArray[tileX, tileY].GetComponent<TileScript>().owner)
						{
							//Debug.Log("Creating room: " + map.roomNode[a, 2]);
							StartCoroutine(FadeInOut(a, 2)); //South
						}
					}
				}
				else if (tileY > 36)
				{
					for (int a = 0; a < map.roomNode.GetLength(0); a++)
					{
						//Debug.Log(map.myTileArray[tileX, tileY].GetComponent<TileScript>().owner);
						if (map.roomNode[a, 0] == map.myTileArray[tileX, tileY].GetComponent<TileScript>().owner)
						{
							//Debug.Log("Creating room: " + map.roomNode[a, 1]);
							StartCoroutine(FadeInOut(a, 1)); //Pacifica North
						}
					}

				}
			}
			

			else if (currentPath != null)
			{
				tileX = currentPath[currNode - 1].x;
				tileY = currentPath[currNode - 1].y;
			}

		}
		// Walking in to portal
		if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().isEndPortal == true)
		{
			Vector2 roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
			if (roundDir == Vector2.up)
			{
				myDirection = direction.Up;
			}
			else if (roundDir == Vector2.down)
			{
				myDirection = direction.Down;
			}
			else if (roundDir == Vector2.left)
			{
				myDirection = direction.Left;
			}
			else if (roundDir == Vector2.right)
			{
				myDirection = direction.Right;
			}
			
			StartCoroutine(EnterPortal((int)myDirection)); //Enter Next Level
		}
		else
		{
			// Lerp to new position
			currActPts--;
			float t = 0.0f;
			while (t <= 1.1f)
			{
				transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
				t += Time.deltaTime / stepDuration;
				yield return new WaitForEndOfFrame();
			}

			if (currentPath != null)
			{
				tileXmoved = currentPath[currNode].x;
				tileYmoved = currentPath[currNode].y;
			}

			map.myTileArray[tileXmoved, tileYmoved].GetComponent<TileScript>().ResetColor();
		}


		if (currentPath != null && currNode == currentPath.Count)
		{
			currentPath = null;
		}
	}

	public int GetGoalTileX()
	{
		if (currentPath != null)
		{
			return currentPath[currentPath.Count - 1].x;
		}
		return 0;
	}
	public int GetGoalTileY()
	{
		return currentPath[currentPath.Count - 1].y;
	}

	private void PerformAttack(direction dir)
	{
		isPerformingAttack = true;
		switch (dir)
		{
			case direction.Up:
				StartCoroutine(PerformAttackMove(Vector2.up));
				animaThor.SetInteger("State", 1);
				break;
			case direction.Down:
				StartCoroutine(PerformAttackMove(Vector2.down));
				animaThor.SetInteger("State", 2);
				break;
			case direction.Left:
				StartCoroutine(PerformAttackMove(Vector2.left));
				animaThor.SetInteger("State", 1);
				break;
			case direction.Right:
				StartCoroutine(PerformAttackMove(Vector2.right));
				animaThor.SetInteger("State", 1);

				break;
			default:
				break;
		}
		map.myTileArray[tileX, tileY].GetComponent<TileScript>().CharOnTileGetHit(attackPower, false);
		map.ClearOldPath();
		StartCoroutine(SetAttackFalse());

	}

	private IEnumerator PerformAttackMove(Vector2 dir)
	{
		Vector2 startPosition = transform.position;
		Vector2 destinationPosition = startPosition + (dir * 40f);
		float t = 0.0f;
		while (t < 1.1f)
		{
			transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
			t += Time.deltaTime / stepAttackDuration;
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(0.6f);
		animaThor.SetInteger("State", 0);
		t = 0.0f;
		while (t < 1.1f)
		{
			transform.position = Vector2.Lerp(destinationPosition, startPosition, t);
			t += Time.deltaTime / stepAttackDuration;
			yield return new WaitForEndOfFrame();
		}
	}

	private IEnumerator SetAttackFalse()
	{
		yield return new WaitForSeconds(1.0f);
		isPerformingAttack = false;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
		{
			other.GetComponent<TileScript>().walkable = false;
			other.GetComponent<TileScript>().occupant = this.gameObject;
			other.GetComponent<TileScript>().hasPlayer = true;
			tile = other.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
		{
			other.GetComponent<TileScript>().walkable = true;
			other.GetComponent<TileScript>().occupant = null;
			other.GetComponent<TileScript>().hasPlayer = false;
		}
	}

	public IEnumerator GetHit(int damageAmount)
	{
		yield return new WaitForSeconds(0.5f);
		int damageTaken = damageAmount - defence;
		damageTaken = Mathf.Clamp(damageTaken, 0, 100000);
		health -= damageTaken;
		healthText.text = health.ToString();
	}

	public void ReceiveActPts()
	{
		currActPts = maxActPts;
	}

	public void GainXP(int gainedXP)
	{
		xp += gainedXP;
	}

	//public void BumpMe(int x, int y)
	//{

	//    Vector2 pos = transform.position;
	//    pos = new Vector2(pos.x + x, pos.y + y);
	//    transform.position = pos;
	//    if (x > 0)
	//        x -= 1;
	//    if (y > 0)
	//        y -= 1;
	//    tileX += x;
	//    tileY += y;
	//}

	public void BumpMe(int dir)
	{
		if (dir == 1 || dir == 2)
		{
			if (tileY == 0)
				tileY = 2;
			tileY = (map.gridSizeY) - tileY;
			Vector2 pos = transform.position;
			if (dir == 2)
				pos = new Vector2(pos.x, tileY * 100 + myOffsetY + stepDoorDistance);
			else
				pos = new Vector2(pos.x, tileY * 100 + myOffsetY - stepDoorDistance);
			transform.position = pos;

		}
		else if (dir == 3 || dir == 4)
		{
			if (tileX == 0)
				tileX = 2;
			tileX = (map.gridSizeX) - tileX;
			Vector2 pos = transform.position;
			if (dir == 3)
				pos = new Vector2(tileX * 100 + stepDoorDistance, pos.y);
			else
				pos = new Vector2(tileX * 100 - stepDoorDistance, pos.y);
			transform.position = pos;
		}
	}

	private IEnumerator FadeInOut(int a, int b)
	{
		if (b == 1)
			StartCoroutine(MoveToDoor(Vector2.up));
		else if (b == 2)
			StartCoroutine(MoveToDoor(Vector2.down));
		else if (b == 3)
			StartCoroutine(MoveToDoor(Vector2.left));
		else if (b == 4)
			StartCoroutine(MoveToDoor(Vector2.right));

		yield return new WaitForSeconds(1f);
		float fadeTime = GetComponent<Fading>().BeginFade(1);
		yield return new WaitForSeconds(fadeTime);
		map.MakeRoom(0, 0, map.roomNode[a, b]);
		BumpMe(b);
		//yield return new WaitForSeconds(fadeTime);

		GetComponent<Fading>().BeginFade(-1);
		yield return new WaitForSeconds(1f);

		if (b == 1)
			StartCoroutine(MoveToDoor(Vector2.up));
		else if (b == 2)
			StartCoroutine(MoveToDoor(Vector2.down));
		else if (b == 3)
			StartCoroutine(MoveToDoor(Vector2.left));
		else if (b == 4)
			StartCoroutine(MoveToDoor(Vector2.right));
	}

	private IEnumerator MoveToDoor(Vector2 dir)
	{
		Vector2 startPosition = transform.position;
		Vector2 destinationPosition = startPosition + (dir * stepDoorDistance);
		float t = 0.0f;
		while (t < 1.1f)
		{
			transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
			t += Time.deltaTime / stepDoorDuration;
			yield return new WaitForEndOfFrame();
		}
	}


	private IEnumerator EnterPortal(int b)
	{
		if (b == 0)
			StartCoroutine(MoveToDoor(Vector2.up));
		else if (b == 1)
			StartCoroutine(MoveToDoor(Vector2.down));
		else if (b == 2)
			StartCoroutine(MoveToDoor(Vector2.left));
		else if (b == 3)
			StartCoroutine(MoveToDoor(Vector2.right));

		yield return new WaitForSeconds(1f);
		float fadeTime = GetComponent<Fading>().BeginFade(1);
		yield return new WaitForSeconds(fadeTime);
		//map.MakeRoom(0, 0, map.roomNode[a, b]);
		//BumpMe(b);
		//yield return new WaitForSeconds(fadeTime);
		map.GoToLevel(1);
		GetComponent<Fading>().BeginFade(-1);
	}
}
