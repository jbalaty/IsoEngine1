using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;
using System.Linq;


namespace IsoEngine1
{
    [System.Serializable]
    public class Tile
    {
        public GridObject[] GridObjectReferences;

        public Tile(int maxObjects)
        {
            GridObjectReferences = new GridObject[maxObjects];
        }
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
        public int LayerIndex = -1;
        public Tile[,] OccupiedTiles;
        public TileGridManager GridManager;
        public EState State { get; set; }
        public string Name;

        public Vector2Int Size
        {
            get
            {
                return new Vector2Int(OccupiedTiles.GetLength(0), OccupiedTiles.GetLength(1));
            }
        }
        public Tile MainTile
        {
            get
            {
                return this.OccupiedTiles[0, 0];
            }
        }

        public virtual void OnSetup()
        {
        }
        public virtual void OnDestroy()
        {
        }
        public virtual void OnLayerChanged()
        {
        }
        public void Move(Vector2Int coords, int? newLayerIndex)
        {
            this.GridManager.MoveObject(this, coords, newLayerIndex);
            this.OnMoved();
        }
        public virtual void OnMoved()
        {
        }
    }

    [System.Serializable]
    public class CollisionHitInfo
    {
        public Vector2Int AbsoluteCoordinates;
        public Vector2Int RelativeCoordinates;
        public Tile Tile;
        public bool OutOfBounds = false;
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

        public List<GridObject> GetObjects(Vector2Int coords, Vector2Int size, int layerIndex)
        {
            var result = new List<GridObject>();
            GenerateAllCoords(coords, size).ForEach(c =>
            {
                var o = GetObject(c, layerIndex);
                if (o != null) result.Add(o);
            });
            return result.Distinct().ToList();
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
            if (obj != null && obj.State != GridObject.EState.Destroyed)
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

        public GridObject SetupObject(Vector2Int coords, int layerIndex, GridObject obj, Vector2Int size)
        {
            // clean object at this coordinates and all other coordinates according to size
            DestroyObjects(coords, size, layerIndex);
            obj.GridManager = this;
            obj.State = GridObject.EState.Active;
            AllocateObject(coords, layerIndex, obj, size);
            obj.OnSetup();
            obj.OnLayerChanged();
            return obj;
        }
        public Tile[,] AllocateObject(Vector2Int coords, int layerIndex, GridObject obj, Vector2Int size)
        {
            var result = new Tile[size.x, size.y];
            // set prefab on first tile and set references on all other tiles
            //var mainTile = GetTile(coords);
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
            obj.LayerIndex = layerIndex;
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

        public List<CollisionHitInfo> GetCollisionTiles(Vector2Int coords, Vector2Int size, int layerIndex, GridObject omitObject)
        {
            var result = new List<CollisionHitInfo>();
            GenerateAllCoords(coords, size).ForEach(c =>
            {
                var hit = new CollisionHitInfo();
                if (CheckBounds(c))
                {
                    var tile = GetTile(c);
                    var obj = tile.GridObjectReferences[layerIndex];
                    if (obj != null && (omitObject == null || omitObject != obj))
                    {
                        hit.Tile = tile;
                        hit.AbsoluteCoordinates = c;
                        hit.RelativeCoordinates = (c - coords);
                        result.Add(hit);
                    }
                }
                //else
                //{
                //    // add some info about bound overflow
                //    hit.OutOfBounds = true;
                //    result.Add(hit);
                //}
            });
            return result;
        }

        public Tile[,] MoveObject(GridObject obj, Vector2Int newCoords, int? newLayerIndex)
        {
            // asure the object stays in grid
            newCoords = RestrictToGrid(newCoords, obj.Size);
            var existingObjects = GetObjects(newCoords, obj.Size, newLayerIndex ?? obj.LayerIndex);
            foreach (var eo in existingObjects)
            {
                if (eo != null && obj != eo)
                {
                    DestroyObject(eo);
                }
            }
            obj.OccupiedTiles.ForEach(tile => tile.GridObjectReferences[obj.LayerIndex] = null);
            var oldLayerIndex = obj.LayerIndex;
            var result = AllocateObject(newCoords, newLayerIndex ?? obj.LayerIndex, obj, obj.Size);
            if (oldLayerIndex != obj.LayerIndex)
            {
                obj.OnLayerChanged();
            }
            return result;
        }

        public void MoveObjectToLayer(GridObject obj, int newLayerIndex)
        {
            if (obj.LayerIndex != newLayerIndex)
            {
                this.MoveObject(obj, obj.MainTile.Coordinates, newLayerIndex);
            }
        }

        public Vector2Int RestrictToGrid(Vector2Int coords, Vector2Int size)
        {
            var result = new Vector2Int();
            result.x = Mathf.Clamp(coords.x, 0, SizeX - size.x);
            result.y = Mathf.Clamp(coords.y, 0, SizeY - size.y);
            return result;
        }
    }



}