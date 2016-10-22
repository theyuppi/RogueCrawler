using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    [Serializable]
    public class SlotScript : MonoBehaviour, IDropHandler
    {
        public int id;
        private InventoryScript inv;
        public string slotCategory = "";
        PlayerScript player;
        private GameObject sPanelChest;
        public GameObject owner;

        void Start()
        {
            inv = GameObject.Find("Inventory").GetComponent<InventoryScript>();
            player = GameObject.Find("Player").GetComponent<PlayerScript>();
            sPanelChest = GameObject.Find("Slot Panel Chest");
        }

        public void OnDrop(PointerEventData eventData)
        {
            ItemDataScript droppedItem = eventData.pointerDrag.GetComponent<ItemDataScript>();

            if (id >= 100) //We're in a chest
            {
                if (this.transform.childCount == 0)
                {
                    int freeSlot = -1;
                    for (int i = 0; i < sPanelChest.transform.childCount; i++) //Loop through all slots in the chest
                    {
                        if (freeSlot >= 0)
                        {
                            break;
                        }

                        if (sPanelChest.transform.GetChild(i).childCount == 0) //Found empty slot
                        {
                            freeSlot = i;
                        }
                    }
                    if (freeSlot >= 0)
                    {
                        //Reset name of old slot
                        if (droppedItem.slot >= 100)
                        {
                            sPanelChest.transform.GetChild(droppedItem.slot - 100).gameObject.name = "Slot(Clone)";

                            //Empty old slot
                            droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Remove(droppedItem.item);
                        }
                        else
                        {
                            inv.slots[droppedItem.slot].name = "Slot(Clone)";

                            //Empty old slot
                            inv.items[droppedItem.slot] = new Item();
                        }


                        //Assign new slot
                        droppedItem.slot = id;

                        //Name new slot
                        sPanelChest.transform.GetChild(id - 100).gameObject.name = "Slot: " + droppedItem.item.title;
                        droppedItem.item.belongsToChest = this.owner;
                    }
                }
                else
                {
                    if (droppedItem.slot >= 100) //Swap inside chest
                    {
                        Transform item = this.transform.GetChild(0);

                        item.GetComponent<ItemDataScript>().slot = droppedItem.slot;
                        item.transform.SetParent(sPanelChest.transform.GetChild(droppedItem.slot - 100).transform);
                        item.transform.position = sPanelChest.transform.GetChild(droppedItem.slot - 100).transform.position;

                        //Name swappers slot
                        sPanelChest.transform.GetChild(droppedItem.slot - 100).name = "Slot: " + item.GetComponent<ItemDataScript>().item.title;

                        droppedItem.transform.SetParent(this.transform);
                        droppedItem.transform.position = this.transform.position;

                        //Name new slot
                        sPanelChest.transform.GetChild(item.GetComponent<ItemDataScript>().slot - 100).name = "Slot: " + droppedItem.item.title;

                        droppedItem.slot = id;
                    }
                    else if (droppedItem.slot < 100) //Dragged item from inventory and dropped onto occupied slot in chest
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
                        sPanelChest.transform.GetChild(id - 100).name = "Slot: " + droppedItem.item.title;

                        inv.items[droppedItem.slot] = item.GetComponent<ItemDataScript>().item;
                        droppedItem.slot = id;

                        //Untested
                        item.GetComponent<ItemDataScript>().item.belongsToChest = droppedItem.item.belongsToChest;
                        droppedItem.item.belongsToChest = this.owner;
                    }
                }
            }
            else
            {
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
                            if (droppedItem.slot >= 100)
                            {
                                sPanelChest.transform.GetChild(droppedItem.slot - 100).gameObject.name = "Slot(Clone)";

                                //Empty old slot
                                droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Remove(droppedItem.item);
                            }
                            else
                            {
                                //Reset name of old slot
                                inv.slots[droppedItem.slot].name = "Slot(Clone)";

                                //Empty old slot
                                inv.items[droppedItem.slot] = new Item();
                            }

                            //Assign new slot
                            inv.items[id] = droppedItem.item;
                            droppedItem.slot = id;

                            //Name new slot
                            inv.slots[id].name = "Slot: " + droppedItem.item.title;
                        }
                        else //Item was not eligible for slot, put back where it came from
                        {
                            if (droppedItem.slot >= 100)
                            {
                                sPanelChest.transform.GetChild(droppedItem.slot - 100).gameObject.name = "Slot: " + droppedItem.item.title;
                                droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Add(droppedItem.item);
                            }
                            else
                            {
                                inv.items[droppedItem.slot] = droppedItem.item;
                            }
                        }
                    }

                    if (!eqSlot)
                    {
                        if (droppedItem.slot >= 100)
                        {
                            sPanelChest.transform.GetChild(droppedItem.slot - 100).gameObject.name = "Slot(Clone)";

                            //Empty old slot
                            droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Remove(droppedItem.item);
                        }
                        else
                        {
                            //Reset name of old slot
                            inv.slots[droppedItem.slot].name = "Slot(Clone)";

                            //Empty old slot
                            inv.items[droppedItem.slot] = new Item();
                        }

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
                            if (droppedItem.slot >= 100)
                            {
                                item.GetComponent<ItemDataScript>().slot = droppedItem.slot;
                                item.transform.SetParent(sPanelChest.transform.GetChild(droppedItem.slot - 100).transform);
                                item.transform.position = sPanelChest.transform.GetChild(droppedItem.slot - 100).transform.position;

                                //Name swappers slot
                                sPanelChest.transform.GetChild(droppedItem.slot - 100).name = "Slot: " + item.GetComponent<ItemDataScript>().item.title;

                                droppedItem.transform.SetParent(this.transform);
                                droppedItem.transform.position = this.transform.position;

                                //Name new slot
                                inv.slots[id].name = "Slot: " + droppedItem.item.title;

                                droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Add(item.GetComponent<ItemDataScript>().item);
                                droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Remove(droppedItem.item);
                            }
                            else
                            {
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
                            }
                            droppedItem.slot = id;
                        }
                        else //Item was not eligible for slot, put back where it came from
                        {
                            if (droppedItem.slot >= 100)
                            {
                                sPanelChest.transform.GetChild(droppedItem.slot - 100).gameObject.name = "Slot: " + droppedItem.item.title;
                                droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Add(droppedItem.item);
                            }
                            else
                            {
                                inv.items[droppedItem.slot] = droppedItem.item;
                            }
                        }
                    }
                    else
                    {
                        Transform item = this.transform.GetChild(0);
                        //Player dragged item from EQ and dropped in Inventory.

                        //Swap chest to inventory
                        if (id < 100 && droppedItem.slot >= 100)
                        {
                            item.GetComponent<ItemDataScript>().slot = droppedItem.slot;
                            item.transform.SetParent(sPanelChest.transform.GetChild(droppedItem.slot - 100).transform);
                            item.transform.position = sPanelChest.transform.GetChild(droppedItem.slot - 100).transform.position;

                            //Name swappers slot
                            sPanelChest.transform.GetChild(droppedItem.slot - 100).name = "Slot: " + item.GetComponent<ItemDataScript>().item.title;

                            droppedItem.transform.SetParent(this.transform);
                            droppedItem.transform.position = this.transform.position;

                            //Name new slot
                            inv.slots[id].name = "Slot: " + droppedItem.item.title;

                            droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Remove(droppedItem.item);
                            droppedItem.item.belongsToChest.GetComponent<ChestScript>().ItemList.Add(item.GetComponent<ItemDataScript>().item);

                            droppedItem.slot = id;
                            droppedItem.item.belongsToChest = item.GetComponent<ItemDataScript>().item.belongsToChest;
                        }
                        else if (droppedItem.slot < 100 && id >= 100) //Dragged item from inventory and dropped onto occupied slot in chest
                        {
                            item.GetComponent<ItemDataScript>().slot = droppedItem.slot;
                            item.transform.SetParent(inv.slots[droppedItem.slot].transform);
                            item.transform.position = inv.slots[droppedItem.slot].transform.position;

                            //Name swappers slot
                            inv.slots[droppedItem.slot].name = "Slot: " + item.GetComponent<ItemDataScript>().item.title;

                            droppedItem.transform.SetParent(this.transform);
                            droppedItem.transform.position = this.transform.position;

                            //Name new slot
                            sPanelChest.transform.GetChild(id - 100).name = "Slot: " + droppedItem.item.title;

                            inv.items[droppedItem.slot] = item.GetComponent<ItemDataScript>().item;
                            inv.items[id] = droppedItem.item;
                            droppedItem.slot = id;

                            item.GetComponent<ItemDataScript>().item.belongsToChest = droppedItem.item.belongsToChest;
                            droppedItem.item.belongsToChest = this.owner;
                        }
                        else  //Normal swap in chest
                        {
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
                            droppedItem.item.belongsToChest = item.GetComponent<ItemDataScript>().item.belongsToChest;
                        }
                    }
                }
            }
        }
    }
}
