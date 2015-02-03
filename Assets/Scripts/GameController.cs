﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;
using System.Linq;
using System;

namespace Dungeon
{
    public enum EMouseHitType
    {
        Nothing, UI, GameMap, GameObject
    }

    [System.Serializable]
    public struct Dialogs
    {
        public GameObject ObjectsInventoryDialog;
    }

    //[ExecuteInEditMode()]
    public class GameController : MonoBehaviour
    {
        static GameController _Instance;
        public static GameController Instance
        {
            get {
                if (_Instance == null)
                {
                    _Instance = GameObject.FindObjectOfType<GameController>();
                }
                return _Instance; }
        }

        public DungeonMapManager MapManager;
        public EntitiesManager EntitiesManager;
        public GameObject MapObject;
        public GameObject Player;
        public Dialogs Dialogs;


        #region input handling vars
        /*GridObject LastMouseUpMapObject;
        GridObject LastMouseDownMapObject;
        private Vector3? MouseDownPoint;
        private EMouseHitType LastMouseDownHitType = EMouseHitType.Nothing;
        private float MouseMoveTreshold = 4f;
         */
        #endregion

        bool _SimulatingTurn = false;
        float _timeOfLastClick = 0f;
        public bool DebugShowNotWalkableTilesRed = false;

        void Awake()
        {
            //Debug.Log("GameController awake");
            //MapManager = new DungeonMapManager(new Vector2Int(50, 50), astar);
            EntitiesManager = GameObject.Find("Entities").GetComponent<EntitiesManager>();
            MapObject = GameObject.Find("Map");
            MapManager = MapObject.GetComponent<DungeonMapManager>();
        }
        // Use this for initialization
        void Start()
        {
            //Debug.Log("GameController start");
            var tiles = MapObject.GetComponentsInChildren<TileComponent>();
            foreach (var tile in tiles)
            {
                var coords = tile.transform.position;
                var c = new Vector2Int(coords.x, coords.z);
                if (tile.PrefabType == EPrefabType.Tile)
                {
                    var gridobj = new GridObject(tile.gameObject);
                    try
                    {
                        var layer = ETileLayer.Ground0;
                        if (!tile.IsWalkable) layer = ETileLayer.Ground1;
                        MapManager.SetupObject(c, layer.Int(), gridobj, Vector2Int.One, false);
                    }
                    catch (Exception excp)
                    {
                        Debug.LogWarning("Duplicate tiles on coords: " + coords.x + "," + coords.z);
                        Debug.LogException(excp);
                    }
                }
            }
#if !UNITY_EDITOR
            this.DebugHighlightNotWalkableTiles = false;
#endif
        }

        // Update is called once per frame
        void Update()
        {
            DebugHighlightNotWalkableTiles(true);
            //UpdateInputTouch();
            // if mouseDown is on GUI do nothing (GUI will handle that)
            // if mouseUp is on GUI do nothing
            // if mouseUp is really close to mouseDown, consider that a click
            // if mouseUp is farer, it is mouseUp after dragging
            // click selects some game object or nothing

            // MOUSE HANDLING
            // MOUSE BUTTON UP
            if (Input.GetMouseButtonUp(0))
            {
                if (!IsUIHit())
                {
                    var coords = GetTilePositionFromMouse(Input.mousePosition);
                    if (coords.HasValue && (Time.time - _timeOfLastClick) >= 0.5f)
                    {
                        Debug.Log("Mouse pick: " + coords);
                        var entities = EntitiesManager.GetEntitiesOnPosition(coords.Value);
                        // try to find combat entity
                        var someAction = false;
                        var endTurn = false;
                        foreach (var e in entities)
                        {
                            if (e.enabled && e.GetComponent<Combat>() != null)
                            {
                                Player.GetComponent<Player>().Attack(e);
                                someAction = true;
                                endTurn = true;
                            }
                            else if (e.enabled && e.GetComponent<ObjectComponent>() != null
                            && e.GetComponent<ObjectComponent>().CanInteract(Player.GetComponent<Entity>()))
                            {
                                e.GetComponent<ObjectComponent>().Interact(Player.GetComponent<Entity>());
                                someAction = true;
                            }
                            if (someAction) break;
                        }
                        if (!someAction && MapManager.IsTileWalkable(coords.Value
                            , new PathFinderInfo(EntitiesManager.AllEntities)))
                        {
                            Player.GetComponent<Player>().MoveTo(coords.Value);
                            HighlightTile(coords.Value);
                            endTurn = true;
                        }
                        if (endTurn) NextTurn();
                        _timeOfLastClick = Time.time;
                        // if not, move there
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && !_SimulatingTurn)
            {
                NextTurn();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                Player.GetComponent<Player>().ToggleInventoryDialog();
            }
        }

        public Vector2Int? GetTilePositionFromMouse(Vector3 mousePosition)
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1.0f);
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

        public void HighlightTile(Vector2Int coords)
        {
            //DebugHighlightNotMovementTiles(true);
            //var obj = new GridObjectMultiSprite("SelectedTileIndicator", TileIndicator, Vector2.zero);
            //var tiles = MapManager.SetupObject(coords, ETileLayer.Overlay0.Int(), obj, new Vector2Int(3, 3));
            //var c = Color.green; c.a = .7f;
            //obj.SetColor(c);
        }


        public void NextTurn()
        {
            EntitiesManager.PlanAllEntitiesAction();
            EntitiesManager.ProcessAllEntitiesAction();
        }

        #region DEBUG FUNCTIONS
        public void DebugHighlightNotWalkableTiles(bool highlight)
        {
            MapManager.ForEach((tile) =>
            {
                if (tile != null)
                {
                    var Movement = MapManager.IsTileWalkable(tile.Coordinates, new PathFinderInfo(EntitiesManager.AllEntities));
                    foreach (var gor in tile.GridObjectReferences)
                    {
                        if (gor != null)
                        {
                            foreach (var s in gor.GameObject.GetComponentsInChildren<SpriteRenderer>())
                            {
                                s.color = Movement || !DebugShowNotWalkableTilesRed ? Color.white : Color.red;
                            }
                        }
                    }
                }
            });
        }
        #endregion
    }

}
