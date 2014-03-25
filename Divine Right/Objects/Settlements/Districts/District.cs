using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Settlements.Districts
{
    /// <summary>
    /// A district belonging in a city
    /// </summary>
   public class District
    {
       public DistrictType Type { get; set; }
       public int Level { get; set; }

       private static string[,] descriptions;

       /// <summary>
       /// Returns the maplet tag for this particular district and level
       /// </summary>
       /// <returns></returns>
       public string GetDistrictMapletTag()
       {
           return Type.ToString() + Level;
       }

       /// <summary>
       /// Returns a description of the District
       /// </summary>
       /// <returns></returns>
       public string GetDistrictDescription()
       {
           return descriptions[(int)this.Type, this.Level];
       }

       public District(DistrictType type, int level)
       {
           this.Type = type;
           this.Level = level;
       }

       /// <summary>
       /// Static constructor, creates the descriptions
       /// </summary>
       static District()
       {
           //This is sadly hardcoded :(
           descriptions = new string[,]
           {
               {"Shacks","Slums","Shanty Town"},
               {"Cottages","Messuages","Insulae"},
               {"Domi","Villas","Palaces"},
               {"Lord's Villa","Lord's Mansion","Lord's Palace"},
               {"Library","Grand Library","Great Library"},
               {"Shrine","Temple","High Temple"},
               {"General Store","General Store","General Store"},
               {"Smithy","Foundry","Steelworks"},
               {"Carpenter's House","Carpentry Workshop","Woodworks"},
               {"Mason's House","Stoneworker's Shop","Stoneworks"},
               {"Town Watch","Militia Barracks","Barracks"},
               {"Marketplace","Warehouses","Trading Port"},
               {"Tavern","Inn","Caravanserai"}
           };
       }
    }
}
