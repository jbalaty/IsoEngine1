using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;

public enum ETileSprite
{
    GroundSprite0 = 0,
    GroundSprite1,
    ObjectSprite0,
    ObjectSprite1,
}

public interface IPathfidningAdapter
{
    void Init(Vector2Int size);
    void SetTile(Vector2Int position, Tile tile);
    Path FindPath(Vector2Int start, Vector2Int end);
}



[System.Serializable]
public class Tile
{
    public Transform[] Sprites = new Transform[4];

    public Transform GroundSprite0
    {
        get { return Sprites[(int)ETileSprite.GroundSprite0]; }
        set { Sprites[(int)ETileSprite.GroundSprite0] = value; }
    }

    public Transform GroundSprite1
    {
        get { return Sprites[(int)ETileSprite.GroundSprite1]; }
        set { Sprites[(int)ETileSprite.GroundSprite1] = value; }

    }

    public Transform ObjectSprite0
    {
        get { return Sprites[(int)ETileSprite.ObjectSprite0]; }
        set { Sprites[(int)ETileSprite.ObjectSprite0] = value; }

    }

    public Transform ObjectSprite1
    {
        get { return Sprites[(int)ETileSprite.ObjectSprite1]; }
        set { Sprites[(int)ETileSprite.ObjectSprite1] = value; }

    }

    public Tile[] TileReferences = new Tile[4];
    public Vector2Int Coordinates;

    public Transform GetSprite(ETileSprite sprite)
    {
        return this.Sprites[(int)sprite];
    }
}



[System.Serializable]
public class TileGridManager
{
    public int sizeX = 0;
    public int sizeY = 0;
    public Tile[,] tiles;
    public AStarPathfinding PathFinding;

    //	public event ChangeGridHandler ChangeHandler;

    public TileGridManager(Vector2Int size)
    {
        Debug.Log("TileGridManager constructor");
        this.sizeX = (int)size.x;
        this.sizeY = (int)size.y;
        this.tiles = new Tile[this.sizeX, this.sizeY];
        for (var x = 0; x < this.sizeX; x++)
        {
            for (var y = 0; y < this.sizeY; y++)
            {
                tiles[x, y] = new Tile();
                tiles[x, y].Coordinates = new Vector2Int(x, y);
            }
        }

        // create Quad for detecting click events
        var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.position = new Vector3(this.sizeX / 2f, 0f, this.sizeY / 2f);
        quad.transform.Rotate(new Vector3(90, 0, 0));
        quad.transform.localScale = new Vector3(this.sizeX, this.sizeY, 0f);
        quad.GetComponent<MeshRenderer>().enabled = false;

        PathFinding = new AStarPathfinding();
        PathFinding.Init(size);
        //		this.ForEach((vec,tile)=> {
        //			PathFinding.SetTile(vec,tile);
        //		});
    }

    public Tile GetTile(Vector2Int coords)
    {
        return this.tiles[(int)coords.x, (int)coords.y];
    }

    public Tile SetupTile(Vector2Int coords, ETileSprite ets, Transform prefab, Vector2Int size, Vector2 offset, bool notWalkable)
    {
        CleanTileObjects(coords);
        var tile = GetTile(coords);
        var sprite = InstantiatePrefab(prefab, new Vector3(coords.x + offset.x, 0, coords.y + offset.y));
        tile.Sprites[(int)ets] = sprite;
        AllocateMultiTileObject(coords, ets, size, notWalkable);
        return tile;
    }

    public Tile[,] SetupTiles(Vector2Int coords, ETileSprite ets, Transform prefab, Vector2Int size, bool notWalkable)
    {
        CleanTiles(coords, ets,size);
        var mainTile = GetTile(coords);
        var tiles = AllocateMultiTileObject(coords, ets, size, notWalkable);
        tiles.ForEach(tile =>
        {
            var sprite = InstantiatePrefab(prefab, tile.Coordinates.Vector3(EVectorComponents.XZ)) as Transform;
            tile.Sprites[(int)ets] = sprite;
        });
        return tiles;
    }

    public Tile[,] AllocateMultiTileObject(Vector2Int coords, ETileSprite ets, Vector2Int size, bool notWalkable)
    {
        var result = new Tile[size.x, size.y];
        // set prefab on first tile and set references on all other tiles
        var mainTile = GetTile(coords);
        for (var x = 0; x < size.x; x++)
        {
            for (var y = 0; y < size.y; y++)
            {
                var currcoods = coords + new Vector2Int(x, y);
                var tile = GetTile(currcoods);
                tile.TileReferences[(int)ets] = mainTile;
                result[x, y] = tile;
                if (notWalkable) PathFinding.SetTile(currcoods, tile);
            }
        }
        return result;
    }

    public void CleanTile(Vector2Int coords, ETileSprite ets)
    {
        var tile = GetTile(coords);
        var reftile = tile.TileReferences[(int)ets];
        if (reftile != null && reftile != tile)
        {
            // use recursion, but tiles should not be referenced in chains
            CleanTile(reftile.Coordinates, ets);
        }
        else
        {
            this.tiles.ForEach(t =>
            {
                if (t.TileReferences[(int)ets] != null && t.TileReferences[(int)ets].Equals(tile))
                {
                    if (t.Sprites[(int)ets] != null)
                    {
                        GameObject.DestroyImmediate(t.Sprites[(int)ets].gameObject);
                    }
                }
            });
        }
    }

    public void CleanTiles(Vector2Int coords, ETileSprite ets, Vector2Int size)
    {
        var allcoords = GenerateAllCoords(coords, size);
        allcoords.ForEach(c =>
        {
            CleanTile(c, ets);
        });
    }

    public void CleanTileGround(Vector2Int coords)
    {
        CleanTile(coords, ETileSprite.GroundSprite0);
        CleanTile(coords, ETileSprite.GroundSprite1);
    }
    public void CleanTileObjects(Vector2Int coords)
    {
        CleanTile(coords, ETileSprite.ObjectSprite0);
        CleanTile(coords, ETileSprite.ObjectSprite1);
    }

    public void ForEach(System.Action<Tile> callback)
    {
        this.tiles.ForEach(callback);
    }

    public Transform InstantiatePrefab(Transform prefab, Vector3 position)
    {
        return GameObject.Instantiate(prefab, position, Quaternion.Euler(45f, 45f, 0f)) as Transform;
    }



    public short[,] GetPathfindingView()
    {
        return null;
    }

    public bool GetIsWalkable(Vector2Int coords)
    {
        if (CheckBounds(coords))
        {
            var tile = this.GetTile(coords);
            return TileGridManager.IsTileWalkable(tile);
        }
        else
        {
            return false;
        }
    }

    public static bool IsTileWalkable(Tile tile)
    {
        return tile.ObjectSprite0 == null &&
            tile.TileReferences[(int)ETileSprite.ObjectSprite0] == null;
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
        if (0 <= coords.x && coords.x < sizeX &&
            0 <= coords.y && coords.y < sizeY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public Path FindPath(Vector2Int start, Vector2Int end)
    {

        return PathFinding.FindPath(start, end);
    }

    #region DEBUG FUNCTIONS
    public void DebugHighlightNotWalkableTiles(bool highlight)
    {
        PathFinding.DebugHighlightNotWalkableTiles(this, highlight);
        //		this.ForEach ((v,tile) => {
        //			if (!IsTileWalkable (tile)) {
        //				foreach (var sprite in tile.sprites) {
        //					if (sprite != null) {
        //						var color = sprite.GetComponent<SpriteRenderer> ().color;
        //						color.a = 0.5f;
        //						sprite.GetComponent<SpriteRenderer> ().color = color;
        //					}
        //				}
        //			}
        //		});
    }
    #endregion
}
