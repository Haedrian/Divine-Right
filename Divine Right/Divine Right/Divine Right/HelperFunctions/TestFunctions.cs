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

            house.MapletContents = new List<MapletContents>();

            MapletContentsItem beds = new MapletContentsItem();
            beds.ItemID = 37;
            beds.ItemCategory = "mundaneitems";
            beds.MaxAmount = 5;
            beds.ProbabilityPercentage = 50;

            house.MapletContents.Add(beds);

            MapletContentsItemTag decorations = new MapletContentsItemTag();
            decorations.Category = "mundaneitems";
            decorations.MaxAmount = 7;
            decorations.ProbabilityPercentage = 50;
            decorations.Tag = "decoration";

            house.MapletContents.Add(decorations);

            MapletContentsMaplet library = new MapletContentsMaplet();
            Maplet libraryMaplet = new Maplet();
            library.Maplet = libraryMaplet;
            library.MaxAmount = 2;
            library.ProbabilityPercentage = 100;

            libraryMaplet.MapletContents = new List<MapletContents>();
            libraryMaplet.MapletName = "Library";
            libraryMaplet.SizeX = 5;
            libraryMaplet.SizeY = 6;
            libraryMaplet.Tiled = true;
            libraryMaplet.TileID = 1;
            libraryMaplet.Walled = true;

            MapletContentsItemTag libraryStuff = new MapletContentsItemTag();
            libraryStuff.Category = "mundaneitems";
            libraryStuff.MaxAmount = 2;
            libraryStuff.ProbabilityPercentage = 75;
            libraryStuff.Tag = "reading";

            libraryMaplet.MapletContents.Add(libraryStuff);

            MapletContentsItemTag librarySeats = new MapletContentsItemTag();
            librarySeats.Category = "mundaneitems";
            librarySeats.MaxAmount = 4;
            librarySeats.ProbabilityPercentage = 75;
            librarySeats.Tag = "chair";

            libraryMaplet.MapletContents.Add(librarySeats);

            house.MapletContents.Add(library);

            //Now Generate it
            LocalMapGenerator gen = new LocalMapGenerator();

            MapBlock[,] generatedMap = gen.GenerateMap(1, house);

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


        }


    }
}
