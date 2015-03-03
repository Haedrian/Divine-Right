using DRObjects.Economics.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Economics
{
    [Serializable]
    /// <summary>
    /// Represents a stockpile of a particular type of resource - owned by a particular city, and the prices it is ready to pay for
    /// </summary>
    public class Stockpile
    {
        public const int MAXSIZE = 100;

        public ResourceCategory Category { get; set; }
        public ResourceType Resource { get; set; }
        /// <summary>
        /// The actual total amount of resources in this stockpile
        /// </summary>
        public int StockpileSize { get; set; }
        /// <summary>
        /// The total amount of resources we wish to reach
        /// </summary>
        public int StockpileTarget {get;set;}

        public int MaxBuyPrice { get; set; }
        public int MinSellPrice { get; set; }

        /// <summary>
        /// Determines whether the prices have been calculated through buying/selling and not through other settlement's prices
        /// </summary>
        public bool PricesReal { get; set; }

        public Stockpile()
        {
            this.PricesReal = false;
        }
    }
}
