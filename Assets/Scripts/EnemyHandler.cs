using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class EnemyHandler : MonoBehaviour {

        // Use this for initialization
        public GameObject skeletonPrefab;

        // TODO: Don't set these through unity. Just find them once. Makes code easier
        public GameObject cScript;
        public GameObject levelHandler; 
        public List<GameObject> enemyList = null;
        public int enemyID;
        public enum enemies
        {
            axeSkeleton,
            fireSkeleton
        }
        //enemies enemyType;

        public List<string> killedEnemies = new List<string>();


        void Start () {
	
        }
	
        // Update is called once per frame
        void FixedUpdate () {
            //if (Time.time % 2 == 0)
            //{


            //    for (int i = 0; i < enemyList.Count - 1; i++)
            //    {
            //        if (enemyList[i].GetComponent<SpriteRenderer>().isVisible || enemyList[i].GetComponent<EnemyScript>().myTurn)
            //        {
            //            enemyList[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            //        }
            //        else
            //        {
            //            enemyList[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            //        }

            //    }
            //}
        }

        public GameObject SpawnEnemy(enemies enemyType, Vector2 position, int tileX, int tileY, int currentLvl, string currentRoom)
        {
            switch (enemyType)
            {
                case enemies.axeSkeleton:
                    GameObject enemy = Instantiate(skeletonPrefab, position, transform.rotation) as GameObject;
                    enemy.transform.parent = transform;
                    enemy.GetComponent<EnemyScript>().tileX = tileX;
                    enemy.GetComponent<EnemyScript>().tileY = tileY;
                    enemy.GetComponent<EnemyScript>().eHandler = this;
                    enemy.GetComponent<EnemyScript>().cScript = cScript.GetComponent<CameraScript>();
                    enemy.GetComponent<EnemyScript>().map = cScript.GetComponent<ReadSpriteScript>();
                    enemyList.Add(enemy);
                    enemy.GetComponent<EnemyScript>().myIdString = currentLvl + currentRoom + tileX + tileY;
                    enemy.GetComponent<EnemyScript>().LevelBoostEnemy(currentLvl);
                    return enemy;

                case enemies.fireSkeleton:
                    return null;

                default:
                    return null;
                
            }
        }

        public void RemoveFromList()
        {
            enemyList.RemoveAll(gameObject => gameObject.GetComponent<EnemyScript>().isDead == true);
            enemyList.RemoveAll(gameObject => gameObject == null);
            cScript.GetComponent<CameraScript>().MergeList();
        }

        public void PassTurn()
        {
            cScript.GetComponent<CameraScript>().NextTurn(true);
        }

        public void SendXPtoPlayer(int gainedXP)
        {
            cScript.GetComponent<CameraScript>().pHandler.GetComponent<PlayerHandler>().DistributeXP(gainedXP);
        }
    }
}
