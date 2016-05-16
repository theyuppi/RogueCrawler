using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChestScript : MonoBehaviour
{
	public CameraScript GameHandler;
	public ReadSpriteScript RSS;
	public List<Item> itemList;
	public GameObject inv;
	public InventoryScript invScript;
	public ItemDbScript invDB;
	private int slots = 2; //How many slots in this chest?
	public bool playerInRange = false;
	public string myIdString = "";

    public GameObject player;

	public GameObject item;

	public void Start()
	{
		GameHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
		RSS = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadSpriteScript>();
		inv = GameObject.FindGameObjectWithTag("Inventory");
		invDB = inv.GetComponent<ItemDbScript>();
		invScript = inv.GetComponent<InventoryScript>();
        player = GameObject.FindGameObjectWithTag("Player");

        //Query itemDatabase for items
        itemList = invDB.FillChest(slots);
		//PopulateChest();
	}

	public void Click(BaseEventData bEventData)
	{
        PointerEventData pEventData = (PointerEventData)bEventData;
        if (pEventData.button == PointerEventData.InputButton.Right) //React on right-click only
        {
            float xDiff = Mathf.Abs(player.transform.position.x - transform.position.x);
            float yDiff = Mathf.Abs(player.transform.position.y - transform.position.y);
            //Debug.Log("xDiff: " + xDiff + " yDiff: " + yDiff);
            if (xDiff < 150 && yDiff < 150)
            {
                if (GameHandler.inChest == false)
                {
                    GameHandler.ChestClicked();
                    invScript.ShowItemsInChest(this.gameObject);
                    RSS.lootedChests.Add(myIdString);
                }
            }
        }
        else
        {
            this.GetComponentInParent<TileScript>().GotClicked();
        }
    }

	public void PopulateChest(List<GameObject> slots)
	{
		for (int i = 0; i < itemList.Count; i++)
		{
			if (slots[i].transform.childCount == 0) //Don't add item if slot already has item
			{
				GameObject itm = Instantiate(item);
				itm.GetComponent<ItemDataScript>().item = itemList[i];
				itm.GetComponent<ItemDataScript>().slot = i + 100;
				itm.transform.SetParent(slots[i].transform);
				itm.GetComponent<Image>().sprite = itemList[i].sprite;
				itm.transform.position = Vector2.zero;
				itm.name = itemList[i].title;
				itm.GetComponent<RectTransform>().localPosition = Vector2.zero;
				itm.GetComponent<ItemDataScript>().item.belongsToChest = this.gameObject;
				itm.GetComponent<ItemDataScript>().item.myInti = i;
            }
			
		}
		GameObject go = GameObject.Find("Slot Panel Chest");
		for (int i = 0; i < go.transform.childCount; i++)
		{
			go.transform.GetChild(i).GetComponent<SlotScript>().owner = this.gameObject;
			//slots[i].GetComponent<SlotScript>().owner = this.gameObject;
		}
	}
}
