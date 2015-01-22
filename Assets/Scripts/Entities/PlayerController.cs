using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : Entity
    {
        Animator[] AnimControllers;
        Combat Combat;
        int _lastDirection = 3;
        bool _lastIsIdle = true;
        EntityAction UserAction;
        // Use this for initialization
        void Awake()
        {
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Combat = this.GetComponent<Combat>();
            Movement.StateChange += Movement_StateChange;
            Movement.DirectionChange += Movement_DirectionChange;
            Movement.MoveStart += Movement_MoveStart;
            Movement.MoveEnd += Movement_MoveEnd;
        }

        /*void Start()
        {
            //Movement.MapManager.SetupObject(new Vector2Int(transform.position, EVectorComponents.XZ),
            //    ETileLayer.Object1.Int(), new GridObject(this.gameObject), Vector2Int.One);
        }*/

        void OnGUI()
        {
            var style = new GUIStyle();
            GUI.Box(new Rect(10, 10, (Screen.width / 2) * (Combat.CurrentHitPoints / (float)Combat.MaxHitPoints), 20),
                Combat.CurrentHitPoints + "/" + Combat.MaxHitPoints);//, style);
        }

        #region Movement events
        void Movement_DirectionChange(Vector2Int direction)
        {
            var newDirection = Utils.GetDirectionFromVector(direction) ?? _lastDirection;
            SetDirection(newDirection);
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
                SetIsIdle(isidle);
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
            if (action.Name == "MoveToPosition")
            {
                Movement.DoNextStep();
            }
        }
        public void MoveTo(Vector2Int coords)
        {
            this.Movement.SetTargetTile(coords);
            this.UserAction = new EntityAction<Vector2Int>("MoveToPosition", coords);
        }
        #endregion

        
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
    }
}
