using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame;
using DRObjects;
using DivineRightGame.ItemFactory;
using DivineRightGame.MapFactory;
using DRObjects.Graphics;

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

        
        /// <summary>
        /// prepares a hard coded test map and puts the player in the middle
        /// </summary>
        public static void PrepareHardCodedTestMap()
        {
            GameState.LocalMap = new LocalMap(15,15,0,0);

            //Player character
            DivineRightGame.GameState.PlayerCharacter = new DRObjects.Actor();
            GameState.PlayerCharacter.IsPlayerCharacter = true;
            
            //tile player is standing on
            MapBlock block = new MapBlock();
            ItemFactory factory = new ItemFactory();

            for (int i = -15; i < 15; i++)
            {
                for (int j = -15; j < 15; j++)
                {

                    if (i % 2 == 0)
                    {
                        block = new MapBlock();
                        block.Tile = factory.CreateItem("tile", "Wood");
                        block.Tile.Coordinate = new MapCoordinate(i, j, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                        GameState.LocalMap.AddToLocalMap(block);
                    }
                    else if (j % 2 == 0)
                    {
                        block = new MapBlock();
                        block.Tile = factory.CreateItem("tile", "Pavement");
                        block.Tile.Coordinate = new MapCoordinate(i, j, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                        GameState.LocalMap.AddToLocalMap(block);
                    }
                    else
                    {
                        block = new MapBlock();
                        block.Tile = factory.CreateItem("tile", "Grass");
                        block.Tile.Coordinate = new MapCoordinate(i, j, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                        GameState.LocalMap.AddToLocalMap(block);
                    }
                }
            }

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(0,0,0,DRObjects.Enums.MapTypeEnum.LOCAL));

            MapItem player = new MapItem();
            player.Coordinate = new MapCoordinate(0,0,0,DRObjects.Enums.MapTypeEnum.LOCAL);
            player.Description = "The player character";
            player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR);
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            playerBlock.PutItemOnBlock(player);
            //player character item

            GameState.PlayerCharacter.MapCharacter = player;

            //GameState.LocalMap.AddToLocalMap(playerBlock);
            /*
            #region other blocks

            MapBlock block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(1, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-1, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(1, 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(1, -1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-1, -1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-1, 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(0, -1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(0, 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(2, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Wooden Tile";
            block.Tile.Description = "Wooden Tile of Wooden Tiling";
            block.Tile.Graphic = "WoodTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Parquet Floor";

            GameState.LocalMap.AddToLocalMap(block);

            #endregion

            #region Castle

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-2, 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Grass Tile";
            block.Tile.Description = "A patch of Grass";
            block.Tile.Graphic = "GrassTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Grass";

            MapItem item = new MapItem();
            item.Coordinate = new MapCoordinate(-2, 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
            item.Description = "The wall of a castle";
            item.Graphic = "CastleWall";
            item.MayContainItems = false;
            item.InternalName = "Castle wall";
            item.Name = "Castle Wall";

            block.PutItemOnBlock(item);
            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-2, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Grass Tile";
            block.Tile.Description = "A patch of Grass";
            block.Tile.Graphic = "GrassTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Grass";

            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-3, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Grass Tile";
            block.Tile.Description = "A patch of Grass";
            block.Tile.Graphic = "GrassTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Grass";

            item = new MapItem();
            item.Coordinate = new MapCoordinate(-3, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
            item.Description = "A beautiful long necked creature";
            item.Graphic = "Llama";
            item.InternalName = "llama";
            item.MayContainItems = false;
            item.Name = "Llama";

            block.PutItemOnBlock(item);
            GameState.LocalMap.AddToLocalMap(block);

            block = new MapBlock();
            block.Tile = new MapItem(new MapCoordinate(-2, -1, 0, DRObjects.Enums.MapTypeEnum.LOCAL));
            block.Tile.InternalName = "Grass Tile";
            block.Tile.Description = "A patch of Grass";
            block.Tile.Graphic = "GrassTile";
            block.Tile.MayContainItems = true;
            block.Tile.Name = "Grass";

            item = new MapItem();
            item.Coordinate = new MapCoordinate(-2, -1, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
            item.Description = "The wall of a castle";
            item.Graphic = "CastleWall";
            item.MayContainItems = false;
            item.InternalName = "Castle wall";
            item.Name = "Castle Wall";

            block.PutItemOnBlock(item);
            GameState.LocalMap.AddToLocalMap(block);
            
            #endregion
            */
        }

    }
}
