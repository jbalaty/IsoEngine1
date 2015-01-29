using UnityEngine;
using System.Collections;
using System;

namespace Dungeon.Items
{
    [Serializable]
    public class HealingPotion : Item
    {
        public HealingPotion()
        {
            this.IsStackable = true;
            this.Description = "Healing potion, that heals some small injury";
        }
    }
}
