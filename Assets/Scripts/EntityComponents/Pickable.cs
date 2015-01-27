using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    public class Pickable : MonoBehaviour, IEntityTrigger
    {
        public int Gold = 0;
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
            if (entity.GetComponent<Inventory>() != null)
            {
                Utils.PlayClip(PickupSound);
                Debug.Log("Picking " + this.Gold + " gold");
                entity.GetComponent<Inventory>().AddGold(this.Gold);
                Destroy(this.gameObject);
                Utils.PlayClip(PickupSound);
            }
        }

        void IEntityTrigger.OnEntityOut(Entity entity)
        {
        }
    }
}