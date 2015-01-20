using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Local;

namespace DRObjects.ActorHandling.CharacterSheet
{
    [Serializable]
    /// <summary>
    /// Represents the set of what the character knows
    /// </summary>
    public class ActorKnowledge
    {
        public ActorKnowledge(Actor actor)
        {
            this.Actor = actor;
        }

        public Actor Actor { get; set; }
        /// <summary>
        /// What the actor knows about identifying potions
        /// </summary>
        private List<PotionIdentification> potionIdentification = new List<PotionIdentification>();

        public bool CanIdentifyPotion(PotionType type)
        {
            return (potionIdentification.Any(pi => pi.PotionType == type));
        }

        public void LearnToIdentify(PotionType type)
        {
            potionIdentification.Add(new PotionIdentification() { PotionType = type });

            //Go through each potion within the actor's inventory and see if there's a new type
            foreach(var potion in this.Actor.Inventory.Inventory.GetObjectsByGroup(InventoryCategory.POTION))
            {
                if (potion.GetType() == typeof(Potion))
                {
                    //Was it unknown?
                    var cPotion = potion as Potion;
                    
                    if (!cPotion.IsIdentified && cPotion.PotionType == type)
                    {
                        cPotion.IsIdentified = true;
                    }
                }
            }
        }

    }
}
