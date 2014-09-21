using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Local;
using DRObjects.DataStructures;

namespace DRObjects.ActorHandling
{
    [Serializable]
    /// <summary>
    /// Details pertaining to a vendor such as his stock inventory and his money.
    /// </summary>
    public class VendorDetails
    {
        /// <summary>
        /// The type of the vendor
        /// </summary>
        public VendorType VendorType { get; set; }
        /// <summary>
        /// The level of the vendor. That is to say the level of the building they're in
        /// </summary>
        public int VendorLevel { get; set; }

        /// <summary>
        /// The stock of items that can be sold.
        /// </summary>
        public GroupedList<InventoryItem> Stock { get; set; }

        /// <summary>
        /// The number of funds this vendor has 
        /// </summary>
        public int Money { get; set; }

        /// <summary>
        /// Gets the price multiplier for a particular category of item.
        /// The base price is multiplied by a value which depends on the category of the item (and whether the vendor would be interested), as well as the level of the vendor in question 
        /// Generally it's better to buy from specific big vendors and sell to specific small ones
        /// </summary>
        /// <param name="category"></param>
        /// <param name="vendorIsBuying"></param>
        /// <returns></returns>
        public double GetPriceMultiplier(InventoryCategory category, bool vendorIsBuying)
        {
            if (this.VendorType == DRObjects.Enums.VendorType.GENERAL)
            {
                //Everything is at a markup, regardless of the type. Later food won't be
                double multiplier = 1;

                if (vendorIsBuying)
                {
                    multiplier = 0.50;
                }
                else
                {
                    switch (this.VendorLevel)
                    {
                        case 1:
                            multiplier = 1.50;
                            break;
                        case 2:
                            multiplier = 1.40; break;
                        case 3:
                            multiplier = 1.30;
                            break;
                    }
                }
                
                return multiplier;
            }
            else if (this.VendorType == DRObjects.Enums.VendorType.TRADER)
            {
                //Will buy loot at best price
                double multiplier = 1;

                if (vendorIsBuying)
                {
                    if (category == InventoryCategory.LOOT)
                    {
                        multiplier = 1;
                    }
                    else
                    {
                        multiplier = 0.50;
                    }
                }
                else
                {
                    if (category == InventoryCategory.LOOT)
                    {
                        switch (this.VendorLevel)
                        {
                            case 1:
                                multiplier = 1.20;
                                break;
                            case 2:
                                multiplier = 1.10; break;
                            case 3:
                                multiplier = 1.00;
                                break;
                        }
                    }
                    else
                    {
                        switch (this.VendorLevel)
                        {
                            case 1:
                                multiplier = 1.50;
                                break;
                            case 2:
                                multiplier = 1.40; break;
                            case 3:
                                multiplier = 1.30;
                                break;
                        }
                    }
                }

                return multiplier;
            }
            else if (this.VendorType == DRObjects.Enums.VendorType.SMITH)
            {
                //Will buy weapons and armour at best price
                double multiplier = 1;

                if (vendorIsBuying)
                {
                    if (category == InventoryCategory.WEAPON || category == InventoryCategory.ARMOUR)
                    {
                        multiplier = 0.8;
                    }
                    else
                    {
                        multiplier = 0.50;
                    }
                }
                else
                {
                    if (category == InventoryCategory.WEAPON || category == InventoryCategory.ARMOUR)
                    {
                        switch (this.VendorLevel)
                        {
                            case 1:
                                multiplier = 1.20;
                                break;
                            case 2:
                                multiplier = 1.10; break;
                            case 3:
                                multiplier = 1.00;
                                break;
                        }
                    }
                    else
                    {
                        switch (this.VendorLevel)
                        {
                            case 1:
                                multiplier = 1.50;
                                break;
                            case 2:
                                multiplier = 1.40; break;
                            case 3:
                                multiplier = 1.30;
                                break;
                        }
                    }
                }

                return multiplier;
            }
            else
            {
                throw new NotImplementedException("No code for vendor of type " + VendorType);
            }
        }
    }
}
