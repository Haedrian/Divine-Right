using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.DataStructures
{
    [Serializable]
    /// <summary>
    /// A List of components grouped together by a particular type
    /// </summary>
    public class GroupedList<T>
    {
        private Dictionary<object,List<T>> list;
        private List<T> totalList;

        public GroupedList()
        {
            list = new Dictionary<object, List<T>>();
            totalList = new List<T>();
        }

        public void Add(object group,T item)
        {
            if (list.ContainsKey(group))
            {
                //Add to that list
                list[group].Add(item);
            }
            else 
            {
                //Create a new list
                list.Add(group,new List<T>());
                list[group].Add(item);
            }

            //Add to total list
            totalList.Add(item);
        }

        /// <summary>
        /// Returns all objects of a particular group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IEnumerable<T> GetObjectsByGroup(object group)
        {
            if (list.ContainsKey(group))
            {
                return list[group];
            }
            else
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// Returns all objects held in the list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAllObjects()
        {
            return new List<T>(totalList);
        }

        /// <summary>
        /// Removes a particular object from the collection
        /// </summary>
        /// <param name="group"></param>
        /// <param name="element"></param>
        public void Remove(object group,T element)
        {
            if (list.ContainsKey(group))
            {
                list[group].Remove(element);
            }
        }
    }
}
