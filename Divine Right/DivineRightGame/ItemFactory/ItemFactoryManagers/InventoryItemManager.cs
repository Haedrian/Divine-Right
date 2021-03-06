﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Graphics;
using DRObjects;
using DRObjects.Database;
using DRObjects.Enums;
using DRObjects.Extensions;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    /// <summary>
    /// For creating inventory items
    /// </summary>
    public class InventoryItemManager : IItemFactoryManager
    {
        public DRObjects.MapItem CreateItem(List<string> parameters)
        {
            return CreateItem(Int32.Parse(parameters[0]), parameters[1], parameters[2], parameters[3], Int32.Parse(parameters[4]), Boolean.Parse(parameters[5]), Int32.Parse(parameters[6])
                , Int32.Parse(parameters[7]), parameters[8], parameters[10], parameters[11], Int32.Parse(string.IsNullOrEmpty(parameters[12]) ? "0" : parameters[12]), Int32.Parse(string.IsNullOrEmpty(parameters[13]) ? "0" : parameters[13]),
                Boolean.Parse(parameters[14]), Int32.Parse(string.IsNullOrEmpty(parameters[15]) ? "-1" : parameters[15]), Int32.Parse(string.IsNullOrEmpty(parameters[16]) ? "-1" : parameters[16]),Boolean.Parse(parameters[17]));
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
        /// Returns the most expensive item of that particular tag which still costs less or equal to the maximum
        /// </summary>
        /// <param name="category"></param>
        /// <param name="maximum"></param>
        /// <returns></returns>
        public InventoryItem GetBestCanAfford(string tag, int maximum)
        {
            //Get the entire database
            var database = DatabaseHandling.GetDatabase(Archetype.INVENTORYITEMS);

            //The reason there are two orders is that two items might have the same cost, in that case we'll want to pick one at random
            var chosenValue = database.Where(d => tag == null || d.Value[9].ToLower().Split(',').Contains(tag.ToLower().Trim()) && Int32.Parse(d.Value[4]) <= maximum).OrderByDescending(d => Int32.Parse(d.Value[4])).ThenBy(d => GameState.Random.Next(10)).Select(d => d.Key).FirstOrDefault();

            if (chosenValue == 0)
            {
                return null;
            }

            return CreateItem(chosenValue) as InventoryItem;
        }

        /// <summary>
        /// Fills a treasure chest as follows:
        /// 1. Pick a random category. Produce an item of that category costing between 1/5 and 3/3 of the remaining value
        /// 2. Repeat 1 for 6 more times
        /// 3. Done
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="spendValue"></param>
        /// <returns></returns>
        public List<InventoryItem> FillTreasureChest(InventoryCategory[] categories,int spendValue)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            for (int i = 0; i < 7; i++)
            {
                InventoryCategory cat = categories.GetRandom();

                var item = GetItemWithinPriceRange(cat.ToString(), spendValue / 5, spendValue);

                if (item != null)
                {
                    spendValue -= item.BaseValue;
                    item.InInventory = true; //to get the proper description

                    items.Add(item);
                }

            }

            return items;

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
            ,bool equippable,int armourRating, int damageRating, string damageType,string category,string equippableLocation,
            int woundPotential, int stunAmount,bool stackable,int effect, int effectPower,bool isRanged)
        {
            InventoryItem item = null;

            if (effect == -1)
            {
                item = new InventoryItem();
            }
            else
            {
                item = new ConsumableItem();
            }

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
            item.DamageDice = damageRating;
            item.StunAmount = stunAmount;
            item.WoundPotential = woundPotential;
            item.WeaponType = damageType;
            item.Stackable = stackable;
            item.TotalAmount = 1;
            item.IsRanged = isRanged;

            if (effect != -1)
            {
                (item as ConsumableItem).Effects = (ConsumableEffect) effect;
                (item as ConsumableItem).EffectPower = effectPower;
            }

            if (!String.IsNullOrEmpty(equippableLocation))
            {
                item.EquippableLocation = (EquipmentLocation)Enum.Parse(typeof(EquipmentLocation), equippableLocation);
            }

            return item;
        }
    }
}
