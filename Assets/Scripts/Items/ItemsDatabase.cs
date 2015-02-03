using UnityEngine;
using UnityEditor;
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
                if(_instance == null)
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
            return Items[id];
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

        public Entity SpawnWorldItem(Vector2Int coords, string itemname, float amount)
        {
            var item = FindByName(itemname);
            return SpawnWorldItem(coords, item, amount);
        }

        public Entity SpawnWorldItem(Vector2Int coords, int itemid, float amount)
        {
            var item = FindByID(itemid);
            return SpawnWorldItem(coords, item, amount);
        }

        Entity SpawnWorldItem(Vector2Int coords, Item item, float amount)
        {

            var spawn = Instantiate(item.WorldObject, coords.Vector3(EVectorComponents.XZ), Quaternion.identity) as Transform;
            spawn.parent = EntitiesManager.Instance.transform;
            var pickable = spawn.GetComponent<Pickable>();
            pickable.Item = item;
            pickable.Amount = amount;
            return spawn.GetComponent<Entity>();
        }
    }
}