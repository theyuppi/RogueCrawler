using UnityEngine;
using System.Collections;

public class playerMovementScript : MonoBehaviour {

	private Rigidbody2D rBody;
	public const float stepDuration = 0.5f;
	private Coroutine playerMovement;

	void Start () {
		rBody = GetComponent<Rigidbody2D>();
			
	}

	private void FixedUpdate()
	{
		if (playerMovement == null)
		{
			if (Input.GetKey(KeyCode.W))
				playerMovement = StartCoroutine(Move(Vector2.up));
			else if (Input.GetKey(KeyCode.S))
				playerMovement = StartCoroutine(Move(Vector2.down));
			else if (Input.GetKey(KeyCode.D))
				playerMovement = StartCoroutine(Move(Vector2.right));
			else if (Input.GetKey(KeyCode.A))
				playerMovement = StartCoroutine(Move(Vector2.left));
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
}
