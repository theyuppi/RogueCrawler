using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

[System.Serializable]
public class SlotScript : MonoBehaviour, IDropHandler
{
	public int id;
	private InventoryScript inv;
	public string slotCategory = "";
	PlayerScript player;

	void Start()
	{
		inv = GameObject.Find("Inventory").GetComponent<InventoryScript>();
		player = GameObject.Find("Player").GetComponent<PlayerScript>();
	}

	public void OnDrop(PointerEventData eventData)
	{
		ItemDataScript droppedItem = eventData.pointerDrag.GetComponent<ItemDataScript>();
		//Slot was empty
		if (inv.items[id].id == -1)
		{
			bool eqSlot = false;
			//If slot has requirements
			if (inv.slots[id].GetComponent<SlotScript>().slotCategory != "")
			{
				eqSlot = true;
				//Is item eligible?
				if (droppedItem.GetComponent<ItemDataScript>().item.category == inv.slots[id].GetComponent<SlotScript>().slotCategory)
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
				else //Item was not eligible for slot, put back where it came from
				{
					inv.items[droppedItem.slot] = droppedItem.item;
				}
			}

			if (!eqSlot)
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
		}
		//Slot is occupied
		else
		{
			bool eqSlot = false;
			if (inv.slots[id].GetComponent<SlotScript>().slotCategory != "")  //Dropped in EQ-slot
			{
				eqSlot = true;
				//Is item eligible?
				if (droppedItem.GetComponent<ItemDataScript>().item.category == inv.slots[id].GetComponent<SlotScript>().slotCategory)
				{
					Transform item = this.transform.GetChild(1);
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
				else //Item was not eligible for slot, put back where it came from
				{
					inv.items[droppedItem.slot] = droppedItem.item;
				}
			}
			else
			{
				Transform item = this.transform.GetChild(0);
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
		//player.UpdateStats();
	}
}
