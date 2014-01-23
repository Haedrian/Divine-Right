using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.Graphics
{
    /// <summary>
    /// Used to obtain Sprite Data from an enum
    /// </summary>
    public class SpriteManager
    {
        #region Constants

        private static string HOUSEITEM = @"Graphics/Local/HouseItems";
        private static string RESOURCES_AND_TOOLS = @"Graphics/Local/ResourcesAndTools";
        private static string TILES = @"Graphics/Local/TilesCropped";
        private static string FORESTS_AND_MINES = @"Graphics/Local/ForestsAndMines";
        private static string OUTDOORS = @"Graphics/Local/OutdoorItems";
        private static string DUNGEON = @"Graphics/Local/dungeonitems";

        #endregion


        private static SpriteData[] localSprites;
        private static SpriteData[] globalSprites;
        private static SpriteData[] colourSprites;
        private static SpriteData[] interfaceSprites;

        static SpriteManager()
        {
            localSprites = new SpriteData[400]; //TODO: INCREASE WHEN YOU HAVE MORE
            globalSprites = new SpriteData[100];
            colourSprites = new SpriteData[100];
            interfaceSprites = new SpriteData[100];

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

            localSprites[(int)LocalSpriteName.SOIL_TILE] = new SpriteData(TILES, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WET_SOIL_TILE] = new SpriteData(TILES, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.GRASS_TILE] = new SpriteData(TILES, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_TILE_1] = new SpriteData(TILES, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.ROAD_TILE] = new SpriteData(TILES, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.CAVE_TILE] = new SpriteData(TILES, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.MARBLE_TILE] = new SpriteData(TILES, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.PAVEMENT_TILE_1] = new SpriteData(TILES, new Rectangle(350, 0, 50, 50));
            localSprites[(int)LocalSpriteName.PAVEMENT_TILE_2] = new SpriteData(TILES, new Rectangle(400, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_TILE_2] = new SpriteData(TILES, new Rectangle(450, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_TILE_3] = new SpriteData(TILES, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_TILE_4] = new SpriteData(TILES, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_TILE_5] = new SpriteData(TILES, new Rectangle(100, 50, 50, 50));
            localSprites[(int)LocalSpriteName.DUNGEON_TILE] = new SpriteData(TILES, new Rectangle(150, 50, 50, 50));
            localSprites[(int)LocalSpriteName.NONE] = new SpriteData(TILES, new Rectangle(200, 50, 1, 1));

            //todo: dungeons

            localSprites[(int)LocalSpriteName.PLAYERCHAR] = new SpriteData(@"Graphics/Local/Player");
            localSprites[(int)LocalSpriteName.BLUE_BED] = new SpriteData(HOUSEITEM, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_CHAIR_LEFT] = new SpriteData(HOUSEITEM, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_CHAIR_RIGHT] = new SpriteData(HOUSEITEM, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_CHAIR_BOTTOM] = new SpriteData(HOUSEITEM, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_CHAIR_TOP] = new SpriteData(HOUSEITEM, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.EMPTY_SHELFING] = new SpriteData(HOUSEITEM, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.BOOKCASE_FULL] = new SpriteData(HOUSEITEM, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.BOOKCASE_HALF_EMPTY] = new SpriteData(HOUSEITEM, new Rectangle(350, 0, 50, 50));
            localSprites[(int)LocalSpriteName.GLASSWARE_CABINET] = new SpriteData(HOUSEITEM, new Rectangle(400, 0, 50, 50));
            localSprites[(int)LocalSpriteName.SMALL_RUG] = new SpriteData(HOUSEITEM, new Rectangle(450, 0, 50, 50));
            localSprites[(int)LocalSpriteName.CHEST_OF_DRAWERS] = new SpriteData(HOUSEITEM, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WARDROBE_1] = new SpriteData(HOUSEITEM, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WARDROBE_2] = new SpriteData(HOUSEITEM, new Rectangle(100, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WARDROBE_3] = new SpriteData(HOUSEITEM, new Rectangle(200, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WARDROBE_4] = new SpriteData(HOUSEITEM, new Rectangle(150, 50, 50, 50));
            localSprites[(int)LocalSpriteName.STOVE_UNLIT] = new SpriteData(HOUSEITEM, new Rectangle(250, 50, 50, 50));
            localSprites[(int)LocalSpriteName.STOVE_LIT] = new SpriteData(HOUSEITEM, new Rectangle(300, 50, 50, 50));
            localSprites[(int)LocalSpriteName.PUMP] = new SpriteData(HOUSEITEM, new Rectangle(350, 50, 50, 50));
            localSprites[(int)LocalSpriteName.DISHES] = new SpriteData(HOUSEITEM, new Rectangle(400, 50, 50, 50));
            localSprites[(int)LocalSpriteName.TABLE_1] = new SpriteData(HOUSEITEM, new Rectangle(450, 50, 50, 50));
            localSprites[(int)LocalSpriteName.DOOR_CLOSED] = new SpriteData(HOUSEITEM, new Rectangle(50, 100, 32, 47));
            localSprites[(int)LocalSpriteName.DOOR_OPEN] = new SpriteData(HOUSEITEM, new Rectangle(100, 100, 50, 49));
            localSprites[(int)LocalSpriteName.FOUNTAIN_SMALL] = new SpriteData(HOUSEITEM, new Rectangle(150, 100, 50, 50));
            localSprites[(int)LocalSpriteName.STATUE_AXEMAN] = new SpriteData(HOUSEITEM, new Rectangle(200, 100, 50, 50));
            localSprites[(int)LocalSpriteName.WHITE_BED] = new SpriteData(HOUSEITEM, new Rectangle(250, 100, 50, 50));
            localSprites[(int)LocalSpriteName.PLANT_POT_1] = new SpriteData(HOUSEITEM, new Rectangle(300, 100, 50, 50));
            localSprites[(int)LocalSpriteName.PLANT_POT_2] = new SpriteData(HOUSEITEM, new Rectangle(350, 100, 50, 50));
            localSprites[(int)LocalSpriteName.TABLE_2] = new SpriteData(HOUSEITEM, new Rectangle(400, 100, 50, 50));
            localSprites[(int)LocalSpriteName.BEARSKIN_RUG] = new SpriteData(HOUSEITEM, new Rectangle(450, 100, 50, 50));
            localSprites[(int)LocalSpriteName.TABLE_3] = new SpriteData(HOUSEITEM, new Rectangle(0, 150, 50, 50));
            localSprites[(int)LocalSpriteName.BOOKCASE_LARGE] = new SpriteData(HOUSEITEM, new Rectangle(50, 150, 50, 50));
            localSprites[(int)LocalSpriteName.BLUE_WALL] = new SpriteData(HOUSEITEM, new Rectangle(101,150,48,48));
            localSprites[(int)LocalSpriteName.POTS] = new SpriteData(HOUSEITEM, new Rectangle(150, 150, 50, 50));
            localSprites[(int)LocalSpriteName.RED_BED_DOUBLE] = new SpriteData(HOUSEITEM, new Rectangle(200, 150, 50, 50));
            localSprites[(int)LocalSpriteName.RED_BED] = new SpriteData(HOUSEITEM, new Rectangle(250, 150, 50, 50));
            localSprites[(int)LocalSpriteName.TABLE_4] = new SpriteData(HOUSEITEM, new Rectangle(300, 150, 50, 50));
            localSprites[(int)LocalSpriteName.LAMP_1] = new SpriteData(HOUSEITEM, new Rectangle(350, 150, 50, 50));
            localSprites[(int)LocalSpriteName.LAMP_2] = new SpriteData(HOUSEITEM, new Rectangle(400, 150, 50, 50));
            localSprites[(int)LocalSpriteName.PLANT_POT_3] = new SpriteData(HOUSEITEM, new Rectangle(450, 150, 50, 50));
            localSprites[(int)LocalSpriteName.NICE_CHAIR_RIGHT] = new SpriteData(HOUSEITEM, new Rectangle(50, 200, 50, 50));
            localSprites[(int)LocalSpriteName.NICE_CHAIR_LEFT] = new SpriteData(HOUSEITEM, new Rectangle(0, 200, 50, 50));
            localSprites[(int)LocalSpriteName.NICE_CHAIR_TOP] = new SpriteData(HOUSEITEM, new Rectangle(100, 200, 50, 50));
            localSprites[(int)LocalSpriteName.NICE_CHAIR_BOTTOM] = new SpriteData(HOUSEITEM, new Rectangle(150, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BLUE_POT_LARGE] = new SpriteData(HOUSEITEM, new Rectangle(200, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BARSTOOL] = new SpriteData(HOUSEITEM, new Rectangle(250, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BLUE_POT_SMALL] = new SpriteData(HOUSEITEM, new Rectangle(300, 200, 50, 50));
            localSprites[(int)LocalSpriteName.DARK_RED_WALL] = new SpriteData(HOUSEITEM, new Rectangle(350, 200, 49, 50));
            localSprites[(int)LocalSpriteName.LIGHT_RED_WALL] = new SpriteData(HOUSEITEM, new Rectangle(400, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BARREL] = new SpriteData(HOUSEITEM, new Rectangle(450, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BARRELS] = new SpriteData(HOUSEITEM, new Rectangle(50, 250, 50, 50));
            localSprites[(int)LocalSpriteName.CHEST_CLOSED] = new SpriteData(HOUSEITEM, new Rectangle(100, 250, 50, 50));
            localSprites[(int)LocalSpriteName.CHEST_OPEN] = new SpriteData(HOUSEITEM, new Rectangle(150, 250, 50, 50));

            localSprites[(int)LocalSpriteName.BOOKCASE_1] = new SpriteData(HOUSEITEM, new Rectangle(200, 250, 50, 50));
            localSprites[(int)LocalSpriteName.BOOKCASE_2] = new SpriteData(HOUSEITEM, new Rectangle(250, 250, 50, 50));
            localSprites[(int)LocalSpriteName.BOOKCASE_3] = new SpriteData(HOUSEITEM, new Rectangle(300, 250, 50, 50));
            localSprites[(int)LocalSpriteName.DESK_1] = new SpriteData(HOUSEITEM, new Rectangle(350, 250, 50, 50));
            localSprites[(int)LocalSpriteName.PLANT_CABINET_1] = new SpriteData(HOUSEITEM, new Rectangle(400, 250, 50, 50));
            localSprites[(int)LocalSpriteName.BOOKCASE_4] = new SpriteData(HOUSEITEM, new Rectangle(450, 250, 50, 50));
            localSprites[(int)LocalSpriteName.DESK_2] = new SpriteData(HOUSEITEM, new Rectangle(0, 300, 50, 50));
            localSprites[(int)LocalSpriteName.DESK_3] = new SpriteData(HOUSEITEM, new Rectangle(50, 300, 50, 50));
            localSprites[(int)LocalSpriteName.PLANT_CABINET_2] = new SpriteData(HOUSEITEM, new Rectangle(100, 300, 50, 50));
            localSprites[(int)LocalSpriteName.PLANT_CABINET_3] = new SpriteData(HOUSEITEM, new Rectangle(150, 300, 50, 50));

            localSprites[(int)LocalSpriteName.WINDOW_1] = new SpriteData(HOUSEITEM, new Rectangle(200, 300, 50, 50));
            localSprites[(int)LocalSpriteName.WINDOW_2] = new SpriteData(HOUSEITEM, new Rectangle(250, 300, 50, 50));
            localSprites[(int)LocalSpriteName.WINDOW_3] = new SpriteData(HOUSEITEM, new Rectangle(300, 300, 50, 50));

            localSprites[(int)LocalSpriteName.DOOR_1_CLOSED] = new SpriteData(HOUSEITEM, new Rectangle(350, 300, 31, 41));
            localSprites[(int)LocalSpriteName.DOOR_1_OPEN] = new SpriteData(HOUSEITEM, new Rectangle(400, 301, 30, 42));
            localSprites[(int)LocalSpriteName.DOOR_2_CLOSED] = new SpriteData(HOUSEITEM, new Rectangle(451, 300, 32, 45));
            localSprites[(int)LocalSpriteName.DOOR_2_OPEN] = new SpriteData(HOUSEITEM, new Rectangle(2, 350, 32, 45));

            localSprites[(int)LocalSpriteName.CHEST_OF_DRAWERS_1] = new SpriteData(HOUSEITEM, new Rectangle(50, 350, 50, 50));
            localSprites[(int)LocalSpriteName.CHEST_OF_DRAWERS_2] = new SpriteData(HOUSEITEM, new Rectangle(100, 350, 50, 50));
            localSprites[(int)LocalSpriteName.CHEST_OF_DRAWERS_3] = new SpriteData(HOUSEITEM, new Rectangle(150, 350, 50, 50));
            localSprites[(int)LocalSpriteName.CHEST_OF_DRAWERS_4] = new SpriteData(HOUSEITEM, new Rectangle(200, 350, 50, 50));

            localSprites[(int)LocalSpriteName.POT_PLANT_1] = new SpriteData(HOUSEITEM, new Rectangle(300, 350, 50, 50));
            localSprites[(int)LocalSpriteName.POT_PLANT_2] = new SpriteData(HOUSEITEM, new Rectangle(350, 350, 50, 50));
            localSprites[(int)LocalSpriteName.POT_PLANT_3] = new SpriteData(HOUSEITEM, new Rectangle(400, 350, 50, 50));
            localSprites[(int)LocalSpriteName.POT_PLANT_4] = new SpriteData(HOUSEITEM, new Rectangle(450, 350, 50, 50));

            localSprites[(int)LocalSpriteName.CABINET_1] = new SpriteData(HOUSEITEM, new Rectangle(250, 350, 50, 50));

            localSprites[(int)LocalSpriteName.BATH] = new SpriteData(HOUSEITEM, new Rectangle(0, 400, 50, 50));

            localSprites[(int)LocalSpriteName.CARPET_1] = new SpriteData(HOUSEITEM, new Rectangle(50, 400, 50, 50));
            localSprites[(int)LocalSpriteName.CARPET_2] = new SpriteData(HOUSEITEM, new Rectangle(100, 400, 50, 50));
            localSprites[(int)LocalSpriteName.CARPET_3] = new SpriteData(HOUSEITEM, new Rectangle(150, 400, 50, 50));

            localSprites[(int)LocalSpriteName.RED_BED_TOP] = new SpriteData(HOUSEITEM, new Rectangle(200, 400, 50, 50));
            localSprites[(int)LocalSpriteName.RED_BED_BOTTOM] = new SpriteData(HOUSEITEM, new Rectangle(200, 450, 50, 50));

            localSprites[(int)LocalSpriteName.BUCKET] = new SpriteData(HOUSEITEM, new Rectangle(250, 400, 50, 50));

            localSprites[(int)LocalSpriteName.BAR] = new SpriteData(HOUSEITEM, new Rectangle(300, 400, 50, 50));

            localSprites[(int)LocalSpriteName.ANGEL_STATUE_TOP] = new SpriteData(HOUSEITEM, new Rectangle(350, 400, 50, 50));
            localSprites[(int)LocalSpriteName.ANGEL_STATUE_BOTTOM] = new SpriteData(HOUSEITEM, new Rectangle(350, 450, 50, 50));

            localSprites[(int)LocalSpriteName.POT_PLANT_5] = new SpriteData(HOUSEITEM, new Rectangle(100, 450, 50, 50));
            localSprites[(int)LocalSpriteName.FOOD_CABINET_1] = new SpriteData(HOUSEITEM, new Rectangle(0, 450, 50, 50));
            localSprites[(int)LocalSpriteName.FOOD_CABINET_2] = new SpriteData(HOUSEITEM, new Rectangle(50, 450, 50, 50));

            localSprites[(int)LocalSpriteName.LARGE_TABLE_TL] = new SpriteData(HOUSEITEM, new Rectangle(400, 400, 50, 50));
            localSprites[(int)LocalSpriteName.LARGE_TABLE_TR] = new SpriteData(HOUSEITEM, new Rectangle(450, 400, 50, 50));
            localSprites[(int)LocalSpriteName.LARGE_TABLE_BL] = new SpriteData(HOUSEITEM, new Rectangle(400, 450, 50, 50));
            localSprites[(int)LocalSpriteName.LARGE_TABLE_BR] = new SpriteData(HOUSEITEM, new Rectangle(450, 450, 50, 50));

            localSprites[(int)LocalSpriteName.LARGE_TABLE_TEA_TL] = new SpriteData(HOUSEITEM, new Rectangle(0, 500, 50, 50));
            localSprites[(int)LocalSpriteName.LARGE_TABLE_TEA_TR] = new SpriteData(HOUSEITEM, new Rectangle(50, 500, 50, 50));
            localSprites[(int)LocalSpriteName.LARGE_TABLE_TEA_BL] = new SpriteData(HOUSEITEM, new Rectangle(0, 550, 50, 50));
            localSprites[(int)LocalSpriteName.LARGE_TABLE_TEA_BR] = new SpriteData(HOUSEITEM, new Rectangle(50, 550, 50, 50));


            localSprites[(int)LocalSpriteName.WOOD_LOGS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.WOOD_LOGS_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOUR_SACK] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOUR_SACK_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.GRAIN_OPEN] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.GRAIN_OPEN_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.GREEN_OPEN] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.GREEN_OPEN_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(350, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STEAK] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(400, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STEAK_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(450, 0, 50, 50));
            localSprites[(int)LocalSpriteName.DRUMSTICKS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.DRUMSTICKS_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.CHICKEN] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 50, 50, 50));
            localSprites[(int)LocalSpriteName.CHICKEN_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 50, 50, 50));
            localSprites[(int)LocalSpriteName.MEAT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 50, 50, 50));
            localSprites[(int)LocalSpriteName.MEAT_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 50, 50, 50));
            localSprites[(int)LocalSpriteName.ANVIL] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 50, 50, 50));
            localSprites[(int)LocalSpriteName.ANVIL_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(350, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SMITH_TOOLS_1] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(400, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SMITH_TOOLS_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(450, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SMITH_TOOLS_3] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 100, 50, 50));
            localSprites[(int)LocalSpriteName.EMPTY_STALL] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_1] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_1] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_3] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_3] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(350, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_4] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(400, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_4] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(450, 100, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_5] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 150, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_5] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 150, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_6] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 150, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_6] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 150, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_7] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 150, 50, 50));
            localSprites[(int)LocalSpriteName.VEG_STALL_7] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FISH_1] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FISH_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(350, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FISH_3] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(400, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FISH_4] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(450, 150, 50, 50));
            localSprites[(int)LocalSpriteName.GOLD_BARS_1] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 200, 50, 50));
            localSprites[(int)LocalSpriteName.GOLD_BARS_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BREAD_THREE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BREAD_TWO] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 200, 50, 50));
            localSprites[(int)LocalSpriteName.FLOUR_MILL] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 200, 50, 50));
            localSprites[(int)LocalSpriteName.BOXES] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 200, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_STUMPS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 200, 50, 50));
            localSprites[(int)LocalSpriteName.PICKAXE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(350, 200, 50, 50));
            localSprites[(int)LocalSpriteName.HATCHET_SHOVEL] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(400, 200, 50, 50));
            localSprites[(int)LocalSpriteName.MILK_JUGS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(450, 200, 50, 50));
            localSprites[(int)LocalSpriteName.OIL_JARS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 250, 50, 50));
            localSprites[(int)LocalSpriteName.GOLD_ORE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 250, 50, 50));
            localSprites[(int)LocalSpriteName.IRON_ORE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 250, 50, 50));
            localSprites[(int)LocalSpriteName.COPPER_ORE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 250, 50, 50));
            localSprites[(int)LocalSpriteName.MINING_TOOLS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 250, 50, 50));
            localSprites[(int)LocalSpriteName.FURNACE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 250, 50, 50));
            localSprites[(int)LocalSpriteName.IRON_BARS_1] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(350, 250, 50, 50));
            localSprites[(int)LocalSpriteName.IRON_BARS_2] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(400, 250, 50, 50));

            
            localSprites[(int)LocalSpriteName.TOMATO_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 300, 50, 50));
            localSprites[(int)LocalSpriteName.POTATO_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 300, 50, 50));
            localSprites[(int)LocalSpriteName.CARROT_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 300, 50, 50));
            localSprites[(int)LocalSpriteName.ARTICHOKE_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 300, 50, 50));
            localSprites[(int)LocalSpriteName.PEPPER_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200,300,50,50));
            localSprites[(int)LocalSpriteName.COURGETTE_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 300, 50, 50));
            localSprites[(int)LocalSpriteName.CORN_PLANT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 300, 50, 50));
            localSprites[(int)LocalSpriteName.TOMATO] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(0, 350, 50, 50));
            localSprites[(int)LocalSpriteName.POTATO] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(50, 350, 50, 50));
            localSprites[(int)LocalSpriteName.CARROT] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(100, 350, 50, 50));
            localSprites[(int)LocalSpriteName.ARTICHOKE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(150, 350, 50, 50));
            localSprites[(int)LocalSpriteName.PEPPERS] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(200, 350, 50, 50));
            localSprites[(int)LocalSpriteName.COURGETTE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(250, 350, 50, 50));
            localSprites[(int)LocalSpriteName.CORN] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(300, 350, 50, 50));

            localSprites[(int)LocalSpriteName.TREE_1] = new SpriteData(FORESTS_AND_MINES, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.DEAD_TREE] = new SpriteData(FORESTS_AND_MINES, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_2] = new SpriteData(FORESTS_AND_MINES, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_3] = new SpriteData(FORESTS_AND_MINES, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_4] = new SpriteData(FORESTS_AND_MINES, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_5] = new SpriteData(FORESTS_AND_MINES, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_6] = new SpriteData(FORESTS_AND_MINES, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_7] = new SpriteData(FORESTS_AND_MINES, new Rectangle(350, 0, 50, 50));

            localSprites[(int)LocalSpriteName.FLOWER_1] = new SpriteData(OUTDOORS, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_2] = new SpriteData(OUTDOORS, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_3] = new SpriteData(OUTDOORS, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_4] = new SpriteData(OUTDOORS, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_5] = new SpriteData(OUTDOORS, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_6] = new SpriteData(OUTDOORS, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_7] = new SpriteData(OUTDOORS, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_8] = new SpriteData(OUTDOORS, new Rectangle(350, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_9] = new SpriteData(OUTDOORS, new Rectangle(400, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_10] = new SpriteData(OUTDOORS, new Rectangle(450, 0, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_11] = new SpriteData(OUTDOORS, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_12] = new SpriteData(OUTDOORS, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_13] = new SpriteData(OUTDOORS, new Rectangle(100, 50, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_14] = new SpriteData(OUTDOORS, new Rectangle(150, 50, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_15] = new SpriteData(OUTDOORS, new Rectangle(200, 50, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_16] = new SpriteData(OUTDOORS, new Rectangle(250, 50, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_17] = new SpriteData(OUTDOORS, new Rectangle(300, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BENCH_LEFT] = new SpriteData(OUTDOORS, new Rectangle(350, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BENCH_RIGHT] = new SpriteData(OUTDOORS, new Rectangle(400, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BENCH_DOWN] = new SpriteData(OUTDOORS, new Rectangle(450, 50, 50, 50));
            
            localSprites[(int)LocalSpriteName.FLOWER_18] = new SpriteData(OUTDOORS, new Rectangle(0, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_19] = new SpriteData(OUTDOORS, new Rectangle(50, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_20] = new SpriteData(OUTDOORS, new Rectangle(100, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_21] = new SpriteData(OUTDOORS, new Rectangle(150, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_22] = new SpriteData(OUTDOORS, new Rectangle(200, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_23] = new SpriteData(OUTDOORS, new Rectangle(250, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FOUNTAIN_1] = new SpriteData(OUTDOORS, new Rectangle(300, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_24] = new SpriteData(OUTDOORS, new Rectangle(350, 100, 50, 50));
            localSprites[(int)LocalSpriteName.BENCH_2_DOWN] = new SpriteData(OUTDOORS, new Rectangle(400, 100, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_25] = new SpriteData(OUTDOORS, new Rectangle(450, 100, 50, 50));

            localSprites[(int)LocalSpriteName.FLOWER_26] = new SpriteData(OUTDOORS, new Rectangle(0, 150, 50, 50));
            localSprites[(int)LocalSpriteName.COLUMN] = new SpriteData(OUTDOORS, new Rectangle(50, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FOUNTAIN_2] = new SpriteData(OUTDOORS, new Rectangle(100, 150, 50, 50));
            localSprites[(int)LocalSpriteName.POTS_1] = new SpriteData(OUTDOORS, new Rectangle(150, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_27] = new SpriteData(OUTDOORS, new Rectangle(200, 150, 50, 50));
            localSprites[(int)LocalSpriteName.GIRL_STATUE] = new SpriteData(OUTDOORS, new Rectangle(250, 150, 50, 50));
            localSprites[(int)LocalSpriteName.WELL_1] = new SpriteData(OUTDOORS, new Rectangle(300, 150, 50, 50));
            localSprites[(int)LocalSpriteName.FLOWER_28] = new SpriteData(OUTDOORS, new Rectangle(350, 150, 50, 50));

            localSprites[(int)LocalSpriteName.STONE_1] = new SpriteData(DUNGEON, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_2] = new SpriteData(DUNGEON, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_3] = new SpriteData(DUNGEON, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_4] = new SpriteData(DUNGEON, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_5] = new SpriteData(DUNGEON, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_6] = new SpriteData(DUNGEON, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_7] = new SpriteData(DUNGEON, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_8] = new SpriteData(DUNGEON, new Rectangle(350, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_9] = new SpriteData(DUNGEON, new Rectangle(400, 0, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_10] = new SpriteData(DUNGEON, new Rectangle(450, 0, 50, 50));

            localSprites[(int)LocalSpriteName.BIG_STONE_1] = new SpriteData(DUNGEON, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_2] = new SpriteData(DUNGEON, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_3] = new SpriteData(DUNGEON, new Rectangle(100, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_4] = new SpriteData(DUNGEON, new Rectangle(150, 50, 50, 50));

            localSprites[(int)LocalSpriteName.BONES_1] = new SpriteData(DUNGEON, new Rectangle(200, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BONES_2] = new SpriteData(DUNGEON, new Rectangle(250, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BONES_3] = new SpriteData(DUNGEON, new Rectangle(300, 50, 50, 50));

            localSprites[(int)LocalSpriteName.BIG_STONE_5] = new SpriteData(DUNGEON, new Rectangle(350, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_6] = new SpriteData(DUNGEON, new Rectangle(400, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_7] = new SpriteData(DUNGEON, new Rectangle(450, 50, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_8] = new SpriteData(DUNGEON, new Rectangle(0, 100, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_9] = new SpriteData(DUNGEON, new Rectangle(50, 100, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONE_10] = new SpriteData(DUNGEON, new Rectangle(100, 100, 50, 50));

            localSprites[(int)LocalSpriteName.STONE_11] = new SpriteData(DUNGEON, new Rectangle(150, 100, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_12] = new SpriteData(DUNGEON, new Rectangle(200, 100, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_13] = new SpriteData(DUNGEON, new Rectangle(250, 100, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_14] = new SpriteData(DUNGEON, new Rectangle(300, 100, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_15] = new SpriteData(DUNGEON, new Rectangle(350, 100, 50, 50));

            localSprites[(int)LocalSpriteName.BIG_STONES_TL] = new SpriteData(DUNGEON, new Rectangle(400, 100, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONES_TR] = new SpriteData(DUNGEON, new Rectangle(450, 100, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONES_BL] = new SpriteData(DUNGEON, new Rectangle(400, 150, 50, 50));
            localSprites[(int)LocalSpriteName.BIG_STONES_BR] = new SpriteData(DUNGEON, new Rectangle(450, 150, 50, 50));

            localSprites[(int)LocalSpriteName.BIG_STONE_11] = new SpriteData(DUNGEON, new Rectangle(0, 150, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_16] = new SpriteData(DUNGEON, new Rectangle(50, 150, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_17] = new SpriteData(DUNGEON, new Rectangle(100, 150, 50, 50));
            localSprites[(int)LocalSpriteName.STONE_18] = new SpriteData(DUNGEON, new Rectangle(150, 150, 50, 50));

            localSprites[(int)LocalSpriteName.BONES_4] = new SpriteData(DUNGEON, new Rectangle(200, 150, 50, 50));
            localSprites[(int)LocalSpriteName.BONES_5] = new SpriteData(DUNGEON, new Rectangle(250, 150, 50, 50));
            localSprites[(int)LocalSpriteName.BONES_6] = new SpriteData(DUNGEON, new Rectangle(300, 150, 50, 50));

            localSprites[(int)LocalSpriteName.DRAGON_STATUE_1] = new SpriteData(DUNGEON, new Rectangle(350, 150, 50, 50));
            localSprites[(int)LocalSpriteName.DRAGON_STATUE_2] = new SpriteData(DUNGEON, new Rectangle(0, 200, 50, 50));

            localSprites[(int)LocalSpriteName.TREASURE_1] = new SpriteData(DUNGEON, new Rectangle(50, 200, 50, 50));
            localSprites[(int)LocalSpriteName.TREASURE_2] = new SpriteData(DUNGEON, new Rectangle(100, 200, 50, 50));
            localSprites[(int)LocalSpriteName.CANDLE_1] = new SpriteData(DUNGEON, new Rectangle(150, 200, 50, 50));
            localSprites[(int)LocalSpriteName.CANDLE_2] = new SpriteData(DUNGEON, new Rectangle(200, 200, 50, 50));
            localSprites[(int)LocalSpriteName.ARMOUR_RACK_1] = new SpriteData(DUNGEON, new Rectangle(250, 200, 50, 50));
            localSprites[(int)LocalSpriteName.ARMOUR_RACK_2] = new SpriteData(DUNGEON, new Rectangle(300, 200, 50, 50));
            localSprites[(int)LocalSpriteName.WEAPON_RACK_1] = new SpriteData(DUNGEON, new Rectangle(350, 200, 50, 50));
            localSprites[(int)LocalSpriteName.WEAPON_RACK_2] = new SpriteData(DUNGEON, new Rectangle(400, 200, 50, 50));

            interfaceSprites[(int)InterfaceSpriteName.SCROLL] = new SpriteData(@"Graphics/Interface/scrollsandblocks", new Rectangle(224, 190, 96, 34));
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

        public static SpriteData GetSprite(InterfaceSpriteName name)
        {
            return interfaceSprites[(int)name];
        }
    }
}
