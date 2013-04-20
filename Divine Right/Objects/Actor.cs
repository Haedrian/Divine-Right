using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects
{
    /// <summary>
    /// Represents an Actor on a map which can take certain actions.
    /// This is additional data which is stored seperatly
    /// </summary>
    public class Actor
    {
        /// <summary>
        /// Which map item this Actor represents
        /// </summary> 
        public MapItem MapCharacter { get; set; }

        /// <summary>
        /// The global coordinate of this current actor. Only useful if the user can leave the local map
        /// </summary>
        public MapCoordinate GlobalCoordinates { get; set; }

        /// <summary>
        /// Represents whether this actor is the player character or not
        /// </summary>
        public bool IsPlayerCharacter { get; set; }

        /// <summary>
        /// A unique ID for determining which Actors are which
        /// </summary>
        public Guid UniqueId { get; set; }

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

    }
}
