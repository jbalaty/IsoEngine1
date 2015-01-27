using UnityEngine;
using System.Collections;
using System;

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

        public static IEnumerator WaitForSeconds(float secs, Action callback)
        {
            yield return new WaitForSeconds(secs);
            if (callback != null) callback();
        }

        public static void PlayClip(AudioClip aclip, Action callback = null)
        {
            var waitSecs = 0f;
            if (aclip != null)
            {
                waitSecs = aclip.length;
                AudioSource.PlayClipAtPoint(aclip, Vector3.zero);
            }
            WaitForSeconds(waitSecs, callback);
        }

        public static int DiagonalDistance(Vector2Int start, Vector2Int end)
        {
            var dx = Mathf.Abs(start.x - end.x);
            var dy = Mathf.Abs(start.y - end.y);
            return Mathf.Max(dx, dy);
        }
    }
}