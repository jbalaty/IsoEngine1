using UnityEngine;
using System.Collections;
using System;

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

    public static IEnumerator WaitEndOfFrame(Action callback)
    {
        yield return new WaitForEndOfFrame();
        if (callback != null) callback();
    }

    public static IEnumerator WaitForFixedUpdate(Action callback)
    {
        yield return new WaitForFixedUpdate();
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

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();
        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }

    static public T FindComponentInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();
        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }

    public static T GetInterface<T>(GameObject go) where T : class
    {
        //var comps = go.GetComponents<MonoBehaviour>();
        //foreach (var c in comps)
        //{
        //    if (c is T)
        //    {
        //        return c as T;
        //    }
        //}
        //return null;
        return go.GetComponent(typeof(T)) as T;
    }

    public static Transform FindByNameInChildren(Transform target, string name, bool directChildrenOnly = false)
    {
        throw new NotImplementedException("Use Transform.Find() or FindChild()");
    }
}
