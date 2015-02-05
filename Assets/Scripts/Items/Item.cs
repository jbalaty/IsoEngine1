using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Dungeon.Items
{
    public enum EItemType { GeneralItem, Money, Weapon, Armor, Potion, Necklace, Ring, Scroll }
    public enum EItemEffectApplicationType { None, Use, Equip, HaveInInventory };


    [Serializable]
    public class ItemEffect
    {
        public EItemEffectApplicationType ApplicationType;
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
        public int ID;
        public EItemType Type = EItemType.GeneralItem;
        public bool IsStackable = true;
        public bool IsQuestItem = false;
        public int MaxDurability = -1;
        public string Name = "Item";
        public string Description = "Some item";
        public float UnitAmount = 1f;
        public Sprite Icon;
        public Transform WorldObject;
        public AudioClip DropSound;
        public AudioClip PickupSound;
        public AudioClip UseSound;
        public AudioClip EquipSound;
        public List<ItemEffect> Effects = new List<ItemEffect>();


        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            var result = Name.GetHashCode();
            foreach (var b in Effects)
            {
                result += b.GetHashCode();
            }
            return result;
        }

    }
}
