using UnityEngine;
using System.Collections;
using System;

namespace Dungeon.Items
{
    [Serializable]
    public abstract class Potion : Item
    {
        public Potion()
        {
            this.IsStackable = true;
            this.Description = "Magical potion of some sort";
        }
    }
}
