using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DivineRightGame.ItemFactory.Object;
using DRObjects.Graphics;
using DRObjects.Enums;
using DRObjects.Database;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    public class TilesManager: IItemFactoryManager
    {
        private const Archetype ARCHETYPE = Archetype.TILES;

        public MapItem CreateItem(int itemID)
        {
            //get the file
            List<string> parameters = DatabaseHandling.GetItemProperties(ARCHETYPE, itemID);

            if (parameters == null)
            {
                throw new Exception("There is no such item with id " + itemID);
            }

            //otherwise create it
            return CreateItem(parameters);

        }

        /// <summary>
        /// Creates a mundane item from particular parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public MapItem CreateItem(List<string> parameters)
        {
            return CreateItem(parameters[1], parameters[2], parameters[3], parameters[4],parameters[5]);
        }

        /// <summary>
        /// Creates a mundane map item from the parameters.
        /// </summary>
        /// <param name="internalName">The internal name</param>
        /// <param name="name">The name to be shown when the user clicks on it</param>
        /// <param name="description">A description shown when the item is examined</param>
        /// <param name="graphic">The graphic to show</param>
        /// <param name="canHaveItems">Whether it can have items placed on it or not</param>
        /// <returns></returns>
        public MapItem CreateItem(string name, string description, string graphic, string canHaveItems,string tags)
        {
            MapItem item = new MapItem();
            item.Description = description;
            item.Graphic = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), graphic)); 
            item.MayContainItems = Boolean.Parse(canHaveItems);
            item.Name = name;
            item.InternalName = tags;

            return item;
        }


    }
}
