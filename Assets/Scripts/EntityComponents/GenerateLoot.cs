using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    public class GenerateLoot : MonoBehaviour
    {
        public Entity Entity;
        public Combat Combat;
        public int MaxGoldAmount;
        public int MinGoldAmount;
        public bool UseHitPointsForGoldRange = true;
        [Range(0, 1)]
        public float LootProbability = 0.7f;

        void Awake()
        {
            this.Entity = this.GetComponent<Entity>();
            this.Combat = this.GetComponent<Combat>();
        }

        // Use this for initialization
        void Start()
        {
            if (this.UseHitPointsForGoldRange)
            {
                MaxGoldAmount = this.Combat.MaxHitPoints;
                MinGoldAmount = MaxGoldAmount / 2;
            }
            this.Combat.EntityDead += OnDead;

        }

        protected void OnDead(Vector3 pos)
        {
            if (UnityEngine.Random.value < LootProbability)
            {
                var typeIdx = UnityEngine.Random.Range(0, 2);
                //var typeIdx = 0;
                Entity entity = null;
                if (typeIdx == 0)
                {
                    int amount = Random.Range(this.MinGoldAmount, this.MaxGoldAmount + 1);
                    entity = Items.ItemsDatabase.Instance.SpawnWorldItem(this.Entity.GetTilePosition(),"Gold", amount);
                }
                else if (typeIdx == 1)
                {
                    entity = Items.ItemsDatabase.Instance.SpawnWorldItem(this.Entity.GetTilePosition(), "Minor healing potion"
                        , 1f);

                }
                if (entity != null && entity.GetComponent<Pickable>().Item != null
                    && entity.GetComponent<Pickable>().Item.PickupSound != null)
                {
                    Utils.PlayClip(entity.GetComponent<Pickable>().Item.PickupSound);
                }
            }
        }

    }
}
