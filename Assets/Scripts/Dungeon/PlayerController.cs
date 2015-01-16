using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(WalkableComponent))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerController : BaseClass
    {
        Animator[] AnimControllers;
        WalkableComponent Walkable;
        public AudioClip SwordSwing;
        int _lastDirection = 3;
        bool _lastIsIdle = true;
        // Use this for initialization
        new void Awake()
        {
            base.Awake();
            //Debug.Log("CharacterController start");
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Walkable = this.GetComponent<WalkableComponent>();
            Walkable.StateChange += Walkable_StateChange;
            Walkable.DirectionChange += Walkable_DirectionChange;
            Walkable.MoveStart += Walkable_MoveStart;
            Walkable.MoveEnd += Walkable_MoveEnd;
            GameController.UpdateLight(new Vector2(this.transform.position.x, this.transform.position.z));
        }
        #region Walkable events
        void Walkable_DirectionChange(Vector2Int direction)
        {
            var newDirection = GetDirectionFromVector(direction) ?? _lastDirection;
            SetDirection(newDirection);
            _lastDirection = newDirection;
        }
        void Walkable_MoveStart(Vector2Int nextPosition)
        {
            GameController.GameTurnStart();
            GameController.StartLightBlending(nextPosition);
        }
        void Walkable_MoveEnd(Vector2Int nextPosition)
        {
            GameController.GameTurnEnd();
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
        #endregion

        // Update is called once per frame
        void FixedUpdate()
        {
            Walkable.DoNextStep();
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

        public bool CanAttack(GameObject go)
        {
            var v = (GetTilePosition() - new Vector2Int(go.transform.position, EVectorComponents.XZ));
            var be = go.GetComponent<BasicEnemy>();
            if (Mathf.Abs(v.x) <= 1f && Mathf.Abs(v.y) <= 1f && be != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Attack(GameObject go)
        {
            var result = false;
            if (CanAttack(go))
            {
                var be = go.GetComponent<BasicEnemy>();
                be.TakeDamage(1);
                audio.PlayOneShot(SwordSwing);
                result = true;
            }
            return result;
        }
    }
}
