using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventoryScript : MonoBehaviour
{
	GameObject inventoryPanel;
	GameObject slotPanel;
	ItemDbScript database;
	public GameObject inventorySlot;
	public GameObject inventoryItem;
	public PlayerHandler phandler;

	public List<Item> items = new List<Item>();
	public List<GameObject> slots = new List<GameObject>();
	public List<GameObject> eqSlots = new List<GameObject>();
	public List<GameObject> lootSlots = new List<GameObject>();
	public int slotCount;

	void Start()
	{
		database = GetComponent<ItemDbScript>();
		slotCount = 40;
		inventoryPanel = GameObject.Find("Inventory Panel");
		slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;

		for (int i = 0; i < slotCount; i++)
		{
			items.Add(new Item());
			slots.Add(Instantiate(inventorySlot));
			slots[i].GetComponent<SlotScript>().id = i;
			slots[i].transform.SetParent(slotPanel.transform);
		}
		/*
		AddItem(2); //Kniv
		items[0] = slots[0].transform.GetChild(0).GetComponent<ItemDataScript>().item;  //lägg till items manuellt i itemlist också
		AddItem(7); //Hp pot
		items[1] = slots[1].transform.GetChild(0).GetComponent<ItemDataScript>().item;
		*/
		//Debug.Log("Items is: " + items.Count);
		for (int i = 0; i < eqSlots.Count; i++)
		{
			items.Add(new Item());
			slots.Add(eqSlots[i]);
			slots[slotCount + i].GetComponent<SlotScript>().id = slotCount + i;
		}
		//Debug.Log("Items is: " + items.Count);
	}

	public void AddItem(int id)
	{
		Item itemToAdd = database.FetchItemByID(id);

		for (int i = 0; i < items.Count; i++)
		{
			if (items[i].id == -1)
			{
				items[i] = itemToAdd;
				GameObject itemObj = Instantiate(inventoryItem);
				itemObj.GetComponent<ItemDataScript>().item = itemToAdd;
				itemObj.GetComponent<ItemDataScript>().slot = i;  //Tell the item which slot it belongs to
				itemObj.transform.SetParent(slots[i].transform);
				itemObj.GetComponent<Image>().sprite = itemToAdd.sprite;
				//itemObj.transform.position = Vector2.zero;
				itemObj.transform.localPosition = Vector2.zero;
				itemObj.name = itemToAdd.title;
				slots[i].name = "Slot: " + itemToAdd.title;

				break;
			}
		}
	}

	public void AddItemFromSave(int id, int slot)
	{
		Item itemToAdd = database.FetchItemByID(id);

		items[slot] = itemToAdd;
		GameObject itemObj = Instantiate(inventoryItem);
		itemObj.GetComponent<ItemDataScript>().item = itemToAdd;
		itemObj.GetComponent<ItemDataScript>().slot = slot;  //Tell the item which slot it belongs to
		itemObj.transform.SetParent(slots[slot].transform);
		itemObj.GetComponent<Image>().sprite = itemToAdd.sprite;
		itemObj.transform.localPosition = Vector2.zero;
		itemObj.name = itemToAdd.title;
		slots[slot].name = "Slot: " + itemToAdd.title;
	}

	public void clearLootSlots()
	{
		for (int i = 0; i < lootSlots.Count; i++)
		{
			lootSlots[i] = null;
		}
	}

	public void ShowItemsInChest(GameObject chest)
	{
		chest.GetComponent<ChestScript>().PopulateChest(lootSlots);

		//for (int i = 0; i < chest.GetComponent<ChestScript>().itemList.Count; i++)
		//{
		//	Debug.Log(chest.GetComponent<ChestScript>().itemList[i].description);
		//	lootSlots[i].name = chest.GetComponent<ChestScript>().itemList[i].ToString();
		//}
	}

	internal void AddStartItems()
	{
		AddItem(2); //Kniv
		items[0] = slots[0].transform.GetChild(0).GetComponent<ItemDataScript>().item;  //lägg till items manuellt i itemlist också
		AddItem(7); //Hp pot
		items[1] = slots[1].transform.GetChild(0).GetComponent<ItemDataScript>().item;
	}
}