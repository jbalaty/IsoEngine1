using UnityEngine;
using System.Collections;
using IsoEngine1;
using System;

namespace Dungeon
{
    public class Combat : MonoBehaviour
    {

        public int MaxHitPoints = 10;
        public int CurrentHitPoints = 0;
        public int DefenceValue = 1;
        public int AttackValue = 1;
        public Entity LastAttacker = null;

        public AudioClip DeathSound;
        public AudioClip HitSound;
        public AudioClip AttackSound;

        public event Action<Vector2Int> EntityDead;

        Entity Entity;

        void Awake()
        {
            Entity = this.GetComponent<Entity>();
        }

        void Start()
        {
            CurrentHitPoints = MaxHitPoints;
        }



        public int TakeDamage(int damage, Combat cmb)
        {
            this.CurrentHitPoints -= damage;
            if (this.CurrentHitPoints > 0)
            {
                Debug.Log(this.name + ": taking damage (" + CurrentHitPoints + "/" + MaxHitPoints + ")");
                StartCoroutine(ChangeColorCoroutine(Color.red));
                if (HitSound != null) audio.PlayOneShot(HitSound);
            }
            else
            {
                if (DeathSound != null) audio.PlayOneShot(DeathSound);
                Debug.Log(this.name + ": dying");
                GameObject.Destroy(Entity, DeathSound != null ? DeathSound.length : 0.3f);
                if (EntityDead != null) EntityDead(Entity.GetTilePosition());
            }
            LastAttacker = cmb.Entity;
            return damage;
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
                var be = go.GetComponent<Combat>();
                be.TakeDamage(1, this);
                if (AttackSound != null) audio.PlayOneShot(AttackSound);
                result = true;
            }
            return result;
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