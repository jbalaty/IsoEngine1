using UnityEngine;
using System.Collections;
using IsoEngine1;

namespace Dungeon
{
    [RequireComponent(typeof(Combat))]
    public class DestructibleObject : MonoBehaviour
    {
        Combat Combat;
        public bool SetWalkable;
        
        // Use this for initialization
        void Start()
        {
            Combat = GetComponent<Combat>();
            Combat.EntityDead += OnDead;

        }

        public void OnDead(Vector3 position)
        {
            if (this.SetWalkable)
            {
                this.GetComponent<Entity>().IsWalkable = true;
            }
        }
    }
}