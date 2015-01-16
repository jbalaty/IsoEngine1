﻿using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    public class DungeonMapManager : MapManager
    {
        public DungeonMapManager(Vector2Int size, IPathfidningAdapter pathfinding)
            : base(size, pathfinding)
        {
        }
        protected override bool IsTileWalkableTest(IsoEngine1.Tile tile)
        {
            return tile != null
                && tile.GridObjectReferences[ETileLayer.Ground0.Int()] != null // floor is not null
                && tile.GridObjectReferences[ETileLayer.Object0.Int()] == null // wall is null
                && tile.GridObjectReferences[ETileLayer.Object1.Int()] == null; // entity is null
        }
    }
}