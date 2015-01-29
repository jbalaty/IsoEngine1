using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Dungeon.Items;

namespace Dungeon
{
    [System.Serializable]
    public class InventoryItem
    {
        public Item Item;
        public float Amount = 0;

        public InventoryItem(Item item, float amount)
        {
            this.Item = item;
            this.Amount = amount;
        }
    }


    public class Inventory : MonoBehaviour
    {
        public bool PickItems = true;


        [SerializeField]
        public List<InventoryItem> Items = new List<InventoryItem>();

        Entity Entity;

        [SerializeField]
        public int GoldAmount
        {
            get
            {
                var invitem = Items.Find((ii) => ii.GetHashCode() == new Gold().GetHashCode());
                return invitem != null ? (int)invitem.Amount : 0;
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
            AddItem(new Gold(), 0);
        }

        // Update is called once per frame
        void Update()
        {
            Entity = this.GetComponent<Entity>();
        }

        public void AddItem(Item item, float amount)
        {
            /*if (item is Gold)
            {
                AddGold((int)amount);
            }
            else
            {
                InnerAddItem(item);
            }*/
            if (amount > 0f)
            {
                var ii = FindItem(item);
                if (ii != null && item.IsStackable)
                {
                    ii.Amount += amount;
                }
                else
                {
                    Items.Add(new InventoryItem(item, amount));
                }
            }
        }

        protected InventoryItem FindItem(Item it, float amount = 1f)
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
    }
}