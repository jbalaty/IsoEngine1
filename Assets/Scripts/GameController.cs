using UnityEngine;
using System.Collections;

public class Tile
{
	public Transform GroundSprite;
	public Transform ObjectSprite;
	public bool WalkAble = true ;
}

//[ExecuteInEditMode()]
public class GameController : MonoBehaviour
{

	public Transform prefab1;
	public Transform prefab2;
	public Transform tree;
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
				var prefab = (x + y) % 2 == 0 ? prefab1 : prefab2;
				var tile = new Tile ();
				tile.GroundSprite = Instantiate (prefab, transform.position 
				                                 + new Vector3 (x, 0, y), Quaternion.Euler (45f, 45f, 0f)) as Transform;
				tiles [x, y] = tile;
			}
		}


		for (var i=0; i<100; i++) {
			var x = Random.Range (0, sizeX);
			var y = Random.Range (0, sizeY);
			var tile = tiles [x, y];
			if (tile.ObjectSprite == null) {
				tile.ObjectSprite = Instantiate (tree, transform.position 
					+ new Vector3 (x + 0.5f, // +1 for tree sprite, it scaled
			               0, y + 0.5f), Quaternion.Euler (45f, 45f, 0f)) as Transform;
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
