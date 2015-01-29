using UnityEngine;
using System.Collections;


namespace Dungeon.Items
{
    [System.Serializable]
    public class Gold : Item
    {
        public Gold()
        {
            IsStackable = true;
            Name = "Gold";
            Description = "Gold coins";
        }

    }
}
