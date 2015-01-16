using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    public class BasicEnemy : GameLogicComponent, IGameLogicEntity
    {
        Animator[] AnimControllers;
        protected GameController GameController;
        protected MapManager Map;
        protected WalkableComponent Walkable;
        [Range(0, 1)]
        public float WalkStandRatio = 0.7f;
        public AudioClip DeathSound;
        public AudioClip HitSound;
        int _lastDirection = 3;
        bool _lastIsIdle = true;
        public int HitPoints = 10;
        int _currentHitPoints = 0;
        void Awake()
        {
            GameController = GameObject.Find("GameController").GetComponent<GameController>();
            Walkable = this.GetComponent<WalkableComponent>();
            Walkable.StateChange += Walkable_StateChange;
            Walkable.DirectionChange += Walkable_DirectionChange;
            Walkable.MoveStart += Walkable_MoveStart;
            Walkable.MoveEnd += Walkable_MoveEnd;
            AnimControllers = this.GetComponentsInChildren<Animator>();
            Map = GameController.MapManager;

            _currentHitPoints = HitPoints;
        }

        // Use this for initialization
        void Start()
        {


        }

        // Update is called once per frame
        void FixedUpdate()
        {
        }

        public void GameTurnStart()
        {
            //Debug.Log("BasicEnemy GameTurnStart - " + this.name);
            Walkable.DoNextStep();
        }
        public void GameTurnEnd()
        {
            //Debug.Log("BasicEnemy GameTurnEnd - " + this.name);
            if (Walkable.MovingDone)
            {
                if (UnityEngine.Random.value < WalkStandRatio)
                {
                    // plan path
                    var coords = GameController.MapManager.GetRandomWalkableTile(Walkable.GetTilePosition(), 3);
                    if (coords.HasValue)
                    {
                        Debug.Log(this.name + " chooses its destination point: " + coords.Value);
                        Walkable.SetTargetTile(coords.Value);
                    }
                }
                else
                {
                    Debug.Log(this.name + " is standing still");
                }
            }
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

        #region Walkable events
        void Walkable_DirectionChange(Vector2Int direction)
        {
            var newDirection = GetDirectionFromVector(direction) ?? _lastDirection;
            SetDirection(newDirection);
            _lastDirection = newDirection;
        }
        void Walkable_MoveStart(Vector2Int nextPosition)
        {
        }
        void Walkable_MoveEnd(Vector2Int nextPosition)
        {
        }
        private void Walkable_StateChange(bool isidle)
        {
            SetIsIdle(isidle);
            this._lastIsIdle = isidle;
        }
        #endregion

        public int TakeDamage(int damage)
        {
            this._currentHitPoints -= damage;
            if (this._currentHitPoints > 0)
            {
                Debug.Log(this.name + ": taking damage (" + _currentHitPoints + "/" + HitPoints + ")");
                StartCoroutine(ChangeColorCoroutine(Color.red));
                audio.PlayOneShot(HitSound);
            }
            else
            {
                audio.PlayOneShot(DeathSound);
                Debug.Log(this.name + ": dying");
                GameObject.Destroy(this.gameObject, 0.8f);
            }
            return damage;
        }

        public int DealDamage(int damage)
        {
            return damage;
        }

        IEnumerator ChangeColorCoroutine(Color c)
        {
            var elapsedTime = 0f;
            var sprites = GetComponentsInChildren<SpriteRenderer>();
            while (elapsedTime < 0.5f)
            {
                elapsedTime += Time.deltaTime;
                foreach (var s in sprites)
                {
                    s.color = Color.Lerp(Color.white, c, elapsedTime / 0.5f);
                }
                yield return null;
            }
            foreach (var s in sprites)
            {
                s.color = Color.white;
            }
        }
    }
}