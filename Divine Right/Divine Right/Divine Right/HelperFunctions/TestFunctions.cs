﻿using System;
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
using DivineRightGame.SettlementHandling;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.CharacterSheet;
using DRObjects.Sites;
using DRObjects.Enums;
using DRObjects.CivilisationHandling;
using DRObjects.Items.Archetypes;
using DRObjects.Items.Archetypes.Global;

namespace Divine_Right.HelperFunctions
{
    public static class TestFunctions
    {
        public static void PrepareFileTestMap()
        {
            GameState.LocalMap = new LocalMap(15, 15, 0, 0);

            //GameState.LocalMap.LoadLocalMap(mgr.GetMap("testmap"),0);

            //Add player character
            //Player character
            DivineRightGame.GameState.PlayerCharacter = new DRObjects.Actor();
            GameState.PlayerCharacter.IsPlayerCharacter = true;
            GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

            MapBlock block = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(5, 5, 0, DRObjects.Enums.MapType.LOCAL));

            MapItem player = new MapItem();
            player.Coordinate = new MapCoordinate(5, 5, 0, DRObjects.Enums.MapType.LOCAL);
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            block.PutItemOnBlock(player);
            GameState.PlayerCharacter.MapCharacter = player;

        }

        public static void GenerateDungeon()
        {
            MapCoordinate start = null;
            Actor[] actors = null;
            List<PointOfInterest> pointsOfInterest = null;

            string getOwner = ActorGeneration.GetEnemyType(true);

            Dungeon dungeon = null;

            MapBlock[,] generatedMap = DungeonGenerator.GenerateDungeonLevel(1, 30, out start, out actors,out dungeon);

            GameState.LocalMap = new LocalMap(500, 500, 1, 0);
            GameState.LocalMap.PointsOfInterest = pointsOfInterest;
            GameState.LocalMap.IsUnderground = true;
            GameState.LocalMap.Location = dungeon;

            for(int i=0; i < 21; i++)
            {
                GameState.LocalMap.MinuteChanged(null,null); //Summon!
            }

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
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";
           

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(start);
            playerBlock.PutItemOnBlock(player);

            GameState.PlayerCharacter = new Actor();
            GameState.PlayerCharacter.MapCharacter = player;
            GameState.PlayerCharacter.IsPlayerCharacter = true;

            GameState.PlayerCharacter.Attributes = ActorGeneration.GenerateAttributes("human", DRObjects.ActorHandling.CharacterSheet.Enums.ActorProfession.WARRIOR, 10, GameState.PlayerCharacter);
            GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

            GameState.PlayerCharacter.Attributes.Health = GameState.PlayerCharacter.Anatomy;

            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

            //Generate the pathfinding map
            GameState.LocalMap.GeneratePathfindingMap();
        }

        public static void GenerateSettlement()
        {
            var settlement = SettlementGenerator.GenerateSettlement(new MapCoordinate(50, 50, 0, DRObjects.Enums.MapType.GLOBAL), GameState.Random.Next(10) + 2, new List<DRObjects.Enums.GlobalResourceType>(), new Civilisation() { Name = "Llama Empire", ID = 0 });

            GameState.LocalMap = new LocalMap(250, 250, 1, 0);
            GameState.LocalMap.Location = settlement;

            settlement.Civilisation = new Civilisation()
            {
                Faction = OwningFactions.HUMANS,
                ID = 1,
                Name = "Holy Llama Kingdom"
            };

            List<Actor> actors = null;

            PointOfInterest startPoint = null;

            var gennedMap = SettlementGenerator.GenerateMap(settlement, out actors, out startPoint);

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (MapBlock block in gennedMap)
            {
                collapsedMap.Add(block);
            }

            GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

            MapItem player = new MapItem();
            player.Coordinate = startPoint.Coordinate;
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(player.Coordinate);
            playerBlock.PutItemOnBlock(player);

            GameState.PlayerCharacter = new Actor();
            GameState.PlayerCharacter.MapCharacter = player;
            GameState.PlayerCharacter.IsPlayerCharacter = true;

            GameState.PlayerCharacter.Attributes = ActorGeneration.GenerateAttributes("human", DRObjects.ActorHandling.CharacterSheet.Enums.ActorProfession.WARRIOR, 10, GameState.PlayerCharacter);

            GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

            GameState.PlayerCharacter.Attributes.Health = GameState.PlayerCharacter.Anatomy;

            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);
            GameState.LocalMap.Actors.AddRange(actors);

        }

        public static void ParseXML()
        {
            LocalMapXMLParser parser = new LocalMapXMLParser();

            //Maplet maplet = parser.ParseMaplet(@"Maplets/IronMine.xml");

            Actor[] tempy = null;

            //MapBlock[,] generatedMap = gen.GenerateMap(0, null, maplet, true,"human",DRObjects.Enums.OwningFactions.HUMANS,out tempy);

            //put in the map

            ////Generate it
            LocalMapGenerator gen = new LocalMapGenerator();

            SiteData siteData = new SiteData();
            siteData.SiteTypeData = SiteDataManager.GetData(SiteType.STABLES);

            siteData.Biome = GlobalBiome.DENSE_FOREST;
            siteData.Owners = OwningFactions.HUMANS;

            //Locate the right actor counts
            siteData.LoadAppropriateActorCounts();

            MapBlock[,] generatedMap = SiteGenerator.GenerateSite(siteData, out tempy);

            GameState.LocalMap = new LocalMap(50, 50, 1, 0);
            GameState.LocalMap.Actors = tempy.ToList();

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (MapBlock block in generatedMap)
            {
                collapsedMap.Add(block);
            }

            GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

            MapItem player = new MapItem();
            player.Coordinate = new MapCoordinate(10, 5, 0, DRObjects.Enums.MapType.LOCAL);
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(5, 5, 0, DRObjects.Enums.MapType.LOCAL));
            playerBlock.PutItemOnBlock(player);
            GameState.PlayerCharacter = new Actor();
            GameState.PlayerCharacter.MapCharacter = player;
            GameState.PlayerCharacter.IsPlayerCharacter = true;

            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

            GameState.PlayerCharacter.Attributes = ActorGeneration.GenerateAttributes("human", DRObjects.ActorHandling.CharacterSheet.Enums.ActorProfession.WARRIOR, 10, GameState.PlayerCharacter);
            GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

            GameState.PlayerCharacter.Attributes.Health = GameState.PlayerCharacter.Anatomy;
        }

    }
}
