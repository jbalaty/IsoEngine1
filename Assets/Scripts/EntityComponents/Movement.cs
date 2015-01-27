using UnityEngine;
using System.Collections;
using IsoEngine1;
using System;
using System.Collections.Generic;

namespace Dungeon
{
    public class Movement : MonoBehaviour
    {
        public event Action<bool> StateChange;
        public event Action<Vector2Int> DirectionChange;
        public event Action<Vector2Int> MoveStart;
        public event Action<Vector2Int> MoveEnd;
        Vector2Int? TargetTile;
        public Vector2Int PreviousPosition;
        public Vector2Int NextPosition;
        Path currentPath;
        bool _isMoving = false;
        public float speed = 0.5f;
        public bool MovingDone { get { return !_isMoving && (currentPath == null || currentPath.Count == 0); } }
        public MapProxy MapProxy { get { return Entity.MapProxy; } }
        public Vector2Int StartPosition;
        public Entity Entity;
        // Use this for initialization
        void Awake()
        {
            Entity = GetComponent<Entity>();
        }

        void Start()
        {
            NextPosition = StartPosition = GetTilePosition();
        }

        public void DoNextStep()
        {
            if (!this._isMoving && currentPath != null)
            {
                if (currentPath.IsEmpty())
                {
                    if (this.TargetTile != null && this.GetTilePosition() != TargetTile)
                    {
                        if (StateChange != null) StateChange(true);
                        currentPath = null;
                    }
                }
                else // if current path is not empty
                {
                    // follow the path, but check everytime if the tile is really empty
                    var nextposition = currentPath.PopFirst();
                    DebugShowPath(nextposition);
                    if (currentPath != null && MapProxy.IsTileWalkable(nextposition))
                    {
                        //start movement to next tile
                        //MapProxy.MoveObject(nextposition);
                        //Debug.Log ("Moving to next position " + nextposition);
                        if (DirectionChange != null) DirectionChange(nextposition - new Vector2Int(this.transform.position, EVectorComponents.XZ));
                        if (MoveStart != null) MoveStart(nextposition);
                        if (StateChange != null) StateChange(false);
                        PreviousPosition = GetTilePosition();
                        NextPosition = nextposition;
                        StartCoroutine(MoveToPosition(nextposition.Vector3(EVectorComponents.XZ), () =>
                        {
                            if (MoveEnd != null) MoveEnd(new Vector2Int(this.transform.position, EVectorComponents.XZ));
                            if (StateChange != null) StateChange(true);
                            if (currentPath.IsEmpty())
                            {
                                //if (StateChange != null) StateChange(true);
                            }
                            Entity.EntitiesManager.EntityMoved(Entity, PreviousPosition, NextPosition);
                        }));
                    }
                    else // if tile is not Movement
                    {
                        currentPath = null;
                    }
                }
            }
        }

        public IEnumerator MoveToPosition(Vector3 nexttcoords, System.Action onFinished)
        {
            this._isMoving = true;
            var lastDiff = float.MaxValue;
            var diff = (nexttcoords - transform.position);
            // if current diff is bigger than the last one, stop moving
            while (diff.magnitude < lastDiff && diff.magnitude > 0.01f)
            {
                var m = (diff.normalized * speed * Time.deltaTime);
                transform.Translate(m, Space.World);
                lastDiff = diff.magnitude;
                diff = (nexttcoords - transform.position);
                yield return new WaitForFixedUpdate();
            }
            transform.position = nexttcoords;
            if (onFinished != null) onFinished();
            this._isMoving = false;
        }

        public void SetTargetTile(Vector2Int? targettile, IEnumerable<Entity> collisionEntities = null)
        {
            this.TargetTile = targettile;
            currentPath = MapProxy.FindPath(targettile.Value, collisionEntities);
            if (currentPath != null && currentPath.Count > 0) currentPath.PopFirst();
        }
        public Vector2Int GetTilePosition()
        {
            return new Vector2Int(transform.position, EVectorComponents.XZ);
        }
        public void Wander()
        {
            var rndcoords = MapProxy.GetRandomMovementTile(GetTilePosition());
            if (rndcoords.HasValue)
            {
                SetTargetTile(rndcoords);
            }
        }

        public void DebugShowPath(Vector2Int nextposition)
        {
            // DEBUG CODE
            var prev = nextposition;
            foreach (var n in currentPath)
            {
                Debug.DrawLine(prev.Vector3(EVectorComponents.XZ) + new Vector3(.5f, 0f, .5f), n.Vector3(EVectorComponents.XZ) + new Vector3(.5f, 0f, .5f), Color.yellow, 0.2f);
                prev = n;
            }
            // DEBUG END
        }
    }
}


/*var currentDirection = AnimControllers[0].GetInteger("Direction");
            var currentIsIdle = AnimControllers[0].GetBool("IsIdle");
            var newDirection = _lastDirection;
            var newIsIdle = true;
            var translation = new Vector2();
            var mapManager = GameController.MapManager;
            var currentPosition = new Vector2Int(this.transform.position, EVectorComponents.XZ);

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                newDirection = 0;
                newIsIdle = false;
                translation = Vector2.right * -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                newDirection = 2;
                newIsIdle = false;
                translation = Vector2.right;
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                newDirection = 1;
                newIsIdle = false;
                translation = Vector2.up;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                newDirection = 3;
                newIsIdle = false;
                translation = Vector2.up * -1;
            }

            var v = new Vector3(translation.x, 0f, translation.y) * speed * Time.deltaTime;
            var newPosition = this.transform.position + new Vector3(translation.x, 0f, translation.y) / 2f;
            if (mapManager.IsTileMovement(new Vector2Int(newPosition, EVectorComponents.XZ)))
            {
                this.transform.position += v;
            }
            SetDirection(newDirection);
            SetIsIdle(newIsIdle);
            _lastDirection = newDirection;
            //Debug.Log("Axis values: " + horizontal + " | " + vertical + " (speed: " + speed + ", dir: " + direction + ")");
             */