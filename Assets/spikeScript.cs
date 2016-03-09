using UnityEngine;
using System.Collections;

public class SpikeScript : MonoBehaviour {

	Vector3 startPos;
	Vector3 currPos;

	void Start () {
		startPos = transform.position;
	}
	
	void Update () {
		transform.position = startPos + new Vector3(0.0f,0.05f * Mathf.Sin(Time.time), 0.0f);
	}
}
