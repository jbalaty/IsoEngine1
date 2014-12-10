using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;



[System.Serializable]
public class Tile
{
    public GridObject[] GridObjectReferences;

    public Tile(int maxObjects)
    {
        GridObjectReferences = new GridObject[maxObjects];
    }
    public Tile[] TileReferences = new Tile[4];
    public Vector2Int Coordinates;

    public GO GetObject<GO>(int layerIndex) where GO : GridObject
    {
        return this.GridObjectReferences[layerIndex] as GO;
    }
}


public class GridObject
{
    public enum EState
    {
        None, Active, Inactive, Destroyed
    }
    public int LayerIndex;
    public Tile[,] OccupiedTiles;
    public TileGridManager GridManager;
    public EState State { get; set; }

    public virtual void OnSetup()
    {
    }
    public virtual void OnDestroy()
    {
    }
    public void Destroy()
    {

    }
}

[System.Serializable]
public class TileGridManager
{
    public int SizeX = 0;
    public int SizeY = 0;
    public Vector2Int Size
    {
        get { return new Vector2Int(SizeX, SizeY); }
    }

    public Tile[,] tiles;
    public List<GridObject>[] ObjectsLayers = new List<GridObject>[2];
    int NumberOfObjectsPerTile = 10;

    //	public event ChangeGridHandler ChangeHandler;

    public TileGridManager(Vector2Int size)
    {
        Debug.Log("TileGridManager constructor");
        this.SizeX = (int)size.x;
        this.SizeY = (int)size.y;
        this.tiles = new Tile[this.SizeX, this.SizeY];
        for (var x = 0; x < this.SizeX; x++)
        {
            for (var y = 0; y < this.SizeY; y++)
            {
                tiles[x, y] = new Tile(NumberOfObjectsPerTile);
                tiles[x, y].Coordinates = new Vector2Int(x, y);
            }
        }

        // create Quad for detecting click events
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = new Vector3(this.SizeX / 2f, 0f, this.SizeY / 2f);
        quad.transform.Rotate(new Vector3(90, 0, 0));
        quad.transform.localScale = new Vector3(this.SizeX, this.SizeY, 0f);
        quad.GetComponent<MeshRenderer>().enabled = false;


    }

    public Tile GetTile(Vector2Int coords)
    {
        return this.tiles[(int)coords.x, (int)coords.y];
    }

    public GO GetObject<GO>(Vector2Int coords, int layerIndex) where GO : GridObject
    {
        var t = GetTile(coords);
        return (GO)t.GridObjectReferences[layerIndex];
    }

    public GridObject GetObject(Vector2Int coords, int layerIndex)
    {
        return GetObject<GridObject>(coords, layerIndex);
    }

    public GridObject SetupObject(Vector2Int coords, int layerIndex, GridObject obj, Vector2Int size)
    {
        // clean object at this coordinates and all other coordinates according to size
        DestroyObjects(coords, size, layerIndex);
        obj.GridManager = this;
        obj.LayerIndex = layerIndex;
        obj.State = GridObject.EState.Active;
        AllocateObject(coords, layerIndex, obj, size);
        obj.OnSetup();
        return obj;
    }

    public GridObject DestroyObject(Vector2Int coords, int layerIndex)
    {
        var obj = GetObject(coords, layerIndex);
        DestroyObject(obj);
        return obj;
    }
    public void DestroyObjects(Vector2Int coords, Vector2Int size, int layerIndex)
    {
        GenerateAllCoords(coords, size).ForEach(c => DestroyObject(c, layerIndex));
    }

    public GridObject DestroyObject(GridObject obj)
    {
        if (obj != null)
        {
            obj.OccupiedTiles.ForEach(tile =>
            {
                tile.GridObjectReferences[obj.LayerIndex] = null;
            });
            obj.OnDestroy();
            obj.State = GridObject.EState.Destroyed;
        }
        return obj;
    }

    public Tile[,] AllocateObject(Vector2Int coords, int layerIndex, GridObject obj, Vector2Int size)
    {
        var result = new Tile[size.x, size.y];
        // set prefab on first tile and set references on all other tiles
        var mainTile = GetTile(coords);
        obj.OccupiedTiles = new Tile[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var currcoods = coords + new Vector2Int(x, y);
                var tile = GetTile(currcoods);
                tile.GridObjectReferences[(int)layerIndex] = obj;
                obj.OccupiedTiles[x, y] = tile;
                result[x, y] = tile;
                OnTileAllocated(tile, layerIndex);
            }
        }
        return result;
    }

    public virtual void OnTileAllocated(Tile tile, int layerIndex)
    {
    }

    public void ForEach(System.Action<Tile> callback)
    {
        this.tiles.ForEach(callback);
    }

    public static Vector2Int[,] GenerateAllCoords(Vector2Int coords, Vector2Int size)
    {
        var result = new Vector2Int[size.x, size.y];
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                result[x, y] = new Vector2Int(coords.x + x, coords.y + y);
            }
        }
        return result;
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


}



