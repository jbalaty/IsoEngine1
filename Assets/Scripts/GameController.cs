using UnityEngine;
using System.Collections;
using Extensions;
using System.Linq;






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
    public MapManager MapManager;
    public int sizeX;
    public int sizeY;
    public InputTile GroundTile1;
    public InputTile GroundTile2;
    public InputTile TreeTile1;
    public InputTile TreeTile2;
    public InputTile Townhall;
    public Transform TileIndicator;

    GridObject LastSelectedMapObject;
    ETileLayer LastSelectedMapObjectLayer;
    AStarPathfinding astar;

    private bool FadeSpriteCoroutine;

    void Awake()
    {
        Debug.Log("GameController awake");
        astar = new AStarPathfinding();
        MapManager = new MapManager(new Vector2Int(sizeX, sizeY), astar);
        MapManager.ForEach((tile) =>
        {
            var v = tile.Coordinates;
            var itile = (v.x + v.y) % 2 == 0 ? GroundTile1 : GroundTile2;
            var obj = new GridObjectSprite(itile.Prefab, Vector2Int.One, Vector2.zero);
            MapManager.SetupObject(v, ETileLayer.Ground0.Int(), obj, obj.Size);
        });

        var treesLayer = ETileLayer.Object0.Int();
        for (var i = 0; i < 50; i++)
        {
            var x = Random.Range(0, (sizeX - 1) / 3) * 3;
            var y = Random.Range(0, (sizeY - 1) / 3) * 3;
            var tile = MapManager.GetTile(new Vector2Int(x, y));
            if (tile.GridObjectReferences[treesLayer] == null && (new Vector2(x, y) - new Vector2(sizeX / 2, sizeY / 2)).magnitude > 4)
            {
                var itile = i % 2 == 0 ? TreeTile1 : TreeTile2;
                var obj = new GridObjectSprite(itile.Prefab, new Vector2Int(itile.Size), itile.Offset);
                MapManager.SetupObject(new Vector2Int(x, y), treesLayer, obj, new Vector2Int(itile.Size));
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

    #region INPUT HANDLING
    public ELastMouseDownHit MouseDown(Vector2Int mousePosition)
    {
        var coords = GetTilePositionFromMouse(mousePosition);
        if (coords != null)
        {
            var tile = MapManager.GetTile(coords.Value);
            // select first object by topdown order Overlay->Objects(eg. Buldings)->...
            var layers = new ETileLayer[] { ETileLayer.Overlay, ETileLayer.Object0 };
            foreach (var l in layers)
            {
                this.LastSelectedMapObject = tile.GridObjectReferences[(int)l];
                this.LastSelectedMapObjectLayer = l;
                if (this.LastSelectedMapObject != null) return ELastMouseDownHit.GameObject;
            }
            return ELastMouseDownHit.GameMap;
        }
        return ELastMouseDownHit.Nothing;
    }

    public void MouseUpAfterMove(Vector2Int mousePosition)
    {
        if (this.LastSelectedMapObject != null)
        {

            (this.LastSelectedMapObject as GridObjectMultiSprite).SetColor(Color.cyan);
        }
        this.LastSelectedMapObject = null;
        this.LastSelectedMapObjectLayer = ETileLayer.Ground0;
    }

    public void MouseUp(Vector2Int mousePosition)
    {
        var coords = GetTilePositionFromMouse(mousePosition);
        if (coords != null)
        {
            if (this.LastSelectedMapObject != null && this.LastSelectedMapObjectLayer == ETileLayer.Overlay)
            {
                MapManager.DestroyObject(coords.Value, ETileLayer.Overlay.Int());
            }
            else
            {
                HighlightTile(coords.Value);

            }
            this.LastSelectedMapObject = null;
            this.LastSelectedMapObjectLayer = ETileLayer.Ground0;
        }
    }

    public void MouseMove(Vector2Int mousePosition, Vector2 delta)
    {
        if (this.LastSelectedMapObject != null && this.LastSelectedMapObjectLayer == ETileLayer.Overlay)
        {
            var obj = (this.LastSelectedMapObject as GridObjectMultiSprite);
            obj.SetColor(Color.yellow);
            var coords = GetTilePositionFromMouse(mousePosition);
            if (coords != null)
            {
                obj.Move(coords.Value);
            }
        }
    }

    public Vector2Int? GetTilePositionFromMouse(Vector2Int mousePosition)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            int x = Mathf.FloorToInt(hit.point.x);
            int y = Mathf.FloorToInt(hit.point.z);
            return new Vector2Int(x, y);
        }
        else return null;
    }
    #endregion
    public void HighlightTile(Vector2Int coords)
    {
        DebugHighlightNotWalkableTiles(true);
        var red = Color.red;
        red.a = .7f;
        var obj = new GridObjectMultiSprite(TileIndicator, new Vector2Int(3, 3), Vector2.zero);
        var tiles = MapManager.SetupObject(coords, ETileLayer.Overlay.Int(), obj, new Vector2Int(3, 3));
        obj.SetColor(red);
        CharacterController.SetTargetTile(coords);
    }

    public void NewBuilding()
    {
        // create townhall
        var mapCenter = new Vector2Int(sizeX / 2, sizeY / 2);
        var obj = new GridObjectSprite(Townhall.Prefab, new Vector2Int(Townhall.Size), Townhall.Offset);
        MapManager.SetupObject(mapCenter, ETileLayer.Object0.Int(), obj, new Vector2Int(Townhall.Size));
    }

    #region DEBUG FUNCTIONS
    public void DebugHighlightNotWalkableTiles(bool highlight)
    {

        MapManager.GenerateAllCoords(Vector2Int.Zero, MapManager.Size).ForEach(vec =>
        {
            if (!astar.GetNode(vec).IsWalkable(null))
            {
                var tile = MapManager.GetTile(vec);
                foreach (var o in tile.GridObjectReferences)
                {
                    if (o != null && (o as GridObjectSprite) != null && (o as GridObjectSprite).Sprite != null)
                    {
                        var sprite = ((GridObjectSprite)o).Sprite;
                        var color = sprite.GetComponent<SpriteRenderer>().color;
                        color.a = 0.5f;
                        sprite.GetComponent<SpriteRenderer>().color = color;
                    }
                }
            }

        });
    }
    #endregion
}
