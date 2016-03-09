using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHandler : MonoBehaviour {

    // Use this for initialization
    public GameObject skeletonPrefab;
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
	    
	}

    public GameObject SpawnEnemy(enemies enemyType, Vector2 position)
    {
        switch (enemyType)
        {
            case enemies.axeSkeleton:
                GameObject enemy = Instantiate(skeletonPrefab, position, transform.rotation) as GameObject;
                enemy.transform.parent = transform;
                enemyList.Add(enemy);
                enemy.GetComponent<EnemyScript>().eHandler = this;
                return enemy;

            case enemies.fireSkeleton:
                return null;

            default:
                return null;
                
        }
    }
}
