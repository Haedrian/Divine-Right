﻿using System;
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

            throw new NotImplementedException("No code for the " + name + " was found");
        }

    }
}