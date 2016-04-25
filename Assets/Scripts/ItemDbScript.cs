using UnityEngine;
using System.Collections;
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ItemDbScript : MonoBehaviour
{
	private List<Item> database = new List<Item>();
	private List<Item> itemData;

	void Start()
	{
		itemData = JsonMapper.ToObject<List<Item>>(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));


		for (int i = 0; i < itemData.Count; i++)
		{
			itemData[i].sprite = Resources.Load<Sprite>("Inventory/" + itemData[i].slug);
		}
	}

	public Item FetchItemByID(int id)
	{
		for (int i = 0; i < itemData.Count; i++)
		{
			if (itemData[i].id == id)
			{
				return itemData[i];
			}
		}
		return null;
	}
}

public class Item
{
	public int id = 0;
	public string title = "";
	public int value = 0;
	public int power = 0;
	public int defence = 0;
	public int vitality = 0;
	public string description = "";
	public bool stackable = false;
	public int rarity = 0;
	public string slug = "";
	public Sprite sprite;

	public Item()
	{
		this.id = -1;
		//this.sprite = Resources.Load<Sprite>("Inventory/" + slug);
	}
}