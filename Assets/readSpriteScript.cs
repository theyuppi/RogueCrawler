using UnityEngine;
using System.Collections;

public class readSpriteScript : MonoBehaviour
{
	SpriteRenderer sr;
	Sprite spr;

	public GameObject TilePrefab;

	void Start()
	{
			CreateRoom("maproom" + 1);
	}

	void Update()
	{
	}

	void CreateRoom(string levelName)
	{
		spr = Resources.Load<Sprite>(levelName);

		float sizeY = spr.texture.width; //Rows
		float sizeX = spr.texture.width; //Columns

		for (float j = sizeY; j > 0; j--) //Rows
		{
			for (float i = 0; i < sizeX; i++) //Columns
			{
				Color pixelCol = new Color(spr.texture.GetPixel((int)i, (int)j).r, spr.texture.GetPixel((int)i, (int)j).g, spr.texture.GetPixel((int)i, (int)j).b, spr.texture.GetPixel((int)i, (int)j).a);
				string tileType = ColorToHex(pixelCol);
                if (!tileType.Equals("FFFFFF"))
                {
                    GameObject tile = Instantiate(TilePrefab, new Vector2(i * 0.8f, (sizeX - j) * -0.8f), transform.rotation) as GameObject;

                    if (tileType.Equals("C4AA6C"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Floor;
                        //Debug.Log("Floor");
                    }
                    else if (tileType.Equals("584A33"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Wall;
                        //Debug.Log("Wall");
                    }
                    else if (tileType.Equals("FF0000"))
                    {
                        //Debug.Log("Enemy");
                    }
                    else if (tileType.Equals("FFFF00"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Chest;
                        //Debug.Log("Treasure");
                    }
                    else if (tileType.Equals("505050"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Spike;
                        //Debug.Log("Spikes");
                    }
                    else if (tileType.Equals("000000"))
                    {
                        tile.GetComponent<TileScript>().myTileType = TileScript.TileTypes.Hole;
                        //Debug.Log("Hole");
                    }
                }
			}
		}
		Application.Quit();
	}

	string ColorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}

	Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r, g, b, 255);
	}
}
