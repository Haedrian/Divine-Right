using DRObjects.Enums;
using DRObjects.Feedback;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Local
{
    /// <summary>
    /// This is a special kind of well where you can (once) toss coins into it and see if somethign comes out :)
    /// </summary>
    public class WishingWell
        : MapItem
    {
        private List<SpriteData> graphics { get; set; }

        private bool isUsed = false;

        public override List<SpriteData> Graphics
        {
            get
            {
                if (graphics == null)
                {
                    graphics = new List<SpriteData>();
                    graphics.Add(SpriteManager.GetSprite(LocalSpriteName.WELL_1));
                }

                return graphics;
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override string Description
        {
            get
            {
                if (!isUsed)
                {
                    return "A wishing well. Throw some money in and make a wish";
                }
                else
                {
                    return "A wishing well. Those the magic seems to have died out";
                }
            }
            set
            {
                base.Description = value;
            }
        }

        public override string Name
        {
            get
            {
                if (!isUsed)
                {
                    return "Wishing Well";
                }
                else
                {
                    return "Well";
                }
            }
            set
            {
                base.Name = value;
            }
        }

        public override bool MayContainItems
        {
            get
            {
                return true;
            }
            set
            {
                base.MayContainItems = value;
            }
        }

        public override Enums.ActionType[] GetPossibleActions(Actor actor)
        {
            if (isUsed)
            {
                return base.GetPossibleActions(actor);
            }
            else
            {
                List<ActionType> actions = base.GetPossibleActions(actor).ToList();

                //We can either toss in 10, 50 or 100 coins

                if (actor.Inventory.TotalMoney > 10)
                {
                    actions.Add(ActionType.TOSS_IN_10_COINS);
                    actor.Inventory.TotalMoney -= 10;
                }

                if (actor.Inventory.TotalMoney > 50)
                {
                    actions.Add(ActionType.TOSS_IN_50_COINS);
                    actor.Inventory.TotalMoney -= 50;
                }

                if (actor.Inventory.TotalMoney > 100)
                {
                    actions.Add(ActionType.TOSS_IN_100_COINS);
                    actor.Inventory.TotalMoney -= 100;
                }

                return actions.ToArray();
            }
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType != ActionType.TOSS_IN_10_COINS && actionType != ActionType.TOSS_IN_50_COINS && actionType != ActionType.TOSS_IN_100_COINS)
            {
                return base.PerformAction(actionType, actor, args);
            }
            else
            {
                Random random = new Random();
                //We have a 10% chance of giving something (weapon or armour) back
                int result = random.Next(10);

                if (result == 0)
                {
                    //Return something!
                    //Return a weapon or piece of armour worth 10 times the money we put in
                    int money = 0;

                    switch(actionType)
                    {
                        case ActionType.TOSS_IN_10_COINS: money = 100; break;
                        case ActionType.TOSS_IN_100_COINS: money = 1000; break;
                        case ActionType.TOSS_IN_50_COINS: money = 500; break;
                    }

                    isUsed = true;

                    return new ActionFeedback[]{new ReceiveItemFeedback(){Category = random.Next(2) == 0 ? InventoryCategory.ARMOUR : InventoryCategory.WEAPON,MaxValue = money} };
                }
                else
                {
                    //Nothing :(
                    return new ActionFeedback[] { new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "You make your offering to the well. You wait awhile but nothing happens") };
                }
            }
            
        }
    }
}
