using UnityEngine;
using System.Collections.Generic;

namespace Dungeon
{
    public class BonusApplicator : MonoBehaviour
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

        public bool ApplyBonus(Items.Item item, Items.EItemBonusApplicationType currentApplicationType)
        {
            foreach (var bonus in item.Bonuses)
            {
                if (bonus.ApplicationType == currentApplicationType && item.UnitAmount >= item.UnitAmount)
                {
                    Debug.Log("Applying bonus " + bonus.ToString());
                    if (bonus.AttributeName == "HitPoints")
                    {
                        var combat = this.GetComponent<Combat>();
                        combat.CurrentHitPoints += bonus.Value;
                        combat.CurrentHitPoints = Mathf.Clamp(combat.CurrentHitPoints, 0, combat.MaxHitPoints);
                    }
                }
                else
                {
                    Debug.Log("Cannot apply bonus: " + bonus.ToString() + " under current application type: " + currentApplicationType);
                }
            }
            return true;
        }
    }
}