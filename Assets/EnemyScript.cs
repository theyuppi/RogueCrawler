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
    public bool isBlocked = false;
    public bool turnIsOver = false;
    public List<Node> currentPath = null;
    public ReadSpriteScript map;
    public const float stepDuration = 0.2f;
    public const float stepAttackDuration = 0.15f;
    public int tileX = 0;
    public int tileY = 0;
    public int tileXmoved = 0;
    public int tileYmoved = 0;
    private SpriteRenderer sRender;
    private Animator animaThor;
    public bool isPerformingAttack = false;
    private int attackPower = 10;
    public int currActPts = 0;
    public int maxActPts = 10;
    Vector2 roundDir;

    private enum direction
    {
        Up,
        Down,
        Left,
        Right
    }
    private direction myDirection = direction.Right;

    // Use this for initialization
    void Start () {
        healthText = GetComponentInChildren<Text>();
        healthText.text = health.ToString();
        sRender = GetComponent<SpriteRenderer>();
        animaThor = GetComponent<Animator>();
        animaThor.SetInteger("State", 0);
        ReceiveActPts();
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
            // de va här de sprack osv
            eHandler.levelHandler.GetComponent<ReadSpriteScript>().GeneratePathTo(
                    cScript.pHandler.playerList[0].GetComponent<PlayerScript>().tileX,
                    cScript.pHandler.playerList[0].GetComponent<PlayerScript>().tileY,
                    this.gameObject, false);
            //if (currActPts >= 2 && isMoving == false)
            //{
                
                StartCoroutine(MakeAMove());

            //}
            
            myTurn = false;
        }
        if (turnIsOver == true)
        {
            eHandler.GetComponent<EnemyHandler>().PassTurn();
        }
        turnIsOver = false;
    }

    public IEnumerator MakeAMove()
    {

        int currNode = 0;
        yield return new WaitForSeconds(0.8f);
        while (currentPath != null && currNode < currentPath.Count - 1)
        {
            if (currActPts > 0)
            {
                currNode++;
                
                StartCoroutine(MoveNextTile(currNode));

                if (currNode == currentPath.Count - 1)
                {
                    isMoving = false;
                }
                yield return new WaitForSeconds(stepDuration);
            }
            else
            {
                isMoving = false;
                break;
            }
        }

        while (currActPts > 1 && isBlocked == false)
        {
            if (isPerformingAttack == false)
            {
                PerformAttack(myDirection);
            }
            yield return new WaitForSeconds(1.2f);
        }

        //map.ClearOldPath();
        yield return new WaitForSeconds(0.5f);
        turnIsOver = true;
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
        dir.Normalize();
        roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
        switch ((int)dir.x)
        {
            case 1:
                sRender.flipX = true;
                break;

            case -1:
                sRender.flipX = false;
                break;

            default:
                break;
        }

        // Check if tile in path is walkable
        if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().walkable == false)
        {
            // Make current stepTile the last stepTile
            if (currentPath != null)
            {
                tileX = currentPath[currNode - 1].x;
                tileY = currentPath[currNode - 1].y;
            }

            // Check if tile has a player, if so check direction and perform attack
            if (map.myTileArray[tileX + (int)roundDir.y, tileY + (int)roundDir.x].GetComponent<TileScript>().hasPlayer == true)
            {
                if (roundDir == Vector2.up)
                {
                    myDirection = direction.Up;
                }
                else if (roundDir == Vector2.down)
                {
                    myDirection = direction.Down;
                }
                else if (roundDir == Vector2.left)
                {
                    myDirection = direction.Left;
                }
                else if (roundDir == Vector2.right)
                {
                    myDirection = direction.Right;
                }
                if (isPerformingAttack == false)
                {
                    if (currActPts >= 2)
                    {
                        PerformAttack(myDirection);
                    }
                }
            }
            else if (map.myTileArray[tileX + (int)roundDir.y, tileY + (int)roundDir.x].GetComponent<TileScript>().hasEnemy == true)
            {
                isBlocked = true;
            }

            

        }

        else
        {
            // Lerp to new position
            currActPts--;
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
        if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole" || other.tag == "Chest")
        {
            other.GetComponent<TileScript>().walkable = false;
            other.GetComponent<TileScript>().hasEnemy = true;
            if (other.GetComponent<TileScript>().occupant == null)
            {
                other.GetComponent<TileScript>().occupant = this.gameObject;
            }
            tile = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole" || other.tag == "Chest")
        {
            other.GetComponent<TileScript>().walkable = true;
            other.GetComponent<TileScript>().hasEnemy = false;
            other.GetComponent<TileScript>().occupant = null;
            isBlocked = false;
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

    private void PerformAttack(direction dir)
    {
        isPerformingAttack = true;
        switch (dir)
        {
            case direction.Up:
                StartCoroutine(PerformAttackMove(Vector2.up));
                animaThor.SetInteger("State", 1);
                break;
            case direction.Down:
                StartCoroutine(PerformAttackMove(Vector2.down));
                animaThor.SetInteger("State", 2);
                break;
            case direction.Left:
                StartCoroutine(PerformAttackMove(Vector2.left));
                animaThor.SetInteger("State", 1);
                break;
            case direction.Right:
                StartCoroutine(PerformAttackMove(Vector2.right));
                animaThor.SetInteger("State", 1);

                break;
            default:
                break;
        }
        currActPts -= 2;
        map.myTileArray[tileX + (int)roundDir.y, tileY + (int)roundDir.x].GetComponent<TileScript>().CharOnTileGetHit(attackPower, true);
        map.ClearOldPath();
        StartCoroutine(SetAttackFalse());

        //if (currActPts >= 2)
        //{
        //    StartCoroutine(AttackAgain(dir));
        //}
    }

    private IEnumerator PerformAttackMove(Vector2 dir)
    {
        Vector2 startPosition = transform.position;
        Vector2 destinationPosition = startPosition + (dir * 0.2f);
        float t = 0.0f;
        // Moves toward target
        while (t < 1.1f)
        {
            transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
            t += Time.deltaTime / stepAttackDuration;
            yield return new WaitForEndOfFrame();
        }
        // Stay for 0.6 seconds
        yield return new WaitForSeconds(0.6f);

        // Back to idle anim
        animaThor.SetInteger("State", 0);
        t = 0.0f;

        // Moves backwards to start tile
        while (t < 1.1f)
        {
            transform.position = Vector2.Lerp(destinationPosition, startPosition, t);
            t += Time.deltaTime / stepAttackDuration;
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator SetAttackFalse()
    {
        yield return new WaitForSeconds(1.0f);
        isPerformingAttack = false;
    }

    //private IEnumerator AttackAgain(direction dir)
    //{
    //    yield return new WaitForSeconds(1.0f);
    //    PerformAttack(dir);
    //}

    public void ReceiveActPts()
    {
        currActPts = maxActPts;
    }
}
