using UnityEngine;
using System.Collections.Generic;

namespace Dungeon
{
    public class EffectsApplicator : MonoBehaviour
    {
        Entity Entity;

        // Use this for initialization
        void Awake()
        {
            Entity = this.GetComponent<Entity>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool ApplyEffect(Items.Item item, Items.EItemEffectApplicationType currentApplicationType)
        {
            var result = false;
            foreach (var bonus in item.Effects)
            {
                if (bonus.ApplicationType == currentApplicationType)
                {
                    Debug.Log("Applying bonus " + bonus.ToString());
                    if (bonus.AttributeName.ToLower() == "hitpoints")
                    {
                        var combat = this.GetComponent<Combat>();
                        combat.CurrentHitPoints += bonus.Value;
                        combat.CurrentHitPoints = Mathf.Clamp(combat.CurrentHitPoints, 0, combat.MaxHitPoints);
                        Utils.PlayClip(item.UseSound);
                        result = true;
                    }
                }
                else
                {
                    Debug.Log("Cannot apply bonus: " + bonus.ToString() + " under current application type: " + currentApplicationType);
                }
            }
            return result;
        }
    }
}