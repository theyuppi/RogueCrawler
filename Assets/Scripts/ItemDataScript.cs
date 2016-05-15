﻿using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public class ItemDataScript : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{

	public Item item;
	public int amount;
	public int slot;

	private Vector2 offset;
	private InventoryScript inv;
	private Transform invCanvasTransform;

	void Start()
	{
		inv = GameObject.Find("Inventory").GetComponent<InventoryScript>();
		invCanvasTransform = GameObject.Find("InventoryCanvas").transform;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (item.slug == "potion_healing" && Input.GetMouseButtonDown(1))
		{
			Debug.Log("Healed " + item.stats.power.ToString() + " hp");
			GameObject.Find("PlayerHandler").GetComponent<PlayerHandler>().player.GetComponent<PlayerScript>().Heal(item.stats.power);
			inv.items[slot] = new Item();
			this.transform.SetParent(this.transform.parent.parent);  //Onödig men flyttar object från slot iaf
			Destroy(this.gameObject);

		}
		if (item != null)
		{
			offset = eventData.position - new Vector2(this.transform.position.x, this.transform.position.y);
			this.transform.position = eventData.position - offset;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (item != null)
		{
			if (transform.parent.parent.parent.tag == "LootPanel")
			{
                //Debug.Log("item.myInti: " + item.myInti);
                if (item.belongsToChest.GetComponent<ChestScript>().itemList[item.myInti] != null)
				    item.belongsToChest.GetComponent<ChestScript>().itemList.RemoveAt(item.myInti);
			}
			this.transform.SetParent(this.transform.parent.parent);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (item != null)
		{
			this.transform.position = eventData.position - offset;
			inv.items[slot] = new Item();
			this.transform.SetParent(invCanvasTransform);
			Cursor.visible = false;
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this.transform.SetParent(inv.slots[slot].transform);
		this.transform.position = inv.slots[slot].transform.position;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		Cursor.visible = true;
        
    }
}
