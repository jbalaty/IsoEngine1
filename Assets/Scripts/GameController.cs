using UnityEngine;
using System.Collections;

public class Tile
{
	public Transform GroundSprite;
	public Transform ObjectSprite;
	public bool WalkAble = true ;
}

[System.Serializable]
public class InputTile
{
	public Transform Prefab;
	public Vector2 Offset;
}

//[ExecuteInEditMode()]
public class GameController : MonoBehaviour
{

	public InputTile GroundTile1;
	public InputTile GroundTile2;
	public InputTile TreeTile1;
	public InputTile TreeTile2;
	public InputTile Townhall;
	public int sizeX;
	public int sizeY;
	private Tile[,] tiles;
	private bool FadeSpriteCoroutine;
	// Use this for initialization
	void Start ()
	{
		tiles = new Tile[sizeX, sizeY];
		for (var x=0; x<sizeX; x++) {
			for (var y=0; y<sizeY; y++) {
				var itile = (x + y) % 2 == 0 ? GroundTile1 : GroundTile2;
				var tile = new Tile ();
				tile.GroundSprite = Instantiate (itile.Prefab, transform.position 
					+ new Vector3 (x, 0, y), Quaternion.Euler (45f, 45f, 0f)) as Transform;
				tiles [x, y] = tile;
			}
		}

		// create townhall
		var thtile = tiles [sizeX / 2, sizeY / 2];
		thtile.ObjectSprite = Instantiate (this.Townhall.Prefab, transform.position 
			+ new Vector3 (sizeX / 2 + 1 + this.Townhall.Offset.x, 0, sizeY / 2 + 1 + this.Townhall.Offset.y),
		                                   Quaternion.Euler (45f, 45f, 0f)) as Transform;


		for (var i=0; i<80; i++) {
			var x = Random.Range (0, sizeX / 3) * 3;
			var y = Random.Range (0, sizeY / 3) * 3;
			var tile = tiles [x, y];
			if (tile.ObjectSprite == null && (new Vector2 (x, y) - new Vector2 (sizeX / 2, sizeY / 2)).magnitude > 2) {
				var itile = i % 2 == 0 ? TreeTile1 : TreeTile2;
				tile.ObjectSprite = Instantiate (itile.Prefab, transform.position 
					+ new Vector3 (x + 1 + itile.Offset.x, 0, y + 1 + itile.Offset.y), Quaternion.Euler (45f, 45f, 0f)) as Transform;
			}
		}

		// create Quad for detecting click events
		var quad = GameObject.CreatePrimitive (PrimitiveType.Quad);
		quad.transform.position = new Vector3 (sizeX / 2f, 0f, sizeY / 2f);
		quad.transform.Rotate (new Vector3 (90, 0, 0));
		quad.transform.localScale = new Vector3 (sizeX, sizeY, 0f);
		quad.GetComponent<MeshRenderer> ().enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		
				
	}

	public void HighlightTile (int x, int y)
	{
		var tile = this.tiles [x, y];
		//if (tile.ObjectSprite == null) {
		var sprite = tile.GroundSprite.GetComponent<SpriteRenderer> ();
		if (!this.FadeSpriteCoroutine) {
			StartCoroutine (FadeSpriteColor (tile.GroundSprite.GetComponent<SpriteRenderer> ()));
			//StartCoroutine (FadeSpriteColor (tile.ObjectSprite.GetComponent<SpriteRenderer> ()));
		} else {
			Debug.Log ("Cannot highlight tile, some other is in process");
		}
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
