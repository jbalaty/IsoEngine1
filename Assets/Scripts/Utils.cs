using UnityEngine;
using System.Collections;

namespace IsoEngine1
{
    public class Utils
    {
        public static float ColorSize(Color c)
        {
            return c.r * c.r + c.g * c.g + c.b * c.b;
        }

        public static int? GetDirectionFromVector(Vector2Int vec)
        {
            int? result = null;
            if (vec.x < 0) result = 0;
            else if (vec.x > 0) result = 2;
            else if (vec.y < 0) result = 3;
            else if (vec.y > 0) result = 1;
            return result;
        }
    }
}