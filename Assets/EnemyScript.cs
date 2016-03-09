using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyScript : MonoBehaviour {

    private int health = 50;
    public Text healthText;
    public GameObject tile;
    public EnemyHandler eHandler;
    public bool isDead = false;
	// Use this for initialization
	void Start () {
        healthText = GetComponentInChildren<Text>();
        healthText.text = health.ToString();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (health <= 0)
        {
            Destroy();
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Floor")
        {
            other.GetComponent<TileScript>().walkable = false;
            tile.GetComponent<TileScript>().hasEnemy = true;
            tile = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Floor")
        {
            other.GetComponent<TileScript>().walkable = true;
            tile.GetComponent<TileScript>().hasEnemy = false;
        }
    }

    public IEnumerator GetHit(int damageAmount)
    {
        yield return new WaitForSeconds(0.5f);
        health -= damageAmount;
        healthText.text = health.ToString();
    }

    public void Destroy()
    {
        tile.GetComponent<TileScript>().hasEnemy = false;
        tile.GetComponent<TileScript>().walkable = true;
        tile.GetComponent<TileScript>().occupant = null;
        isDead = true;
        RemoveFromList();
        Destroy(this.gameObject);
    }

    public void RemoveFromList()
    {
        eHandler.enemyList.RemoveAll(gameObject => gameObject.GetComponent<EnemyScript>().isDead == true);
        eHandler.enemyList.RemoveAll(gameObject => gameObject == null);
    }
}
