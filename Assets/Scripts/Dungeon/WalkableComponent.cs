using UnityEngine;
using System.Collections;
using IsoEngine1;
using System;

namespace Dungeon
{
    public class WalkableComponent : BaseClass
    {
        public event Action<bool> StateChange;
        public event Action<Vector2Int> DirectionChange;
        public event Action<Vector2Int> StartingNextMove;
        GameController GameController;
        IPathFinding PathFinding;
        Vector2Int? TargetTile;
        Path currentPath;
        bool IsMoving = false;
        public float speed = 0.5f;
        int _lastDirection = 3;
        // Use this for initialization
        void Start()
        {
            Debug.Log("CharacterController start");
            GameController = GameObject.Find("GameController").GetComponent<GameController>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
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
            if (mapManager.IsTileWalkable(new Vector2Int(newPosition, EVectorComponents.XZ)))
            {
                this.transform.position += v;
            }
            SetDirection(newDirection);
            SetIsIdle(newIsIdle);
            _lastDirection = newDirection;
            //Debug.Log("Axis values: " + horizontal + " | " + vertical + " (speed: " + speed + ", dir: " + direction + ")");
             */
            if (!this.IsMoving && currentPath != null)
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
                    // DEBUG CODE
                    var prev = nextposition;
                    foreach (var n in currentPath)
                    {
                        Debug.DrawLine(prev.Vector3(EVectorComponents.XZ) + new Vector3(.5f, 0f, .5f), n.Vector3(EVectorComponents.XZ) + new Vector3(.5f, 0f, .5f), Color.yellow, 0.2f);
                        prev = n;
                    }
                    // DEBUG END
                    if (currentPath != null && GameController.MapManager.IsTileWalkable(nextposition))
                    {
                        //start movement to next tile
                        //Debug.Log ("Moving to next position " + nextposition);
                        if (DirectionChange != null) DirectionChange(nextposition - new Vector2Int(this.transform.position, EVectorComponents.XZ));
                        if (StartingNextMove != null) StartingNextMove(nextposition);
                        if (StateChange != null) StateChange(false);
                        StartCoroutine(MoveToPosition(nextposition.Vector3(EVectorComponents.XZ), () =>
                        {
                            if (currentPath.IsEmpty())
                            {
                                // choose next point to go
                                //Wander();
                                if (StateChange != null) StateChange(true);
                            }
                        }));
                    }
                    else // if tile is not walkable
                    {
                        //StartCoroutine(JumpJump(.5f));
                        currentPath = null;
                    }
                }
            }
        }

        public IEnumerator MoveToPosition(Vector3 nexttcoords, System.Action onFinished)
        {
            this.IsMoving = true;
            var lastDiff = float.MaxValue;
            var diff = (nexttcoords - transform.position);
            //while (diff.magnitude > .01f && diff.magnitude <= 1f)
            // if current diff is bigger than the last one, stop moving
            while (diff.magnitude < lastDiff && diff.magnitude > 0.01f)
            {
                var m = (diff.normalized * speed * Time.deltaTime);
                transform.Translate(m, Space.World);
                lastDiff = diff.magnitude;
                diff = (nexttcoords - transform.position);
                yield return null;
            }
            transform.position = nexttcoords;
            if (onFinished != null) onFinished();
            this.IsMoving = false;
        }

        public void SetTargetTile(Vector2Int? targettile)
        {
            this.TargetTile = targettile;
            currentPath = GameController.MapManager.FindPath(new Vector2Int(this.transform.position, EVectorComponents.XZ), targettile.Value);
        }
        public Vector2Int GetTilePosition()
        {
            return new Vector2Int(transform.position, EVectorComponents.XZ);
        }
        public void Wander()
        {
            var rndcoords = GameController.MapManager.GetRandomWalkableTile(GetTilePosition());
            if (rndcoords.HasValue)
            {
                SetTargetTile(rndcoords);
            }
        }
    }
}