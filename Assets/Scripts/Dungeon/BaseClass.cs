using UnityEngine;
using System.Collections;

namespace Dungeon
{
    public class BaseClass : MonoBehaviour
    {
        protected GameController GameController;
        // Use this for initialization
        protected void Awake()
        {
            GameController = GameObject.Find("GameController").GetComponent<GameController>();

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}