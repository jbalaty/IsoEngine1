using UnityEngine;
using System.Collections;

[System.Serializable]
public class InputTile
{
    public Transform Prefab;
    public Vector2 Offset;
    public Vector2 Size = new Vector2(1, 1);
}

//[ExecuteInEditMode()]
public class GameController : MonoBehaviour
{
    public UIManager UIManager;
    public IsoEngine1.CharacterController CharacterController;
    public TileGridManager tilesGrid;
    public int sizeX;
    public int sizeY;
    public InputTile GroundTile1;
    public InputTile GroundTile2;
    public InputTile TreeTile1;
    public InputTile TreeTile2;
    public InputTile Townhall;
    
    private bool FadeSpriteCoroutine;

    void Awake()
    {
        Debug.Log("GameController awake");
        tilesGrid = new TileGridManager(new Vector2Int(sizeX, sizeY));
        tilesGrid.ForEach((v, tile) =>
        {
            var itile = (v.x + v.y) % 2 == 0 ? GroundTile1 : GroundTile2;
            tile.GroundSprite0 = tilesGrid.InstantiatePrefab(itile.Prefab, transform.position + new Vector3(v.x, 0, v.y));
        });

        // create townhall
        //var mapCenter = new Vector2Int (sizeX / 2, sizeY / 2);
        //tilesGrid.SetupTile (mapCenter, ETileSprite.ObjectSprite0, Townhall.Prefab, new Vector2Int(Townhall.Size), Townhall.Offset);


        for (var i = 0; i < 50; i++)
        {
            var x = Random.Range(0, (sizeX - 1) / 3) * 3;
            var y = Random.Range(0, (sizeY - 1) / 3) * 3;
            var tile = tilesGrid.GetTile(new Vector2Int(x, y));
            if (tile.ObjectSprite0 == null && (new Vector2(x, y) - new Vector2(sizeX / 2, sizeY / 2)).magnitude > 4)
            {
                var itile = i % 2 == 0 ? TreeTile1 : TreeTile2;
                tilesGrid.SetupTile(new Vector2Int(x, y), ETileSprite.ObjectSprite0, itile.Prefab, new Vector2Int(itile.Size), itile.Offset);
            }
        }
    }
    // Use this for initialization
    void Start()
    {
        Debug.Log("GameController start");

    }

    // Update is called once per frame
    void Update()
    {


    }

    public void HighlightTile(int x, int y)
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
        if (!UIManager.IsShopPanelVisible)
        {
            this.tilesGrid.DebugHighlightNotWalkableTiles(true);
            CharacterController.SetTargetTile(new Vector2Int(x, y));
        }
    }

    public IEnumerator FadeSpriteColor(SpriteRenderer sprite)
    {
        this.FadeSpriteCoroutine = true;
        var oldcolor = sprite.color;
        var speed = 3f;
        sprite.color = Color.white;
        while (sprite.color != oldcolor)
        {
            sprite.color = Color.Lerp(sprite.color, oldcolor, Time.deltaTime * speed);
            if (Utils.ColorSize(sprite.color - oldcolor) < 0.1f)
                sprite.color = oldcolor;
            yield return null;
        }
        this.FadeSpriteCoroutine = false;
        //Debug.Log ("FadeSpriteColor couroutine end");
    }

    public void NewBuilding()
    {
        // create townhall
        var mapCenter = new Vector2Int(sizeX / 2, sizeY / 2);
        tilesGrid.SetupTile(mapCenter, ETileSprite.ObjectSprite0, Townhall.Prefab, new Vector2Int(Townhall.Size), Townhall.Offset);
    }
}
