using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IsoEngine1;
using IsoEngine1.Components;


public enum ETileLayer : int
{
    Ground0 = 0,
    Ground1,
    Ground2,
    Ground3,
    Object0,
    Object1,
    Object2,
    Object3,
    Overlay0,
    Overlay1,
    Overlay2,
    Overlay3,
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

#region Grid object interfaces
public interface ISelectable
{
    void OnSelected();
    void OnDeselected();
}

public interface IDraggable
{
    void OnDragStart();
    void OnDragMove(Vector2Int newCoords);
    void OnDragEnd();
    bool IsDragged { get; set; }
}
#endregion
public class GridObjectSprite : GridObject
{
    public Transform Sprite;
    public Transform Prefab;
    public Vector2 Offset;

    public GridObjectSprite(string name, Transform prefab, Vector2 offset)
    {
        this.Prefab = prefab;
        this.Offset = offset;
        this.Name = name;
    }
    public override void OnSetup()
    {
        base.OnSetup();
        var position = this.OccupiedTiles[0, 0].Coordinates;
        this.Sprite = MapManager.InstantiatePrefab(Prefab, position.Vector3(EVectorComponents.XZ) + new Vector3(Offset.x, 0f, Offset.y));
        this.Sprite.name = this.Name;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        GameObject.DestroyImmediate(this.Sprite.gameObject);
    }

    public override void OnMoved()
    {
        base.OnMoved();
        var sprite = this.Sprite;
        sprite.position = (this.MainTile.Coordinates).Vector3(EVectorComponents.XZ) + new Vector3(this.Offset.x, 0f, this.Offset.y);
    }
    public Color SetColor(Color color)
    {
        var o = this.Sprite.GetComponent<SpriteRenderer>().color;
        this.Sprite.GetComponent<SpriteRenderer>().color = color;
        return o;
    }
    public override void OnLayerChanged()
    {
        base.OnLayerChanged();
        this.Sprite.GetComponent<SpriteRenderer>().sortingLayerID = this.LayerIndex;
    }
}

public class GridObjectMultiSprite : GridObject
{
    public Transform[,] Sprites;
    public Transform Prefab;
    public Vector2 Offset;

    public GridObjectMultiSprite(string name, Transform prefab, Vector2 offset)
    {
        this.Prefab = prefab;
        this.Offset = offset;
        this.Name = name;
    }
    public override void OnSetup()
    {
        base.OnSetup();
        var start = this.OccupiedTiles[0, 0].Coordinates;
        this.Sprites = new Transform[Size.x, Size.y];
        this.OccupiedTiles.ForEach(tile =>
        {
            var position = tile.Coordinates;
            this.Sprites[position.x - start.x, position.y - start.y] = MapManager.InstantiatePrefab(Prefab, position.Vector3(EVectorComponents.XZ)
                + new Vector3(Offset.x, 0f, Offset.y));
            this.Sprites[position.x - start.x, position.y - start.y].name = this.Name + "(multisprite[" + (position.x - start.x) + "," + (position.y - start.y) + "])";
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
    public override void OnMoved()
    {
        base.OnMoved();
        var start = this.MainTile.Coordinates;
        this.OccupiedTiles.ForEach((vec, tile) =>
        {
            var sprite = this.Sprites[vec.x, vec.y];
            sprite.position = (tile.Coordinates).Vector3(EVectorComponents.XZ);
        });
    }

    public override void OnLayerChanged()
    {
        base.OnLayerChanged();
        this.Sprites.ForEach(s =>
        {
            s.GetComponent<SpriteRenderer>().sortingLayerID = this.LayerIndex;
        });
    }

    public Color SetColor(Color color)
    {
        Color result = new Color();
        this.Sprites.ForEach(s =>
        {
            result = s.GetComponent<SpriteRenderer>().color;
            s.GetComponent<SpriteRenderer>().color = color;
        });
        return result;
    }
    public void SetColor(Vector2Int relativeCoords, Color color)
    {
        var s = this.Sprites[relativeCoords.x, relativeCoords.y];
        s.GetComponent<SpriteRenderer>().color = color;
    }
}

public class GridObjectSpriteSDGameObject : GridObjectSprite, ISelectable, IDraggable
{
    Color OldColor;
    DraggableComponent DraggableComponent;

    public GridObjectSpriteSDGameObject(string name, Transform prefab, Vector2 offset, Transform tileindicatorprefab)
        : base(name, prefab, offset)
    {
        DraggableComponent = new DraggableComponent(this, tileindicatorprefab);
    }

    #region ISelectable
    public void OnSelected()
    {
        Debug.Log("Sprite selected - " + this.Name);
        this.OldColor = SetColor(Color.green);
    }

    public void OnDeselected()
    {
        Debug.Log("Sprite deselected - " + this.Name);
        SetColor(this.OldColor);
    }
    #endregion

    #region IDraggable
    public bool IsDragged
    {
        get { return DraggableComponent.IsDragged; }
        set { DraggableComponent.IsDragged = value; }
    }

    public void OnDragStart()
    {
        DraggableComponent.OnDragStart();
    }

    public void OnDragMove(Vector2Int newCoords)
    {
        DraggableComponent.OnDragMove(newCoords);
    }

    public void OnDragEnd()
    {
        DraggableComponent.OnDragEnd();
    }
    #endregion
}

[Obsolete("Not used yet, maybe it will be better to use MultiSprite class for everything", true)]
public class GridObjectMultiSpriteSDGameObject : GridObjectMultiSprite, ISelectable, IDraggable
{
    Color OldColor;
    DraggableComponent DraggableComponent;

    public GridObjectMultiSpriteSDGameObject(string name, Transform prefab, Vector2 offset, Transform tileindicatorprefab)
        : base(name, prefab, offset)
    {
        DraggableComponent = new DraggableComponent(this, tileindicatorprefab);
    }

    #region ISelectable
    public void OnSelected()
    {
        Debug.Log("Sprite selected - " + this.Name);
        this.OldColor = SetColor(Color.green);
    }

    public void OnDeselected()
    {
        Debug.Log("Sprite deselected - " + this.Name);
        SetColor(this.OldColor);
    }
    #endregion

    #region IDraggable
    public bool IsDragged
    {
        get { return DraggableComponent.IsDragged; }
        set { DraggableComponent.IsDragged = value; }
    }

    public void OnDragStart()
    {
        DraggableComponent.OnDragStart();
    }

    public void OnDragMove(Vector2Int newCoords)
    {
        DraggableComponent.OnDragMove(newCoords);
    }

    public void OnDragEnd()
    {
        DraggableComponent.OnDragEnd();
    }
    #endregion
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
