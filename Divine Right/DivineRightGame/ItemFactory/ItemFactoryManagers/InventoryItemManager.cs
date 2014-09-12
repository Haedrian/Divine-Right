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
            return CreateItem(Int32.Parse(parameters[0]), parameters[1], parameters[2], parameters[3], Int32.Parse(parameters[4]), Boolean.Parse(parameters[5]), Int32.Parse(parameters[6])
                , Int32.Parse(parameters[7]), parameters[8],parameters[10],parameters[11]);
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

        /// <summary>
        /// Creates an item with an optional category costing between a minimum and maximum price
        /// </summary>
        /// <param name="category"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public InventoryItem GetItemWithinPriceRange(string category,int minimum, int maximum)
        {
            //Get the entire database
            var database = DatabaseHandling.GetDatabase(Archetype.INVENTORYITEMS);

            var chosenValue = database.Where(d => category == null || category.Equals(d.Value[10]) && Int32.Parse(d.Value[4]) >= minimum && Int32.Parse(d.Value[4]) <= maximum).OrderBy(d => GameState.Random.Next(1000)).Select(d => d.Key).FirstOrDefault();

            if (chosenValue == 0)
            {
                return null;
            }

            //Otherwise create the item
            return CreateItem(chosenValue) as InventoryItem;
        }

        /// <summary>
        /// Creates a number of items having a total maximum value (or as close as possible to) the assigned value.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public List<MapItem> GetItemsWithAMaxValue(string category, int maximum)
        {
            int value = 0;
            List<MapItem> items = new List<MapItem>();

            while (value < maximum)
            {
                InventoryItem newItem = GetItemWithinPriceRange(category, 0, maximum - value);

                if (newItem == null)
                {
                    //No more items
                    break;
                }

                items.Add(newItem);
                value += newItem.BaseValue; 
            }

            return items;
        }

        public MapItem CreateItem(int itemID,string name,string description, string graphic, int value  
            ,bool equippable,int armourRating, int damageRating, string damageType,string category,string equippableLocation)
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
            item.Category = (InventoryCategory) Enum.Parse(typeof(InventoryCategory), category);
            item.BaseValue = value;
            item.ArmourRating = armourRating;
            item.DamageRating = damageRating;

            if (!String.IsNullOrEmpty(equippableLocation))
            {
                item.EquippableLocation = (EquipmentLocation)Enum.Parse(typeof(EquipmentLocation), equippableLocation);
            }

            return item;
        }
    }
}
