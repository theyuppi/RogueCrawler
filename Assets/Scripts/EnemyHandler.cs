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
            GameObject enemy;
            switch (enemyType)
            {
                case enemies.axeSkeleton:
                    enemy = Instantiate(skeletonPrefab, position, transform.rotation) as GameObject;
                    break;
                case enemies.fireSkeleton:
                    return null;

                default:
                    return null;
            }

            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            enemy.transform.parent = transform;
            enemyScript.tileX = tileX;
            enemyScript.tileY = tileY;
            enemyScript.eHandler = this;
            enemyScript.cScript = cScript.GetComponent<CameraScript>();
            enemyScript.map = cScript.GetComponent<ReadSpriteScript>();
            enemyList.Add(enemy);
            enemyScript.myIdString = currentLvl + currentRoom + tileX + tileY;
            enemyScript.LevelBoostEnemy(currentLvl);

            return enemy;
        }

        public void RemoveFromList()
        {
            enemyList.RemoveAll(go => go.GetComponent<EnemyScript>().isDead);
            enemyList.RemoveAll(go => go == null);
            cScript.GetComponent<CameraScript>().MergeList();
        }

        public void PassTurn()
        {
            cScript.GetComponent<CameraScript>().NextTurn(true);
        }

        public void SendXPtoPlayer(int gainedExp)
        {
            cScript.GetComponent<CameraScript>().pHandler.GetComponent<PlayerHandler>().DistributeXP(gainedExp);
        }
    }
}
