using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(WalkableComponent))]
    public class CharacterControllerScript : BaseClass
    {
        Animator[] AnimControllers;
        WalkableComponent Walkable;
        int _lastDirection = 3;
        bool _lastIsIdle = true;
        // Use this for initialization
        void Start()
        {
            base.Start();
            //Debug.Log("CharacterController start");
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Walkable = this.GetComponent<WalkableComponent>();
            Walkable.StateChange += Walkable_StateChange;
            Walkable.DirectionChange += Walkable_DirectionChange;
            Walkable.StartingNextMove += Walkable_StartingNextMove;
            GameController.UpdateLight(new Vector2(this.transform.position.x, this.transform.position.z));
        }

        void Walkable_DirectionChange(Vector2Int direction)
        {
            var newDirection = GetDirectionFromVector(direction) ?? _lastDirection;
            SetDirection(newDirection);
            _lastDirection = newDirection;
        }
        void Walkable_StartingNextMove(Vector2Int nextPosition)
        {
            GameController.StartLightBlending(nextPosition);
        }
        private void Walkable_StateChange(bool isidle)
        {
            if (this._lastIsIdle != isidle)
            {
                SetIsIdle(isidle);
            }
            this._lastIsIdle = isidle;
            //GameController.UpdateLight(new Vector2(this.transform.position.x, this.transform.position.z));
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
            /*var newDirection = _lastDirection;
            var newIsIdle = true;
            if (!this.IsMoving && currentPath != null)
            {
                if (currentPath.IsEmpty())
                {
                    SetIsIdle(true);
                    if (this.TargetTile != null && this.GetTilePosition() != TargetTile)
                    {
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
                        //						Debug.Log ("Moving to next position " + nextposition);
                        newDirection = GetDirectionFromVector(nextposition - new Vector2Int(this.transform.position, EVectorComponents.XZ)) ?? newDirection;
                        //SetIsIdle(false);
                        StartCoroutine(MoveToPosition(nextposition.Vector3(EVectorComponents.XZ), () =>
                        {
                            if (currentPath.IsEmpty())
                            {
                                // choose next point to go
                                //Wander();
                                SetIsIdle(true);
                            }
                        }));
                    }
                    else // if tile is not walkable
                    {
                        //StartCoroutine(JumpJump(.5f));
                        currentPath = null;
                        SetIsIdle(true);
                    }
                }
                SetDirection(newDirection);
                _lastDirection = newDirection;
            }*/
        }

        int? GetDirectionFromVector(Vector2Int vec)
        {
            int? result = null;
            if (vec.x < 0) result = 0;
            else if (vec.x > 0) result = 2;
            else if (vec.y < 0) result = 3;
            else if (vec.y > 0) result = 1;
            return result;
        }
        void SetDirection(int dir)
        {
            foreach (Animator ac in AnimControllers)
            {
                ac.SetInteger("Direction", dir);
            }
        }

        void SetIsIdle(bool isIdle)
        {
            foreach (Animator ac in AnimControllers)
            {
                ac.SetBool("IsIdle", isIdle);
            }
        }

        public Vector2Int GetTilePosition()
        {
            return new Vector2Int(transform.position, EVectorComponents.XZ);
        }
    }
}
