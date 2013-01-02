using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DivineRightGame.ItemFactory.Object;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    public class TilesManager: IItemFactoryManager
    {
        private const string FILENAME = "Tiles";

        public MapItem CreateItem(string internalName)
        {
            //get the file

            FileReader reader = new FileReader();
            MultiDictionary dict = reader.Read(FILENAME);

            List<string> parameters = dict[internalName];

            if (parameters == null)
            {
                throw new Exception("There is no such item called " + internalName);
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
            return CreateItem(parameters[0], parameters[1], parameters[2], parameters[3], parameters[4]);
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
        public MapItem CreateItem(string internalName, string name, string description, string graphic, string canHaveItems)
        {
            MapItem item = new MapItem();
            item.Description = description;
            item.Graphic = graphic;
            item.InternalName = internalName;
            item.MayContainItems = Boolean.Parse(canHaveItems);
            item.Name = name;

            return item;
        }


    }
}
