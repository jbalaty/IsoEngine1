using UnityEngine;
using System.Collections;
using IsoEngine1;
using System;

namespace Dungeon
{
    [RequireComponent(typeof(AudioSource))]
    public class Combat : MonoBehaviour
    {

        public int MaxHitPoints = 10;
        public float CurrentHitPoints = 0;
        public float DefenceValue = 1;
        public float AttackValue = 1;
        public long ExperiencePoints = 0;
        public bool OnDeadDestroy = true;
        public bool OnDeadSetWalkable = true;
        public Entity LastAttacker = null;
        [Tooltip("How many HPs is healed in one turn")]
        public float RegenerationSpeed = 0;
        float _currentRegenerationProgress = 0;

        public AudioClip DeathSound;
        public AudioClip HitSound;
        public AudioClip AttackSound;


        public event Action<Vector3> EntityDead;

        public HealthIndicator HealthIndicator;
        Entity Entity;
        TextMeshSpawner TextMeshSpawner;

        public bool IsAlive
        {
            get
            {
                return CurrentHitPoints > 0;
            }
        }

        void Awake()
        {
            Entity = this.GetComponent<Entity>();
            TextMeshSpawner = this.GetComponent<TextMeshSpawner>();
        }

        void Start()
        {
            CurrentHitPoints = MaxHitPoints;
            Entity.EntityTurnStart += EntityTurnStart;
        }

        void Update()
        {
            if (HealthIndicator != null)
            {
                HealthIndicator.SetHealth((int)CurrentHitPoints, MaxHitPoints);
            }
        }

        public int TakeDamage(float damage, Combat from)
        {
            int realDamageWithoutDefence = (int)(damage - DefenceValue);
            if (realDamageWithoutDefence > 0)
            {
                this.CurrentHitPoints -= realDamageWithoutDefence;
                if (TextMeshSpawner != null)
                {
                    TextMeshSpawner.SpawnTextMesh("- " + realDamageWithoutDefence + "HP", Color.red, 1f);
                }
                Debug.Log(this.name + ": taking damage " + damage + "-" + DefenceValue + "=" + realDamageWithoutDefence
                    + " (Life: " + CurrentHitPoints + "/" + MaxHitPoints + ")");
                if (this.CurrentHitPoints > 0)
                {
                    StartCoroutine(ChangeColorCoroutine(Color.red));
                    if (HitSound != null) audio.PlayOneShot(HitSound);
                }
                else
                {
                    Debug.Log(this.name + ": dying");
                    Utils.PlayClip(DeathSound);
                    if (OnDeadDestroy) GameObject.Destroy(this.gameObject);
                    if (OnDeadSetWalkable) Entity.IsWalkable = true;
                    if (EntityDead != null) EntityDead(this.transform.position);
                }
                LastAttacker = from.Entity;
            }

            return realDamageWithoutDefence;
        }

        public bool CanAttack(Combat go)
        {
            if (go == null) return false;
            if (go == this.GetComponent<Combat>()) return false;
            var be = go.GetComponent<Combat>();
            if (be == null) return false;
            var v = (Entity.GetTilePosition() - new Vector2Int(go.gameObject.transform.position, EVectorComponents.XZ));
            if (IsInSquareRange(v, 1) && be != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsInSquareRange(Vector2Int coords1, Vector2Int coords2, int range)
        {
            return Mathf.Abs(coords1.x - coords2.x) <= range && Mathf.Abs(coords1.y - coords2.y) <= range;
        }

        public static bool IsInSquareRange(Vector2Int diff, int range)
        {
            return Mathf.Abs(diff.x) <= range && Mathf.Abs(diff.y) <= range;
        }

        public bool Attack(Combat go)
        {
            var result = false;
            if (CanAttack(go))
            {
                var enemy = go.GetComponent<Combat>();
                DealDamage(enemy);
                if (AttackSound != null) audio.PlayOneShot(AttackSound);
                result = true;
            }
            return result;
        }

        public int DealDamage(Combat enemy)
        {
            int realDamageDealt = enemy.TakeDamage(AttackValue, this);
            if (!enemy.IsAlive)
            {
                TextMeshSpawner.SpawnTextMesh("+ " + enemy.MaxHitPoints + "XP", Color.cyan, 1f);
                ExperiencePoints += enemy.MaxHitPoints;
            }
            return realDamageDealt;
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

        public void EntityTurnStart(EntityAction current)
        {
            // use auto healing
            _currentRegenerationProgress += RegenerationSpeed * 1f; //1 turn
            if (_currentRegenerationProgress > 1f)
            {
                int chp = (int)CurrentHitPoints;
                CurrentHitPoints += (int)_currentRegenerationProgress;
                CurrentHitPoints = Mathf.Min(CurrentHitPoints, MaxHitPoints);
                if ((int)CurrentHitPoints - chp > 0)
                {
                    TextMeshSpawner.SpawnTextMesh("+ " + ((int)CurrentHitPoints - chp) + "HP", Color.green, TextMeshSpawner.DefaultFadeOutTime);
                }
                _currentRegenerationProgress -= (int)_currentRegenerationProgress;
            }
        }
    }
}