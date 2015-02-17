﻿using UnityEngine;
using System.Collections.Generic;
using IsoEngine1;
using Dungeon.Items;

namespace Dungeon
{
    public class Pickable : MonoBehaviour, IEntityTrigger
    {
        [HideInInspector]
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
                //Debug.Log("Picking " + this. Gold + " gold");
                //entity.GetComponent<Inventory>().AddGold(this.Gold);
                var ii = inventory.AddItem(this.Item, this.Amount);
                if (this.Item.Type == EItemType.Money)
                {
                    var tm = entity.GetComponent<TextMeshSpawner>();
                    if (tm != null)
                    {
                        tm.SpawnTextMesh("+ " + this.Amount + "G", Color.yellow, TextMeshSpawner.DefaultFadeOutTime);
                    }
                }
                Utils.PlayClip(Item.PickupSound ?? PickupSound);
                Destroy(this.gameObject);
                entity.gameObject.SendMessage("ItemPickedUp", this.Item, SendMessageOptions.DontRequireReceiver);
            }
        }

        void IEntityTrigger.OnEntityOut(Entity entity)
        {
        }
    }
}