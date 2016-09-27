using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChestScript : MonoBehaviour
{
	public CameraScript GameHandler { get; set; }
	public ReadSpriteScript RSS { get; set; }
    public List<Item> ItemList { get; set; }
    public GameObject Inventory { get; set; }
	public InventoryScript InventoryScript { get; set; }
    public ItemDbScript InventoryDatabase { get; set; }
    public GameObject player { get; set; }

	public bool playerInRange = false;
	public string myIdString { get; set; }
	public GameObject item;

    const int Slots = 2; //How many Slots in this chest?

	public void Start()
	{
		GameHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
		RSS = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ReadSpriteScript>();
		Inventory = GameObject.FindGameObjectWithTag("Inventory");
		InventoryDatabase = Inventory.GetComponent<ItemDbScript>();
		InventoryScript = Inventory.GetComponent<InventoryScript>();
        player = GameObject.FindGameObjectWithTag("Player");
	    myIdString = "";
        //Query itemDatabase for items
        ItemList = InventoryDatabase.FillChest(Slots);
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
                    InventoryScript.ShowItemsInChest(gameObject);
                    RSS.lootedChests.Add(myIdString);
                }
            }
        }
        else
        {
            GetComponentInParent<TileScript>().GotClicked();
        }
    }

	public void PopulateChest(List<GameObject> slots)
	{
		for (int i = 0; i < ItemList.Count; i++)
		{
			if (slots[i].transform.childCount == 0) //Don't add item if slot already has item
			{
				GameObject itm = Instantiate(item);
				itm.GetComponent<ItemDataScript>().item = ItemList[i];
				itm.GetComponent<ItemDataScript>().slot = i + 100;
				itm.transform.SetParent(slots[i].transform);
				itm.GetComponent<Image>().sprite = ItemList[i].sprite;
				itm.transform.position = Vector2.zero;
				itm.name = ItemList[i].title;
				itm.GetComponent<RectTransform>().localPosition = Vector2.zero;
				itm.GetComponent<ItemDataScript>().item.belongsToChest = gameObject;
				itm.GetComponent<ItemDataScript>().item.myInti = i;
            }
			
		}
		GameObject go = GameObject.Find("Slot Panel Chest");
		for (int i = 0; i < go.transform.childCount; i++)
		{
			go.transform.GetChild(i).GetComponent<SlotScript>().owner = gameObject;
			//Slots[i].GetComponent<SlotScript>().owner = this.gameObject;
		}
	}
}
