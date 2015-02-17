using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    public class Chest : ObjectComponent
    {
        Inventory Inventory;

        void Start()
        {
            GetComponentInChildren<Animator>().SetBool("IsOpen", Value == 1);
            Inventory = GetComponent<Inventory>();
            {
                Inventory.AddItem(Items.ItemsDatabase.Instance.FindByName("Gold"), Random.Range(1, 11));
            }
        }


        public override void Interact(Entity e)
        {
            if (Value == 0)
            {
                Utils.PlayClip(InteractionSound1);
                Value = 1;
                if (Inventory != null)
                {
                    Inventory.ToggleInventoryDialog(e.GetComponent<Inventory>());
                }
                SendMessage("ChestOpened", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Utils.PlayClip(InteractionSound2);
                Value = 0;
            }
            GetComponentInChildren<Animator>().SetBool("IsOpen", Value == 1);

        }
    }
}