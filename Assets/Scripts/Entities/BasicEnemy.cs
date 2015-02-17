using UnityEngine;
using System.Collections;
using System.Linq;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    public class BasicEnemy : Character
    {
        protected GameController GameController;
        //int _NumberFollowAttackerSteps;

        [Range(0, 1)]
        public float WalkStandRatio = 0.7f;

        new void Awake()
        {
            base.Awake();
            GameController = GameObject.Find("GameController").GetComponent<GameController>();
        }

        new void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
        }

        #region Entities
        protected override EntityAction DoPlanNextAction(Dungeon.EntityAction currentAction)
        {
            //if (_NumberFollowAttackerSteps <= -10) _NumberFollowAttackerSteps = 0;
            var result = base.DoPlanNextAction(currentAction);
            var playersClose = EntitiesManager.GetEntities((e) =>
            {
                return e.GetComponent<Player>() != null && Utils.DiagonalDistance(this.GetTilePosition(), e.GetTilePosition()) <= 3;
            });
            if (playersClose.Count > 0)
            {
                Combat.LastAttacker = playersClose[0];
                SendMessage("EnemySpotted", SendMessageOptions.DontRequireReceiver);
            }

            if (Combat.LastAttacker != null && Combat.LastAttacker.GetComponent<Combat>().IsAlive)
            {
                if (Combat.IsInSquareRange(Combat.LastAttacker.GetTilePosition(), this.GetTilePosition(), 1))
                {
                    result = new EntityAction<Entity>("AttackEntity", Combat.LastAttacker);
                    //_NumberFollowAttackerSteps = 0;
                }
                else //if (_NumberFollowAttackerSteps >= 0 && _NumberFollowAttackerSteps < 10)
                {
                    // follow the attacker
                    var pp = Combat.LastAttacker.GetTilePosition(true);
                    Movement.SetTargetTile(pp, EntitiesManager.AllEntities.FindAll((e) =>
                    {
                        return e != this.Entity && e != Combat.LastAttacker;
                    }));
                    result = new EntityAction<Vector2Int>("MoveToPosition", pp);
                    //_NumberFollowAttackerSteps++;
                }
                //else
                //{
                //    result = PlanRandomPath();
                //    Combat.LastAttacker = null;
                //    if (_NumberFollowAttackerSteps >= 10) _NumberFollowAttackerSteps = -1;
                //    else _NumberFollowAttackerSteps--;
                //}
            }
            else if (currentAction.Name == "MoveToPosition" && Movement.CurrentPath != null && Movement.CurrentPath.Count > 0)
            {
                result = CurrentAction;
            }
            else if ((UnityEngine.Random.value < WalkStandRatio)
                || (Combat.CurrentHitPoints <= Combat.MaxHitPoints * 0.2f))
            {
                result = PlanRandomPath();
            }
            else
            {
                //Debug.Log(this.name + " is standing still");
                result = new EntityAction<Vector2Int>("StandingStill", Movement.GetTilePosition());
            }
            return result;
        }

        EntityAction PlanRandomPath()
        {
            var result = NoAction;
            // plan path
            var coords = MapProxy.GetRandomMovementTile(Entity.StartPosition, (c) =>
            {
                return c != Entity.StartPosition && c != this.GetTilePosition();
            }, 2);
            if (coords.HasValue)
            {
                //Debug.Log(this.name + " chooses its destination point: " + coords.Value);
                Movement.SetTargetTile(coords.Value);
                result = new EntityAction<Vector2Int>("MoveToPosition", coords.Value);
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
        #endregion
    }
}