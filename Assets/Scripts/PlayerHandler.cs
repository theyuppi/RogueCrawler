﻿using UnityEngine;
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
	void FixedUpdate () {

	    
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

    public void ResetPlayer(bool PD)
    {
        if (PD)
        {
            player.GetComponent<PlayerScript>().PermaDeathSpawn();
        }
        else
        {
            player.GetComponent<PlayerScript>().SavePlayer();
        }
        
    }

    // Också dumt (ta bort sen)
    //public void BumpPlayer(int x, int y)
    //{
    //    player.GetComponent<PlayerScript>().BumpMe(x, y);
    //}

}
