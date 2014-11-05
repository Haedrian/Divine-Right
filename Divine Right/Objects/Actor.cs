using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.ActorHandling;
using Newtonsoft.Json;
using DRObjects.Items.Archetypes.Local;
using DRObjects.DataStructures;
using DRObjects.ActorHandling.Enums;

namespace DRObjects
{
    [Serializable]
    /// <summary>
    /// Represents an Actor on a map which can take certain actions.
    /// This is additional data which is stored seperatly
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// The name of the actor
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gender
        /// </summary>
        public Gender Gender { get; set; }

        public ActorInventory Inventory { get; set; }
        /// <summary>
        /// If its an enemy, holds some auxillary data
        /// </summary>
        public EnemyData EnemyData {get;set;}

        /// <summary>
        /// Determines whether the actor is still alive
        /// </summary>
        public bool IsAlive { get; set; }

        /// <summary>
        /// The method that this character is using to travel
        /// </summary>
        public TravelMethod TravelMethod { get; set; }

        /// <summary>
        /// Which map item this Actor represents
        /// </summary> 
        public MapItem MapCharacter { get; set; }

        /// <summary>
        /// The global coordinate of this current actor. Only useful if the user can leave the local map
        /// </summary>
        public MapCoordinate GlobalCoordinates { get; set; }

        /// <summary>
        /// The stack of missions for the actor to perform
        /// </summary>
        public Stack<ActorMission> MissionStack { get; set; }

        /// <summary>
        /// The actor's current mission.
        /// </summary>
        public ActorMission CurrentMission { get; set; }

        /// <summary>
        /// Represents whether this actor is the player character or not
        /// </summary>
        public bool IsPlayerCharacter { get; set; }

        /// <summary>
        /// Represents whether the actor is stunned or not. If they are stunned, they do nothing in that turn
        /// </summary>
        public bool IsStunned { get; set; }

        /// <summary>
        /// How many tiles the actor can see. Null means they're blind
        /// </summary>
        public int? LineOfSight { get; set; }

        /// <summary>
        /// A unique ID for determining which Actors are which
        /// </summary>
        public Guid UniqueId { get; set; }

        /// <summary>
        /// The anatomy of the actor. For now we'll stick with humanoid. Later we'll expand.
        /// </summary>
        public HumanoidAnatomy Anatomy { get; set; }

        /// <summary>
        /// Whether this actor is aggressive or not. Later we'll change this to some other measure - like a list of who they're aggressive towards.
        /// </summary>
        public bool IsAggressive { get; set; }

        /// <summary>
        /// Whether it's an animal or not. Animals can be attacked even if they're not aggressive. That'll make them aggressive. Unless they're domesticated.
        /// </summary>
        public bool IsAnimal { get; set; }

        /// <summary>
        /// Whether it's a domesticated animal or not. Animals which are domesticated may be attacked and won't attack back.
        /// </summary>
        public bool IsDomesticatedAnimal { get; set; }

        /// <summary>
        /// The unarmed damage dice done by this actor
        /// </summary>
        public int UnarmedDamageDice { get; set; }

        /// <summary>
        /// Additional details if this actor is a vendor.
        /// </summary>
        public VendorDetails VendorDetails { get; set; }
        /// <summary>
        /// Attributes pertaining to the actor
        /// </summary>
        public SkillsAndAttributes Attributes { get; set; }

        //These are the total effective points of attributes after temporary and equipment has been taken into consideration
        public int TotalBrawn { get { return Attributes.Brawn - FeedingLevel == 0 ? 3 : 0; } }
        public int TotalAgil { get { return Attributes.Agil - FeedingLevel == 0 ? 3 : 0; } }
        public int TotalDex { get { return Attributes.Char - FeedingLevel == 0 ? 3 : 0; } }
        public int TotalPerc { get { return Attributes.Perc - FeedingLevel == 0 ? 3 : 0; } }
        public int TotalIntel { get { return Attributes.Intel - FeedingLevel == 0 ? 3 : 0; } }

        public FeedingLevel FeedingLevel { get; set; }

        /// <summary>
        /// The character's combat stance
        /// </summary>
        public ActorStance CombatStance { get; set; }

        /// <summary>
        /// Two LocalActors are considered equal if they share the same UniqueID
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (typeof(Actor).Equals(obj.GetType()))
            {

                if (UniqueId.Equals((obj as Actor).UniqueId))
                {
                    return true;
                }

            }

            return false;
        }

        public Actor()
        {
            this.MissionStack = new Stack<ActorMission>();
            this.CombatStance = ActorStance.NEUTRAL;
            this.IsAlive = true;
            this.Inventory = new ActorInventory();
            this.Attributes = new SkillsAndAttributes();
            this.Attributes.Actor = this;
            this.UnarmedDamageDice = 1;
            this.TravelMethod = Enums.TravelMethod.WALKING;
            this.FeedingLevel = Enums.FeedingLevel.STUFFED;
        }

    }
}
