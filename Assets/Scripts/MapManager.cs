using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Extensions;


public enum ETileLayer : int
{
    Ground0 = 0,
    Ground1,
    Ground2,
    Ground3,
    Ground4,
    Object0,
    Object1,
    Object2,
    Overlay,
    Object4,
}
public static class ETileLayerExtensions
{
    public static int Int(this ETileLayer e)
    {
        var r = (int)e;
        return r;
    }
}

public interface IPathfidningAdapter
{
    void Init(Vector2Int size);
    void SetTile(Vector2Int coords, bool isWalkable);
    Path FindPath(Vector2Int start, Vector2Int end);
}

public class GridObjectSprite : GridObject
{
    public Transform Sprite;
    public Transform Prefab;
    public Vector2 Offset;
    public Vector2Int Size;

    public GridObjectSprite(Transform prefab, Vector2Int size, Vector2 offset)
    {
        this.Prefab = prefab;
        this.Size = size;
        this.Offset = offset;
    }
    public override void OnSetup()
    {
        base.OnSetup();
        var position = this.OccupiedTiles[0, 0].Coordinates;
        this.Sprite = MapManager.InstantiatePrefab(Prefab, position.Vector3(EVectorComponents.XZ) + new Vector3(Offset.x, 0f, Offset.y));
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        GameObject.DestroyImmediate(this.Sprite.gameObject);
    }
}

public class GridObjectMultiSprite : GridObject
{
    public Transform[,] Sprites;
    public Transform Prefab;
    public Vector2 Offset;
    public Vector2Int Size;

    public GridObjectMultiSprite(Transform prefab, Vector2Int size, Vector2 offset)
    {
        this.Prefab = prefab;
        this.Size = size;
        this.Offset = offset;
        this.Sprites = new Transform[size.x, size.y];
    }
    public override void OnSetup()
    {
        base.OnSetup();
        var start = this.OccupiedTiles[0, 0].Coordinates;
        this.OccupiedTiles.ForEach(tile =>
        {
            var position = tile.Coordinates;
            this.Sprites[position.x - start.x, position.y - start.y] = MapManager.InstantiatePrefab(Prefab, position.Vector3(EVectorComponents.XZ)
                + new Vector3(Offset.x, 0f, Offset.y));
        });

    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        this.Sprites.ForEach(s =>
        {
            GameObject.DestroyImmediate(s.gameObject);
        });
    }
    public void SetColor(Color color)
    {
        this.Sprites.ForEach(s =>
        {
            s.GetComponent<SpriteRenderer>().color = color;
        });
    }
    public void Move(Vector2Int coords)
    {
        var start = this.OccupiedTiles[0, 0].Coordinates;
        this.OccupiedTiles.ForEach((vec, item) =>
        {
            var sprite = this.Sprites[vec.x, vec.y];
            sprite.position = (coords + vec).Vector3(EVectorComponents.XZ);
        });
    }
}

public class MapManager : TileGridManager
{
    public IPathfidningAdapter PathFinding;

    public MapManager(Vector2Int size, IPathfidningAdapter pathfinding)
        : base(size)
    {
        this.PathFinding = pathfinding;
        PathFinding.Init(size);
    }

    public static Transform InstantiatePrefab(Transform prefab, Vector3 position)
    {
        return GameObject.Instantiate(prefab, position, Quaternion.Euler(45f, 45f, 0f)) as Transform;
    }

    #region PATHFINDING

    public bool IsTileWalkable(Vector2Int coords)
    {
        if (CheckBounds(coords))
        {
            var tile = this.GetTile(coords);
            return IsTileWalkableTest(tile);
        }
        else
        {
            return false;
        }
    }

    protected bool IsTileWalkableTest(Tile tile)
    {
        return tile != null && tile.GridObjectReferences[ETileLayer.Object0.Int()] == null;
    }

    public Path FindPath(Vector2Int start, Vector2Int end)
    {

        return PathFinding.FindPath(start, end);
    }

    public override void OnTileAllocated(Tile tile, int layerIndex)
    {
        this.PathFinding.SetTile(tile.Coordinates, this.IsTileWalkableTest(tile));
    }
    #endregion


}
