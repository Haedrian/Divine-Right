using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Graphics;
using DRObjects;
using DRObjects.Database;
using DRObjects.Enums;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    /// <summary>
    /// For creating inventory items
    /// </summary>
    public class InventoryItemManager
    {
        public DRObjects.MapItem CreateItem(List<string> parameters)
        {
            return InventoryItemManager(Int32.Parse(parameters[0]), parameters[1], parameters[2], parameters[3], Int32.Parse(parameters[4]), Boolean.Parse(parameters[5]), Int32.Parse(parameters[6])
                , Int32.Parse(parameters[7]), parameters[8]);
        }

        public DRObjects.MapItem CreateItem(int internalID)
        {
            //get the traits from the database

            List<string> parameters = DatabaseHandling.GetItemProperties(Archetype.INVENTORYITEMS, internalID);

            if (parameters == null)
            {
                throw new Exception("There is no such item with id " + internalID);
            }

            //otherwise create it
            return CreateItem(parameters);
        }

        public MapItem InventoryItemManager(int itemID,string name,string description, string graphic, int value  
            ,bool equippable,int armourRating, int damageRating, string damageType)
        {
            InventoryItem item = new InventoryItem();
            item.Description = description;

            string chosenGraphic = String.Empty;

            //Does graphic contain multiple choices?
            if (graphic.Contains(","))
            {
                //yes, lets split it
                var graphics = graphic.Split(',');

                //use random to determine which one we want
                chosenGraphic = graphics[GameState.Random.Next(graphics.Length)];

                item.Graphic = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), chosenGraphic));
            }
            else
            {
                //nope
                item.Graphic = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), graphic));
            }

            item.InInventory = false;
            item.InternalName = name;
            item.IsEquippable = equippable;
            item.IsEquipped = false;
            item.MayContainItems = true;
            item.Name = name;

            return item;
        }
    }
}
