using UnityEngine;
using System.Collections;

public class EnemyHandler : MonoBehaviour {

    // Use this for initialization
    public GameObject skeletonPrefab;
    public enum enemies
    {
        axeSkeleton,
        fireSkeleton
    }
    //enemies enemyType;
    
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SpawnEnemy(enemies enemyType, Vector2 position)
    {
        switch (enemyType)
        {
            case enemies.axeSkeleton:
                GameObject enemy = Instantiate(skeletonPrefab, position, transform.rotation) as GameObject;
                enemy.transform.parent = transform;
                break;
            case enemies.fireSkeleton:
                break;
            default:
                break;
        }

    }
}
