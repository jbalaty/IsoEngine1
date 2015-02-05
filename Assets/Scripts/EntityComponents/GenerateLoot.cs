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
                var typeIdx = UnityEngine.Random.Range(0, 2 + 1);
                Entity entity = null;
                typeIdx = 2;
                if (typeIdx == 0)
                {
                    int amount = Random.Range(this.MinGoldAmount, this.MaxGoldAmount + 1);
                    entity = Items.ItemsDatabase.Instance.SpawnWorldItems(this.Entity.GetTilePosition(), "Gold", amount)[0];
                }
                else if (typeIdx == 1)
                {
                    entity = Items.ItemsDatabase.Instance.SpawnWorldItems(this.Entity.GetTilePosition(), "Minor healing potion", 1f)[0];
                }
                else if (typeIdx == 2)
                {
                    entity = Items.ItemsDatabase.Instance.SpawnWorldItems(this.Entity.GetTilePosition(), "Dagger", 1f)[0];
                }
                if (entity != null
                    && entity.GetComponent<Pickable>() != null
                    && entity.GetComponent<Pickable>().Item != null)
                {
                    Utils.PlayClip(entity.GetComponent<Pickable>().Item.DropSound);
                }
            }
        }

    }
}
