using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.Graphics
{
    /// <summary>
    /// A manager for handling of graphical sets
    /// </summary>
    public class GraphicSetManager
    {
        private static List<Color> hairColours;
        private static List<Color> clothingColours;
        private static List<Color> priestClothingColours;

        private static Random random = new Random();

        static GraphicSetManager()
        {
            hairColours = new List<Color>() { Color.DarkGray, Color.SaddleBrown, Color.SandyBrown, Color.Brown, Color.LightYellow, Color.Silver, Color.OrangeRed, Color.Gray, Color.SlateGray };
            clothingColours = new List<Color>() 
            {
                Color.Red,
                Color.DarkRed,
                Color.Green,
                Color.Olive,
                Color.Brown,
                Color.RosyBrown,
                Color.Blue,
                Color.AliceBlue,
                Color.Purple,
                Color.MediumPurple
            };

            priestClothingColours = new List<Color>()
            {
                Color.White,
                Color.Brown,
                Color.RosyBrown,
                Color.SandyBrown,
                Color.DarkGray
            };
        }

        public static List<SpriteData> GetSprites(GraphicSetName name)
        {
            if (name == GraphicSetName.HUMANMERCHANT)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANMERCHANT_BODY)));
                sprites.Add(SpriteManager.GetSprite(LocalSpriteName.HUMANMERCHANT_HEAD));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANMERCHANT_HAIR)));

                sprites[0].ColorFilter = clothingColours[random.Next(clothingColours.Count)];
                sprites[2].ColorFilter = hairColours[random.Next(hairColours.Count)];

                return sprites;
            }
            else if (name == GraphicSetName.HUMANGUARD)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANGUARD_HAIR)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANGUARD_BODY)));

                sprites[0].ColorFilter = hairColours[random.Next(hairColours.Count)];

                return sprites;
            }
            else if (name == GraphicSetName.HUMANPEASANTFEMALE)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTGIRL_HAIR)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTGIRL_FACE)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTGIRL_DRESS1)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTGIRL_DRESS2)));

                sprites[0].ColorFilter = hairColours[random.Next(hairColours.Count)];
                sprites[2].ColorFilter = clothingColours[random.Next(clothingColours.Count)];
                sprites[3].ColorFilter = clothingColours[random.Next(clothingColours.Count)];

                return sprites;
            }
            else if (name == GraphicSetName.HUMANPEASANTMALE)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTMALE_HAIR)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTMALE_FACE)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTMALE_TOP)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.HUMANPEASANTMALE_PANTS)));

                sprites[0].ColorFilter = hairColours[random.Next(hairColours.Count)];
                sprites[2].ColorFilter = clothingColours[random.Next(clothingColours.Count)];
                sprites[3].ColorFilter = clothingColours[random.Next(clothingColours.Count)];

                return sprites;
            }
            else if (name == GraphicSetName.HUMANRICHMALE)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.RICHMALE_HAIR)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.RICHMALE_FACE)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.RICHMALE_CLOTHES)));

                sprites[0].ColorFilter = hairColours[random.Next(hairColours.Count)];
                sprites[2].ColorFilter = clothingColours[random.Next(clothingColours.Count)];

                return sprites;
            }
            else if (name == GraphicSetName.HUMANRICHFEMALE)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.RICHFEMALE_HAIR)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.RICHFEMALE_FACE)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.RICHFEMALE_CLOTHES)));

                sprites[0].ColorFilter = hairColours[random.Next(hairColours.Count)];
                sprites[2].ColorFilter = clothingColours[random.Next(clothingColours.Count)];

                return sprites;
            }
            else if (name == GraphicSetName.HUMANPRIEST)
            {
                List<SpriteData> sprites = new List<SpriteData>();

                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.PRIEST_BODY)));
                sprites.Add(new SpriteData(SpriteManager.GetSprite(LocalSpriteName.PRIEST_CLOTHES)));

                sprites[1].ColorFilter = priestClothingColours[random.Next(priestClothingColours.Count)];

                return sprites;
            }

            throw new NotImplementedException("No code for the " + name + " was found");
        }

    }
}
