using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Grid2DObjectContainer<O>
{
    public Vector2Int MainTileCoordinates;
    public O ObjectReference;

    public Grid2DObjectContainer(O obj)
    {
        this.ObjectReference = obj;
    }
}

public class Grid2DItem<O>
{
    public Vector2Int Coordinates;
    public O[] ObjectReference = new O[10];

    public Grid2DItem()
    {
    }
}



[System.Serializable]
public class Grid2D<T, O>
    where T : Grid2DItem<O>, new()
    where O : class
{
    T[,] Tiles;
    public int SizeX { get { return Tiles.GetLength(0); } }
    public int SizeY { get { return Tiles.GetLength(1); } }
    List<Grid2DObjectContainer<O>>[] ObjectLayers = new List<Grid2DObjectContainer<O>>[10];

    public Grid2D(Vector2Int size)
    {
        Debug.Log("GenericGrid constructor");
        this.Tiles = new T[size.x, size.y];
        for (var x = 0; x < this.SizeX; x++)
        {
            for (var y = 0; y < this.SizeY; y++)
            {
                Tiles[x, y] = new T();
                Tiles[x, y].Coordinates = new Vector2Int(x, y);
            }
        }
    }

    public T GetItem(Vector2Int coords)
    {
        return this.Tiles[coords.x, coords.y];
    }

    public bool CheckBounds(Vector2Int coords)
    {
        if (0 <= coords.x && coords.x < SizeX &&
            0 <= coords.y && coords.y < SizeY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void AllocateTiles(Vector2Int coords, int objectReferenceIndex, O obj, Vector2Int size, Action<Grid2DItem<O>> callback)
    {
        if (!IsObjectInLayer(objectReferenceIndex, obj)) throw new ArgumentException("Object reference is not in any layer");
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var currcoods = coords + new Vector2Int(x, y);
                var item = GetItem(currcoods);
                item.ObjectReference[objectReferenceIndex] = obj;
                if (callback != null) callback(item);
            }
        }
    }

    public void AddObject(Vector2Int coords, int objectLayerIndex, O obj, Vector2Int size, Action<Grid2DItem<O>> callback)
    {
        if (obj == null) throw new NullReferenceException("Object cannot be null");
        var objcontainer = new Grid2DObjectContainer<O>(obj);
        this.ObjectLayers[objectLayerIndex].Add(objcontainer);
        //AllocateTiles()
    }
    public bool IsObjectInLayer(int objectLayerIndex, O obj)
    {
        if (obj == null) throw new NullReferenceException("Object cannot be null");
        return this.ObjectLayers[objectLayerIndex].Find((o) => o.Equals(obj)) != null;
    }
}
