using UnityEngine;
using System.Collections;
using Dungeon.Items;

namespace Dungeon
{
    public enum EEquipmentSlotType {
        OneHand, TwoHand, Helmet, Armor, Ring, Amulet, Boots, Pants
    }


    public class Equipment : MonoBehaviour
    {
        public int NumberOneHandWeaponSlots = 1;
        public int NumberTwoHandWeaponSlots = 1;
        public int NumberArmorSlots = 1;
        public int NumberHelmetSlots = 1;



        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GetSlots(EEquipmentSlotType type)
        {

        }

        public void EquipItem(Item item)
        {

        }
    }
}