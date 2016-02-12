using UnityEngine;
using System.Collections;

public class playerMovementScript : MonoBehaviour {

	private Rigidbody2D rBody;
	private Coroutine playerMovement;
	private SpriteRenderer sRender;
	private Animator animaThor;

	public const float stepDuration = 0.5f;
	

	void Start () {
		rBody = GetComponent<Rigidbody2D>();
		sRender = GetComponent<SpriteRenderer>();
		animaThor = GetComponent<Animator>();
	}

	private void FixedUpdate()
	{
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
