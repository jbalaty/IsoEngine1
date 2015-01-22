using UnityEngine;
using System.Collections;
using IsoEngine1;
using System;

namespace Dungeon
{
    public enum EObjectType
    {
        Switch, Chest, Door
    }

    public class ObjectComponent : MonoBehaviour
    {
        public bool Destructable = false;
        public EObjectType Type = EObjectType.Chest;
        public int Value = 0;
        public AudioClip InteractionSound1;
        public AudioClip InteractionSound2;

        Entity Entity;

        void Awake()
        {
            Entity = this.GetComponent<Entity>();
        }

        public virtual bool CanInteract(Entity e)
        {
            if (e == null) return false;
            var v = (Entity.GetTilePosition() - e.GetTilePosition());
            if (Mathf.Abs(v.x) <= 1f && Mathf.Abs(v.y) <= 1f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public virtual void Interact(Entity e)
        {
            if (Value == 0)
            {
                audio.PlayOneShot(InteractionSound1);
                Value = 1;
            }
            else
            {
                audio.PlayOneShot(InteractionSound2);
                Value = 0;
            }

        }
    }
}