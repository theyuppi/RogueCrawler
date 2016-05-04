using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ChestScript : MonoBehaviour
{
	public CameraScript GameHandler;
	public List<Item> itemList;
	public GameObject inv;
	public InventoryScript invScript;
	public ItemDbScript invDB;
	private int slots = 5; //How many slots in this chest?

	public void Start()
	{
		GameHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
		inv = GameObject.FindGameObjectWithTag("Inventory");
		invDB = inv.GetComponent<ItemDbScript>();
		invScript = inv.GetComponent<InventoryScript>();

		
		//Query itemDatabase for items
		itemList = invDB.FillChest(slots);
	}

	public void Click(BaseEventData bEventData)
	{
		PointerEventData pEventData = (PointerEventData)bEventData;
		if (pEventData.button == PointerEventData.InputButton.Right) //React on right-click only
		{
			//GameHandler.inInv = true;
			GameHandler.ChestClicked();
			invScript.ShowItemsInChest(this.gameObject);
		}

		////Print all items in list
		//for (int i = 0; i < itemList.Count; i++)
		//{
		//	Debug.Log(itemList[i].description);
		//}
	}
}
