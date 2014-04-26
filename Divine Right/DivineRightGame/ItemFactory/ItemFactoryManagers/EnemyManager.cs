using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Items.Archetypes.Local;
using DRObjects;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.Database;

namespace DivineRightGame.ItemFactory.ItemFactoryManagers
{
    class EnemyManager: IItemFactoryManager
    {
        private const Archetype ARCHETYPE = Archetype.ENEMIES;
        private static Random _random = new Random();

        public DRObjects.MapItem CreateItem(List<string> parameters)
        {
            return CreateItem(parameters[1], parameters[2], parameters[3],Int32.Parse(parameters[6]),parameters[11]);
        }

        public DRObjects.MapItem CreateItem(int internalID)
        {
            //get the traits from the database

            List<string> parameters = DatabaseHandling.GetItemProperties(ARCHETYPE, internalID);

            if (parameters == null)
            {
                throw new Exception("There is no such item with id " + internalID);
            }

            //otherwise create it
            return CreateItem(parameters);
        }

        public LocalCharacter CreateItem(string enemyName,string enemyDescription,string graphic,int lineOfSight,string graphicSet)
        {
            LocalCharacter enemy = new LocalCharacter();
            enemy.Description = enemyDescription;
            enemy.EnemyThought = DRObjects.Enums.EnemyThought.WAIT;

            string chosenGraphic = String.Empty;

            if (!String.IsNullOrEmpty(graphicSet))
            {
                //Instead of a single graphic, use a graphical set
                enemy.Graphics = GraphicSetManager.GetSprites((GraphicSetName) Enum.Parse(typeof(GraphicSetName),graphicSet.ToUpper()));
            }
            else
            {
                //Does graphic contain multiple choices?
                if (graphic.Contains(","))
                {
                    //yes, lets split it
                    var graphics = graphic.Split(',');

                    //use random to determine which one we want
                    chosenGraphic = graphics[_random.Next(graphics.Length)];

                    enemy.Graphic = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), chosenGraphic));
                }
                else
                {
                    //nope
                    enemy.Graphic = SpriteManager.GetSprite((LocalSpriteName)Enum.Parse(typeof(LocalSpriteName), graphic));
                }
            }
            enemy.LineOfSightRange = lineOfSight;
            enemy.InternalName = enemyName;
            enemy.MayContainItems = false;
            enemy.Name = enemyName;

            return enemy;
        }
    }
}
