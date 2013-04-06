using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    /// <summary>
    /// Interface for Item Factory Managers
    /// </summary>
    public interface IItemFactoryManager
    {
        /// <summary>
        /// Creates an item based on a list of parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        MapItem CreateItem(List<string> parameters);
        /// <summary>
        /// Creates an item from the file based on its internal name
        /// </summary>
        /// <param name="internalName"></param>
        /// <returns></returns>
        MapItem CreateItem(int internalID);
    }
}
