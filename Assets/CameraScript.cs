using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public Transform target;
    public static float smoothTime = 0.0f;
    public int currentTarget = 0;
    private Vector3 velocity = Vector3.zero;
    public EnemyHandler eHandler;
    public PlayerHandler pHandler;
    public List<GameObject> characterList = null;
    public Text[] UItext;
    int xp;

    void Start()
    {
        MergeList();
        characterList[0].GetComponent<PlayerScript>().myTurn = true;
        UItext = GetComponentsInChildren<Text>();
        UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts.ToString();
        UItext[1].text = "XP: " + characterList[currentTarget].GetComponent<PlayerScript>().xp.ToString();
		UItext[2].text = "LVL: " + characterList[currentTarget].GetComponent<PlayerScript>().xp.ToString();
    }

    void Update()
    {
        Vector3 goalPos = target.position;
        goalPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        if (currentTarget > 0)
            UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<EnemyScript>().currActPts.ToString();
        else
            UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts.ToString();

		if (currentTarget == 0)
		{
			UItext[1].text = "XP: " + characterList[currentTarget].GetComponent<PlayerScript>().xp.ToString() + " / " + characterList[currentTarget].GetComponent<PlayerScript>().xpLevels[characterList[currentTarget].GetComponent<PlayerScript>().charLVL - 1].ToString();
			UItext[2].text = "LVL: " + characterList[currentTarget].GetComponent<PlayerScript>().charLVL.ToString();
		}

        

        if (Input.GetKeyUp(KeyCode.Space) && currentTarget == 0 && !characterList[0].GetComponent<PlayerScript>().isMoving)
        {
            NextTurn(true);

            //Debug.Log("whose turn: " + currentTarget);
        }

        if (Input.GetKeyUp(KeyCode.Backspace) && currentTarget == 0)
        {
            NextTurn(false);
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

    public IEnumerator ChangeCameraSmoothness()
    {
        smoothTime = 0.2f;
        yield return new WaitForSeconds(0.8f);
        smoothTime = 0;
    }

    public void NextTurn(bool forwardTurnOrder)
    {
        StartCoroutine(ChangeCameraSmoothness());
        if (currentTarget > 0)  //It's an enemys turn
        {
            characterList[currentTarget].GetComponent<EnemyScript>().myTurn = false;
            characterList[currentTarget].GetComponent<EnemyScript>().GetComponent<SpriteRenderer>().sortingOrder = 2;
            //eHandler.levelHandler.GetComponent<TileScript>().OccupyTile();
            //eHandler.GetComponent<EnemyHandler>().levelHandler.GetComponent<ReadSpriteScript>().
            //    ClearCertainPath(characterList[currentTarget].GetComponent<EnemyScript>().currentPath);
        }
        else  //It's players turn
        {
            characterList[currentTarget].GetComponent<PlayerScript>().myTurn = false;
			characterList[currentTarget].GetComponent<PlayerScript>().GetComponent<SpriteRenderer>().sortingOrder = 2;
        }

        if (forwardTurnOrder)
        {
            currentTarget++;
            if (currentTarget >= characterList.Count)
            {
                currentTarget = 0;
            }
            SetTarget(currentTarget);
        }
        else
        {
            currentTarget--;
            if (currentTarget < 0)
            {
                currentTarget = characterList.Count - 1;
            }
            SetTarget(currentTarget);
        }

        if (currentTarget > 0) //Pass turn to an enemy
        {
			characterList[currentTarget].GetComponent<EnemyScript>().gameObject.SetActive(true);
            characterList[currentTarget].GetComponent<EnemyScript>().myTurn = true;
            characterList[currentTarget].GetComponent<EnemyScript>().GetComponent<SpriteRenderer>().sortingOrder = 3;
            //eHandler.levelHandler.GetComponent<TileScript>().occupant = null;
            characterList[currentTarget].GetComponent<EnemyScript>().ReceiveActPts();
            UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<EnemyScript>().currActPts.ToString();
        }
        else  //Pass turn to a player
        {
            characterList[currentTarget].GetComponent<PlayerScript>().myTurn = true;
			characterList[currentTarget].GetComponent<PlayerScript>().GetComponent<SpriteRenderer>().sortingOrder = 3;
            characterList[currentTarget].GetComponent<PlayerScript>().ReceiveActPts();
            UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts.ToString();
        }
    }
}
