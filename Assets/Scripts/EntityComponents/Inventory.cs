﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Dungeon.Items;

namespace Dungeon
{
    [System.Serializable]
    public class InventoryItem
    {
        public int ItemID = 0;
        public float Amount = 0;

        [System.NonSerialized]
        Item _Item;
        public Item Item
        {
            get
            {
                if (_Item == null)
                {
                    _Item = ItemsDatabase.Instance.FindByID(ItemID);
                }
                return _Item;
            }
            set { _Item = value; }
        }

        public InventoryItem(Item item, float amount)
        {
            this.Item = item;
            this.ItemID = item.ID;
            this.Amount = amount;
        }
    }


    public class Inventory : MonoBehaviour
    {
        public bool PickItems = true;
        public GameObject InventoryDialog;

        [SerializeField]
        List<InventoryItem> Items = new List<InventoryItem>();

        Entity Entity;

        [SerializeField]
        public int GoldAmount
        {
            get
            {
                //var invitem = Items.Find((ii) => ii.GetHashCode() == new Gold().GetHashCode());
                //return invitem != null ? (int)invitem.Amount : 0;
                return 0;
            }
        }

        void Awake()
        {
            if (InventoryDialog == null)
            {
                // try to find foreign invetory dialog
                //InventoryDialog = GameObject.Find("Canvas");
                //GameObject.FindGameObjectWithTag("Dialog_ObjectsInventory")
                InventoryDialog = GameController.Instance.Dialogs.ObjectsInventoryDialog;
            }
        }

        // Use this for initialization
        void Start()
        {
            //var g1 = new Gold();
            //var g2 = new Gold();
            //var x = g1.GetHashCode() == g2.GetHashCode();
            //var xx = g1 == g2;
            //var xxx = g1.Equals(g2);
            //var a = 123;
            //AddItem(new Gold(), 0);
        }

        // Update is called once per frame
        void Update()
        {
            Entity = this.GetComponent<Entity>();
        }

        public List<InventoryItem> GetItems()
        {
            return Items;
        }

        public int ItemsCount
        {
            get { return Items.Count; }
        }

        public InventoryItem AddItem(Item item, float amount)
        {
            /*if (item is Gold)
            {
                AddGold((int)amount);
            }
            else
            {
                InnerAddItem(item);
            }*/
            InventoryItem ii = null;
            if (amount > 0f)
            {
                ii = FindItem(item);
                if (ii != null && item.IsStackable)
                {
                    ii.Amount += amount;
                }
                else
                {
                    Items.Add(new InventoryItem(item, amount));
                }
            }
            else if (amount < 0f)
            {
                var pamount = amount * -1;
                ii = FindItem(item);
                if (ii != null)
                {
                    if (ii.Amount >= pamount)
                    {
                        ii.Amount -= pamount;
                    }
                    else
                    {
                        var r = (pamount - ii.Amount) * -1;
                        ii.Amount = 0f;
                    }
                }
            }
            CompactItems();
            return ii;
        }

        public void CompactItems()
        {
            Items.RemoveAll((ii) => ii.Amount < 0.001f);
        }

        public InventoryItem FindItem(Item it, float amount = 1f)
        {
            var foundItems = Items.FindAll((ii) =>
            {
                var hc1 = ii.Item.GetHashCode();
                var hc2 = it.GetHashCode();
                return hc1 == hc2;
            });
            if (foundItems.Count > 0 && foundItems[0].Amount >= amount)
            {
                return foundItems[0];
            }
            return null;
        }

        public bool ToggleInventoryDialog(Inventory targetInv)
        {
            if (InventoryDialog != null)
            {
                if (!InventoryDialog.activeSelf)
                {
                    InventoryDialog.SetActive(true);
                    //var i = InventoryDialog.GetComponent(typeof(IInventoryPanel));
                    //var invpanel = Utils.GetInterface<IInventoryPanel>(InventoryDialog);
                    var invpanel = InventoryDialog.GetComponent(typeof(IInventoryPanel)) as IInventoryPanel;
                    invpanel.Setup(this, targetInv);
                    if (targetInv == null)
                    {
                        invpanel.SetName("Inventory - " + Entity.Name);
                    }
                    else
                    {
                        invpanel.SetName(Entity.Name);
                    }
                    return true;
                }
                else
                {
                    InventoryDialog.SetActive(false);
                    return true;
                }
            }
            return false;
        }

        public bool UseItem(Item item, float amount = -1)
        {
            var result = false;
            return result;
        }


        public InventoryItem DropItem(Item item, float amount)
        {
            var ii = FindItem(item);
            if (item != null)
            {
                var newii = AddItem(ii.Item, -amount);
                return newii;
            }
            else throw new Exception("Cannot find '" + item + "' in inventory");
            return null;
        }
    }
}