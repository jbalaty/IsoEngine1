﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;
using System.Linq;


public enum EMouseHitType
{
    Nothing, UI, GameMap, GameObject
}

public class IngameUIElement
{
    public Transform IngameObject;
    public RectTransform UIObject;
    public Vector2Int UIOffset;
}

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
    public AStarPathfinding astar;
    public InputTile GroundTile1;
    public InputTile GroundTile2;
    public InputTile TreeTile1;
    public InputTile TreeTile2;
    public InputTile Townhall;
    public Transform TileIndicator;
    CameraController CameraController;

    #region ingame UI
    public RectTransform PanelOkCancel;
    public List<IngameUIElement> IngameUIElements = new List<IngameUIElement>();
    #endregion

    #region input handling vars
    GridObject LastMouseUpMapObject;
    GridObject LastMouseDownMapObject;
    private Vector3? MouseDownPoint;
    private EMouseHitType LastMouseDownHitType = EMouseHitType.Nothing;
    private float MouseMoveTreshold = 4f;
    #endregion

    void Awake()
    {
        Debug.Log("GameController awake");
        astar = new AStarPathfinding();
        var mapObject = GameObject.Find("Map").transform;
        MapManager = new MapManager(new Vector2Int(sizeX, sizeY), astar);
        MapManager.ForEach((tile) =>
        {
            var v = tile.Coordinates;
            var itile = (v.x + v.y) % 2 == 0 ? GroundTile1 : GroundTile2;
            var obj = new GridObjectSprite("Tile@" + v.x + "," + v.y, itile.Prefab, Vector2.zero);
            MapManager.SetupObject(v, ETileLayer.Ground0.Int(), obj, new Vector2Int(itile.Size));
            obj.Sprite.SetParent(mapObject, true);
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
                var obj = new GridObjectSpriteSDGameObject("Tree@" + x + "," + y, itile.Prefab,
                    itile.Offset, TileIndicator);
                MapManager.SetupObject(new Vector2Int(x, y), treesLayer, obj, new Vector2Int(itile.Size));
                obj.Sprite.SetParent(mapObject, true);
            }
        }

        ShowOkCancelSprites(new Vector2Int());
        CameraController = Camera.main.GetComponent<CameraController>();

    }
    // Use this for initialization
    void Start()
    {
        Debug.Log("GameController start");
        CameraController = Camera.main.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        //DebugHighlightNotWalkableTiles(true);

        //UpdateInputTouch();


        // if mouseDown is on GUI do nothing (GUI will handle that)
        // if mouseUp is on GUI do nothing
        // if mouseUp is really close to mouseDown, consider that a click
        // if mouseUp is farer, it is mouseUp after dragging
        // click selects some game object or nothing

        // MOUSE HANDLING
        // MOUSE BUTTON DOWN
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse down");
            this.LastMouseDownHitType = EMouseHitType.Nothing;
            // store position
            this.MouseDownPoint = Input.mousePosition;
            if (this.LastMouseDownHitType == EMouseHitType.UI || IsUIHit())
            {
                // do nothing
                this.LastMouseDownHitType = EMouseHitType.UI;
            }
            else
            {
                // set UI hit flag for next frame
                var coords = GetTilePositionFromMouse(Input.mousePosition);
                this.LastMouseDownMapObject = PickMapObject(coords, true);
            }
        }
        // MOUSE BUTTON UP
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Mouse Up");
            if (this.LastMouseDownHitType == EMouseHitType.UI || IsUIHit())
            {
                // do nothing
            }
            else
            {
                // if mouse down was near mouse up => its mouseClick, try to select some object
                if (this.MouseDownPoint == null || (this.MouseDownPoint.Value - Input.mousePosition).magnitude <= MouseMoveTreshold)
                {
                    var oldSelectedObject = this.LastMouseUpMapObject;
                    var coords = GetTilePositionFromMouse(Input.mousePosition);
                    this.LastMouseUpMapObject = PickMapObject(coords, true);
                    if (oldSelectedObject != null && oldSelectedObject != this.LastMouseUpMapObject && oldSelectedObject is ISelectable)
                    {
                        // deselect old object
                        (oldSelectedObject as ISelectable).OnDeselected();
                    }
                    if (this.LastMouseUpMapObject != null && this.LastMouseUpMapObject != oldSelectedObject) (this.LastMouseUpMapObject as ISelectable).OnSelected();
                    else if (coords.HasValue) HighlightTile(coords.Value);
                }
                else
                {
                    //if mouse up is far from mouseDown point, it was a drag move
                    // start dragging
                    if (this.LastMouseUpMapObject != null && this.LastMouseUpMapObject is IDraggable)
                    {
                        var draggable = this.LastMouseUpMapObject as IDraggable;
                        //var coords = GetTilePositionFromMouse(Input.mousePosition);
                        if (draggable.IsDragged)
                        {
                            draggable.IsDragged = false;
                            draggable.OnDragEnd();
                        }
                        //if (coords.HasValue) dragable.OnDragMove(coords.Value);
                    }
                }
            }
            this.MouseDownPoint = null;
        }
        // MOUSE MOVE
        if (Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0 || Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {

            // if mouse button is down it is either camera movement or drag
            if (Input.GetMouseButton(0))
            {
                // if the mouseDown was on another object than what is selected, scroll the map
                if (this.LastMouseDownHitType == EMouseHitType.UI)
                {
                    // do nothing
                }
                else if (this.LastMouseUpMapObject != this.LastMouseDownMapObject || this.LastMouseUpMapObject == null)
                {
                    if (Input.touchCount == 0)
                    {
                        CameraController.PanCamera(new Vector2(Input.GetAxis("Mouse X"),
                            Input.GetAxis("Mouse Y")) * -1);
                    }
                    else
                    {
                        var touch = Input.GetTouch(0);
                        CameraController.PanCamera(touch.deltaPosition * -1, 0.1f);
                    }
                }
                // if there was some mouseDown point and we are moving at least some pixels
                else if (this.MouseDownPoint == null || (this.MouseDownPoint.Value - Input.mousePosition).magnitude > MouseMoveTreshold)
                {
                    // start dragging
                    if (this.LastMouseUpMapObject != null && this.LastMouseUpMapObject is IDraggable)
                    {
                        var draggable = this.LastMouseUpMapObject as IDraggable;
                        var coords = GetTilePositionFromMouse(Input.mousePosition);
                        if (!draggable.IsDragged)
                        {
                            draggable.IsDragged = true;
                            draggable.OnDragStart(coords.Value);
                        }
                        if (coords.HasValue) draggable.OnDragMove(coords.Value);
                    }
                }
            }
        }
        // INPUT KEYS
        if (Input.GetKeyUp(KeyCode.Space))
        {
            DebugHighlightNotWalkableTiles(true);
        }

        foreach (var uie in IngameUIElements)
        {
            var v = uie.IngameObject.transform.position;
            v.y += 4f;
            var screenspacepos = Camera.main.WorldToScreenPoint(v);
            uie.UIObject.gameObject.SetActive(true);
            uie.UIObject.position = screenspacepos + uie.UIOffset.Vector3(EVectorComponents.XY) * CameraController.GetCameraScale();
            uie.UIObject.localScale = new Vector3(CameraController.GetCameraScale(), CameraController.GetCameraScale(), 1f);
        }

    }

    public void UpdateInputTouch()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
                Camera.main.GetComponent<CameraController>().PanCamera(touch.deltaPosition * -1, 0.05f);
        }
    }

    public Vector2Int? GetTilePositionFromMouse(Vector3 mousePosition)
    {
        RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.transform.gameObject == MapManager.GridColliderObject)
            {
                int x = Mathf.FloorToInt(hit.point.x);
                int y = Mathf.FloorToInt(hit.point.z);
                return new Vector2Int(x, y);
            }
        }
        return null;
    }
    bool IsUIHit()
    {
        PointerEventData pe = new PointerEventData(EventSystem.current);
        pe.position = Input.mousePosition;
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pe, hits);
        //if (hits.Count > 0) Debug.Log("UI Hit!!!");
        return hits.Count > 0;
    }
    public GridObject PickMapObject(Vector2Int? coords, bool selectableOnly)
    {
        GridObject result = null;
        if (coords.HasValue)
        {
            var tile = MapManager.GetTile(coords.Value);
            // select first object by topdown order Overlay->Objects(eg. Buldings)->...
            foreach (var obj in tile.GridObjectReferences.Reverse())
            {
                if (obj != null)
                {
                    if ((selectableOnly && obj is ISelectable) || !selectableOnly)
                    {
                        result = obj;
                        break;
                    }
                }
            }
        }
        return result;
    }

    public void NewBuilding()
    {
        // create townhall
        var mapCenter = new Vector2Int(sizeX / 2, sizeY / 2);
        var obj = new GridObjectSpriteSDGameObject("Townhall", Townhall.Prefab, Townhall.Offset, this.TileIndicator);
        MapManager.SetupObject(mapCenter, ETileLayer.Object0.Int(), obj, new Vector2Int(Townhall.Size));
        IngameUIElements.Add(new IngameUIElement { IngameObject = obj.Sprite.transform, UIObject = this.PanelOkCancel, UIOffset = new Vector2Int(0, 0) });
        //IngameUIElements.Add(new KeyValuePair<Transform, RectTransform>(obj.Sprite.transform, CancelButton));
    }

    public void ShowOkCancelSprites(Vector2Int coords)
    {
    }
    public void HighlightTile(Vector2Int coords)
    {
        //DebugHighlightNotWalkableTiles(true);
        //var obj = new GridObjectMultiSprite("SelectedTileIndicator", TileIndicator, Vector2.zero);
        //var tiles = MapManager.SetupObject(coords, ETileLayer.Overlay0.Int(), obj, new Vector2Int(3, 3));
        //var c = Color.green; c.a = .7f;
        //obj.SetColor(c);
        //CharacterController.SetTargetTile(coords);
        CharacterController.Wander();
    }

    #region DEBUG FUNCTIONS
    public void DebugHighlightNotWalkableTiles(bool highlight)
    {

        MapManager.GenerateAllCoords(Vector2Int.Zero, MapManager.Size).ForEach(vec =>
        {
            /*if (!astar.GetNode(vec).IsWalkable(null))
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
            }*/
            var tile = MapManager.GetTile(vec);
            var sr = (tile.GridObjectReferences[0] as GridObjectSprite).Sprite.GetComponent<SpriteRenderer>();
            if (tile.GridObjectReferences[ETileLayer.Object0.Int()] != null)
            {
                sr.color = Color.black;
            }
            else sr.color = Color.green;
        });
    }
    #endregion
}
