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
        public AStarPathfinding astar;
        public DungeonMapManager MapManager;
        EntitiesManager EntitiesManager;
        public GameObject Player;
        public bool Shadows = false;
        float[,] CurrentLightModifiers;
        float[,] NextLightModifiers;

        GameObject PickingPlane;
        GameObject Map;

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
            astar = new AStarPathfinding();
            //MapManager = new DungeonMapManager(new Vector2Int(50, 50), astar);
            PickingPlane = GameObject.Find("PickingPlane");
            Map = GameObject.Find("Map");
            EntitiesManager = GameObject.Find("Entities").GetComponent<EntitiesManager>();
            var tiles = Map.GetComponentsInChildren<TileComponent>();
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
                        if (!tile.IsMovement) layer = ETileLayer.Object0;
                        MapManager.SetupObject(c, layer.Int(), gridobj, Vector2Int.One, false);
                    }
                    catch (Exception excp)
                    {
                        Debug.LogWarning("Duplicate tiles on coords: " + coords.x + "," + coords.z + " " + excp.Message);
                    }
                }
            }

        }
        // Use this for initialization
        void Start()
        {
            //Debug.Log("GameController start");
            EntitiesManager.SetupExistingEntities();
        }

        // Update is called once per frame
        void Update()
        {
            DebugHighlightNotMovementTiles(true);
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
        #region Light/Shadow procedures
        public void UpdateLight(Vector2 currentPosition)
        {
            //Debug.Log("Update Light");
            this.CurrentLightModifiers = ComputeLightModifiers(currentPosition);
            ApplyLightModifiers(this.CurrentLightModifiers);
        }
        public void StartLightBlending(Vector2Int nextPosition)
        {
            //Debug.Log("Start Light Blending");
            StopCoroutine("LightBlendingCouroutine");
            this.NextLightModifiers = ComputeLightModifiers(nextPosition.Vector2);
            //StartCoroutine(LightBlendingCouroutine(CharacterController.GetComponent<CharacterControllerScript>(),
            //    nextPosition));
            StartCoroutine("LightBlendingCouroutine", nextPosition);
        }
        IEnumerator LightBlendingCouroutine(Vector2Int nextPosition)
        {
            var character = Player.GetComponent<PlayerController>();
            //float[,] templm = new float[MapManager.SizeX, MapManager.SizeY];
            float[,] templm = new float[MapManager.SizeX, MapManager.SizeY];//this.CurrentLightModifiers.Clone() as float[,];
            Vector3 np = nextPosition.Vector3(EVectorComponents.XZ);
            float diff = (np - character.transform.position).magnitude;
            while (diff > 0.02f)
            {
                //if (this.CurrentLightModifiers != null && this.NextLightModifiers != null)
                {
                    for (var x = 0; x < templm.GetLength(0); x++)
                        for (var y = 0; y < templm.GetLength(1); y++)
                        //templm.ForEach((coords, val) =>
                        {
                            var from = this.CurrentLightModifiers[x, y];
                            var to = this.NextLightModifiers[x, y];
                            var lerp = Mathf.Lerp(from, to, 1f - diff);
                            //var lerp = Mathf.InverseLerp(from, to, 0.5f);
                            templm[x, y] = lerp;
                        };
                    this.CurrentLightModifiers = templm;
                    ApplyLightModifiers(templm);
                }
                diff = (np - character.transform.position).magnitude;
                yield return null;
            }
            this.NextLightModifiers = null;
        }
        public float[,] ComputeLightModifiers(Vector2 light)
        {
            float[,] result = new float[MapManager.SizeX, MapManager.SizeY];
            MapManager.ForEach((tile) =>
            {
                var m = (light - tile.Coordinates.Vector2).magnitude;
                var lightModifier = 1f;
                if (m < 12)
                {
                    var rayCastTiles = GetIntersectionTiles(new Vector2Int(light), tile.Coordinates);
                    lightModifier = ProcessIntersectionTiles(rayCastTiles) ? 4.6f : 0.8f;
                }
                else
                {
                    lightModifier = 1.8f;
                }
                //result[tile.Coordinates.x, tile.Coordinates.y] = lightModifier;
                //var v = Mathf.Sqrt(m) / 12f * lightModifier;
                var v = m / 36f * lightModifier;
                result[tile.Coordinates.x, tile.Coordinates.y] = v;
            });
            return result;
        }
        bool ProcessIntersectionTiles(List<Vector2Int> rayCastTiles)
        {
            bool hit = false;
            var counter = 0;
            foreach (var c in rayCastTiles)
            {
                var t = MapManager.GetTile(c);
                if (t != null)
                {
                    foreach (var go in t.GridObjectReferences)
                    {
                        if (go != null)
                        {
                            /*if (go.GameObject.tag == "Floor")
                            {
                                var sprites = go.GameObject.GetComponentsInChildren<SpriteRenderer>();
                                foreach (var sprite in sprites)
                                {
                                    sprite.color = hit ? Color.black: Color.white;
                                }
                            }*/
                            //lightModifiers[t.Coordinates.x, t.Coordinates.y] = hit ? 2f : 1f;
                            if (go.GameObject.tag == "Wall"
                                && counter < rayCastTiles.Count() - 1) // last tile cannot cast shadow on itself
                            {
                                hit = true;
                                return true;
                            }
                        }
                    }
                }
                counter++;
            }
            return hit;
        }
        List<Vector2Int> GetIntersectionTiles(Vector2Int start, Vector2Int end)
        {
            var result = new List<Vector2Int>();

            var x0 = start.x;
            var y0 = start.y;
            var x1 = end.x;
            var y1 = end.y;
            int dx = Mathf.Abs(x1 - x0);
            int dy = Mathf.Abs(y1 - y0);
            int x = x0;
            int y = y0;
            int n = 1 + dx + dy;
            int x_inc = (x1 > x0) ? 1 : -1;
            int y_inc = (y1 > y0) ? 1 : -1;
            int error = dx - dy;
            dx *= 2;
            dy *= 2;

            for (; n > 0; --n)
            {
                //visit(x, y);
                result.Add(new Vector2Int(x, y));
                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                }
            }

            return result;
        }
        void ApplyLightModifiers(float[,] lightModifiers)
        {
            if (this.Shadows)
            {
                MapManager.ForEach((tile) =>
                {
                    var lm = lightModifiers[tile.Coordinates.x, tile.Coordinates.y];
                    foreach (var go in tile.GridObjectReferences)
                    {
                        if (go != null)
                        {
                            var sprites = go.GameObject.GetComponentsInChildren<SpriteRenderer>();
                            foreach (var sprite in sprites)
                            {

                                sprite.color = Color.Lerp(Color.white, Color.black, lm);
                            }
                        }
                    }
                });

                // apply to entities too
                var entities = GameObject.FindGameObjectsWithTag("Entity");
                foreach (var entity in entities)
                {
                    var coord = new Vector2Int(entity.transform.position, EVectorComponents.XZ);
                    var lm = lightModifiers[coord.x, coord.y];
                    var sprites = entity.GetComponentsInChildren<SpriteRenderer>();
                    foreach (var sprite in sprites)
                    {
                        //lm = lm > 0.25f ? Mathf.Pow(lm, 1f / 3f) : lm;
                        lm = Mathf.Pow(lm, 0.7f);
                        sprite.color = Color.Lerp(Color.white, Color.black, lm);
                        // hide entity
                    }

                }
            }
        }
        #endregion

        public void NextTurn()
        {
            EntitiesManager.PlanAllEntitiesAction();
            EntitiesManager.ProcessAllEntitiesAction();
        }

        #region DEBUG FUNCTIONS
        public void DebugHighlightNotMovementTiles(bool highlight)
        {
            MapManager.ForEach((tile) =>
            {
                if (tile != null)
                {
                    var Movement = MapManager.IsTileMovement(tile.Coordinates);
                    var o = tile.GridObjectReferences[ETileLayer.Ground0.Int()];
                    if (o != null)
                    {
                        foreach (var s in o.GameObject.GetComponentsInChildren<SpriteRenderer>())
                        {
                            s.color = Movement ? Color.white : Color.red;
                        }
                    }
                }
            });
        }
        #endregion
    }

}
