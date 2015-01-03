using DRObjects.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Local
{
    public class TreasureChest:
        MapItem
    {
        /// <summary>
        /// The items this chest contains
        /// </summary>
        public List<InventoryItem> Contents
        {
            get
            {
                if (contents == null)
                {
                    GenerateContents();
                }

                return contents;
            }
        }
        /// <summary>
        /// The categories this item is allowed to generate.
        /// </summary>
        public InventoryCategory[] Categories { get; set; }
        /// <summary>
        /// The total value of goods to create.
        /// </summary>
        public int TotalValue { get; set; }

        private List<InventoryItem> contents = null;


        /// <summary>
        /// Generates the contents as per the allowed categories and value 
        /// Works as follows - first picks a category at random and tries to spend all the money on it.
        /// With the remainder will try to spend all the money on the second and third category at random.
        /// </summary>
        private void GenerateContents()
        {

        }

    }
}
