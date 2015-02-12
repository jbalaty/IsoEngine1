using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(AudioSource))]
    public class Character : Entity
    {
        protected Animator[] AnimControllers;
        public Combat Combat;
        public Entity Entity;
        protected int _lastDirection = 3;
        protected bool _lastIsIdle = true;

        // Use this for initialization
        new protected void Awake()
        {
            base.Awake();
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Combat = this.GetComponent<Combat>();
            Entity = this.GetComponent<Entity>();
        }

        new protected void Start()
        {
            base.Start();
            Movement.StateChange += Movement_StateChange;
            Movement.DirectionChange += Movement_DirectionChange;
            Combat.EntityDead += OnDead;
        }

        public void SetDirection(int newDirection)
        {
            foreach (var a in AnimControllers) a.SetInteger("Direction", newDirection);
        }
        public void SetIdle(bool isidle)
        {
            foreach (var a in AnimControllers) a.SetBool("IsIdle", isidle);
        }

        #region Movement events
        void Movement_DirectionChange(Vector2Int direction)
        {
            var newDirection = Utils.GetDirectionFromVector(direction) ?? _lastDirection;
            SetDirection(newDirection);
            _lastDirection = newDirection;
        }
        
        private void Movement_StateChange(bool isidle)
        {
            if (this._lastIsIdle != isidle)
            {
                SetIdle(isidle);
            }
            this._lastIsIdle = isidle;
        }
        #endregion

        void OnDead(Vector3 position)
        {
            foreach (var a in AnimControllers) a.SetBool("IsDead", true);
            foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.sortingLayerName = "Ground1";
            Entity.enabled = false;
        }
    }
}
