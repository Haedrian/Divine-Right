using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.ActorHandling;

namespace DRObjects
{
    /// <summary>
    /// Represents an Actor on a map which can take certain actions.
    /// This is additional data which is stored seperatly
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// If its an enemy, holds some auxillary data
        /// </summary>
        public EnemyData EnemyData {get;set;}

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
        /// Represents whether The actor has moved last turn
        /// </summary>
        public bool HasActedLastTurn { get; set; }

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
        /// Attributes pertaining to the actor
        /// </summary>
        public ActorAttributes Attributes { get; set; }

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
        }

    }
}
