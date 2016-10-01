using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
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
                //Debug.Log("Healed " + item.stats.power.ToString() + " hp");
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
                if (slot >= 100)
                {
                    //Debug.Log("item.myInti: " + item.myInti);
                    //if (item.belongsToChest.GetComponent<ChestScript>().ItemList[item.myInti] != null)
                    //	item.belongsToChest.GetComponent<ChestScript>().ItemList.RemoveAt(item.myInti);
                    //Debug.Log(item.belongsToChest.transform.position);
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
                this.transform.SetParent(invCanvasTransform);
                Cursor.visible = false;

                if (slot < 100)
                {
                    inv.items[slot] = new Item();
                }
                else
                {
                    GameObject go = GameObject.Find("Slot Panel Chest").transform.GetChild(slot - 100).gameObject;
                    item.belongsToChest.GetComponent<ChestScript>().ItemList.Remove(item);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (slot < 100)
            {
                this.transform.SetParent(inv.slots[slot].transform);
                this.transform.position = inv.slots[slot].transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                Cursor.visible = true;
            }
            else
            {
                GameObject go = GameObject.Find("Slot Panel Chest").transform.GetChild(slot - 100).gameObject;
                this.transform.SetParent(go.transform);
                this.transform.position = go.transform.position;
                GetComponent<CanvasGroup>().blocksRaycasts = true;
                Cursor.visible = true;
                item.belongsToChest = go.GetComponent<SlotScript>().owner;
            }
        }
    }
}
