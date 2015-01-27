using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using IsoEngine1;

/// <summary>
/// Info of entity trying to find path
/// </summary>
public class PathFinderInfo
{
    public bool FindWalkingPath = true;
    public bool FindFlyPath = false;
    public IEnumerable<Dungeon.Entity> CollisionEntities = null;

    public PathFinderInfo()
    {

    }
    public PathFinderInfo(IEnumerable<Dungeon.Entity> collisionEnts)
    {
        this.CollisionEntities = collisionEnts;
    }

    public List<Dungeon.Entity> FindEntitiesOnPosition(Vector2Int pos)
    {
        return Dungeon.EntitiesManager.FindEntitiesOnPosition(CollisionEntities, pos);
    }
}

/// <summary>
/// Info about node in graph
/// </summary>
public class MyPathNode : SettlersEngine.IPathNode<PathFinderInfo>
{
    static PathFinderInfo DefaultPathFinderInfo = new PathFinderInfo();
    public Int32 X;
    public Int32 Y;
    public bool NodeIsWalkable;
    public bool NodeIsFlyable;

    public bool IsWalkable(PathFinderInfo pathfinderinfo)
    {
        var result = true;
        pathfinderinfo = pathfinderinfo ?? DefaultPathFinderInfo;
        if (pathfinderinfo.FindWalkingPath && !NodeIsWalkable)
        {
            result = false;
        }
        else if (pathfinderinfo.FindFlyPath && !NodeIsFlyable)
        {
            result = false;
        }
        else if (pathfinderinfo.CollisionEntities != null)
        {
            var collEntsOnCurrentPosition = pathfinderinfo.FindEntitiesOnPosition(new Vector2Int(X, Y));
            result = collEntsOnCurrentPosition.FindAll((e) => e.IsWalkable).Count == collEntsOnCurrentPosition.Count;
        }
        return result;
    }
}

public class AStarPathfinding : IPathfidningAdapter
{

    public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode,
    TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
    {
        protected override Double Heuristic(PathNode inStart, PathNode inEnd)
        {
            int formula = 3;
            int dx = Math.Abs(inStart.X - inEnd.X);
            int dy = Math.Abs(inStart.Y - inEnd.Y);

            if (formula == 0)
                return Math.Sqrt(dx * dx + dy * dy); //Euclidean distance

            else if (formula == 1)
                return (dx * dx + dy * dy); //Euclidean distance squared

            else if (formula == 2)
                return Math.Min(dx, dy); //Diagonal distance

            else if (formula == 3)
                return (dx + dy); //Manhatten distance
            else
                throw new Exception("No pathfinding heuristic selected");
            //return 1*(Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y) - 1); //optimized tile based Manhatten
            //return ((dx * dx) + (dy * dy)); //Khawaja distance
        }

        protected override Double NeighborDistance(PathNode inStart, PathNode inEnd)
        {
            return Heuristic(inStart, inEnd);
        }

        public MySolver(TPathNode[,] inGrid)
            : base(inGrid)
        {
        }
    }

    MySolver<MyPathNode, PathFinderInfo> aStar;
    MyPathNode[,] grid;

    public AStarPathfinding()
    {
    }

    public void Init(Vector2Int size)
    {
        grid = new MyPathNode[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                grid[x, y] = new MyPathNode();
                grid[x, y].X = x;
                grid[x, y].Y = y;
            }
        }
        aStar = new MySolver<MyPathNode, PathFinderInfo>(grid);
    }

    public void SetTile(Vector2Int position, bool? isWalkable, bool? isFlyable)
    {
        var node = grid[position.x, position.y];
        node.NodeIsWalkable = isWalkable ?? node.NodeIsWalkable;
        node.NodeIsFlyable = isFlyable ?? node.NodeIsFlyable;
        if (typeof(MyPathNode).IsValueType)
        {
            grid[position.x, position.y] = node;
        }
        aStar = new MySolver<MyPathNode, PathFinderInfo>(grid);
    }

    public MyPathNode GetNode(Vector2Int position)
    {
        return grid[position.x, position.y];
    }

    public Path FindPath(Vector2Int start, Vector2Int end, PathFinderInfo pathFinderInfo = null)
    {
        pathFinderInfo = pathFinderInfo ?? new PathFinderInfo();
        var path = aStar.Search(start, end, pathFinderInfo);
        var result = new Path();
        if (path != null)
        {
            foreach (var node in path)
            {
                result.AddLast(new Vector2Int(node.X, node.Y));
            }
        }
        return result;
    }

    public bool IsTileWalkable(Vector2Int position, PathFinderInfo pfi)
    {
        return grid[position.x, position.y].IsWalkable(pfi);
    }
}
