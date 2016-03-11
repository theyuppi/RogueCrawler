using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rBody;
    private Coroutine playerMovement;
    private SpriteRenderer sRender;
    private Animator animaThor;
    public TileScript tScript;
    public GameObject tile;

    public const float stepDuration = 0.2f;
    public const float stepAttackDuration = 0.15f;
    public List<Node> currentPath = null;
    public ReadSpriteScript map;
    public int tileX = 16;
    public int tileY = 0;
    public int tileXmoved = 16;
    public int tileYmoved = 0;
    int moveSpeed = 100;
    public bool isMoving = false;
    public bool isPerformingAttack = false;
    public bool lastNotWalkable = false;
    public int attackPower = 20;
    public int currActPts = 0;
    public int maxActPts = 10;
    public bool myTurn = false;

    private enum direction
    {
        Up,
        Down,
        Left,
        Right
    }

    private direction myDirection = direction.Right;

    //Time stuff
    private float timeBetweenSteps = 1.0f;
    private float currentTime = 0.0f;

    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        sRender = GetComponent<SpriteRenderer>();
        animaThor = GetComponent<Animator>();
        animaThor.SetInteger("State", 0);
    }

    private void Update()
    {
        if (playerMovement == null)
        {
            if (Input.GetKey(KeyCode.W))
                playerMovement = StartCoroutine(Move(Vector2.up));
            else if (Input.GetKey(KeyCode.S))
                playerMovement = StartCoroutine(Move(Vector2.down));
            else if (Input.GetKey(KeyCode.D))
            {
                sRender.flipX = true;
                playerMovement = StartCoroutine(Move(Vector2.right));
            }
            else if (Input.GetKey(KeyCode.A))
            {
                sRender.flipX = false;
                playerMovement = StartCoroutine(Move(Vector2.left));
            }
        }
    }

    private IEnumerator Move(Vector2 direction)
    {
        Vector2 startPosition = transform.position;
        Vector2 destinationPosition = new Vector2(transform.position.x, transform.position.y) + (direction * 0.8f);
        float t = 0.0f;

        while (t < 1.0f)
        {
            transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
            t += Time.deltaTime / stepDuration;
            yield return new WaitForEndOfFrame();
        }

        transform.position = destinationPosition;

        playerMovement = null;
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
        dir.Normalize();
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

        if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().walkable == false)
        {
            if (map.myTileArray[tileX, tileY].GetComponent<TileScript>().hasEnemy == true)
            {
                Vector2 roundDir = new Vector2(Mathf.Round(dir.x), Mathf.Round(dir.y));
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
                    PerformAttack(myDirection);
                }
            }

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

    //public void MoveNextTile(int currNode)
    //{
    //    //float remainingMovement = moveSpeed;

    //    //while (remainingMovement > 0)
    //    //{
    //    if (currentPath == null)
    //        return;

    //    // Get cost from current tile to next tile
    //    //remainingMovement -= map.CostToEnterTile(tileX, tileY, currentPath[1].x, currentPath[1].y);

    //    // Move us to the next tile in the sequence
    //    tileX = currentPath[currNode].x;
    //    tileY = currentPath[currNode].y;

    //    transform.position = map.TileCoordToWorldCoord(tileX, tileY);   // Update our unity world position

    //    map.myTileArray[currentPath[0].x, currentPath[0].y].GetComponent<TileScript>().ResetColor();
    //    map.myTileArray[tileX, tileY].GetComponent<TileScript>().ResetColor();
    //    // Remove the old "current" tile
    //    //currentPath.RemoveAt(0);

    //    //if (currentPath.Count == 1)
    //    //{
    //    //	// We only have one tile left in the path, and that tile MUST be our ultimate
    //    //	// destination -- and we are standing on it!
    //    //	// So let's just clear our pathfinding info.
    //    //	currentPath = null;
    //    //}
    //    if (currNode == currentPath.Count)
    //    {
    //        currentPath = null;
    //    }
    //    //}
    //}

    public int GetGoalTileX()
    {
        if (currentPath != null)
        {
            return currentPath[currentPath.Count - 1].x;
        }
        return 0;
    }
    public int GetGoalTileY()
    {
        return currentPath[currentPath.Count - 1].y;
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
        map.myTileArray[tileX, tileY].GetComponent<TileScript>().CharOnTileGetHit(attackPower);
        map.ClearOldPath();
        StartCoroutine(SetAttackFalse());

    }

    private IEnumerator PerformAttackMove(Vector2 dir)
    {
        Vector2 startPosition = transform.position;
        Vector2 destinationPosition = startPosition + (dir * 0.2f);
        float t = 0.0f;
        while (t < 1.1f)
        {
            transform.position = Vector2.Lerp(startPosition, destinationPosition, t);
            t += Time.deltaTime / stepAttackDuration;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.6f);
        animaThor.SetInteger("State", 0);
        t = 0.0f;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
        {
            other.GetComponent<TileScript>().walkable = false;
            other.GetComponent<TileScript>().occupant = this.gameObject;
            //tile.GetComponent<TileScript>().hasEnemy = true;
            tile = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Floor" || other.tag == "Spike" || other.tag == "Hole")
        {
            other.GetComponent<TileScript>().walkable = true;
            other.GetComponent<TileScript>().occupant = null;
            //tile.GetComponent<TileScript>().hasEnemy = false;
        }
    }
}
