using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using IsoEngine1;
using System.Linq;

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

        GameObject PickingPlane;
        GameObject Map;
        GameObject[,] tileObjects = new GameObject[100, 100];

        #region input handling vars
        /*GridObject LastMouseUpMapObject;
        GridObject LastMouseDownMapObject;
        private Vector3? MouseDownPoint;
        private EMouseHitType LastMouseDownHitType = EMouseHitType.Nothing;
        private float MouseMoveTreshold = 4f;
         */
        #endregion

        void Awake()
        {
            //Debug.Log("GameController awake");
            astar = new AStarPathfinding();
            PickingPlane = GameObject.Find("PickingPlane");
            Map = GameObject.Find("Map");
            var tiles = Map.GetComponentsInChildren<TileComponent>();
            foreach (var tile in tiles)
            {
                var coords = tile.transform.position;
                if (tileObjects[(int)coords.x, (int)coords.z] == null)
                {
                    tileObjects[(int)coords.x, (int)coords.z] = tile.gameObject;
                }
                else
                {
                    Debug.LogWarning("Duplicate tiles on coords: " + coords.x + "," + coords.z);
                }
            }
        }
        // Use this for initialization
        void Start()
        {
            //Debug.Log("GameController start");
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
            // MOUSE BUTTON UP
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Mouse Up");
                var coords = GetTilePositionFromMouse(Input.mousePosition);
            }

        }

        public Vector2Int? GetTilePositionFromMouse(Vector3 mousePosition)
        {
            RaycastHit hit;
            var ray = Camera.main.ScreenPointToRay(mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.gameObject == PickingPlane)
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
            //DebugHighlightNotWalkableTiles(true);
            //var obj = new GridObjectMultiSprite("SelectedTileIndicator", TileIndicator, Vector2.zero);
            //var tiles = MapManager.SetupObject(coords, ETileLayer.Overlay0.Int(), obj, new Vector2Int(3, 3));
            //var c = Color.green; c.a = .7f;
            //obj.SetColor(c);
            //CharacterController.SetTargetTile(coords);
        }

        #region DEBUG FUNCTIONS
        public void DebugHighlightNotWalkableTiles(bool highlight)
        {
        }
        #endregion
    }

}
