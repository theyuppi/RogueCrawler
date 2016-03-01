using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerMovementScript : MonoBehaviour
{

	private Rigidbody2D rBody;
	private Coroutine playerMovement;
	private SpriteRenderer sRender;
	private Animator animaThor;

	public const float stepDuration = 0.5f;
	public List<Node> currentPath = null;
	public readSpriteScript map;
	public int tileX = 16;
	public int tileY = 0;
	int moveSpeed = 100;
	public bool moving = false;

	//Time stuff
	private float timeBetweenSteps = 1.0f;
	private float currentTime = 0.0f;

	void Start()
	{
		rBody = GetComponent<Rigidbody2D>();
		sRender = GetComponent<SpriteRenderer>();
		animaThor = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
		//currentTime += Time.deltaTime;
		//if (currentTime >= timeBetweenSteps)
		//{
		//	if (currentPath != null)
		//	{
		//		int currNode = 0;

		//		while (currNode < currentPath.Count - 1)
		//		{
		//			Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
		//				new Vector3(0, 0, -1f);
		//			Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
		//				new Vector3(0, 0, -1f);

		//			Debug.DrawLine(start, end, Color.red);

		//			currNode++;
		//			MoveNextTile();
		//		}
		//	//}
		//	//currentTime = 0;
		//}

		if (animaThor.GetInteger("State") != 0)
		{
			animaThor.SetInteger("State", 0);
		}
		if (playerMovement == null)
		{
			if (Input.GetKey(KeyCode.W))
				playerMovement = StartCoroutine(Move(Vector2.up));
			else if (Input.GetKey(KeyCode.S))
				playerMovement = StartCoroutine(Move(Vector2.down));
			else if (Input.GetKey(KeyCode.D))
			{
				sRender.flipX = true;
				playerMovement = StartCoroutine(Move(Vector2.right));
			}
			else if (Input.GetKey(KeyCode.A))
			{
				sRender.flipX = false;
				playerMovement = StartCoroutine(Move(Vector2.left));
			}
		}

		if (Input.GetKey(KeyCode.Alpha1))
			animaThor.SetInteger("State", 0);
		if (Input.GetKey(KeyCode.Alpha2))
			animaThor.SetInteger("State", 1);
		if (Input.GetKey(KeyCode.Alpha3))
			animaThor.SetInteger("State", 2);

		//Debug.Log("My X is = " + tileX);
		//Debug.Log("My Y is = " + tileY);
	}

	//public void MakeAMove()
	//{
	//	if (currentPath != null)
	//	{
	//		int currNode = 0;

	//		while (currentPath != null && currNode < currentPath.Count - 1)
	//		{
	//			Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
	//				new Vector3(0, 0, -1f);
	//			Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
	//				new Vector3(0, 0, -1f);

	//			Debug.DrawLine(start, end, Color.red);
				
	//			currNode++;
	//			MoveNextTile(currNode);
	//		}
	//	}
	//}

	public IEnumerator MakeAMove()
	{
		if (currentPath != null)
		{
			int currNode = 0;

			while (currentPath != null && currNode < currentPath.Count - 1)
			{
				Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
					new Vector3(0, 0, -1f);
				Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
					new Vector3(0, 0, -1f);

				Debug.DrawLine(start, end, Color.red);

				currNode++;
				MoveNextTile(currNode);
				yield return new WaitForSeconds(stepDuration);
			}
		}
	}


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

	public void MoveNextTile(int currNode)
	{
		//float remainingMovement = moveSpeed;

		//while (remainingMovement > 0)
		//{
			if (currentPath == null)
				return;

			// Get cost from current tile to next tile
			//remainingMovement -= map.CostToEnterTile(tileX, tileY, currentPath[1].x, currentPath[1].y);

			// Move us to the next tile in the sequence
			tileX = currentPath[currNode].x;
			tileY = currentPath[currNode].y;

			transform.position = map.TileCoordToWorldCoord(tileX, tileY);	// Update our unity world position

			map.myTileArray[currentPath[0].x, currentPath[0].y].GetComponent<TileScript>().ResetColor();
			map.myTileArray[tileX, tileY].GetComponent<TileScript>().ResetColor();
			// Remove the old "current" tile
			//currentPath.RemoveAt(0);

			//if (currentPath.Count == 1)
			//{
			//	// We only have one tile left in the path, and that tile MUST be our ultimate
			//	// destination -- and we are standing on it!
			//	// So let's just clear our pathfinding info.
			//	currentPath = null;
			//}
			if (currNode == currentPath.Count)
			{
				currentPath = null;
			}
		//}
	}

	public int GetGoalTileX()
	{
		return currentPath[currentPath.Count - 1].x;
	}
	public int GetGoalTileY()
	{
		return currentPath[currentPath.Count - 1].y;
	}
}
