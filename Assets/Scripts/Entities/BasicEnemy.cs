using UnityEngine;
using System.Collections;
using System.Linq;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    public class BasicEnemy : Entity
    {
        Animator[] AnimControllers;
        protected GameController GameController;
        public Entity Entity;
        public Combat Combat;
        [Range(0, 1)]
        public float WalkStandRatio = 0.7f;

        int _lastDirection = 3;
        bool _lastIsIdle = true;


        new void Awake()
        {
            base.Awake();
            GameController = GameObject.Find("GameController").GetComponent<GameController>();
            Entity = this.GetComponent<Entity>();
            Movement = this.GetComponent<Movement>();
            Combat = this.GetComponent<Combat>();
            AnimControllers = this.GetComponentsInChildren<Animator>();

        }

        new void Start()
        {
            base.Start();
            Movement.StateChange += Movement_StateChange;
            Movement.DirectionChange += Movement_DirectionChange;
            Movement.MoveStart += Movement_MoveStart;
            Movement.MoveEnd += Movement_MoveEnd;
            Combat.EntityDead += OnDead;
        }

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

        #region Movement events
        void Movement_DirectionChange(Vector2Int direction)
        {
            var newDirection = GetDirectionFromVector(direction) ?? _lastDirection;
            foreach (var a in AnimControllers) a.SetInteger("Direction", newDirection);
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
            //SetIsIdle(isidle);
            foreach (var a in AnimControllers) a.SetBool("IsIdle", isidle);
            this._lastIsIdle = isidle;
        }
        #endregion



        #region Entities

        protected override Dungeon.EntityAction DoPlanNextAction(Dungeon.EntityAction currentAction)
        {
            var result = base.DoPlanNextAction(currentAction);
            var playersClose = EntitiesManager.GetEntities((e) =>
            {
                return e.GetComponent<Player>() != null && Utils.DiagonalDistance(this.GetTilePosition(), e.GetTilePosition()) <= 2;
            });
            if (playersClose.Count > 0)
            {
                Combat.LastAttacker = playersClose[0];
            }

            if (Combat.LastAttacker != null && Combat.LastAttacker.GetComponent<Combat>().IsAlive)
            {
                if (Combat.IsInSquareRange(Combat.LastAttacker.GetTilePosition(), this.GetTilePosition(), 1))
                {
                    result = new EntityAction<Entity>("AttackEntity", Combat.LastAttacker);
                }
                else
                {
                    // follow the attacker
                    var pp = Combat.LastAttacker.GetTilePosition();
                    Movement.SetTargetTile(pp, EntitiesManager.AllEntities.FindAll((e) =>
                    {
                        return e != this.Entity && e != Combat.LastAttacker;
                    }));
                    result = new EntityAction<Vector2Int>("MoveToPosition", pp);
                }
            }
            else if (currentAction.Name == "MoveToPosition" && Movement.CurrentPath != null && Movement.CurrentPath.Count > 0)
            {
                result = CurrentAction;
            }
            else if ((UnityEngine.Random.value < WalkStandRatio)
                || (Combat.CurrentHitPoints <= Combat.MaxHitPoints * 0.2f))
            {
                // plan path
                var coords = MapProxy.GetRandomMovementTile(Entity.StartPosition, (c) =>
                {
                    return c != Entity.StartPosition && c != this.GetTilePosition();
                }, 2);
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
                if (!Movement.DoNextStep() && Entity.GetTilePosition(true) != GetTilePosition())
                {
                    var path = Movement.SetTargetTile((action as EntityAction<Vector2Int>).Param0);
                    // if there is no path to this tile, stop and next plan choose other action
                    if (path == null || path.Count == 0)
                    {
                        CurrentAction = NoAction;
                    }
                }
            }
            else if (action.Name == "AttackEntity")
            {
                var enemy = (action as EntityAction<Entity>).Param0.GetComponent<Combat>();
                this.Combat.Attack(enemy);
            }
        }

        protected void OnDead(Vector3 position)
        {
            foreach (var a in AnimControllers) a.SetBool("IsDead", true);
            foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.sortingLayerName = "Ground1";
            Entity.enabled = false;
        }


        #endregion
    }
}