using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// The Possible Contents of the Maplet.
    /// For each possible amount, there will be a check whether probability is reached
    /// If you reach the probability, one will be created at each loop.
    /// If there is less than min amount after this is done, then we'll forget all about it
    /// </summary>
    public class MapletContents
    {
        /// <summary>
        /// The probability of creating the Contents. Will be run for each potential amount.
        /// </summary>
        public double ProbabilityPercentage { get; set; }
        /// <summary>
        /// The Maximum Amount to be created
        /// </summary>
        public int MaxAmount { get; set; }
    }
}
