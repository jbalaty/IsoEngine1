using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IsoEngine1
{
    public static class Extensions
    {
        public static void ForEach<T>(this T[,] grid, Action<T> callback)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    callback(grid[x, y]);
                }
            }
        }

        public static void ForEach<T>(this T[,] grid, Action<Vector2Int, T> callback)
        {
            for (var x = 0; x < grid.GetLength(0); x++)
            {
                for (var y = 0; y < grid.GetLength(1); y++)
                {
                    callback(new Vector2Int(x,y), grid[x, y]);
                }
            }
        }
    }
}
