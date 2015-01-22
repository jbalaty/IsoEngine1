using UnityEngine;
using System.Collections;
using System;
using IsoEngine1;




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
                return (dx * dy) + (dx + dy); //Manhatten distance
            else
                return Math.Abs(inStart.X - inEnd.X) + Math.Abs(inStart.Y - inEnd.Y);

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

    MySolver<MyPathNode, System.Object> aStar;
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
        aStar = new MySolver<MyPathNode, System.Object>(grid);
    }

    public void SetTile(Vector2Int position, bool isMovement)
    {
        var node = new MyPathNode();
        node.X = position.x;
        node.Y = position.y;
        node.IsWall = !isMovement;
        grid[position.x, position.y] = node;
        aStar = new MySolver<MyPathNode, System.Object>(grid);
    }

    public MyPathNode GetNode(Vector2Int position)
    {
        return grid[position.x, position.y];
    }

    public Path FindPath(Vector2Int start, Vector2Int end)
    {
        var path = aStar.Search(start.Vector2, end.Vector2, null);
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

    public bool IsTileMovement(Vector2Int position)
    {
        return grid[position.x, position.y].IsMovement(null);
    }
}
