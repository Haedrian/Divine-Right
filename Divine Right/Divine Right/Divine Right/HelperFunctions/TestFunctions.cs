using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame;
using DRObjects;
using DivineRightGame.ItemFactory;
using DRObjects.Graphics;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.Managers;
using DivineRightGame.LocalMapGenerator;
using DRObjects.ActorHandling;
using DivineRightGame.ActorHandling;

namespace Divine_Right.HelperFunctions
{
    public static class TestFunctions
    {
        public static void PrepareFileTestMap()
        {
            GameState.LocalMap = new LocalMap(15,15,0,0);

            //GameState.LocalMap.LoadLocalMap(mgr.GetMap("testmap"),0);

            //Add player character
            //Player character
            DivineRightGame.GameState.PlayerCharacter = new DRObjects.Actor();
            GameState.PlayerCharacter.IsPlayerCharacter = true;
            GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

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

        public static void GenerateDungeon()
        {
            DungeonGenerator gen = new DungeonGenerator();
            MapCoordinate start = null;
            DRObjects.Actor[] actors = null;
            List<PointOfInterest> pointsOfInterest = null;

            string getOwner = ActorGeneration.GetEnemyType(true);

            MapBlock[,] generatedMap = gen.GenerateDungeon(5, 2, 2, 2,getOwner,75,2,7,out start,out actors,out pointsOfInterest);

            GameState.LocalMap = new LocalMap(500, 500, 1, 0);
            GameState.LocalMap.PointsOfInterest = pointsOfInterest;

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (MapBlock block in generatedMap)
            {
                collapsedMap.Add(block);
            }

            GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());
            GameState.LocalMap.Actors = actors.ToList<Actor>();

            MapItem player = new MapItem();
            player.Coordinate = start;
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(start);
            playerBlock.PutItemOnBlock(player);

            GameState.PlayerCharacter = new Actor();
            GameState.PlayerCharacter.MapCharacter = player;
            GameState.PlayerCharacter.IsPlayerCharacter = true;

            GameState.PlayerCharacter.Attributes = ActorGeneration.GenerateAttributes("human", DRObjects.ActorHandling.CharacterSheet.Enums.ActorProfession.WARRIOR, 10);
            GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

            GameState.PlayerCharacter.Attributes.Health = GameState.PlayerCharacter.Anatomy;

            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

        }

        public static void ParseXML()
        {
            LocalMapXMLParser parser = new LocalMapXMLParser();

            Maplet maplet = parser.ParseMaplet(@"Maplets/Village.xml");

            //Generate it
            LocalMapGenerator gen = new LocalMapGenerator();

            MapBlock[,] generatedMap = gen.GenerateMap(0, null, maplet, true);

            //put in the map

            GameState.LocalMap = new LocalMap(150, 150, 1, 0);

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (MapBlock block in generatedMap)
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

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(5, 5, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            playerBlock.PutItemOnBlock(player);
            GameState.PlayerCharacter = new Actor();
            GameState.PlayerCharacter.MapCharacter = player;
            GameState.PlayerCharacter.IsPlayerCharacter = true;

            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

        }

        public static void PrepareMapletTestFarmHouse()
        {
            Maplet farmArea = new Maplet();
            farmArea.MapletName = "farmArea";
            farmArea.SizeX = 35;
            farmArea.SizeY = 35;
            farmArea.Tiled = true;
            farmArea.TileID = 3;
            farmArea.Walled = false;

            farmArea.MapletContents = new List<MapletContents>();

            LocalMapXMLParser parser = new LocalMapXMLParser();

            Maplet farmHouse = new Maplet();
            farmHouse.MapletName = "farmHouse";
            farmHouse.SizeX = 17;
            farmHouse.SizeY = 9;
            farmHouse.Tiled = true;
            farmHouse.TileID = 1;
            farmHouse.Walled = true;
            farmHouse.WindowProbability = 10;
            farmHouse.MapletContents = new List<MapletContents>();

            farmArea.MapletContents.Add(new MapletContentsMaplet()
                {
                    MaxAmount = 1,
                    Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES,
                    ProbabilityPercentage = 100,
                    Maplet = parser.ParseMaplet(@"Maplets/SmallHouse.xml")
                });

            MapletContentsMaplet farmHouseWrapper = new MapletContentsMaplet();
            farmHouseWrapper.Maplet = farmHouse;
            farmHouseWrapper.MaxAmount = 1;
            farmHouseWrapper.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            farmHouseWrapper.ProbabilityPercentage = 100;

           // farmArea.MapletContents.Add(farmHouseWrapper);

            MapletContentsItemTag trees = new MapletContentsItemTag();
            trees.Category = "mundaneitems";
            trees.MaxAmount = 150;
            trees.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE;
            trees.ProbabilityPercentage = 60;
            trees.Tag = "tree";

            MapletContentsItemTag wood = new MapletContentsItemTag();
            wood.Category = "mundaneitems";
            wood.MaxAmount = 5;
            wood.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE;
            wood.ProbabilityPercentage = 100;
            wood.Tag = "wood resource";

            MapletContentsItemTag woodTools = new MapletContentsItemTag();
            woodTools.Category = "mundaneitems";
            woodTools.MaxAmount = 2;
            woodTools.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE;
            woodTools.ProbabilityPercentage = 100;
            woodTools.Tag = "woodcutter tools";

            farmArea.MapletContents.Add(wood);
            farmArea.MapletContents.Add(trees);
            farmArea.MapletContents.Add(woodTools);

            Maplet fieldArea = new Maplet();
            fieldArea.MapletName = "fieldArea";
            fieldArea.SizeX = 25;
            fieldArea.SizeY = 15;
            fieldArea.Tiled = false;
            fieldArea.Walled = false;
            fieldArea.MapletContents = new List<MapletContents>();

            MapletContentsMaplet fieldAreaWrapper = new MapletContentsMaplet();
            fieldAreaWrapper.Maplet = fieldArea;
            fieldAreaWrapper.MaxAmount = 1;
            fieldAreaWrapper.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            fieldAreaWrapper.Padding = 3;
            fieldAreaWrapper.ProbabilityPercentage = 100;

            farmArea.MapletContents.Add(fieldAreaWrapper);

            Maplet field = new Maplet();
            field.MapletName = "field";
            field.SizeX = 10;
            field.SizeY = 10;
            field.SizeRange = 3;
            field.Tiled = true;
            field.TileID = 5;
            field.MapletContents = new List<MapletContents>();

            Maplet storeHouse = new Maplet();
            storeHouse.MapletName = "storeHouse";
            storeHouse.SizeX = 7;
            storeHouse.SizeY = 7;
            storeHouse.SizeRange = 2;
            storeHouse.Tiled = true;
            storeHouse.TileID = 4;
            storeHouse.Walled = true;
            storeHouse.WindowProbability = 0;
            storeHouse.MapletContents = new List<MapletContents>();

            MapletContentsMaplet fieldWrapper = new MapletContentsMaplet();
            fieldWrapper.Maplet = field;
            fieldWrapper.MaxAmount = 1;
            fieldWrapper.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            fieldAreaWrapper.Padding = 2;
            fieldWrapper.ProbabilityPercentage = 100;

            fieldArea.MapletContents.Add(fieldWrapper);

            MapletContentsMaplet storeHouseWrapper = new MapletContentsMaplet();
            storeHouseWrapper.Maplet = storeHouse;
            storeHouseWrapper.MaxAmount = 1;
            storeHouseWrapper.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES;
            storeHouseWrapper.Padding = 2;
            storeHouseWrapper.ProbabilityPercentage = 100;

            fieldArea.MapletContents.Add(storeHouseWrapper);

            MapletContentsItemTag fieldProduce = new MapletContentsItemTag();
            fieldProduce.Category = "mundaneitems";
            fieldProduce.MaxAmount = 30;
            fieldProduce.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.MIDDLE;
            fieldProduce.ProbabilityPercentage = 85;
            fieldProduce.Tag = "plant";

            field.MapletContents.Add(fieldProduce);

            MapletContentsItemTag storeHouseItems = new MapletContentsItemTag();
            storeHouseItems.Category = "mundaneitems";
            storeHouseItems.MaxAmount = 20;
            storeHouseItems.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE;
            storeHouseItems.ProbabilityPercentage = 100;
            storeHouseItems.Tag = "farm produce";

            storeHouse.MapletContents.Add(storeHouseItems);

            MapletContentsItemTag tools = new MapletContentsItemTag();
            tools.Category = "mundaneitems";
            tools.MaxAmount = 2;
            tools.Position = DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE;
            tools.ProbabilityPercentage = 75;
            tools.Tag = "farm tool";

            fieldArea.MapletContents.Add(tools);

            //Now Generate it
            LocalMapGenerator gen = new LocalMapGenerator();

            MapBlock[,] generatedMap = gen.GenerateMap(1, null, farmArea, true);

            //put in the map

            GameState.LocalMap = new LocalMap(32, 32, 1, 0);

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (MapBlock block in generatedMap)
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
