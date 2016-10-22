using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerHandler : MonoBehaviour {

        public GameObject player;
        public List<GameObject> playerList = null;

        void Start () {
            // TODO: Get players from code.
            playerList.Add(player);
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
            gainedXP = gainedXP/playerList.Count;
            foreach (var player in playerList)
            {
                player.GetComponent<PlayerScript>().GainXP(gainedXP);
            }
        
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
}
