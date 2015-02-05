using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;

namespace Dungeon.Items
{
    [ExecuteInEditMode]
    public class ItemsDatabase : MonoBehaviour
    {
        static ItemsDatabase _instance;
        public static ItemsDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<ItemsDatabase>();
                }
                return _instance;
            }
        }

        public List<Item> Items;

        public Item FindByName(string name)
        {
            var n = ("" + name).ToLower().Trim();
            return Items.Find((i) => i.Name.ToLower().Trim() == n);
        }

        public Item FindByID(int id)
        {
            return Items.Find((i) => i.ID == id);
        }
        // Use this for initialization
        void Start()
        {
            // verify items

        }

        // Update is called once per frame
        void Update()
        {

        }

        public List<Entity> SpawnWorldItems(Vector2Int coords, int itemid, float amount)
        {
            var item = FindByID(itemid);
            return SpawnWorldItems(coords, item, amount);
        }

        public List<Entity> SpawnWorldItems(Vector2Int coords, string itemName, float amount)
        {
            var item = FindByName(itemName);
            return SpawnWorldItems(coords, item, amount);
        }

        public List<Entity> SpawnWorldItems(Vector2Int coords, Item item, float amount)
        {
            var result = new List<Entity>();
            //var item = FindByID(itemid);
            if (item.UnitAmount == 0f)
            {
                // if item does not have unit amount, spawn one item the supplied amount
                var e = SpawnWorldItemEntity(coords, item, amount);
                result.Add(e);
            }
            else
            {
                // if item have to be divided into units, spawn several units according to total amount
                for (var i = 0; i < (int)(amount / item.UnitAmount); i++)
                {
                    var e = SpawnWorldItemEntity(coords, item, item.UnitAmount);
                    result.Add(e);
                }
            }
            return result;
        }

        Entity SpawnWorldItemEntity(Vector2Int coords, Item item, float amount)
        {
            var randomVariation = Random.insideUnitSphere / 4f;
            randomVariation.y = 0f;
            randomVariation.x = Mathf.Abs(randomVariation.x);
            randomVariation.z = Mathf.Abs(randomVariation.z);

            var spawnPoint = coords.Vector3(EVectorComponents.XZ) + randomVariation;
            var spawn = Instantiate(item.WorldObject, spawnPoint, Quaternion.identity) as Transform;
            spawn.parent = EntitiesManager.Instance.transform;
            var pickable = spawn.GetComponent<Pickable>();
            pickable.Item = item;
            pickable.Amount = amount;
            return spawn.GetComponent<Entity>();
        }
    }
}