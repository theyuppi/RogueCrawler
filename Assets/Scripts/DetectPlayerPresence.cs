using UnityEngine;
using System.Collections;

public class DetectPlayerPresence : MonoBehaviour {

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
