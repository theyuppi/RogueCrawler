using UnityEngine;
using System.Collections;

public class SkeletonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Floor")
        {
            other.GetComponent<TileScript>().walkable = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Floor")
        {
            other.GetComponent<TileScript>().walkable = true;
        }
    }
}
