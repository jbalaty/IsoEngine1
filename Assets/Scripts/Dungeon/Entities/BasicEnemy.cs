using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    public class BasicEnemy : Entity
    {
        Animator[] AnimControllers;
        protected GameController GameController;
        protected Movement Movement;
        public Combat Combat;
        [Range(0, 1)]
        public float WalkStandRatio = 0.7f;

        int _lastDirection = 3;
        bool _lastIsIdle = true;


        void Awake()
        {
            GameController = GameObject.Find("GameController").GetComponent<GameController>();
            Movement = this.GetComponent<Movement>();
            Combat = this.GetComponent<Combat>();
            Movement.StateChange += Movement_StateChange;
            Movement.DirectionChange += Movement_DirectionChange;
            Movement.MoveStart += Movement_MoveStart;
            Movement.MoveEnd += Movement_MoveEnd;
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Combat.EntityDead += (v) =>
            {
                Movement.MapManager.DestroyObject(v, ETileLayer.Object1.Int());
            };
        }

        /*void Start()
        {
            //Movement.MapManager.SetupObject(new Vector2Int(transform.position, EVectorComponents.XZ),
            //    ETileLayer.Object1.Int(), new GridObject(this.gameObject), Vector2Int.One);

            //
        }*/

        // Update is called once per frame
        void FixedUpdate()
        {
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

        #region Movement events
        void Movement_DirectionChange(Vector2Int direction)
        {
            var newDirection = GetDirectionFromVector(direction) ?? _lastDirection;
            SetDirection(newDirection);
            _lastDirection = newDirection;
        }
        void Movement_MoveStart(Vector2Int nextPosition)
        {
        }
        void Movement_MoveEnd(Vector2Int nextPosition)
        {
        }
        private void Movement_StateChange(bool isidle)
        {
            SetIsIdle(isidle);
            this._lastIsIdle = isidle;
        }
        #endregion



        #region Entities

        protected override Dungeon.EntityAction DoPlanNextAction(Dungeon.EntityAction currentAction)
        {
            var result = base.DoPlanNextAction(currentAction);
            if (Combat.LastAttacker != null && Combat.LastAttacker.GetComponent<Combat>().CurrentHitPoints > 0)
            {
                if (Combat.IsInSquareRange(Combat.LastAttacker.GetTilePosition(), this.GetTilePosition(), 1))
                {
                    result = new EntityAction<Entity>("AttackEntity", Combat.LastAttacker);
                }
                else
                {
                    // follow the attacker
                    var pp = Combat.LastAttacker.GetComponent<Movement>().PreviousPosition;
                    Movement.SetTargetTile(pp);
                    result = new EntityAction<Vector2Int>("MoveToPosition", pp);
                }
            }
            else if (!Movement.MovingDone)
            {
                result = CurrentAction;
            }
            else if ((Movement.MovingDone && UnityEngine.Random.value < WalkStandRatio)
                || (Combat.CurrentHitPoints <= Combat.MaxHitPoints * 0.2f))
            {
                // plan path
                var coords = GameController.MapManager.GetRandomMovementTile(Movement.GetTilePosition(), 3);
                if (coords.HasValue)
                {
                    Debug.Log(this.name + " chooses its destination point: " + coords.Value);
                    Movement.SetTargetTile(coords.Value);
                    result = new EntityAction<Vector2Int>("MoveToPosition", coords.Value);
                }
            }
            else
            {
                Debug.Log(this.name + " is standing still");
                result = new EntityAction<Vector2Int>("StandingStill", Movement.GetTilePosition());
            }
            return result;
        }

        protected override void DoProcessNextAction(EntityAction action)
        {
            if (action.Name == "MoveToPosition")
            {
                Movement.DoNextStep();
            }
            else if (action.Name == "AttackEntity")
            {
                var enemy = (action as EntityAction<Entity>).Param0.GetComponent<Combat>();
                this.Combat.Attack(enemy);
            }
        }




        #endregion
    }
}