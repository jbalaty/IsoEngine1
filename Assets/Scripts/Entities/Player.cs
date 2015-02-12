using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(AudioSource))]
    public class Player : Character
    {
        EntityAction UserAction;

        // Use this for initialization
        new void Awake()
        {
            base.Awake();
        }

        new void Start()
        {
            base.Start();
            var inv = this.GetComponent<Inventory>();
            if (inv != null)
            {
                inv.AddItem(Items.ItemsDatabase.Instance.FindByName("Gold"), 5f);
            }
        }

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
                //result = new EntityAction<Vector2Int>("StandingStill", Movement.GetTilePosition());
                result = NoAction;
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
                    Debug.Log("Movement not possible");
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

        public void ToggleInventoryDialog()
        {
            this.GetComponent<Inventory>().ToggleInventoryDialog(null);
        }
    }
}
