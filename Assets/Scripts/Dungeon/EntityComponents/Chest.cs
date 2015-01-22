using UnityEngine;
using System.Collections;
using IsoEngine1;
using System;

namespace Dungeon
{
    public class Chest : ObjectComponent
    {

        void Start()
        {
            GetComponentInChildren<Animator>().SetBool("IsOpen", Value == 1);
        }


        public override void Interact(Entity e)
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
            GetComponentInChildren<Animator>().SetBool("IsOpen", Value == 1);

        }
    }
}