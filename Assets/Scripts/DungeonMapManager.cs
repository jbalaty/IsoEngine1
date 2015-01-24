using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{

    public class DungeonMapManager : TileGridManager
    {
        //public AStarPathfinding astar;
        public IPathfidningAdapter PathFinding;

        void Awake()
        {
            base.Init();
            PathFinding = new AStarPathfinding();
            PathFinding.Init(this.Size);
        }

        protected bool IsTileWalkableTest(IsoEngine1.Tile tile)
        {
            //bool objectsWalkable = true;
            //for (var i = ETileLayer.Object0.Int(); i < tile.GridObjectReferences.Length; i++)
            //{
            //    var gor = tile.GridObjectReferences[i];
            //    if(gor)
            //}
            return tile != null
                && tile.GridObjectReferences[ETileLayer.Ground0.Int()] != null // floor is not null
                && tile.GridObjectReferences[ETileLayer.Ground1.Int()] == null // wall is null
                && tile.GridObjectReferences[ETileLayer.Object0.Int()] == null; //
        }

        //protected virtual bool IsTileWalkableTest(Tile tile)
        //{
        //    return tile != null && tile.GridObjectReferences[ETileLayer.Object0.Int()] == null;
        //}

        #region PATHFINDING

        public bool IsTileWalkable(Vector2Int coords)
        {
            if (CheckBounds(coords))
            {
                //var tile = this.GetTile(coords);
                //return IsTileWalkableTest(tile);
                return PathFinding.IsTileWalkable(coords);
            }
            else
            {
                return false;
            }
        }



        public Path FindPath(Vector2Int start, Vector2Int end, PathFinderInfo pfi = null)
        {
            return PathFinding.FindPath(start, end, pfi);
        }

        public override void OnTileAllocated(Tile tile, int layerIndex)
        {
            this.PathFinding.SetTile(tile.Coordinates, this.IsTileWalkableTest(tile), null);
        }
        public override void OnTileDeallocated(Tile tile, int layerIndex)
        {
            this.PathFinding.SetTile(tile.Coordinates, this.IsTileWalkableTest(tile), null);
        }

        public Vector2Int? GetRandomMovementTile(Vector2Int current, int rectangularRadius = -1)
        {
            Vector2Int? result = null;
            var triesCounter = 0;
            while (result == null && triesCounter++ < SizeX * SizeY)
            {
                Vector2Int coords;
                if (rectangularRadius < 0)
                {
                    coords = GetRandomTileCoords();
                }
                else
                {
                    coords = GetRandomTileCoords(new Rect(current.x - rectangularRadius, current.y - rectangularRadius,
                        2 * rectangularRadius, 2 * rectangularRadius));
                }
                if (CheckBounds(coords))
                {
                    var rndtile = GetTile(coords);
                    if (IsTileWalkableTest(rndtile))
                    {
                        result = rndtile.Coordinates;
                    }
                }
            }
            return result;
        }
        #endregion
    }
}