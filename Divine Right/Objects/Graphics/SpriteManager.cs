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

        private static string WORLDITEMS = @"Graphics/World/WorldItems";
        private static string HOUSEITEM = @"Graphics/Local/HouseItems";
        private static string RESOURCES_AND_TOOLS = @"Graphics/Local/ResourcesAndTools";
        private static string TILES = @"Graphics/Local/TilesCropped";
        private static string FORESTS_AND_MINES = @"Graphics/Local/ForestsAndMines";
        private static string OUTDOORS = @"Graphics/Local/OutdoorItems";
        private static string DUNGEON = @"Graphics/Local/dungeonitems";
        private static string ENEMIES = @"Graphics/Local/enemies";
        private static string HEALTH = @"Graphics/Interface/health2";
        private static string CHARACTER_SHEET = @"Graphics/Interface/CharacterSheetIcons";
        private static string BANNERS = @"Graphics/Interface/Banners";
        private static string LOCATIONINTERFACE = @"Graphics/Interface/locationInterface";
        private static string GOTOWORLDMAP = @"Graphics/Interface/gotoWorldMap";
        private static string TIMEICONS = @"Graphics/Interface/timeIcons";
        private static string BUYSELLICONS = @"Graphics/Interface/buySellIcons";
        private static string ANIMALS = @"Graphics/Local/animals";

        private static string MERCHANT = @"Graphics/Local/merchant";
        private static string GUARD = @"Graphics/Local/humanguard";
        private static string PEASANTWOMAN = @"Graphics/Local/peasantfemale";
        private static string PEASANTMALE = @"Graphics/Local/peasantmale";
        private static string RICHMAN = @"Graphics/Local/richmale";
        private static string RICHFEMALE = @"Graphics/Local/richwoman";
        private static string PRIEST = @"Graphics/Local/priest";

        private static string INVENTORYITEMS = @"Graphics/Local/inventoryItems";

        #endregion

        private static SpriteData[] localSprites;
        private static SpriteData[] globalSprites;
        private static SpriteData[] colourSprites;
        private static SpriteData[] interfaceSprites;

        static SpriteManager()
        {
            localSprites = new SpriteData[800]; //TODO: INCREASE WHEN YOU HAVE MORE
            globalSprites = new SpriteData[100];
            colourSprites = new SpriteData[100];
            interfaceSprites = new SpriteData[100];

            globalSprites[(int)GlobalSpriteName.BIGTREE] = new SpriteData(WORLDITEMS,new Rectangle(650,100,50,50));
            globalSprites[(int)GlobalSpriteName.DEADTREE] = new SpriteData(WORLDITEMS, new Rectangle(250, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.DESERTTILE] = new SpriteData( @"Graphics/World/Tiles/DesertTile");
            globalSprites[(int)GlobalSpriteName.FORESTTILE] = new SpriteData(@"Graphics/World/Tiles/ForestTile");
            globalSprites[(int)GlobalSpriteName.GARIGUETILE] = new SpriteData(@"Graphics/World/Tiles/GarigueTile");
            globalSprites[(int)GlobalSpriteName.GRASSTILE] = new SpriteData(@"Graphics/World/Tiles/GrassTile");
            globalSprites[(int)GlobalSpriteName.HILLSLOPE] = new SpriteData(WORLDITEMS,new Rectangle(150,0,50,50));
            globalSprites[(int)GlobalSpriteName.MOUNTAIN] = new SpriteData(WORLDITEMS, new Rectangle(200, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.RIVER] = new SpriteData(@"Graphics/World/River");
            globalSprites[(int)GlobalSpriteName.SNOWTILE] = new SpriteData(@"Graphics/World/Tiles/SnowTile");
            globalSprites[(int)GlobalSpriteName.SWAMPTILE] = new SpriteData(@"Graphics/World/Tiles/SwampTile");
            globalSprites[(int)GlobalSpriteName.TREE] = new SpriteData(WORLDITEMS, new Rectangle(250, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.TROPICALTREE] = new SpriteData(WORLDITEMS, new Rectangle(650, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.WATERTILE] = new SpriteData(@"Graphics/World/Tiles/WaterTile");
            globalSprites[(int)GlobalSpriteName.HAMLET] = new SpriteData(WORLDITEMS, new Rectangle(600, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE] = new SpriteData(WORLDITEMS, new Rectangle(600, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL] = new SpriteData(WORLDITEMS, new Rectangle(600, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.HORSES] = new SpriteData(WORLDITEMS, new Rectangle(700, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.GAME] = new SpriteData(WORLDITEMS, new Rectangle(750, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.FARMLAND] = new SpriteData(WORLDITEMS, new Rectangle(800, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.STONES] = new SpriteData(WORLDITEMS, new Rectangle(850, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON] = new SpriteData(WORLDITEMS, new Rectangle(150, 50, 50, 50));

            globalSprites[(int)GlobalSpriteName.DUNGEON_1] = new SpriteData(WORLDITEMS, new Rectangle(900, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_2] = new SpriteData(WORLDITEMS, new Rectangle(950, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_3] = new SpriteData(WORLDITEMS, new Rectangle(1000, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_4] = new SpriteData(WORLDITEMS, new Rectangle(900, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_5] = new SpriteData(WORLDITEMS, new Rectangle(950, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_6] = new SpriteData(WORLDITEMS, new Rectangle(1000, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_7] = new SpriteData(WORLDITEMS, new Rectangle(900, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_8] = new SpriteData(WORLDITEMS, new Rectangle(950, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.DUNGEON_9] = new SpriteData(WORLDITEMS, new Rectangle(1000, 100, 50, 50));

            globalSprites[(int)GlobalSpriteName.CAPITAL_1] = new SpriteData(WORLDITEMS, new Rectangle(0, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_2] = new SpriteData(WORLDITEMS, new Rectangle(50, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_3] = new SpriteData(WORLDITEMS, new Rectangle(100, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_4] = new SpriteData(WORLDITEMS, new Rectangle(0, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_5] = new SpriteData(WORLDITEMS, new Rectangle(50, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_6] = new SpriteData(WORLDITEMS, new Rectangle(100, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_7] = new SpriteData(WORLDITEMS, new Rectangle(0, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_8] = new SpriteData(WORLDITEMS, new Rectangle(50, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.CAPITAL_9] = new SpriteData(WORLDITEMS, new Rectangle(100, 100, 50, 50));

            globalSprites[(int)GlobalSpriteName.HAMLET_1] = new SpriteData(WORLDITEMS, new Rectangle(300, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_2] = new SpriteData(WORLDITEMS, new Rectangle(350, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_3] = new SpriteData(WORLDITEMS, new Rectangle(400, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_4] = new SpriteData(WORLDITEMS, new Rectangle(300, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_5] = new SpriteData(WORLDITEMS, new Rectangle(350, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_6] = new SpriteData(WORLDITEMS, new Rectangle(400, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_7] = new SpriteData(WORLDITEMS, new Rectangle(300, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_8] = new SpriteData(WORLDITEMS, new Rectangle(350, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.HAMLET_9] = new SpriteData(WORLDITEMS, new Rectangle(400, 100, 50, 50));

            globalSprites[(int)GlobalSpriteName.VILLAGE_1] = new SpriteData(WORLDITEMS, new Rectangle(450, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_2] = new SpriteData(WORLDITEMS, new Rectangle(500, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_3] = new SpriteData(WORLDITEMS, new Rectangle(550, 0, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_4] = new SpriteData(WORLDITEMS, new Rectangle(450, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_5] = new SpriteData(WORLDITEMS, new Rectangle(500, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_6] = new SpriteData(WORLDITEMS, new Rectangle(550, 50, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_7] = new SpriteData(WORLDITEMS, new Rectangle(450, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_8] = new SpriteData(WORLDITEMS, new Rectangle(500, 100, 50, 50));
            globalSprites[(int)GlobalSpriteName.VILLAGE_9] = new SpriteData(WORLDITEMS, new Rectangle(550, 100, 50, 50));

            globalSprites[(int)GlobalSpriteName.BANDIT_CAMP] = new SpriteData(WORLDITEMS, new Rectangle(700, 50, 50, 50));

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
            localSprites[(int)LocalSpriteName.LAVA_TILE] = new SpriteData(TILES, new Rectangle(200, 50, 50, 50));
            localSprites[(int)LocalSpriteName.WATER_TILE] = new SpriteData(TILES, new Rectangle(250, 50, 50, 50));
            localSprites[(int)LocalSpriteName.JUNGLE_TILE] = new SpriteData(TILES, new Rectangle(300, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SAND] = new SpriteData(TILES, new Rectangle(350, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SNOW] = new SpriteData(TILES, new Rectangle(400, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SNOW_2] = new SpriteData(TILES, new Rectangle(450, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SWAMP] = new SpriteData(TILES, new Rectangle(0, 100, 50, 50));

            localSprites[(int)LocalSpriteName.NONE] = new SpriteData(TILES, new Rectangle(450, 450, 1, 1));

            localSprites[(int)LocalSpriteName.PLAYERCHAR_MALE] = new SpriteData(@"Graphics/Local/Player");
            localSprites[(int)LocalSpriteName.PLAYERCHAR_FEMALE] = new SpriteData(@"Graphics/Local/PlayerFemale");

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

            AddLocalSprite(LocalSpriteName.WATER_TROUGH, HOUSEITEM, 2, 10);
            AddLocalSprite(LocalSpriteName.FOOD_TROUGH, HOUSEITEM, 3, 10);
            AddLocalSprite(LocalSpriteName.HAY_BALE, HOUSEITEM, 4, 10);
            AddLocalSprite(LocalSpriteName.WEAPON_RACK_3, HOUSEITEM, 5, 10);
            AddLocalSprite(LocalSpriteName.MANICLES_1, HOUSEITEM, 6, 10);
            AddLocalSprite(LocalSpriteName.MANICLES_2, HOUSEITEM, 7, 10);
            AddLocalSprite(LocalSpriteName.MANICLES_3, HOUSEITEM, 2, 11);
            AddLocalSprite(LocalSpriteName.MANICLES_4, HOUSEITEM, 3, 11);
            AddLocalSprite(LocalSpriteName.STOCKS_1, HOUSEITEM, 4, 11);
            AddLocalSprite(LocalSpriteName.STOCKS_2, HOUSEITEM, 5, 11);
            AddLocalSprite(LocalSpriteName.PALLISADE_LR, HOUSEITEM, 6, 11);
            AddLocalSprite(LocalSpriteName.PALLISADE_TB, HOUSEITEM, 7, 11);
            AddLocalSprite(LocalSpriteName.PALLISADE_RL, HOUSEITEM, 9, 11);
            AddLocalSprite(LocalSpriteName.PALLISADE_BT, HOUSEITEM, 0, 12);

            AddLocalSprite(LocalSpriteName.BEDROLL, HOUSEITEM, 8, 11);

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
            localSprites[(int)LocalSpriteName.HIDE] = new SpriteData(RESOURCES_AND_TOOLS, new Rectangle(450, 250, 50, 50));
            
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



            AddLocalSprite(LocalSpriteName.PEEL_1, RESOURCES_AND_TOOLS, 0, 8);
            AddLocalSprite(LocalSpriteName.PEEL_2, RESOURCES_AND_TOOLS, 1, 8);
            AddLocalSprite(LocalSpriteName.FURNACE_LARGE_TOP, RESOURCES_AND_TOOLS, 2, 8);
            AddLocalSprite(LocalSpriteName.ROLLING_PIN, RESOURCES_AND_TOOLS, 3, 8);
            AddLocalSprite(LocalSpriteName.BREAD, RESOURCES_AND_TOOLS, 4, 8);
            AddLocalSprite(LocalSpriteName.CHEESE, RESOURCES_AND_TOOLS, 5, 8);
            AddLocalSprite(LocalSpriteName.BASKET_1, RESOURCES_AND_TOOLS, 6, 8);
            AddLocalSprite(LocalSpriteName.BASKET_2, RESOURCES_AND_TOOLS, 7, 8);
            AddLocalSprite(LocalSpriteName.MILK, RESOURCES_AND_TOOLS, 8, 8);
            AddLocalSprite(LocalSpriteName.SUGAR, RESOURCES_AND_TOOLS, 9, 8);
            AddLocalSprite(LocalSpriteName.FLOUR, RESOURCES_AND_TOOLS, 10, 8);
            AddLocalSprite(LocalSpriteName.GRAIN_OPEN_2, RESOURCES_AND_TOOLS, 11, 8);
            AddLocalSprite(LocalSpriteName.GRAIN_OPEN_SMALL, RESOURCES_AND_TOOLS, 12, 8);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_1, RESOURCES_AND_TOOLS, 13, 8);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_2, RESOURCES_AND_TOOLS, 14, 8);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_3, RESOURCES_AND_TOOLS, 15, 8);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_4, RESOURCES_AND_TOOLS, 16, 8);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_5, RESOURCES_AND_TOOLS, 17, 8);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_6, RESOURCES_AND_TOOLS, 18, 8);
            AddLocalSprite(LocalSpriteName.FOOD_PRODUCE, RESOURCES_AND_TOOLS, 19, 8);

            AddLocalSprite(LocalSpriteName.FOOD_SHELF_7, RESOURCES_AND_TOOLS, 0, 9);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_8, RESOURCES_AND_TOOLS, 1, 9);
            AddLocalSprite(LocalSpriteName.FURNACE_LARGE_BOTTOM, RESOURCES_AND_TOOLS, 2, 9);
            AddLocalSprite(LocalSpriteName.BREAD_BASKET, RESOURCES_AND_TOOLS, 3, 9);
            AddLocalSprite(LocalSpriteName.CHEESE_BASKET, RESOURCES_AND_TOOLS, 4, 9);
            AddLocalSprite(LocalSpriteName.BREAD_BASKET_2, RESOURCES_AND_TOOLS, 5, 9);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_9, RESOURCES_AND_TOOLS, 6, 9);
            AddLocalSprite(LocalSpriteName.FOOD_SHELF_10, RESOURCES_AND_TOOLS, 7, 9);
            
            AddLocalSprite(LocalSpriteName.CARPENTRY_HAMMER_1, RESOURCES_AND_TOOLS, 11, 0);
            AddLocalSprite(LocalSpriteName.CARPENTRY_HAMMER_2, RESOURCES_AND_TOOLS, 12, 0);
            AddLocalSprite(LocalSpriteName.HATCHET, RESOURCES_AND_TOOLS, 13, 0);
            AddLocalSprite(LocalSpriteName.SAW, RESOURCES_AND_TOOLS, 14, 0);
            AddLocalSprite(LocalSpriteName.CARPENTRY_HAMMER_3, RESOURCES_AND_TOOLS, 15, 0);
            AddLocalSprite(LocalSpriteName.BOW_SAW_1, RESOURCES_AND_TOOLS, 16, 0);
            AddLocalSprite(LocalSpriteName.BOW_SAW_2, RESOURCES_AND_TOOLS, 17, 0);
            AddLocalSprite(LocalSpriteName.CARPENTRY_WORKBENCH, RESOURCES_AND_TOOLS, 11, 1);
            AddLocalSprite(LocalSpriteName.LOG_PILE, RESOURCES_AND_TOOLS, 12, 0);
            AddLocalSprite(LocalSpriteName.CARPENTRY_WORKBENCH_2, RESOURCES_AND_TOOLS, 13, 1);
            AddLocalSprite(LocalSpriteName.CARPENTRY_WORKBENCH_3, RESOURCES_AND_TOOLS, 14, 1);
            AddLocalSprite(LocalSpriteName.CARPENTRY_WORKBENCH_4, RESOURCES_AND_TOOLS, 15, 1);
            AddLocalSprite(LocalSpriteName.CARPENTRY_WORKBENCH_5, RESOURCES_AND_TOOLS, 16, 1);
            AddLocalSprite(LocalSpriteName.WOOD_LOGS_2, RESOURCES_AND_TOOLS, 17, 1);
            AddLocalSprite(LocalSpriteName.CRATE_EMPTY, RESOURCES_AND_TOOLS, 11, 2);
            AddLocalSprite(LocalSpriteName.CRATE_1, RESOURCES_AND_TOOLS, 12, 2);
            AddLocalSprite(LocalSpriteName.CRATE_2, RESOURCES_AND_TOOLS, 13, 2);
            AddLocalSprite(LocalSpriteName.SAWHORSE_1, RESOURCES_AND_TOOLS, 14, 2);
            AddLocalSprite(LocalSpriteName.SAWHORSE_2, RESOURCES_AND_TOOLS, 15, 2);
            AddLocalSprite(LocalSpriteName.PLANKS_1, RESOURCES_AND_TOOLS, 16, 2);
            AddLocalSprite(LocalSpriteName.PLANKS_2, RESOURCES_AND_TOOLS, 17, 2);

            localSprites[(int)LocalSpriteName.TREE_1] = new SpriteData(FORESTS_AND_MINES, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.DEAD_TREE] = new SpriteData(FORESTS_AND_MINES, new Rectangle(50, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_2] = new SpriteData(FORESTS_AND_MINES, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_3] = new SpriteData(FORESTS_AND_MINES, new Rectangle(150, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_4] = new SpriteData(FORESTS_AND_MINES, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_5] = new SpriteData(FORESTS_AND_MINES, new Rectangle(250, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_6] = new SpriteData(FORESTS_AND_MINES, new Rectangle(300, 0, 50, 50));
            localSprites[(int)LocalSpriteName.TREE_7] = new SpriteData(FORESTS_AND_MINES, new Rectangle(350, 0, 50, 50));
            localSprites[(int)LocalSpriteName.JUNGLE_TREE_1] = new SpriteData(FORESTS_AND_MINES, new Rectangle(400, 0, 50, 50));
            localSprites[(int)LocalSpriteName.JUNGLE_TREE_2] = new SpriteData(FORESTS_AND_MINES, new Rectangle(450, 0, 50, 50));
            localSprites[(int)LocalSpriteName.JUNGLE_TREE_3] = new SpriteData(FORESTS_AND_MINES, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.CACTUS_1] = new SpriteData(FORESTS_AND_MINES, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.CACTUS_2] = new SpriteData(FORESTS_AND_MINES, new Rectangle(100, 50, 50, 50));
            
            localSprites[(int)LocalSpriteName.DEAD_TREE_2] = new SpriteData(FORESTS_AND_MINES, new Rectangle(150, 50, 50, 50));
            localSprites[(int)LocalSpriteName.DEAD_TREE_3] = new SpriteData(FORESTS_AND_MINES, new Rectangle(200, 50, 50, 50));

            localSprites[(int)LocalSpriteName.SNOW_BUSH] = new SpriteData(FORESTS_AND_MINES, new Rectangle(250, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SNOW_TREE_1] = new SpriteData(FORESTS_AND_MINES, new Rectangle(300, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SNOW_TREE_2] = new SpriteData(FORESTS_AND_MINES, new Rectangle(350, 50, 50, 50));
            localSprites[(int)LocalSpriteName.SNOW_TREE_3] = new SpriteData(FORESTS_AND_MINES, new Rectangle(400, 50, 50, 50));

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
            localSprites[(int)LocalSpriteName.SPIKES] = new SpriteData(DUNGEON, new Rectangle(450, 200, 50, 50));

            localSprites[(int)LocalSpriteName.CAMPFIRE] = new SpriteData(DUNGEON, new Rectangle(0, 250, 50, 50));

            localSprites[(int)LocalSpriteName.ENEMY_SKELETON] = new SpriteData(ENEMIES, new Rectangle(0, 0, 50, 50));
            localSprites[(int)LocalSpriteName.ENEMY_ORC_CIV] = new SpriteData(ENEMIES, new Rectangle(50, 0, 50, 50));

            localSprites[(int)LocalSpriteName.DIRE_RAT] = new SpriteData(ENEMIES, new Rectangle(100, 0, 50, 50));
            localSprites[(int)LocalSpriteName.DIRE_BEAR] = new SpriteData(ENEMIES, new Rectangle(150, 0, 50, 50));
            
            localSprites[(int)LocalSpriteName.ENEMY_ORC_LIGHT] = new SpriteData(ENEMIES, new Rectangle(200, 0, 50, 50));
            localSprites[(int)LocalSpriteName.ENEMY_ORC_HEAVY] = new SpriteData(ENEMIES, new Rectangle(250, 0, 50, 50));

            localSprites[(int)LocalSpriteName.ENEMY_THOUGHT_WALK] = new SpriteData(ENEMIES, new Rectangle(0, 50, 50, 50));
            localSprites[(int)LocalSpriteName.ENEMY_THOUGHT_WAIT] = new SpriteData(ENEMIES, new Rectangle(50, 50, 50, 50));
            localSprites[(int)LocalSpriteName.ENEMY_THOUGHT_ATTACK] = new SpriteData(ENEMIES, new Rectangle(100, 50, 50, 50));
            localSprites[(int)LocalSpriteName.ENEMY_THOUGH_CONFUSED] = new SpriteData(ENEMIES, new Rectangle(150, 50, 50, 50));

            AddLocalSprite(LocalSpriteName.HUMANGUARD1, ENEMIES, 0, 2);
            AddLocalSprite(LocalSpriteName.HUMANGUARD2, ENEMIES, 1, 2);
            AddLocalSprite(LocalSpriteName.HUMANGUARD3, ENEMIES, 2, 2);
            AddLocalSprite(LocalSpriteName.HUMANGUARD4, ENEMIES, 3, 2);
            AddLocalSprite(LocalSpriteName.HUMANGUARD5, ENEMIES, 4, 2);
            AddLocalSprite(LocalSpriteName.HUMANGUARD6, ENEMIES, 5, 2);
            AddLocalSprite(LocalSpriteName.HUMANGUARD7, ENEMIES, 6, 2);

            AddLocalSprite(LocalSpriteName.HUMANMERCHANT1, ENEMIES, 0, 3);
            AddLocalSprite(LocalSpriteName.HUMANMERCHANT2, ENEMIES, 1, 3);
            AddLocalSprite(LocalSpriteName.HUMANMERCHANT3, ENEMIES, 2, 3);
            AddLocalSprite(LocalSpriteName.HUMANMERCHANT4, ENEMIES, 3, 3);
            AddLocalSprite(LocalSpriteName.HUMANMERCHANT5, ENEMIES, 4, 3);

            AddLocalSprite(LocalSpriteName.HUMANPOOR1, ENEMIES, 0, 4);
            AddLocalSprite(LocalSpriteName.HUMANPOOR2, ENEMIES, 1, 4);
            AddLocalSprite(LocalSpriteName.HUMANPOOR3, ENEMIES, 2, 4);
            AddLocalSprite(LocalSpriteName.HUMANPOOR4, ENEMIES, 3, 4);
            AddLocalSprite(LocalSpriteName.HUMANPOOR5, ENEMIES, 4, 4);
            AddLocalSprite(LocalSpriteName.HUMANPOOR6, ENEMIES, 5, 4);

            AddLocalSprite(LocalSpriteName.HUMANRICH1, ENEMIES, 0, 5);
            AddLocalSprite(LocalSpriteName.HUMANRICH2, ENEMIES, 1, 5);
            AddLocalSprite(LocalSpriteName.HUMANRICH3, ENEMIES, 2, 5);
            AddLocalSprite(LocalSpriteName.HUMANRICH4, ENEMIES, 3, 5);
            AddLocalSprite(LocalSpriteName.HUMANRICH5, ENEMIES, 4, 5);
            AddLocalSprite(LocalSpriteName.HUMANRICH6, ENEMIES, 5, 5);

            AddLocalSprite(LocalSpriteName.HUMANMERCHANT_HAIR, MERCHANT, 0, 0);
            AddLocalSprite(LocalSpriteName.HUMANMERCHANT_HEAD, MERCHANT, 1, 0);
            AddLocalSprite(LocalSpriteName.HUMANMERCHANT_BODY, MERCHANT, 2, 0);

            AddLocalSprite(LocalSpriteName.HUMANGUARD_HAIR, GUARD, 0, 0);
            AddLocalSprite(LocalSpriteName.HUMANGUARD_BODY, GUARD, 1, 0);

            AddLocalSprite(LocalSpriteName.HUMANPEASANTGIRL_HAIR, PEASANTWOMAN, 0, 0);
            AddLocalSprite(LocalSpriteName.HUMANPEASANTGIRL_FACE, PEASANTWOMAN, 1, 0);
            AddLocalSprite(LocalSpriteName.HUMANPEASANTGIRL_DRESS1, PEASANTWOMAN, 2, 0);
            AddLocalSprite(LocalSpriteName.HUMANPEASANTGIRL_DRESS2, PEASANTWOMAN, 3, 0);

            AddLocalSprite(LocalSpriteName.HUMANPEASANTMALE_HAIR, PEASANTMALE, 0, 0);
            AddLocalSprite(LocalSpriteName.HUMANPEASANTMALE_FACE, PEASANTMALE, 1, 0);
            AddLocalSprite(LocalSpriteName.HUMANPEASANTMALE_TOP, PEASANTMALE, 2, 0);
            AddLocalSprite(LocalSpriteName.HUMANPEASANTMALE_PANTS, PEASANTMALE, 3, 0);

            AddLocalSprite(LocalSpriteName.RICHMALE_HAIR, RICHMAN, 0, 0);
            AddLocalSprite(LocalSpriteName.RICHMALE_FACE, RICHMAN, 1, 0);
            AddLocalSprite(LocalSpriteName.RICHMALE_CLOTHES, RICHMAN, 2, 0);

            AddLocalSprite(LocalSpriteName.RICHFEMALE_HAIR, RICHFEMALE, 0, 0);
            AddLocalSprite(LocalSpriteName.RICHFEMALE_FACE, RICHFEMALE, 1, 0);
            AddLocalSprite(LocalSpriteName.RICHFEMALE_CLOTHES, RICHFEMALE, 2, 0);

            AddLocalSprite(LocalSpriteName.PRIEST_BODY, PRIEST, 0, 0);
            AddLocalSprite(LocalSpriteName.PRIEST_CLOTHES, PRIEST, 1, 0);

            AddLocalSprite(LocalSpriteName.BANDIT_EASY, ENEMIES, 0, 6);
            AddLocalSprite(LocalSpriteName.BANDIT_MEDIUM, ENEMIES, 1, 6);
            AddLocalSprite(LocalSpriteName.BANDIT_HARD, ENEMIES, 2, 6);

            AddLocalSprite(LocalSpriteName.MOOSE, ANIMALS, 0, 0);
            AddLocalSprite(LocalSpriteName.RAT, ANIMALS, 1, 0);
            AddLocalSprite(LocalSpriteName.CAMEL_1, ANIMALS, 2, 0);
            AddLocalSprite(LocalSpriteName.CAMEL_2, ANIMALS, 3, 0);
            AddLocalSprite(LocalSpriteName.COUGAR, ANIMALS, 4, 0);
            AddLocalSprite(LocalSpriteName.ELEPHANT, ANIMALS, 5, 0);
            AddLocalSprite(LocalSpriteName.POLAR_BEAR, ANIMALS, 6, 0);
            AddLocalSprite(LocalSpriteName.WOLF_1, ANIMALS, 7, 0);
            AddLocalSprite(LocalSpriteName.WOLF_2, ANIMALS, 8, 0);
            AddLocalSprite(LocalSpriteName.GIANT_SNAKE, ANIMALS, 9, 0);
            AddLocalSprite(LocalSpriteName.GIANT_LIZARD_1, ANIMALS, 10, 0);
            AddLocalSprite(LocalSpriteName.GIANT_LIZARD_2, ANIMALS, 11, 0);
            AddLocalSprite(LocalSpriteName.CHICKEN_1, ANIMALS, 0, 1);
            AddLocalSprite(LocalSpriteName.COW, ANIMALS, 1, 1);
            AddLocalSprite(LocalSpriteName.LLAMA, ANIMALS, 2, 1);
            AddLocalSprite(LocalSpriteName.PIG, ANIMALS, 3, 1);
            AddLocalSprite(LocalSpriteName.SHEEP, ANIMALS, 4, 1);
            AddLocalSprite(LocalSpriteName.WALRUS, ANIMALS, 5, 1);
            AddLocalSprite(LocalSpriteName.GIANT_TURTLE, ANIMALS, 6, 1);
            AddLocalSprite(LocalSpriteName.RABBIT, ANIMALS, 7, 1);
            AddLocalSprite(LocalSpriteName.PANTHER, ANIMALS, 8, 1);
            AddLocalSprite(LocalSpriteName.BEAR, ANIMALS, 9, 1);

            AddLocalSprite(LocalSpriteName.GREY_GEM, INVENTORYITEMS, 0, 0);
            AddLocalSprite(LocalSpriteName.BROWN_GEM, INVENTORYITEMS, 1, 0);
            AddLocalSprite(LocalSpriteName.GREEN_GEM, INVENTORYITEMS, 2, 0);
            AddLocalSprite(LocalSpriteName.BLUE_GEM, INVENTORYITEMS, 3, 0);
            AddLocalSprite(LocalSpriteName.RED_GEM, INVENTORYITEMS, 4, 0);

            AddLocalSprite(LocalSpriteName.GREY_GEM_RING, INVENTORYITEMS, 0, 1);
            AddLocalSprite(LocalSpriteName.BROWN_GEM_RING, INVENTORYITEMS, 1, 1);
            AddLocalSprite(LocalSpriteName.GREEN_GEM_RING, INVENTORYITEMS, 2, 1);
            AddLocalSprite(LocalSpriteName.BLUE_GEM_RING, INVENTORYITEMS, 3, 1);
            AddLocalSprite(LocalSpriteName.RED_GEM_RING, INVENTORYITEMS, 4, 1);

            AddLocalSprite(LocalSpriteName.GREY_GEM_PENDANT, INVENTORYITEMS, 0, 2);
            AddLocalSprite(LocalSpriteName.BROWN_GEM_PENDANT, INVENTORYITEMS, 1, 2);
            AddLocalSprite(LocalSpriteName.GREEN_GEM_PENDANT, INVENTORYITEMS, 2, 2);
            AddLocalSprite(LocalSpriteName.BLUE_GEM_PENDANT, INVENTORYITEMS, 3, 2);
            AddLocalSprite(LocalSpriteName.RED_GEM_PENDANT, INVENTORYITEMS, 4, 2);

            AddLocalSprite(LocalSpriteName.CLOTH_ARMOUR, INVENTORYITEMS, 0, 3);
            AddLocalSprite(LocalSpriteName.LEATHER_ARMOUR, INVENTORYITEMS, 1, 3);
            AddLocalSprite(LocalSpriteName.CHAIN_ARMOUR, INVENTORYITEMS, 2, 3);
            AddLocalSprite(LocalSpriteName.LIGHT_BREASTPLATE, INVENTORYITEMS, 3, 3);
            AddLocalSprite(LocalSpriteName.HEAVY_BREASTPLATE, INVENTORYITEMS, 4, 3);
            AddLocalSprite(LocalSpriteName.HEAVY_ARMOUR, INVENTORYITEMS, 5, 3);
            AddLocalSprite(LocalSpriteName.ELITE_ARMOUR, INVENTORYITEMS, 6, 3);
            AddLocalSprite(LocalSpriteName.LEGENDARY_ARMOUR, INVENTORYITEMS, 7, 3);

            AddLocalSprite(LocalSpriteName.HELM_1, INVENTORYITEMS, 0, 5);
            AddLocalSprite(LocalSpriteName.HELM_2, INVENTORYITEMS, 1, 5);
            AddLocalSprite(LocalSpriteName.HELM_3, INVENTORYITEMS, 2, 5);
            AddLocalSprite(LocalSpriteName.HELM_4, INVENTORYITEMS, 3, 5);
            AddLocalSprite(LocalSpriteName.HELM_5, INVENTORYITEMS, 4, 5);
            AddLocalSprite(LocalSpriteName.HELM_6, INVENTORYITEMS, 5, 5);
            AddLocalSprite(LocalSpriteName.HELM_7, INVENTORYITEMS, 6, 5);
            AddLocalSprite(LocalSpriteName.HELM_8, INVENTORYITEMS, 7, 5);

            AddLocalSprite(LocalSpriteName.PADDED_LEGGINGS, INVENTORYITEMS, 1, 4);
            AddLocalSprite(LocalSpriteName.LEATHER_LEGGINGS,INVENTORYITEMS,2,4);
            AddLocalSprite(LocalSpriteName.CHAIN_LEGGINGS, INVENTORYITEMS, 3, 4);
            AddLocalSprite(LocalSpriteName.PLATE_LEGGINGS, INVENTORYITEMS, 4, 4);

            AddLocalSprite(LocalSpriteName.SHIELD_1, INVENTORYITEMS, 0, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_2, INVENTORYITEMS, 1, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_3, INVENTORYITEMS, 2, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_4, INVENTORYITEMS, 3, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_5, INVENTORYITEMS, 4, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_6, INVENTORYITEMS, 5, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_7, INVENTORYITEMS, 6, 6);
            AddLocalSprite(LocalSpriteName.SHIELD_8, INVENTORYITEMS, 7, 6);

            AddLocalSprite(LocalSpriteName.SWORD_1, INVENTORYITEMS, 0, 7);
            AddLocalSprite(LocalSpriteName.SWORD_2, INVENTORYITEMS, 1, 7);
            AddLocalSprite(LocalSpriteName.SWORD_3, INVENTORYITEMS, 2, 7);
            AddLocalSprite(LocalSpriteName.SWORD_4, INVENTORYITEMS, 3, 7);
            AddLocalSprite(LocalSpriteName.SWORD_5, INVENTORYITEMS, 4, 7);
            AddLocalSprite(LocalSpriteName.SWORD_6, INVENTORYITEMS, 5, 7);
            AddLocalSprite(LocalSpriteName.SWORD_7, INVENTORYITEMS, 6, 7);
            AddLocalSprite(LocalSpriteName.SWORD_8, INVENTORYITEMS, 7, 7);

            AddLocalSprite(LocalSpriteName.AXE_1, INVENTORYITEMS, 0, 8);
            AddLocalSprite(LocalSpriteName.AXE_2, INVENTORYITEMS, 1, 8);
            AddLocalSprite(LocalSpriteName.AXE_3, INVENTORYITEMS, 2, 8);
            AddLocalSprite(LocalSpriteName.AXE_4, INVENTORYITEMS, 3, 8);
            AddLocalSprite(LocalSpriteName.AXE_5, INVENTORYITEMS, 4, 8);
            AddLocalSprite(LocalSpriteName.AXE_6, INVENTORYITEMS, 5, 8);
            AddLocalSprite(LocalSpriteName.AXE_7, INVENTORYITEMS, 6, 8);
            AddLocalSprite(LocalSpriteName.AXE_8, INVENTORYITEMS, 7, 8);

            AddLocalSprite(LocalSpriteName.COINS, INVENTORYITEMS, 0, 4);

            interfaceSprites[(int)InterfaceSpriteName.SCROLL] = new SpriteData(@"Graphics/Interface/scrollsandblocks", new Rectangle(224, 190, 96, 34));
            interfaceSprites[(int)InterfaceSpriteName.PAPER_TEXTURE] = new SpriteData(@"Graphics/Interface/paperTexture");
            interfaceSprites[(int)InterfaceSpriteName.WOOD_TEXTURE] = new SpriteData(@"Graphics/Interface/woodTexture");

            interfaceSprites[(int)InterfaceSpriteName.HEAD] = new SpriteData(HEALTH, new Rectangle(250, 0, 107, 96));
            interfaceSprites[(int)InterfaceSpriteName.LEFT_ARM] = new SpriteData(HEALTH, new Rectangle(401, 1, 82, 212));
            interfaceSprites[(int)InterfaceSpriteName.CHEST] = new SpriteData(HEALTH, new Rectangle(271, 91, 120, 181));
            interfaceSprites[(int)InterfaceSpriteName.RIGHT_ARM] = new SpriteData(HEALTH, new Rectangle(396, 275, 88, 213));
            interfaceSprites[(int)InterfaceSpriteName.LEGS] = new SpriteData(HEALTH, new Rectangle(250, 275, 124, 248));

            interfaceSprites[(int)InterfaceSpriteName.BRAWN] = new SpriteData(CHARACTER_SHEET, new Rectangle(0, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.CHA] = new SpriteData(CHARACTER_SHEET, new Rectangle(50, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.AGIL] = new SpriteData(CHARACTER_SHEET, new Rectangle(100, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.PERC] = new SpriteData(CHARACTER_SHEET, new Rectangle(150, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.INTEL] = new SpriteData(CHARACTER_SHEET, new Rectangle(200, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.SWORD] = new SpriteData(CHARACTER_SHEET, new Rectangle(250, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.AXE] = new SpriteData(CHARACTER_SHEET, new Rectangle(300, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.MACE] = new SpriteData(CHARACTER_SHEET, new Rectangle(350, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.DEFENSE] = new SpriteData(CHARACTER_SHEET, new Rectangle(400, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.SPEAR] = new SpriteData(CHARACTER_SHEET, new Rectangle(450, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.BLOOD] = new SpriteData(CHARACTER_SHEET, new Rectangle(0, 50, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.SPIRAL] = new SpriteData(CHARACTER_SHEET, new Rectangle(50, 50, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.BLEEDING] = new SpriteData(CHARACTER_SHEET, new Rectangle(100, 50, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.CLOSE] = new SpriteData(CHARACTER_SHEET, new Rectangle(150, 50, 50, 50));

            interfaceSprites[(int)InterfaceSpriteName.BANNER_GREEN] = new SpriteData(BANNERS, new Rectangle(1, 0, 60, 100));
            interfaceSprites[(int)InterfaceSpriteName.BANNER_YELLOW] = new SpriteData(BANNERS, new Rectangle(71, 0, 60, 100));
            interfaceSprites[(int)InterfaceSpriteName.BANNER_RED] = new SpriteData(BANNERS, new Rectangle(141, 0, 60, 100));

            interfaceSprites[(int)InterfaceSpriteName.DISTRICT_BOX] = new SpriteData(LOCATIONINTERFACE, new Rectangle(0, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.DISTRICT_STAR] = new SpriteData(LOCATIONINTERFACE, new Rectangle(75, 0, 30, 30));
            interfaceSprites[(int)InterfaceSpriteName.MIDDLE_HOUSING] = new SpriteData(LOCATIONINTERFACE, new Rectangle(150, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.RICH_HOUSING] = new SpriteData(LOCATIONINTERFACE, new Rectangle(225, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.SLUM_HOUSING] = new SpriteData(LOCATIONINTERFACE, new Rectangle(300, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.TEMPLE] = new SpriteData(LOCATIONINTERFACE, new Rectangle(375, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.LIBRARY] = new SpriteData(LOCATIONINTERFACE, new Rectangle(450, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.MARKET] = new SpriteData(LOCATIONINTERFACE, new Rectangle(600, 0, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.SMITH] = new SpriteData(LOCATIONINTERFACE, new Rectangle(675, 0, 75, 75));

            interfaceSprites[(int)InterfaceSpriteName.CARPENTER] = new SpriteData(LOCATIONINTERFACE, new Rectangle(0, 75, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.STONEWORKER] = new SpriteData(LOCATIONINTERFACE, new Rectangle(75, 75, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.BARRACKS] = new SpriteData(LOCATIONINTERFACE, new Rectangle(150, 75, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.PALACE] = new SpriteData(LOCATIONINTERFACE, new Rectangle(225, 75, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.GENERAL_STORE] = new SpriteData(LOCATIONINTERFACE, new Rectangle(375, 75, 75, 75));
            interfaceSprites[(int)InterfaceSpriteName.INN] = new SpriteData(LOCATIONINTERFACE, new Rectangle(450, 75, 75, 75));
            
            interfaceSprites[(int)InterfaceSpriteName.MAN] = new SpriteData(LOCATIONINTERFACE, new Rectangle(300, 75, 35, 38));
            interfaceSprites[(int)InterfaceSpriteName.HALFMAN] = new SpriteData(LOCATIONINTERFACE, new Rectangle(300, 75, 17, 38));
            
            interfaceSprites[(int)InterfaceSpriteName.GOTO_WORLD_MAP] = new SpriteData(GOTOWORLDMAP);
            interfaceSprites[(int)InterfaceSpriteName.GOTO_WORLD_MAP_DUNGEON] = new SpriteData(GOTOWORLDMAP + "2");

            interfaceSprites[(int)InterfaceSpriteName.DEAD] = new SpriteData(@"Graphics/Interface/death", null);

            interfaceSprites[(int)InterfaceSpriteName.SUN] = new SpriteData(TIMEICONS, new Rectangle(0, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.MOON] = new SpriteData(TIMEICONS, new Rectangle(50, 0, 50, 50));

            interfaceSprites[(int)InterfaceSpriteName.SELL] = new SpriteData(BUYSELLICONS, new Rectangle(0, 0, 50, 50));
            interfaceSprites[(int)InterfaceSpriteName.BUY] = new SpriteData(BUYSELLICONS, new Rectangle(50, 0, 50, 50));

            interfaceSprites[(int)InterfaceSpriteName.BOOK] = new SpriteData(@"Graphics/Interface/book", null);
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

        /// <summary>
        /// Adds a local sprite to the lists, provided they are in a 50x50 grid
        /// </summary>
        /// <param name="name"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private static void AddLocalSprite(LocalSpriteName name, string filePath, int x, int y)
        {
            const int GRIDSIZE = 50;

            localSprites[(int)name] = new SpriteData(filePath, new Rectangle(x*GRIDSIZE, y *GRIDSIZE, GRIDSIZE, GRIDSIZE));

        }
    }
}
