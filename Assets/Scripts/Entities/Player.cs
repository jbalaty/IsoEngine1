using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(AudioSource))]
    public class Player : Entity
    {
        Animator[] AnimControllers;
        public Combat Combat;
        public Entity Entity;
        int _lastDirection = 3;
        bool _lastIsIdle = true;
        EntityAction UserAction;

        // Use this for initialization
        new void Awake()
        {
            base.Awake();
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Combat = this.GetComponent<Combat>();
            Entity = this.GetComponent<Entity>();
        }

        new void Start()
        {
            base.Start();
            Movement.StateChange += Movement_StateChange;
            Movement.DirectionChange += Movement_DirectionChange;
            Movement.MoveStart += Movement_MoveStart;
            Movement.MoveEnd += Movement_MoveEnd;
            Combat.EntityDead += OnDead;
            var inv = this.GetComponent<Inventory>();
            if (inv != null)
            {
                inv.AddItem(Items.ItemsDatabase.Instance.FindByName("Gold"), 5f);
            }
        }

        #region Movement events
        void Movement_DirectionChange(Vector2Int direction)
        {
            var newDirection = Utils.GetDirectionFromVector(direction) ?? _lastDirection;
            foreach (var a in AnimControllers) a.SetInteger("Direction", newDirection);
            _lastDirection = newDirection;
        }
        void Movement_MoveStart(Vector2Int nextPosition)
        {
            //GameController.GameTurnStart();
            //GameController.StartLightBlending(nextPosition);
        }
        void Movement_MoveEnd(Vector2Int nextPosition)
        {
        }
        private void Movement_StateChange(bool isidle)
        {
            if (this._lastIsIdle != isidle)
            {
                foreach (var a in AnimControllers) a.SetBool("IsIdle", isidle);
            }
            this._lastIsIdle = isidle;
        }
        #endregion

        #region Entities
        protected override EntityAction DoPlanNextAction(Dungeon.EntityAction currentAction)
        {
            var result = base.DoPlanNextAction(currentAction);
            if (UserAction != null)
            {
                result = UserAction;
            }
            else if (!Movement.MovingDone)
            {
                result = CurrentAction;
            }
            else
            {
                //Debug.Log(this.name + " is standing still");
                result = new EntityAction<Vector2Int>("StandingStill", Movement.GetTilePosition());
            }
            UserAction = null;
            return result;
        }

        protected override void DoProcessNextAction(EntityAction action)
        {
            base.DoProcessNextAction(action);
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
        public void MoveTo(Vector2Int coords)
        {
            this.Movement.SetTargetTile(coords);
            this.UserAction = new EntityAction<Vector2Int>("MoveToPosition", coords);
        }

        public void Attack(Entity entity)
        {
            if (this.Combat.CanAttack(entity.GetComponent<Combat>()))
            {
                this.UserAction = new EntityAction<Entity>("AttackEntity", entity);
            }
            else
            {
                this.Movement.SetTargetTile(entity.GetTilePosition(true), EntitiesManager.AllEntities.FindAll((e) => e != entity));
                this.UserAction = new EntityAction<Vector2Int>("MoveToPosition", entity.GetTilePosition(true));
            }
        }
        #endregion

        void OnDead(Vector3 position)
        {
            foreach (var a in AnimControllers) a.SetBool("IsDead", true);
            foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.sortingLayerName = "Ground1";
            Entity.enabled = false;
        }

        public void ToggleInventoryDialog()
        {
            //if (!InventoryDialog.activeSelf)
            //{
            //    InventoryDialog.SetActive(true);
            //    var invpanel = InventoryDialog.GetComponent<InventoryPanel>();
            //    invpanel.SetupInventoryComponent(this.GetComponent<Inventory>(), null);
            //}
            //else InventoryDialog.SetActive(false);
            this.GetComponent<Inventory>().ToggleInventoryDialog(null);
        }
    }
}
