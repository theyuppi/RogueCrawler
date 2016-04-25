using UnityEngine;
using System.Collections;

public class CanvasEnablerScript : MonoBehaviour {

	Canvas can;
	//Canvas can;
	void Start () {
		can = GetComponentInChildren<Canvas>();
	
	}
	
	void FixedUpdate () {
		if (GetComponent<SpriteRenderer>().isVisible || GetComponent<EnemyScript>().myTurn)
		{
			//GetComponentInChildren<Canvas>().gameObject.SetActive(true);
			//GetComponentInChildren<CanvasRenderer>().gameObject.SetActive(true);
			gameObject.SetActive(true);
			//GetComponent<SpriteRenderer>().enabled = true;
			
		}
		else
		{
		//GetComponentInChildren<Canvas>().gameObject.SetActive(false);
			//GetComponentInChildren<CanvasRenderer>().gameObject.SetActive(false);
			gameObject.SetActive(false);
			//GetComponent<SpriteRenderer>().enabled = false;
		}
	}
}
