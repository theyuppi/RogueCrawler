using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.0f;
    public int currentTarget = 0;
    private Vector3 velocity = Vector3.zero;
    public EnemyHandler eHandler;
    public PlayerHandler pHandler;
    public List<GameObject> characterList = null;

    void Start()
    {
        MergeList();
        characterList[0].GetComponent<PlayerScript>().myTurn = true;
    }

    void Update()
    {
        Vector3 goalPos = target.position;
        goalPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (currentTarget != 0)
            {
                characterList[currentTarget].GetComponent<EnemyScript>().myTurn = false;
                //eHandler.levelHandler.GetComponent<TileScript>().OccupyTile();
            }
            else
            {
                characterList[currentTarget].GetComponent<PlayerScript>().myTurn = false;
            }


            currentTarget++;
            if (currentTarget >= characterList.Count)
            {
                currentTarget = 0;
            }
            SetTarget(currentTarget);

            
            if (currentTarget != 0)
            {
                characterList[currentTarget].GetComponent<EnemyScript>().myTurn = true;
                //eHandler.levelHandler.GetComponent<TileScript>().occupant = null;
            }
            else
            {
                characterList[currentTarget].GetComponent<PlayerScript>().myTurn = true;
            }

            Debug.Log("whose turn: " + currentTarget);
        }
    }

    public void SetTarget(int following)
    {
        //if (characterList[following].gameObject.transform != null)
        //{
            target = characterList[following].gameObject.transform;
        //}
        //else
        //{
        //    following++;
        //    SetTarget(following);
        //    Debug.Log("Target: " + following);
        //}
    }

    public void MergeList()
    {
        characterList.Clear();
        for (int i = 0; i < pHandler.playerList.Count; i++)
        {
            characterList.Add(pHandler.playerList[i]);
        }
        for (int i = 0; i < eHandler.enemyList.Count; i++)
        {
            characterList.Add(eHandler.enemyList[i]);
        }
    }

    public void RemoveFromList()
    {
        characterList.RemoveAll(gameObject => gameObject.GetComponent<EnemyScript>().isDead == true);
        characterList.RemoveAll(gameObject => gameObject == null);
    }
}
