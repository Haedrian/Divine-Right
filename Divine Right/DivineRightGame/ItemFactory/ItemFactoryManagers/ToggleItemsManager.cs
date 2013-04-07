using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Graphics;
using DRObjects.Database;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    public class ToggleItemsManager:IItemFactoryManager
    {
        private const Archetype ARCHETYPE = Archetype.TOGGLEITEMS;

        public DRObjects.MapItem CreateItem(List<string> parameters)
        {
            return this.CreateItem(parameters[1], parameters[2], parameters[3], parameters[4], parameters[5], parameters[6], parameters[7], parameters[8], parameters[9]);
        }

        public DRObjects.MapItem CreateItem(int itemID)
        {
            //get the traits from the database

            List<string> parameters = DatabaseHandling.GetItemProperties(ARCHETYPE, itemID);

            if (parameters == null)
            {
                throw new Exception("There is no such item with id " + itemID);
            }

            //otherwise create it
            return CreateItem(parameters);
        }

        private ToggleItem CreateItem(string name, string descriptionA, string descriptionB, string graphicA, string graphicB, string messageA, string messageB, string mayContainA, string mayContainB)
        {
            ToggleItem item = new ToggleItem();
            item.AllowItemsStateA = bool.Parse(mayContainA);
            item.AllowItemsStateB = bool.Parse(mayContainB);
            item.DescriptionStateA = descriptionA;
            item.DescriptionStateB = descriptionB;
            item.GraphicStateA = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), graphicA));
            item.GraphicStateB = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), graphicB));
            item.MessageToStateA = messageA;
            item.MessageToStateB = messageB;
            item.Name = name;

            return item;
        }
    }
}
