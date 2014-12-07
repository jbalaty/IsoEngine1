using UnityEngine;
using System.Collections;

public enum ETileSprite
{
	GroundSprite0 = 0,
	GroundSprite1,
	ObjectSprite0,
	ObjectSprite1
}

public enum EVectorComponents
{
	X,
	Y,
	Z,
	XY,
	XZ
}

public struct Vector2Int
{
	public int x;
	public int y;
	
	public Vector2Int (int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2Int (Vector2 v)
	{
		this.x = (int)v.x;
		this.y = (int)v.y;
	}

	public Vector2Int (Vector3 v, EVectorComponents evc)
	{
		if (evc == EVectorComponents.XY) {
			this.x = (int)v.x;
			this.y = (int)v.y;
		} else if (evc == EVectorComponents.XZ) {
			this.x = (int)v.x;
			this.y = (int)v.z;
		} else
			throw new UnityException ("Cannot convert Vector3 to Vector2 according to components specification " + evc);
	}

	public override bool Equals (object obj)
	{
		return base.Equals (obj);
	}

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public static bool operator == (Vector2Int lhs, Vector2Int rhs)
	{
		return lhs.x == rhs.x && lhs.y == rhs.y;
	}

	public static bool operator != (Vector2Int lhs, Vector2Int rhs)
	{
		return lhs.x != rhs.x || lhs.y != rhs.y;
	}

	public static Vector2Int operator + (Vector2Int lhs, Vector2Int rhs)
	{
		return new Vector2Int (lhs.x + rhs.x, lhs.y + rhs.y);
	}

	public static Vector2Int operator - (Vector2Int lhs, Vector2Int rhs)
	{
		return new Vector2Int (lhs.x - rhs.x, lhs.y - rhs.y);
	}

	public float magnitude{
		get {
			return this.Vector2.magnitude;
		}
	}

	public Vector2 normalized {
		get{
			return this.Vector2.normalized;
		}
	}

	public Vector2 Vector2 { 
		get {
			return new Vector2 (this.x, this.y);

		} 
	}

	public Vector3 Vector3 (EVectorComponents evc)
	{
		if (evc == EVectorComponents.XY) {
			return new Vector3 (this.x, this.y, 0f);
		} else if (evc == EVectorComponents.XZ) {
			return new Vector3 (this.x, 0f, this.y);
		} else throw new UnityException ("Cannot convert to Vector3 according to components specification " + evc);
	}
};

[System.Serializable]
public class Tile
{
	public Transform[] sprites = new Transform[4];

	public Transform GroundSprite0 {
		get { return sprites [(int)ETileSprite.GroundSprite0];}
		set { sprites [(int)ETileSprite.GroundSprite0] = value;}
	}

	public Transform GroundSprite1 {
		get { return sprites [(int)ETileSprite.GroundSprite1];}
		set { sprites [(int)ETileSprite.GroundSprite1] = value;}
		
	}

	public Transform ObjectSprite0 {
		get { return sprites [(int)ETileSprite.ObjectSprite0];}
		set { sprites [(int)ETileSprite.ObjectSprite0] = value;}
		
	}

	public Transform ObjectSprite1 {
		get { return sprites [(int)ETileSprite.ObjectSprite1];}
		set { sprites [(int)ETileSprite.ObjectSprite1] = value;}
		
	}

	public Tile ObjectTile0Reference;
	public Tile ObjectTile1Reference;
	public Vector2 coords;

	public Transform GetSprite (ETileSprite sprite)
	{
		return this.sprites [(int)sprite];
	}


}

[System.Serializable]
public class TileGridManager
{

	public int sizeX = 0;
	public int sizeY = 0;
	public Tile[,] tiles;

	public TileGridManager (Vector2Int size)
	{
		this.sizeX = (int)size.x;
		this.sizeY = (int)size.y;
		this.tiles = new Tile[this.sizeX, this.sizeY];
		for (var x=0; x<this.sizeX; x++) {
			for (var y=0; y<this.sizeY; y++) {
				tiles [x, y] = new Tile ();
				tiles [x, y].coords = new Vector2 (x, y);
			}
		}

		// create Quad for detecting click events
		var quad = GameObject.CreatePrimitive (PrimitiveType.Quad);
		quad.transform.position = new Vector3 (this.sizeX / 2f, 0f, this.sizeY / 2f);
		quad.transform.Rotate (new Vector3 (90, 0, 0));
		quad.transform.localScale = new Vector3 (this.sizeX, this.sizeY, 0f);
		quad.GetComponent<MeshRenderer> ().enabled = false;
	}

	public Tile GetTile (Vector2Int coords)
	{
		return this.tiles [(int)coords.x, (int)coords.y];
	}

	public Tile SetupTile (Vector2Int coords, ETileSprite etsprite, Transform prefab, Vector2Int size, Vector2 offset)
	{
		var tile = GetTile (coords);
		var sprite = InstantiatePrefab (prefab, new Vector3 (coords.x + offset.x, 0, coords.y + offset.y));
		AllocateMultiTileObject (coords, etsprite, sprite, size);
		return tile;
	}

	public void ForEach (System.Action<Vector2Int,Tile> callback)
	{
		for (var x=0; x<this.sizeX; x++) {
			for (var y=0; y<this.sizeY; y++) {
				callback (new Vector2Int (x, y), this.tiles [x, y]);
			}
		}
	}

	public Transform InstantiatePrefab (Transform prefab, Vector3 position)
	{
		return GameObject.Instantiate (prefab, position, Quaternion.Euler (45f, 45f, 0f)) as Transform;
	}

	public void AllocateMultiTileObject (Vector2Int coords, ETileSprite ets, Transform sprite, Vector2Int size)
	{
		// set prefab on first tile and set references on all other tiles
		var mainTile = GetTile (coords);
		mainTile.ObjectSprite0 = sprite;
		for (var x=0; x<size.x; x++) {
			for (var y=0; y<size.y; y++) {
				var tile = GetTile (coords + new Vector2Int (x, y));
				tile.ObjectTile0Reference = mainTile;
			}
		}
	}

	public short[,] GetPathfindingView ()
	{
		return null;
	}

	public bool GetIsWalkable (Vector2Int coords)
	{
		if (CheckBounds (coords)) {
			var tile = this.GetTile (coords);
			return IsTileWalkable (tile);
		} else {
			return false;
		}
	}

	bool IsTileWalkable (Tile tile)
	{
		return tile.ObjectSprite0 == null && tile.ObjectSprite1 == null
			&& tile.ObjectTile0Reference == null && tile.ObjectTile1Reference == null;
	}

	public bool CheckBounds (Vector2Int coords)
	{
		if (0 <= coords.x && coords.x < sizeX && 
			0 <= coords.y && coords.y < sizeY) {
			return true;
		} else {
			return false;
		}
	}

	public void DebugHighlightNotWalkableTiles (bool highlight)
	{
		this.ForEach ((v,tile) => {
			if (!IsTileWalkable (tile)) {
				foreach (var sprite in tile.sprites) {
					if (sprite != null) {
						var color = sprite.GetComponent<SpriteRenderer> ().color;
						color.a = 0.5f;
						sprite.GetComponent<SpriteRenderer> ().color = color;
					}
				}
			}
		});
	}
}
