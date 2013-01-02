using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.ItemFactory.Object
{
    public class KeyListItem
    {
        public string Key { get; set; }
        public List<string> ListItems { get; set; }

        public override string ToString()
        {
            return Key + " -> " + ListItems.Count;
        }

        public KeyListItem(string key, List<string> list)
        {
            this.Key = key;
            this.ListItems = list;

        }
    }
}
