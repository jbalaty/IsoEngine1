using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using IsoEngine1;

namespace Dungeon
{
    public class MapProxy : MonoBehaviour
    {
        DungeonMapManager MapManager;
        public Entity Entity;

        void Awake()
        {
            MapManager = GameObject.Find("Map").GetComponent<DungeonMapManager>();
            Entity = GetComponent<Entity>();
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GridObject SetupTile(ETileLayer layer)
        {
            return MapManager.SetupObject(Entity.GetTilePosition(), layer.Int(),
                new GridObject(this.gameObject), Vector2Int.One, false);
        }

        public bool IsTileWalkable(Vector2Int position, IEnumerable<Entity> collisionEntities = null)
        {
            return MapManager.IsTileWalkable(position, new PathFinderInfo(collisionEntities ?? Entity.EntitiesManager.AllEntities));
        }

        public Path FindPath(Vector2Int start, Vector2Int end, IEnumerable<Entity> collisionEntities = null)
        {
            return MapManager.FindPath(start, end, new PathFinderInfo(collisionEntities ?? Entity.EntitiesManager.AllEntities));
        }

        public Path FindPath(Vector2Int end, IEnumerable<Entity> collisionEntities = null)
        {
            return MapManager.FindPath(Entity.GetTilePosition(), end,
                 new PathFinderInfo(collisionEntities ?? Entity.EntitiesManager.AllEntities));
        }

        public void MoveObject(Vector2Int position)
        {
            // reserve next tile in map
            var thisGO = MapManager.GetObject(Entity.GetTilePosition(), Entity.MapLayer.Int());
            if (thisGO == null) throw new Exception(this.gameObject.name
                + " Alarm - there should be object on layer Object1 and position "
                + Entity.GetTilePosition() + " (" + this.name + ")");
            MapManager.MoveObject(thisGO, position);
        }

        public Vector2Int? GetRandomMovementTile(Vector2Int pos, Func<Vector2Int, bool> testCallback = null, int rectRadius = -1)
        {
            return MapManager.GetRandomMovementTile(pos, testCallback, rectRadius);
        }
    }
}