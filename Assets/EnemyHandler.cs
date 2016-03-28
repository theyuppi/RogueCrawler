using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHandler : MonoBehaviour {

    // Use this for initialization
    public GameObject skeletonPrefab;
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
    
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time % 2 == 0)
		{
			
		
		for (int i = 0; i < enemyList.Count -1; i++)
		{
			if (enemyList[i].GetComponent<SpriteRenderer>().isVisible || enemyList[i].GetComponent<EnemyScript>().myTurn)
			{
				enemyList[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
			}
			else
			{
				enemyList[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
			}

		}
		}
	}

    public GameObject SpawnEnemy(enemies enemyType, Vector2 position, int tileX, int tileY)
    {
        switch (enemyType)
        {
            case enemies.axeSkeleton:
                GameObject enemy = Instantiate(skeletonPrefab, position, transform.rotation) as GameObject;
                enemy.transform.parent = transform;
                enemy.GetComponent<EnemyScript>().tileX = tileY;
                enemy.GetComponent<EnemyScript>().tileY = tileX;
                enemy.GetComponent<EnemyScript>().eHandler = this;
                enemy.GetComponent<EnemyScript>().cScript = cScript.GetComponent<CameraScript>();
                enemy.GetComponent<EnemyScript>().map = cScript.GetComponent<ReadSpriteScript>();
                enemyList.Add(enemy);
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
        levelHandler.GetComponent<CameraScript>().NextTurn(true);
    }

    public void SendXPtoPlayer(int gainedXP)
    {
        cScript.GetComponent<CameraScript>().pHandler.GetComponent<PlayerHandler>().DistributeXP(gainedXP);
    }
}
