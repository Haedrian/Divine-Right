using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame;
using DRObjects;
using DivineRightGame.ItemFactory;
using DivineRightGame.MapFactory;
using DRObjects.Graphics;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.Managers;

namespace Divine_Right.HelperFunctions
{
    public static class TestFunctions
    {
        public static void PrepareFileTestMap()
        {
            MapFactoryManager mgr = new MapFactoryManager();

            GameState.LocalMap = new LocalMap(15,15,0,0);

            GameState.LocalMap.LoadLocalMap(mgr.GetMap("testmap"),0);

            //Add player character
            //Player character
            DivineRightGame.GameState.PlayerCharacter = new DRObjects.Actor();
            GameState.PlayerCharacter.IsPlayerCharacter = true;

            MapBlock block = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(5, 5, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            
            MapItem player = new MapItem();
            player.Coordinate = new MapCoordinate(5,5,0,DRObjects.Enums.MapTypeEnum.LOCAL);
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            block.PutItemOnBlock(player);
            GameState.PlayerCharacter.MapCharacter = player;
        }

        public static void PrepareMapletTestMapHouse()
        {
            Maplet house = new Maplet();
            house.MapletName = "house";
            house.SizeX = 15;
            house.SizeY = 11;
            house.Tiled = true;
            house.TileID = 4;
            house.Walled = true;
            house.WindowProbability = 20;

            house.MapletContents = new List<MapletContents>();

            MapletContentsItem beds = new MapletContentsItem();
            beds.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            beds.ItemID = 37;
            beds.ItemCategory = "mundaneitems";
            beds.MaxAmount = 2;
            beds.ProbabilityPercentage = 75;

            house.MapletContents.Add(beds);

            MapletContentsMaplet chairAndTable = new MapletContentsMaplet();
            chairAndTable.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.MIDDLE;
            Maplet chairAndTableMaplet = new Maplet();
            chairAndTable.Maplet = chairAndTableMaplet;
            chairAndTable.MaxAmount = 2;
            chairAndTable.ProbabilityPercentage = 100;

            chairAndTableMaplet.MapletContents = new List<MapletContents>();
            chairAndTableMaplet.MapletName = "Chairs";
            chairAndTableMaplet.SizeX = 3;
            chairAndTableMaplet.SizeY = 3;
            chairAndTableMaplet.Tiled = false;
            chairAndTableMaplet.Walled = false;

            chairAndTableMaplet.MapletContents = new List<MapletContents>();

            MapletContentsItemTag table = new MapletContentsItemTag();
            table.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED;
            table.x = 1;
            table.y = 1;
            table.Category = "mundaneitems";
            table.MaxAmount = 1;
            table.ProbabilityPercentage = 100;
            table.Tag = "table";

            chairAndTableMaplet.MapletContents.Add(table);

            chairAndTableMaplet.MapletContents.Add(new MapletContentsItemTag
            {
                Category = "mundaneitems",
                MaxAmount = 1,
                Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED,
                ProbabilityPercentage = 100,
                Tag = "chair left",
                x = 0,
                y = 1
            });

            chairAndTableMaplet.MapletContents.Add(new MapletContentsItemTag
            {
                Category = "mundaneitems",
                MaxAmount = 1,
                Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED,
                ProbabilityPercentage = 50,
                Tag = "chair right",
                x = 2,
                y = 1
            });

            chairAndTableMaplet.MapletContents.Add(new MapletContentsItemTag
            {
                Category = "mundaneitems",
                MaxAmount = 1,
                Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED,
                ProbabilityPercentage = 75,
                Tag = "chair top",
                x = 1,
                y = 0
            });

            chairAndTableMaplet.MapletContents.Add(new MapletContentsItemTag
            {
                Category = "mundaneitems",
                MaxAmount = 1,
                Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED,
                ProbabilityPercentage = 75,
                Tag = "chair bottom",
                x = 1,
                y = 2
            });

            MapletContentsItemTag decorations = new MapletContentsItemTag();
            decorations.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.MIDDLE;
            decorations.Category = "mundaneitems";
            decorations.MaxAmount = 7;
            decorations.ProbabilityPercentage = 50;
            decorations.Tag = "decoration";

            house.MapletContents.Add(decorations);

            MapletContentsItemTag furniture = new MapletContentsItemTag();
            decorations.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE;
            decorations.Category = "mundaneitems";
            decorations.MaxAmount = 3;
            decorations.ProbabilityPercentage = 50;
            decorations.Tag = "furniture";

            house.MapletContents.Add(furniture);

            MapletContentsMaplet library = new MapletContentsMaplet();
            Maplet libraryMaplet = new Maplet();
            library.Maplet = libraryMaplet;
            library.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            library.Maplet = libraryMaplet;
            library.MaxAmount = 1;
            library.ProbabilityPercentage = 100;

            libraryMaplet.MapletContents = new List<MapletContents>();
            libraryMaplet.MapletName = "Library";
            libraryMaplet.SizeX = 7;
            libraryMaplet.SizeY = 7;
            libraryMaplet.Tiled = true;
            libraryMaplet.TileID = 1;
            libraryMaplet.Walled = true;

            MapletContentsItemTag libraryStuff = new MapletContentsItemTag();
            libraryStuff.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            libraryStuff.Category = "mundaneitems";
            libraryStuff.MaxAmount = 3;
            libraryStuff.ProbabilityPercentage = 66;
            libraryStuff.Tag = "reading";

            libraryMaplet.MapletContents.Add(libraryStuff);

            MapletContentsItemTag libraryDesk = new MapletContentsItemTag();
            libraryDesk.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.MIDDLE;
            libraryDesk.Category = "mundaneitems";
            libraryDesk.MaxAmount = 1;
            libraryDesk.ProbabilityPercentage = 75;
            libraryDesk.Tag = "desk";

            libraryMaplet.MapletContents.Add(libraryDesk);

            house.MapletContents.Add(library);
            house.MapletContents.Add(chairAndTable);

            //Now Generate it
            LocalMapGenerator gen = new LocalMapGenerator();

            MapBlock[,] generatedMap = gen.GenerateMap(1,null, house,true);

            //put in the map

            GameState.LocalMap = new LocalMap(15, 15, 1, 0);

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach(MapBlock block in generatedMap)
            {
                collapsedMap.Add(block);
            }

            GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

            MapItem player = new MapItem();
            player.Coordinate = new MapCoordinate(10, 5, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(10, 5, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            playerBlock.PutItemOnBlock(player);
            GameState.PlayerCharacter = new Actor();
            GameState.PlayerCharacter.MapCharacter = player;
            GameState.PlayerCharacter.IsPlayerCharacter = true;


        }


    }
}
