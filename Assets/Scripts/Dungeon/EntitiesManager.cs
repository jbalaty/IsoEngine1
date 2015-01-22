using UnityEngine;
using System.Collections;
using IsoEngine1;
using System.Collections.Generic;

namespace Dungeon
{
    public class EntitiesManager : MonoBehaviour
    {
        public GameObject Root;
        List<Entity> AllEntities = new List<Entity>();

        void Awake()
        {
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

        public void SetupExistingEntities()
        {
            AllEntities.Clear();
            foreach (var e in AllEntities)
            {
                RegisterEntity(e);

            }
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
            var result = new List<Entity>();
            foreach (var e in GetAllEntities())
            {
                if (new Vector2Int(e.transform.position, EVectorComponents.XZ) == coords)
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
    }
}