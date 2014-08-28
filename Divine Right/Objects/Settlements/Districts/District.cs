using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

namespace DRObjects.Settlements.Districts
{
    [Serializable]
    /// <summary>
    /// A district belonging in a city
    /// </summary>
   public class District
    {
       public DistrictType Type { get; set; }
       public int Level { get; set; }

       /// <summary>
       /// Returns the maplet tag for this particular district and level
       /// </summary>
       /// <returns></returns>
       public string GetDistrictMapletTag()
       {
           return Type.ToString().Replace("_","").ToLower();
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
               case DistrictType.IRONWORKS:
                   return SpriteManager.GetSprite(InterfaceSpriteName.SMITH);
               case DistrictType.LIBRARY:
                   return SpriteManager.GetSprite(InterfaceSpriteName.LIBRARY);
               case DistrictType.PALACE:
                   return SpriteManager.GetSprite(InterfaceSpriteName.PALACE);
               case DistrictType.GENERAL_STORE:
                   return SpriteManager.GetSprite(InterfaceSpriteName.GENERAL_STORE);
               case DistrictType.INN:
                   return SpriteManager.GetSprite(InterfaceSpriteName.INN);
               //case DistrictType.PLEB_HOUSING:
               //    return SpriteManager.GetSprite(InterfaceSpriteName.POOR_HOUSING);
               //case DistrictType.RICH_HOUSING:
               //    return SpriteManager.GetSprite(InterfaceSpriteName.RICH_HOUSING);
               //case DistrictType.SLUM_HOUSING:
               //    return SpriteManager.GetSprite(InterfaceSpriteName.SLUM_HOUSING);
               case DistrictType.TEMPLE:
                   return SpriteManager.GetSprite(InterfaceSpriteName.TEMPLE);
               case DistrictType.COMMERCE:
                   return SpriteManager.GetSprite(InterfaceSpriteName.MARKET);
               case DistrictType.CARPENTRY:
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
          
       }
    }
}
