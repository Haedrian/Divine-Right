using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame;
using DRObjects;

namespace Divine_Right.HelperFunctions
{
    public static class TestFunctions
    {
        /// <summary>
        /// prepares a hard coded test map and puts the player in the middle
        /// </summary>
        public static void PrepareHardCodedTestMap()
        {
            //Player character
            DivineRightGame.GameState.PlayerCharacter = new DRObjects.Actor();
            GameState.PlayerCharacter.IsPlayerCharacter = true;
            
            //tile player is standing on
            MapBlock playerBlock = new MapBlock();
            playerBlock.Tile = new MapItem(new MapCoordinate(0,0,0,DRObjects.Enums.MapTypeEnum.LOCAL));
            playerBlock.Tile.InternalName = "Wooden Tile";
            playerBlock.Tile.Description = "Wooden Tile of Wooden Tiling";
            playerBlock.Tile.Graphic = "WoodTile";
            playerBlock.Tile.MayContainItems = true;
            playerBlock.Tile.Name = "Parquet Floor";
            
            MapItem player = new MapItem();
            player.Coordinate = new MapCoordinate(0,0,0,DRObjects.Enums.MapTypeEnum.LOCAL);
            player.Description = "The player character";
            player.Graphic = "Player";
            player.InternalName = "Player Char";
            player.MayContainItems = false;
            player.Name = "Player";

            playerBlock.PutItemOnBlock(player);
            //player character item

            GameState.PlayerCharacter.MapCharacter = player;
            GameState.LocalMap = new LocalMap();

            GameState.LocalMap.AddToLocalMap(playerBlock);

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
            item.MayContainItems = true;
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
            item.MayContainItems = true;
            item.InternalName = "Castle wall";
            item.Name = "Castle Wall";

            block.PutItemOnBlock(item);
            GameState.LocalMap.AddToLocalMap(block);

            #endregion

        }

    }
}
