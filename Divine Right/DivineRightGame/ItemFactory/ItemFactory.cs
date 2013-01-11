using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DivineRightGame.ItemFactory.ItemFactoryManagers;

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
        public MapItem CreateItem(string category, string internalName)
        {
            IItemFactoryManager mgr = null;
            switch (category.ToLower())
            {
                case "mundaneitem":
                    mgr = new MundaneItemsManager();
                    break;
                case "tile":
                    mgr = new TilesManager();
                    break;
                default:
                    throw new NotImplementedException("The category : " + category + " could not be found");
            }

            return mgr.CreateItem(internalName);
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
                default: throw new NotImplementedException("The category : " + category + " could not be found");
            }

            return mgr.CreateItem(parameters);
        }
    }
}
