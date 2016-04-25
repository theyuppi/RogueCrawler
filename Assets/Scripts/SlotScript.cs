using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class SlotScript : MonoBehaviour, IDropHandler
{
	public int id;
	private InventoryScript inv;

	void Start()
	{
		inv = GameObject.Find("Inventory").GetComponent<InventoryScript>();
	}

	public void OnDrop(PointerEventData eventData)
	{
		ItemDataScript droppedItem = eventData.pointerDrag.GetComponent<ItemDataScript>();
		if (inv.items[id].id == -1)
		{
			//Reset name of old slot
			inv.slots[droppedItem.slot].name = "Slot(Clone)";

			//Empty old slot
			inv.items[droppedItem.slot] = new Item();

			//Assign new slot
			inv.items[id] = droppedItem.item;
			droppedItem.slot = id;

			//Name new slot
			inv.slots[id].name = "Slot: " + droppedItem.item.title;
		}
		else
		{
			Transform item = this.transform.GetChild(0);   //Ska fixa, droppa på samma
			item.GetComponent<ItemDataScript>().slot = droppedItem.slot;
			item.transform.SetParent(inv.slots[droppedItem.slot].transform);
			item.transform.position = inv.slots[droppedItem.slot].transform.position;

			//Name swappers slot
			inv.slots[droppedItem.slot].name = "Slot: " + item.GetComponent<ItemDataScript>().item.title;

			droppedItem.transform.SetParent(this.transform);
			droppedItem.transform.position = this.transform.position;
			

			//Name new slot
			inv.slots[id].name = "Slot: " + droppedItem.item.title;

			inv.items[droppedItem.slot] = item.GetComponent<ItemDataScript>().item;
			inv.items[id] = droppedItem.item;
			droppedItem.slot = id;
		}
	}
}
