using UnityEngine;
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

    //[ExecuteInEditMode()]
    public class GameController : MonoBehaviour
    {
        public DungeonMapManager MapManager;
        public EntitiesManager EntitiesManager;
        public GameObject MapObject;
        public GameObject Player;



        #region input handling vars
        /*GridObject LastMouseUpMapObject;
        GridObject LastMouseDownMapObject;
        private Vector3? MouseDownPoint;
        private EMouseHitType LastMouseDownHitType = EMouseHitType.Nothing;
        private float MouseMoveTreshold = 4f;
         */
        #endregion

        bool _SimulatingTurn = false;

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
            EntitiesManager.SetupExistingEntities();
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
                var coords = GetTilePositionFromMouse(Input.mousePosition);
                if (coords.HasValue)
                {
                    //Debug.Log("Mouse pick: " + coords);
                    var entities = EntitiesManager.GetEntitiesOnPosition(coords.Value);
                    if (entities.Count == 0)
                    {
                        //Player.GetComponent<MovementComponent>().SetTargetTile(coords);
                        Player.GetComponent<PlayerController>().MoveTo(coords.Value);
                        HighlightTile(coords.Value);
                        NextTurn();
                    }
                    else
                    {
                        var e = entities.First();
                        if (Player.GetComponent<Combat>().CanAttack(e.GetComponent<Combat>()))
                        {
                            Player.GetComponent<Combat>().Attack(e.GetComponent<Combat>());
                            NextTurn();
                        }
                        else if (e.GetComponent<ObjectComponent>() != null
                            && e.GetComponent<ObjectComponent>().CanInteract(Player.GetComponent<Entity>()))
                        {
                            e.GetComponent<ObjectComponent>().Interact(Player.GetComponent<Entity>());
                            //NextTurn();
                        }
                        //Debug.Log("Attaaaack!!!! " + e.name);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space) && !_SimulatingTurn)
            {
                NextTurn();
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
                    var Movement = MapManager.IsTileWalkable(tile.Coordinates);
                    var ents = EntitiesManager.GetEntitiesOnPosition(tile.Coordinates);
                    foreach (var gor in tile.GridObjectReferences)
                    {
                        if (gor != null)
                        {
                            foreach (var s in gor.GameObject.GetComponentsInChildren<SpriteRenderer>())
                            {
                                s.color = Movement && ents.Count == 0 ? Color.white : Color.red;
                            }
                        }
                    }
                }
            });
        }
        #endregion
    }

}
