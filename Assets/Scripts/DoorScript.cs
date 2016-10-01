using UnityEngine;

namespace Assets.Scripts
{
    public class DoorScript : MonoBehaviour {

        public ReadSpriteScript levelHandler;
        // Use this for initialization
        void Start () {
	    
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            // Dumma saker som vi inte ens behöver längre
            //if (other.tag == "Player")
            //{
            //    levelHandler.MakeRoom(40, 40, "bigmap1_10");
            //    //levelHandler.GetComponent<PlayerHandler>().BumpPlayer(0, 2);
            //    other.GetComponent<PlayerScript>().BumpMe(0, 2);
            //}
        }
    }
}
