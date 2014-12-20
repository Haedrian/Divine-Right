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
        /// Returns a random 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandom<T>(this List<T> list)
        {
            if (list.Count == 0)
            {
                return default(T);
            }
            else
            {
                return list[random.Next(list.Count)];
            }
        }
    }
}
