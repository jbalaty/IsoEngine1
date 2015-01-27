using UnityEngine;
using System.Collections;

namespace Dungeon
{
    public class Inventory : MonoBehaviour
    {
        [SerializeField]
        protected int _Gold = 0;

        Entity Entity;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Entity = this.GetComponent<Entity>();
        }
        public int AddGold(int amount)
        {
            this._Gold += amount;
            if (Entity.TextMeshSpawner != null)
            {
                Entity.TextMeshSpawner.SpawnTextMesh("+ " + amount + "G", Color.yellow, 1f);
            }
            return this._Gold;
        }
    }
}