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
    public Text apText;

    void Start()
    {
        MergeList();
        characterList[0].GetComponent<PlayerScript>().myTurn = true;
        apText = GetComponentInChildren<Text>();
        apText.text = characterList[currentTarget].GetComponent<PlayerScript>().currActPts.ToString();
    }

    void Update()
    {
        Vector3 goalPos = target.position;
        goalPos.z = transform.position.z;
        transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        if (currentTarget > 0)
            apText.text = characterList[currentTarget].GetComponent<EnemyScript>().currActPts.ToString();
        else
            apText.text = characterList[currentTarget].GetComponent<PlayerScript>().currActPts.ToString();

        if (Input.GetKeyUp(KeyCode.Space) && currentTarget == 0)
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
        if (currentTarget > 0)
        {
            characterList[currentTarget].GetComponent<EnemyScript>().myTurn = false;
            //eHandler.levelHandler.GetComponent<TileScript>().OccupyTile();
            //eHandler.GetComponent<EnemyHandler>().levelHandler.GetComponent<ReadSpriteScript>().
            //    ClearCertainPath(characterList[currentTarget].GetComponent<EnemyScript>().currentPath);
        }
        else
        {
            characterList[currentTarget].GetComponent<PlayerScript>().myTurn = false;
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

        if (currentTarget > 0)
        {
            characterList[currentTarget].GetComponent<EnemyScript>().myTurn = true;
            //eHandler.levelHandler.GetComponent<TileScript>().occupant = null;
            characterList[currentTarget].GetComponent<EnemyScript>().ReceiveActPts();
            apText.text = characterList[currentTarget].GetComponent<EnemyScript>().currActPts.ToString();
        }
        else
        {
            characterList[currentTarget].GetComponent<PlayerScript>().myTurn = true;
            characterList[currentTarget].GetComponent<PlayerScript>().ReceiveActPts();
            apText.text = characterList[currentTarget].GetComponent<PlayerScript>().currActPts.ToString();
        }
    }
}
