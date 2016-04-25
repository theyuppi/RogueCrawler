using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryScript : MonoBehaviour
{
	GameObject inventoryPanel;
	GameObject slotPanel;
	ItemDbScript database;
	public GameObject inventorySlot;
	public GameObject inventoryItem;

	public List<Item> items = new List<Item>();
	public List<GameObject> slots = new List<GameObject>();
	public List<GameObject> eqSlots = new List<GameObject>();
	public int slotCount;

	void Start()
	{
		database = GetComponent<ItemDbScript>();
		slotCount = 32;
		inventoryPanel = GameObject.Find("Inventory Panel");
		slotPanel = inventoryPanel.transform.FindChild("Slot Panel").gameObject;

		for (int i = 0; i < slotCount; i++)
		{
			items.Add(new Item());
			slots.Add(Instantiate(inventorySlot));
			slots[i].GetComponent<SlotScript>().id = i;
			slots[i].transform.SetParent(slotPanel.transform);
		}

		AddItem(0);
		AddItem(1);
		AddItem(2);
		AddItem(3);
		AddItem(4);

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
				itemObj.transform.position = Vector2.zero;
				itemObj.name = itemToAdd.title;
				slots[i].name = "Slot: " + itemToAdd.title;
				break;
			}
		}
	}
}