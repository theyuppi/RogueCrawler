using UnityEngine;
using System.Collections;

public class DetectPlayerPresence : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			GetComponentInParent<ChestScript>().playerInRange = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			GetComponentInParent<ChestScript>().playerInRange = false;
		}
	}
}
