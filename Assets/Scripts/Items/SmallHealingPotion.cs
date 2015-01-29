using UnityEngine;
using System.Collections;
using System;

namespace Dungeon.Items
{
    [Serializable]
    public class SmallHealingPotion : Item
    {
        public SmallHealingPotion()
        {
            this.IsStackable = true;
            this.Name = "Small healing potion";
            this.Description = "Small healing potion, that helps on small injuries";
            this.Bonuses.Add(new ItemBonus { 
                ApplicationType = EItemBonusApplicationType.Use, 
                AttributeName = "HitPoints", 
                Value = +10 });
        }
    }
}
