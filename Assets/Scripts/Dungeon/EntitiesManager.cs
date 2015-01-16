using UnityEngine;
using System.Collections;
using IsoEngine1;
using System.Collections.Generic;

namespace Dungeon
{
    public class EntitiesManager : BaseClass
    {
        public DungeonMapManager MapManager;
        public GameObject Root;
        new void Awake()
        {
            base.Awake();
            SetupExistingEntities();
        }

        // Use this for initialization
        void Start()
        {
            MapManager = GameController.MapManager;
            var entities = GetAllEntities(true);
            foreach (var e in entities)
            {
                MapManager.SetupObject(new Vector2Int(e.transform.position, EVectorComponents.XZ),
                    ETileLayer.Object1.Int(), new GridObject(e), Vector2Int.One);
            }
        }

        IEnumerable<GameObject> GetAllEntities(bool includingPlayers = true)
        {
            var result = new List<GameObject>();
            var ents = GameObject.FindGameObjectsWithTag("Entity");
            result.AddRange(ents);
            if (includingPlayers)
            {
                var players = GameObject.FindGameObjectsWithTag("Player");
                result.AddRange(players);
            }
            return result;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetupExistingEntities()
        {

        }

        public GameObject SpawnEntity(Transform prefab, Vector2Int position)
        {
            var newEntity = GameObject.Instantiate(prefab, position.Vector3(EVectorComponents.XZ), Quaternion.Euler(60f, 0f, 0f)) as Transform;
            newEntity.transform.SetParent(this.transform);
            MapManager.SetupObject(position, ETileLayer.Object1.Int(), new GridObject(newEntity.gameObject), Vector2Int.One);
            return newEntity.gameObject;
        }

        public List<GameObject> GetEntitiesOnPosition(Vector2Int coords)
        {
            var result = new List<GameObject>();
            foreach (var e in GetAllEntities())
            {
                if (new Vector2Int(e.transform.position, EVectorComponents.XZ) == coords)
                {
                    result.Add(e);
                }
            }
            return result;
        }

    }
}