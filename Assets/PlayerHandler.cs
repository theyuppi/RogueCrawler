using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerHandler : MonoBehaviour {

    // Use this for initialization
    public GameObject player;
    public List<GameObject> playerList = null;

    void Start () {
        playerList.Add(player);
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    //public GameObject SpawnPlayer(Vector2 position)
    //{
    //    GameObject player = Instantiate(Player, position, transform.rotation) as GameObject;
    //    enemy.transform.parent = transform;
    //    enemyList.Add(enemy);
    //    enemy.GetComponent<EnemyScript>().eHandler = this;
    //    return enemy;
    //}

    public void DistributeXP(int gainedXP)
    {
        player.GetComponent<PlayerScript>().GainXP(gainedXP);
    }

}
