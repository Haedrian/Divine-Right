using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DivineRightGame.ItemFactory.ItemFactoryManagers;
using DRObjects.Enums;
using DRObjects.Database;

namespace DivineRightGame.ItemFactory
{
    /// <summary>
    /// This class is used to create items from a string.
    /// </summary>
    public class ItemFactory
    {
        /// <summary>
        /// Creates an item from its category and its internal name
        /// </summary>
        /// <param name="category"></param>
        /// <param name="internalName"></param>
        /// <returns></returns>
        public MapItem CreateItem(string category, int itemID)
        {
            IItemFactoryManager mgr = null;
            switch (category.ToLower())
            {
                case "mundaneitem":
                    mgr = new MundaneItemsManager();
                    break;
                case "mundaneitems":
                    mgr = new MundaneItemsManager();
                    break;
                case "tile":
                    mgr = new TilesManager();
                    break;
                case "tiles":
                    mgr = new TilesManager();
                    break;
                case "toggleitems":
                    mgr = new ToggleItemsManager();
                    break;
                case "toggleitem":
                    mgr = new ToggleItemsManager();
                    break;
                case "enemies":
                    mgr = new EnemyManager();
                    break;
                default:
                    throw new NotImplementedException("The category : " + category + " could not be found");
            }

            return mgr.CreateItem(itemID);
        }

        /// <summary>
        /// Creates an item from an Archetype and a tag.
        /// The item to be created is random, as long as it shares the correct tag.
        /// </summary>
        /// <param name="archetype"></param>
        /// <param name="tag"></param>
        /// <param name="itemID">The id of the chosen item</param>
        /// <returns></returns>
        public MapItem CreateItem(Archetype archetype, string tag,out int itemID)
        {
            int id = DatabaseHandling.GetItemIdFromTag(archetype, tag);

            itemID = id;

            //Now get the actual item
            return this.CreateItem(archetype.ToString().ToLower(), id);
        }

        /// <summary>
        /// Creates an item from a category and a tag
        /// The item to be created is random, as long as it shares the correct tag
        /// </summary>
        /// <param name="category"></param>
        /// <param name="tag"></param>
        /// <param name="itemID">The id of the chosen item, in case multiples need to be produced</param>
        /// <returns></returns>
        public MapItem CreateItem(string category, string tag, out int itemID)
        {
            int id = DatabaseHandling.GetItemIdFromTag((Archetype) Enum.Parse(typeof(Archetype),category,true), tag);
            itemID = id;

            //Now get the actual item
            return this.CreateItem(category, id);
        }

        public MapItem CreateItem(string category, List<string> parameters)
        {
            IItemFactoryManager mgr = null;
            switch (category)
            {
                case "mundaneitem":
                    mgr = new MundaneItemsManager();
                    break;
                case "tile":
                    mgr = new TilesManager();
                    break;
                case "toggleitem":
                    mgr = new ToggleItemsManager();
                    break;
                case "enemies":
                    mgr = new EnemyManager();
                    break;
                default: throw new NotImplementedException("The category : " + category + " could not be found");
            }

            return mgr.CreateItem(parameters);
        }
    }
}
