using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Extensions
{
   
    public static class Extensions
    {
        private static Random random = new Random();

        /// <summary>
        /// Returns a random, or a default if the IList is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this IList<T> source)
        {
            if (source.Count() == 0)
            {
                return default(T);
            }
            else
            {
                return source[random.Next(source.Count())];
            }
        }
    }
}
