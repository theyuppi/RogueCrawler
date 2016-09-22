using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEngine.EventSystems;

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

	void Start()
	{
		MergeList();
		characterList[0].GetComponent<PlayerScript>().myTurn = true;
		UItext = GetComponentsInChildren<Text>();
		UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts;
		UItext[1].text = "XP: " + characterList[currentTarget].GetComponent<PlayerScript>().xp;
		UItext[2].text = "LEVEL: " + characterList[currentTarget].GetComponent<PlayerScript>().xp;
		UItext[3].text = "FLOOR: " + GetComponent<ReadSpriteScript>().currentLevel;
	}

	void Update()
	{
		Vector3 goalPos = target.position;
		goalPos.z = transform.position.z;
		transform.position = Vector3.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
		if (currentTarget > 0)
			UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<EnemyScript>().currActPts;
		else
			UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts;

		if (currentTarget == 0)
		{
			UItext[1].text = "XP: " + characterList[currentTarget].GetComponent<PlayerScript>().xp + "/" + characterList[currentTarget].GetComponent<PlayerScript>().xpLevels[characterList[currentTarget].GetComponent<PlayerScript>().charLVL - 1];
			UItext[2].text = "LEVEL: " + characterList[currentTarget].GetComponent<PlayerScript>().charLVL;
			UItext[3].text = "FLOOR: " + GetComponent<ReadSpriteScript>().currentLevel;
		}


		if (Input.GetKeyUp(KeyCode.Space) && currentTarget == 0 && !characterList[0].GetComponent<PlayerScript>().isMoving)
		{
            inInv = false;
            invCanvas.GetComponent<GraphicRaycaster>().enabled = false;
            invCanvas.GetComponent<Canvas>().targetDisplay = 7;
            lootPanel.localScale = new Vector3(0, 0, 0);
            NextTurn(true);
		}

		if (Input.GetKeyUp(KeyCode.Backspace) && currentTarget == 0)
		{
			NextTurn(false);
		}

		//Toggle inventory
		if (Input.GetKeyUp(KeyCode.I) && currentTarget == 0)
		{
			if (inInv == true)
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

	public void SetTarget(int following)
	{
		target = characterList[following].gameObject.transform;
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
			characterList[currentTarget].GetComponent<EnemyScript>().ReceiveActPts();
			UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<EnemyScript>().currActPts;
		}
		else  //Pass turn to a player
		{
			characterList[currentTarget].GetComponent<PlayerScript>().myTurn = true;
			characterList[currentTarget].GetComponent<PlayerScript>().GetComponent<SpriteRenderer>().sortingOrder = 3;
			characterList[currentTarget].GetComponent<PlayerScript>().ReceiveActPts();
			UItext[0].text = "AP: " + characterList[currentTarget].GetComponent<PlayerScript>().currActPts;
		}
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
