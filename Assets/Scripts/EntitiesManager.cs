using UnityEngine;
using System.Collections;
using IsoEngine1;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Dungeon.Items;

namespace Dungeon
{
    public class EntitiesManager : MonoBehaviour
    {
        public GameObject Root;
        public List<Entity> AllEntities = new List<Entity>();

        public static EntitiesManager Instance;

        void Awake()
        {
            if (Instance == null) Instance = this;
        }

        // Use this for initialization
        void Start()
        {
        }

        IEnumerable<Entity> GetAllEntities(bool includingPlayers = true)
        {
            var result = new List<Entity>();
            var ents = this.GetComponentsInChildren<Entity>();
            result.AddRange(ents);
            return result;
        }

        public void RegisterEntity(Entity entity)
        {
            if (!AllEntities.Contains(entity))
            {
                AllEntities.Add(entity);
            }
        }
        public void DeregisterEntity(Entity e)
        {
            AllEntities.Remove(e);
        }
        /*public GameObject SpawnEntity(Transform prefab, Vector2Int position)
        {
            var newEntity = GameObject.Instantiate(prefab, position.Vector3(EVectorComponents.XZ), Quaternion.Euler(60f, 0f, 0f)) as Transform;
            newEntity.transform.SetParent(this.transform);
            RegisterEntity(newEntity.GetComponent<Entity>());
            return newEntity.gameObject;
        }*/

        public List<Entity> GetEntitiesOnPosition(Vector2Int coords)
        {
            /*var result = new List<Entity>();
            foreach (var e in GetAllEntities())
            {
                if (e.GetTilePosition(true) == coords)
                {
                    result.Add(e);
                }
            }
            return result;
             */
            return EntitiesManager.FindEntitiesOnPosition(this.AllEntities, coords);
        }

        public List<Entity> GetEntities(System.Predicate<Entity> predicate)
        {
            var result = new List<Entity>();
            foreach (var e in GetAllEntities())
            {
                if (predicate(e))
                {
                    result.Add(e);
                }
            }
            return result;
        }

        public void PlanAllEntitiesAction()
        {
            foreach (var e in AllEntities)
            {
                e.PlanNextAction();
            }

            //var allEntities = EntitiesManager.gameObject.GetComponentsInChildren<GameLogicComponent>();
            //foreach (var entity in allEntities)
            //{
            //    ExecuteEvents.Execute<IGameLogicEntity>(entity.gameObject, null, (x, y) => x.GameTurnEnd());
            //}
        }

        public void ProcessAllEntitiesAction()
        {
            foreach (var e in AllEntities)
            {
                e.ProcessNextAction();
            }
        }

        public void EntityMoved(Entity entity, Vector2Int prevPosition, Vector2Int nextPosition)
        {
            foreach (var e in GetEntitiesOnPosition(prevPosition))
            {
                //e.OnEntityOut(entity);
                ExecuteEvents.Execute<IEntityTrigger>(e.gameObject, null, (o, p) => o.OnEntityOut(entity));
            }
            foreach (var e in GetEntitiesOnPosition(nextPosition))
            {
                if (e != entity)
                {
                    //e.OnEntityIn(entity);
                    ExecuteEvents.Execute<IEntityTrigger>(e.gameObject, null, (o, p) => o.OnEntityIn(entity));
                }
            }

        }

        public static List<Entity> FindEntitiesOnPosition(IEnumerable<Entity> ents, Vector2Int position)
        {
            var result = new List<Dungeon.Entity>();
            foreach (var e in ents)
            {
                if (e.GetTilePosition(true) == position)
                {
                    result.Add(e);
                }
            }
            return result;
        }

        // specific entities functions, dont know where to put them
        public Transform GoldPile;
        public Transform HealingPotion;
        public Transform TextMesh;
        public AudioClip GoldLootSound;
        public AudioClip PotionLootSound;


        public Entity SpawnGoldPile(Vector2Int coords, int amount)
        {
            var spawn = Instantiate(GoldPile, coords.Vector3(EVectorComponents.XZ), Quaternion.identity) as Transform;
            spawn.parent = this.transform;
            spawn.GetComponent<Pickable>().Item = new Gold();
            spawn.GetComponent<Pickable>().Amount = amount;
            return spawn.GetComponent<Entity>();
        }

        public Entity SpawnHealingPotion(Vector2Int coords)
        {
            var spawn = Instantiate(HealingPotion, coords.Vector3(EVectorComponents.XZ), Quaternion.identity) as Transform;
            spawn.parent = this.transform;
            spawn.GetComponent<Pickable>().Item = new SmallHealingPotion();
            spawn.GetComponent<Pickable>().Amount = 1f;
            return spawn.GetComponent<Entity>();
        }
    }

}