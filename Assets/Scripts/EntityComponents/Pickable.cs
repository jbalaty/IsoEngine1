using UnityEngine;
using System.Collections;
using IsoEngine1;
using Dungeon.Items;

namespace Dungeon
{
    public class Pickable : MonoBehaviour, IEntityTrigger
    {
        public Item Item;
        public float Amount;
        public AudioClip PickupSound;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void IEntityTrigger.OnEntityIn(Entity entity)
        {
            var inventory = entity.GetComponent<Inventory>();
            if (inventory != null && inventory.PickItems)
            {
                Utils.PlayClip(PickupSound);
                //Debug.Log("Picking " + this. Gold + " gold");
                //entity.GetComponent<Inventory>().AddGold(this.Gold);
                inventory.AddItem(this.Item, this.Amount);
                if (this.Item is Gold)
                {
                    var tm = entity.GetComponent<TextMeshSpawner>();
                    if (tm != null)
                    {
                        tm.SpawnTextMesh("+ " + this.Amount + "G", Color.yellow, TextMeshSpawner.DefaultFadeOutTime);
                    }
                }
                Destroy(this.gameObject);
                Utils.PlayClip(PickupSound);
            }
        }

        void IEntityTrigger.OnEntityOut(Entity entity)
        {
        }
    }
}