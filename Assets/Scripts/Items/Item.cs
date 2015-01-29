using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Dungeon.Items
{
    public enum EItemBonusApplicationType { Use, Equip, HaveInInventory };

    public class ItemBonus
    {
        public EItemBonusApplicationType ApplicationType;
        public string AttributeName;
        public float Value;

        public override string ToString()
        {
            return "Type:" + ApplicationType.ToString() + ";Att:" + AttributeName + ";Value:" + Value;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }



    [Serializable]
    public class Item
    {
        public bool IsStackable = true;
        public int MaxDurability = -1;
        public string Name = "Item";
        public string Description = "Some item";
        public List<ItemBonus> Bonuses = new List<ItemBonus>();

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            var result = Name.GetHashCode();
            foreach (var b in Bonuses)
            {
                result += b.GetHashCode();
            }
            return result;
        }

    }
}
