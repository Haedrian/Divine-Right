using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Settlements;
using DRObjects.Items.Archetypes.Global;
using DRObjects;
using DRObjects.Settlements.Districts;

namespace DivineRightGame.SettlementHandling
{
    /// <summary>
    /// Helper Class for generating settlements
    /// </summary>
    public static class SettlementGenerator
    {
        private static Random random = new Random();

        /// <summary>
        /// Generates a completly random settlement with completly random statistics at a particular location having a particular size
        /// </summary>
        /// <param name="globalCoordinates"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Settlement GenerateSettlement(MapCoordinate globalCoordinates, int size)
        {
            Settlement settlement = new Settlement();

            settlement.Coordinate = globalCoordinates.Clone();
            settlement.Name = SettlementNameGenerator.GenerateName();
            settlement.Description = "the settlement of " + settlement.Name;
            settlement.MayContainItems = false;
            settlement.SettlementSize = size;

            //Generate the districts
            settlement.Districts = GenerateDistricts(size);


            return settlement;
        }

        /// <summary>
        /// Generates the districts. For now just do this randomly. Later we'll want to know the resources and set a theme for each settlement
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<District> GenerateDistricts(int size)
        {
            List<District> districts = new List<District>(size);

            var districtTypes = (DistrictType[]) Enum.GetValues(typeof(DistrictType));

            //Generate the rest of it
            for (int i = 0; i < size - 2; i++)
            {
                //Pick one at random
                var type = districtTypes[random.Next(districtTypes.Length)];

                //Does the type exist already?
                var match = districts.Where(dt => dt.Type.Equals(type)).FirstOrDefault();

                if (match == null)
                {
                    //No match, create a new one
                    districts.Add(new District(type, 1));
                }
                else if (match.Level < 3)
                {
                    //Matched, increment the level
                    match.Level++;
                }
                else
                {
                    //Can't go any higher, try again
                    i--;
                }
            }

            return districts;
        }
    }
}
