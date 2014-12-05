using UnityEngine;
using System.Collections;

[System.Serializable]
public class InputTile
{
	public Transform Prefab;
	public Vector2 Offset;
	public Vector2 Size = new Vector2 (1, 1);
}

//[ExecuteInEditMode()]
public class GameController : MonoBehaviour
{
	public int sizeX;
	public int sizeY;
	public InputTile GroundTile1;
	public InputTile GroundTile2;
	public InputTile TreeTile1;
	public InputTile TreeTile2;
	public InputTile Townhall;
	public TileGridManager tilesGrid;
	public IsoEngine1.CharacterController CharacterController;
	private bool FadeSpriteCoroutine;
	// Use this for initialization
	void Start ()
	{
		tilesGrid = new TileGridManager (new Vector2 (sizeX, sizeY));
		tilesGrid.ForEach ((v,tile) => {
			var itile = (v.x + v.y) % 2 == 0 ? GroundTile1 : GroundTile2;
			tile.GroundSprite0 = tilesGrid.InstantiatePrefab (itile.Prefab, transform.position + new Vector3 (v.x, 0, v.y));
		});

		// create townhall
		var mapCenter = new Vector2 (sizeX / 2, sizeY / 2);
		tilesGrid.SetupTile (mapCenter, ETileSprite.ObjectSprite0, Townhall.Prefab, Townhall.Size, Townhall.Offset);


		for (var i=0; i<80; i++) {
			var x = Random.Range (0, (sizeX - 1) / 3) * 3;
			var y = Random.Range (0, (sizeY - 1) / 3) * 3;
			var tile = tilesGrid.GetTile (new Vector2 (x, y));
			if (tile.ObjectSprite0 == null && (new Vector2 (x, y) - new Vector2 (sizeX / 2, sizeY / 2)).magnitude > 4) {
				var itile = i % 2 == 0 ? TreeTile1 : TreeTile2;
				tilesGrid.SetupTile (new Vector2 (x, y), ETileSprite.ObjectSprite0, itile.Prefab, itile.Size, itile.Offset);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		
				
	}

	public void HighlightTile (int x, int y)
	{
//		var tile = this.tiles [x, y];
//		//if (tile.ObjectSprite == null) {
//		var sprite = tile.GroundSprite.GetComponent<SpriteRenderer> ();
//		if (!this.FadeSpriteCoroutine) {
//			StartCoroutine (FadeSpriteColor (tile.GroundSprite.GetComponent<SpriteRenderer> ()));
//			//StartCoroutine (FadeSpriteColor (tile.ObjectSprite.GetComponent<SpriteRenderer> ()));
//		} else {
//			Debug.Log ("Cannot highlight tile, some other is in process");
//		}
		this.tilesGrid.DebugHighlightNotWalkableTiles (true);
		Debug.Log ("Setting targettile to " + new Vector2 (x, y));
		CharacterController.TargetTilePosition = new Vector2 (x, y);
	}

	public IEnumerator FadeSpriteColor (SpriteRenderer sprite)
	{
		this.FadeSpriteCoroutine = true;
		var oldcolor = sprite.color;
		var speed = 3f;
		sprite.color = Color.white;
		while (sprite.color != oldcolor) {
			sprite.color = Color.Lerp (sprite.color, oldcolor, Time.deltaTime * speed);
			if (Utils.ColorSize (sprite.color - oldcolor) < 0.1f)
				sprite.color = oldcolor;
			yield return null;
		}
		this.FadeSpriteCoroutine = false;
		//Debug.Log ("FadeSpriteColor couroutine end");
	}
}
