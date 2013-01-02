using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.ItemFactory.Object
{
    /// <summary>
    /// A 'dictionary' which accepts duplicate keys. If a key is duplicate, it will pick a random value which has that key.
    /// This is slower than a real dictionary obviously.
    /// </summary>
    public class MultiDictionary
    {
        #region Members
        private List<KeyListItem> keyListItems;
        #endregion

        #region Functions

        /// <summary>
        /// Adds an item to the MultiDictionary
        /// </summary>
        /// <param name="stringList"></param>
        public void Add(string key,List<string> stringList)
        {
            keyListItems.Add(new KeyListItem(key,stringList));
        }

        /// <summary>
        /// Gets the item with the particular key.
        /// If the key is not found it will return null
        /// If more than one item has the same key, it will return one at random
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<string> this[string key]
        {
            get 
            {
                List<KeyListItem> items = keyListItems.Where(k => k.Key.Equals(key)).ToList();

                if (items.Count == 0)
                {
                    return null;
                }
                else if (items.Count == 1)
                {
                    return items[0].ListItems;

                }
                else
                {
                    //pick a random one
                    Random random = new Random();

                    return items[random.Next(items.Count)].ListItems;
                }

            }

        }


        #endregion

        #region Constructors
        public MultiDictionary()
        {
            this.keyListItems = new List<KeyListItem>();

        }
        #endregion
    }
}
