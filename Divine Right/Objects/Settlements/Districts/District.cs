using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

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
       /// Gets Sprite to Display in the interface box
       /// </summary>
       /// <returns></returns>
       public SpriteData GetInterfaceSprite()
       {
           switch(this.Type)
           {
               case DistrictType.BARRACKS:
                   return SpriteManager.GetSprite(InterfaceSpriteName.BARRACKS);
               case DistrictType.IRON_WORKS:
                   return SpriteManager.GetSprite(InterfaceSpriteName.SMITH);
               case DistrictType.LIBRARY:
                   return SpriteManager.GetSprite(InterfaceSpriteName.LIBRARY);
               case DistrictType.PALACE:
                   return SpriteManager.GetSprite(InterfaceSpriteName.PALACE);
               //case DistrictType.PLEB_HOUSING:
               //    return SpriteManager.GetSprite(InterfaceSpriteName.POOR_HOUSING);
               //case DistrictType.RICH_HOUSING:
               //    return SpriteManager.GetSprite(InterfaceSpriteName.RICH_HOUSING);
               //case DistrictType.SLUM_HOUSING:
               //    return SpriteManager.GetSprite(InterfaceSpriteName.SLUM_HOUSING);
               case DistrictType.STONE_WORKS:
                   return SpriteManager.GetSprite(InterfaceSpriteName.STONEWORKER);
               case DistrictType.TEMPLE:
                   return SpriteManager.GetSprite(InterfaceSpriteName.TEMPLE);
               case DistrictType.TRADING:
                   return SpriteManager.GetSprite(InterfaceSpriteName.MARKET);
               case DistrictType.WOOD_WORKS:
                   return SpriteManager.GetSprite(InterfaceSpriteName.CARPENTER);
               default:
                   throw new NotImplementedException("No Sprite for type " + this.Type);
           }
       }

       /// <summary>
       /// Static constructor, creates the descriptions
       /// </summary>
       static District()
       {
           //This is sadly hardcoded :(
           descriptions = new string[,]
           {
               //{"Shacks","Slums","Shanty Town"},
               //{"Cottages","Hovels","Insulae"},
               //{"Domi","Villas","Palaces"},
               {"Governor's Villa","Governor's Mansion","Governor's Palace"},
               {"Library","Grand Library","Great Library"},
               {"Shrine","Temple","High Temple"},
               {"Smithy","Foundry","Steelworks"},
               {"Carpenter's House","Carpentry Workshop","Woodworks"},
               {"Mason's House","Stoneworker's Shop","Stoneworks"},
               {"Town Watch","Militia Barracks","Barracks"},
               {"Marketplace","Warehouses","Trading Port"},
           };
       }
    }
}
