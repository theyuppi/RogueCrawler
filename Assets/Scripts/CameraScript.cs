using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [System.Serializable]
    public class CameraScript : MonoBehaviour//, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler
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

        public Canvas invCanvas;
        public bool inInv = false;
        public bool inChest = false;
        public RectTransform lootPanel;

        public bool CombatMode { get; set; }
        public bool PlayerTurn { get; set; }

        private GameObject _currentUnit;

        void Start()
        {
            MergeList();
            _currentUnit = characterList[0];
            _currentUnit.GetComponent<PlayerScript>().IsMyTurn(true);
            UItext = GetComponentsInChildren<Text>();
            UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts;
            UItext[1].text = "XP: " + characterList[currentTarget].GetComponent<PlayerScript>().xp;
            UItext[2].text = "LEVEL: " + characterList[currentTarget].GetComponent<PlayerScript>().xp;
            UItext[3].text = "FLOOR: " + GetComponent<ReadSpriteScript>().currentLevel;
            PlayerTurn = true;
        }

        void Update()
        {
            if (!target)
            {
                NextTurn(true);
            }

            // if enemies live
            CombatMode = characterList.Count > pHandler.playerList.Count;

            Vector3 goalPos = target.position;
            goalPos.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);

            if (PlayerTurn)
            {
                UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts;
                UItext[1].text = "XP: " + characterList[currentTarget].GetComponent<PlayerScript>().xp + "/" +
                                 characterList[currentTarget].GetComponent<PlayerScript>().xpLevels[
                                     characterList[currentTarget].GetComponent<PlayerScript>().charLVL - 1];
                UItext[2].text = "LEVEL: " + characterList[currentTarget].GetComponent<PlayerScript>().charLVL;
                UItext[3].text = "FLOOR: " + GetComponent<ReadSpriteScript>().currentLevel;
            }
            else
            {
                UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<EnemyScript>().currActPts;
            }


            if (Input.GetKeyUp(KeyCode.Space) && PlayerTurn && !characterList[currentTarget].GetComponent<PlayerScript>().isMoving)
            {
                inInv = false;
                invCanvas.GetComponent<GraphicRaycaster>().enabled = false;
                invCanvas.GetComponent<Canvas>().targetDisplay = 7;
                lootPanel.localScale = new Vector3(0, 0, 0);
                NextTurn(true);
            }

            if (Input.GetKeyUp(KeyCode.Backspace) && PlayerTurn)
            {
                NextTurn(false);
            }

            //Toggle inventory
            if (Input.GetKeyUp(KeyCode.I) && PlayerTurn)
            {
                if (inInv)
                {
                    inInv = false;
                    invCanvas.GetComponent<GraphicRaycaster>().enabled = false;
                    invCanvas.GetComponent<Canvas>().targetDisplay = 7;
                    lootPanel.localScale = new Vector3(0, 0, 0);
                }
                else
                {
                    inInv = true;
                    invCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                    invCanvas.GetComponent<Canvas>().targetDisplay = 0;
                    lootPanel.localScale = new Vector3(0, 0, 0);
                }
            }

            //Escape to exit
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Application.Quit();
            }
        }

        public void SetTarget(int newTarget)
        {
            target = characterList[newTarget].gameObject.transform;
            PlayerTurn = (characterList[currentTarget].tag == "Player");
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

            // sorts with every character's (enemy and player) initiative
            // TODO: Fix so we dont just ++ an array to choose whose turn, or at least always start on 0 if combat mode
            var character = from entry in characterList
                orderby entry.GetComponent<ICharacter>().GetInitiative() descending 
                select entry;

            characterList = character.ToList();
        }

        public void RemoveFromList()
        {
            characterList.RemoveAll(gameObject => gameObject.GetComponent<EnemyScript>().isDead);
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

            characterList[currentTarget].GetComponent<ICharacter>().IsMyTurn(false);
            characterList[currentTarget].GetComponent<SpriteRenderer>().sortingOrder = 2;

            if (forwardTurnOrder)
            {
                currentTarget++;
                if (currentTarget >= characterList.Count)
                {
                    currentTarget = 0;
                }
            }
            else
            {
                currentTarget--;
                if (currentTarget < 0)
                {
                    currentTarget = characterList.Count - 1;
                }
            }
            SetTarget(currentTarget);

            characterList[currentTarget].GetComponent<ICharacter>().IsMyTurn(true);
            characterList[currentTarget].GetComponent<SpriteRenderer>().sortingOrder = 3;
            characterList[currentTarget].GetComponent<ICharacter>().ReceiveActPts();
            UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<ICharacter>().GetCurrentActionPoints();

            /* Doesn't seem to be needed
            if (!PlayerTurn)
                characterList[currentTarget].GetComponent<EnemyScript>().gameObject.SetActive(true);
             */
        }

        //public void OnPointerDown(PointerEventData eventData)
        //{
        //	Debug.Log(eventData.pointerEnter.name);
        //	//Debug.Log("Now over tile: " + eventData.pointerCurrentRaycast.gameObject.GetComponent<TileScript>().myID);
        //	Debug.Log("OnPointerDown");
        //}

        //public void OnPointerClick(PointerEventData eventData)
        //{
        //	Debug.Log(eventData.pointerPressRaycast);

        //	Debug.Log("OnPointerClick");
        //	//eventData.pointerCurrentRaycast.gameObject.GetComponent<TileScript>().GotClicked();
        //	if (eventData.pointerCurrentRaycast.gameObject.tag == "Floor")
        //	{
        //		Debug.Log("OnPointerClick");
        //	}
        //}

        //public void OnPointerUp(PointerEventData eventData)
        //{
        //	Debug.Log("OnPointerUp");
        //}

        //public void OnPointerEnter(PointerEventData eventData)
        //{
        //	Debug.Log(eventData.pointerEnter.name);
        //}

        public void ChestClicked()
        {
            inInv = true;
            inChest = true;
            invCanvas.GetComponent<GraphicRaycaster>().enabled = true;
            invCanvas.GetComponent<Canvas>().targetDisplay = 0;
            lootPanel.localScale = new Vector3(1, 1, 1);
        }

        public void ChestClosed()
        {
            inInv = false;
            inChest = false;
            invCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            invCanvas.GetComponent<Canvas>().targetDisplay = 7;
            lootPanel.localScale = new Vector3(0, 0, 0);
        }

        public void InvClosed()
        {
            inInv = false;
            inChest = false;
            invCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            invCanvas.GetComponent<Canvas>().targetDisplay = 7;
            lootPanel.localScale = new Vector3(0, 0, 0);
        }
    }
}
