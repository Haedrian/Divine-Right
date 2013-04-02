using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Graphics
{
    /// <summary>
    /// Used to obtain Sprite Data from an enum
    /// </summary>
    public class SpriteManager
    {
        private static SpriteData[] localSprites;
        private static SpriteData[] globalSprites;
        private static SpriteData[] colourSprites;

        static SpriteManager()
        {
            localSprites = new SpriteData[100]; //TODO: INCREASE WHEN YOU HAVE MORE
            globalSprites = new SpriteData[100];
            colourSprites = new SpriteData[100];

            globalSprites[(int)GlobalSpriteName.BIGTREE] = new SpriteData( @"Graphics/World/BigTree");
            globalSprites[(int)GlobalSpriteName.DEADTREE] = new SpriteData(@"Graphics/World/DeadTree");
            globalSprites[(int)GlobalSpriteName.DESERTTILE] = new SpriteData( @"Graphics/World/Tiles/DesertTile");
            globalSprites[(int)GlobalSpriteName.FORESTTILE] = new SpriteData(@"Graphics/World/Tiles/ForestTile");
            globalSprites[(int)GlobalSpriteName.GARIGUETILE] = new SpriteData(@"Graphics/World/Tiles/GarigueTile");
            globalSprites[(int)GlobalSpriteName.GRASSTILE] = new SpriteData(@"Graphics/World/Tiles/GrassTile");
            globalSprites[(int)GlobalSpriteName.HILLSLOPE] = new SpriteData(@"Graphics/World/Hill");
            globalSprites[(int)GlobalSpriteName.MOUNTAIN] = new SpriteData(@"Graphics/World/Mountain");
            globalSprites[(int)GlobalSpriteName.RIVER] = new SpriteData(@"Graphics/World/River");
            globalSprites[(int)GlobalSpriteName.SNOWTILE] = new SpriteData(@"Graphics/World/Tiles/SnowTile");
            globalSprites[(int)GlobalSpriteName.SWAMPTILE] = new SpriteData(@"Graphics/World/Tiles/SwampTile");
            globalSprites[(int)GlobalSpriteName.TREE] = new SpriteData(@"Graphics/World/Tree");
            globalSprites[(int)GlobalSpriteName.TROPICALTREE] = new SpriteData(@"Graphics/World/TropicalTree");
            globalSprites[(int)GlobalSpriteName.WATERTILE] = new SpriteData(@"Graphics/World/Tiles/WaterTile");
            globalSprites[(int)GlobalSpriteName.HAMLET] = new SpriteData(@"Graphics/World/Settlements/SmallSettlement");
            globalSprites[(int)GlobalSpriteName.VILLAGE] = new SpriteData(@"Graphics/World/Settlements/MediumSettlement");
            globalSprites[(int) GlobalSpriteName.TOWN] = new SpriteData(@"Graphics/World/Settlements/LargeSettlement");

            colourSprites[(int)ColourSpriteName.BROWN] = new SpriteData(@"Graphics/World/Overlay/Regions/Brown");
            colourSprites[(int)ColourSpriteName.GREEN] = new SpriteData(@"Graphics/World/Overlay/Regions/Green");
            colourSprites[(int)ColourSpriteName.INDIGO] = new SpriteData(@"Graphics/World/Overlay/Regions/Indigo");
            colourSprites[(int)ColourSpriteName.MARBLEBLUE] = new SpriteData(@"Graphics/World/Overlay/Regions/MarbleBlue");
            colourSprites[(int)ColourSpriteName.ORANGE] = new SpriteData(@"Graphics/World/Overlay/Regions/Orange");
            colourSprites[(int)ColourSpriteName.PINK] = new SpriteData(@"Graphics/World/Overlay/Regions/Pink");
            colourSprites[(int)ColourSpriteName.PURPLE] = new SpriteData(@"Graphics/World/Overlay/Regions/Purple");
            colourSprites[(int)ColourSpriteName.RED] = new SpriteData(@"Graphics/World/Overlay/Regions/Red");
            colourSprites[(int)ColourSpriteName.WHITE] = new SpriteData(@"Graphics/World/Overlay/Regions/White");
            colourSprites[(int)ColourSpriteName.YELLOW] = new SpriteData(@"Graphics/World/Overlay/Regions/Yellow");

            localSprites[(int)LocalSpriteName.PLAYERCHAR] = new SpriteData(@"Graphics/Local/Player");
            localSprites[(int)LocalSpriteName.CASTLEWALL] = new SpriteData(@"CastleWall");
            localSprites[(int)LocalSpriteName.GRASSTILE] = new SpriteData(@"GrassTile");
            localSprites[(int)LocalSpriteName.LLAMA] = new SpriteData(@"Llama");
            localSprites[(int)LocalSpriteName.PAVEMENTTILE] = new SpriteData(@"PavementTile");
            localSprites[(int)LocalSpriteName.WOODTILE] = new SpriteData(@"WoodTile");
        }

        public static SpriteData GetSprite(GlobalSpriteName name)
        {
            return globalSprites[(int)name];
        }

        public static SpriteData GetSprite(LocalSpriteName name)
        {
            return localSprites[(int)name];
        }

        public static SpriteData GetSprite(ColourSpriteName name)
        {
            return colourSprites[(int)name];
        }
    }
}
