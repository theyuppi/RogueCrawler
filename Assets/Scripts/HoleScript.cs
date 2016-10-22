using UnityEngine;

namespace Assets.Scripts
{
    public class HoleScript : MonoBehaviour {

        private int myDmg = 999;
        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                StartCoroutine(other.GetComponent<PlayerScript>().GetHit(myDmg));
            }
        }
    }
}
