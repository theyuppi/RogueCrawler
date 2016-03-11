using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {

    private int health = 50;
    public Text healthText;
    public GameObject tile;
    public EnemyHandler eHandler;
    public CameraScript cScript;
    public bool isDead = false;
    public bool isMoving = false;
    public bool myTurn = false;
    public List<Node> currentPath = null;
    public ReadSpriteScript map;
    public const float stepDuration = 0.2f;
    public int tileX = 0;
    public int tileY = 0;
    public int tileXmoved = 0;
    public int tileYmoved = 0;

    // Use this for initialization
    void Start () {
        healthText = GetComponentInChildren<Text>();
        healthText.text = health.ToString();
        //cScript = GetComponent<CameraScript>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	    if (health <= 0)
        {
            Destroy();
        }

        if (myTurn == true)
        {
            //do pathfinding ya dummy
            eHandler.levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo(
                cScript.pHandler.playerList[0].GetComponent<PlayerScript>().tileX, 
                cScript.pHandler.playerList[0].GetComponent<PlayerScript>().tileY,
                this.gameObject, false);
            StartCoroutine(MakeAMove());
            
            myTurn = false;
        }
	}

    public IEnumerator MakeAMove()
    {

        int currNode = 0;
        isMoving = true;
        while (isMoving == true && currentPath != null && currNode < currentPath.Count - 1)
        {
            Vector3 start = map.TileCoordToWorldCoord(currentPath[currNode].x, currentPath[currNode].y) +
                new Vector3(0, 0, -1f);
            Vector3 end = map.TileCoordToWorldCoord(currentPath[currNode + 1].x, currentPath[currNode + 1].y) +
                new Vector3(0, 0, -1f);

            Debug.DrawLine(start, end, Color.red);

            currNode++;

            StartCoroutine(MoveNextTile(currNode));
            if (currNode == currentPath.Count - 1)
            {
                isMoving = false;
            }
            yield return new WaitForSeconds(stepDuration);
        }
        map.ClearOldPath();
        currentPath = null;
    }

    public IEnumerator MoveNextTile(int currNode)
    {
        if (currentPath == null)
            yield return new WaitForEndOfFrame();

        // Move us to the next tile in the sequence

        tileX = currentPath[currNode].x;
        tileY = currentPath[currNode].y;

        Vector2 startPosition = transform.position;

        Vector2 destinationPosition = map.TileCoordToWorldCoord(tileX, tileY);
        Vector2 dir = destinationPosition - startPosition;
        //dir.Normalize();
        //switch ((int)dir.x)
        //{
        //    case 1:
        //        sRender.flipX = true;
        //        break;

        //    case -1:
        //        sRender.flipX = false;
        //        break;

        //    default:
        //        break;
        //}

        if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().walkable == false)
        {
            //if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().hasEnemy == true)
            //{
            //    Vector2 roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
            //    if (roundDir == Vector2.up)
            //    {
            //        myDirection = direction.Up;
            //    }
            //    else if (roundDir == Vector2.down)
            //    {
            //        myDirection = direction.Down;
            //    }
            //    else if (roundDir == Vector2.left)
            //    {
            //        myDirection = direction.Left;
            //    }
            //    else if (roundDir == Vector2.right)
            //    {
            //        myDirection = direction.Right;
            //    }
            //    if (isPerformingAttack == false)
            //    {
            //        PerformAttack(myDirection);
            //    }
            //}

            if (currentPath != null)
            {
                tileX = currentPath[currNode - 1].x;
                tileY = currentPath[currNode - 1].y;
            }

        }

        else
        {
            // Lerp to new position
            float t = 0.0f;
            while (t <= 1.1f)
            {
                transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
                t += Time.deltaTime / stepDuration;
                yield return new WaitForEndOfFrame();
            }

            if (currentPath != null)
            {
                tileXmoved = currentPath[currNode].x;
                tileYmoved = currentPath[currNode].y;
            }

            map.myTileArray[tileXmoved, tileYmoved].GetComponent<TileScript>().ResetColor();
        }

        if (currentPath != null && currNode == currentPath.Count)
        {
            currentPath = null;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
        {
            other.GetComponent<TileScript>().walkable = false;
            other.GetComponent<TileScript>().hasEnemy = true;
            other.GetComponent<TileScript>().occupant = this.gameObject;
            tile = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
        {
            other.GetComponent<TileScript>().walkable = true;
            other.GetComponent<TileScript>().hasEnemy = false;
            other.GetComponent<TileScript>().occupant = null;
        }
    }

    //void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
    //    {
    //        other.GetComponent<TileScript>().occupant = null;
    //    }
    //}

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
        eHandler.RemoveFromList();
        Destroy(this.gameObject);
        
    }

    
}
