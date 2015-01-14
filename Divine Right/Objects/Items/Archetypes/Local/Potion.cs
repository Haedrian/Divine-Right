using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.Enums;
using DRObjects.Enums;
using DRObjects.Feedback;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;

namespace DRObjects.Items.Archetypes.Local
{
    /// <summary>
    /// Represents a Potion of some sort
    /// </summary>
    public class Potion
        : InventoryItem
    {
        private List<SpriteData> _graphics { get; set; }
        public PotionType PotionType { get; set; }
        private bool _isIdentified;

        public bool IsIdentified
        {
            get
            {
                return _isIdentified;
            }
            set
            {
                //Regenerate the graphics
                this._graphics = null;

                this._isIdentified = value;
            }
        }

        public override string Description
        {
            get
            {
                if (IsIdentified)
                {
                    return "A potion of " + PotionType.ToString().Replace("_", " ").ToLower();
                }
                else
                {
                    return "A potion of uncertain effect.";
                }
            }
            set
            {
                base.Description = value;
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

        public override List<SpriteData> Graphics
        {
            get
            {
                if (this._graphics == null)
                {
                    this._graphics = new List<SpriteData>();

                    //generate it
                    if (!this.IsIdentified)
                    {
                        _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_UNKNOWN));
                    }
                    else
                    {
                        switch (this.PotionType)
                        {
                            case PotionType.AGILITY:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_AGIL));
                                break;
                            case Enums.PotionType.BLEEDING:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_BLEED));
                                break;
                            case Enums.PotionType.BLINDING:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_BLIND));
                                break;
                            case Enums.PotionType.BRAWN:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_BRAWN));
                                break;
                            case Enums.PotionType.CHARISMA:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_CHARISMA));
                                break;
                            case Enums.PotionType.DEFENCE:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_ARMOUR));
                                break;
                            case Enums.PotionType.DEFENSIVE_KNOWLEDGE:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_DEFEND));
                                break;
                            case Enums.PotionType.FEEDING:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_FEED));
                                break;
                            case Enums.PotionType.FIRE:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_FIRE));
                                break;
                            case Enums.PotionType.HEALING:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_HEAL));
                                break;
                            case Enums.PotionType.ICE:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_ICE));
                                break;
                            case Enums.PotionType.INTELLIGENCE:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_INTEL));
                                break;
                            case Enums.PotionType.OFFENSIVE_KNOWLEDGE:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_ATTACK));
                                break;
                            case Enums.PotionType.PARALYSIS:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_PARALYSE));
                                break;
                            case Enums.PotionType.PERCEPTION:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_PERC));
                                break;
                            case Enums.PotionType.POISON:
                                _graphics.Add(SpriteManager.GetSprite(LocalSpriteName.POTION_POISON));
                                break;
                            default:
                                throw new NotImplementedException("Type of potion " + this.PotionType + " not implemented");
                        }
                    }
                }

                return _graphics;
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override string Name
        {
            get
            {
                if (!IsIdentified)
                {
                    return "Unknown Potion";
                }
                else
                {
                    return "Potion of " + this.PotionType;
                }
            }
            set
            {
                base.Name = value;
            }
        }

        public Potion(PotionType type)
        {
            this.PotionType = type;
            this.IsEquippable = false;
            this.IsEquipped = false;
            this.BaseValue = 200; //TODO PROPERLY LATER
            this.Category = InventoryCategory.POTION;
            this.Stackable = false;
            this.TotalAmount = 1;
        }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();
            //Base does proper handling of most actions, so let's retain them
            actions.AddRange(base.GetPossibleActions(actor));

            if (this.InInventory)
            {
                //Add drink
                actions.Add(ActionType.CONSUME);
                //TODO: LATER ADD THROW
            }

            return actions.ToArray();
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            //See if we can identify the item
            if (actor.Knowledge.CanIdentifyPotion(this.PotionType))
            {
                this.IsIdentified = true; //identify!
            }

            if (actionType == ActionType.CONSUME)
            {
                //Take the effects of the potion
                //Identify it too!
                this.IsIdentified = true;
                actor.Knowledge.LearnToIdentify(this.PotionType);

                //remove from inventory
                actor.Inventory.Inventory.Remove(this.Category, this);

                switch (this.PotionType)
                {
                    case Enums.PotionType.AGILITY:
                        {
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 2;
                            effect.MinutesLeft = 5;
                            effect.Name = EffectName.AGIL;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.Blue, "The effect wears off");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect }, new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You feel more agile") };
                        }
                    case Enums.PotionType.BLEEDING:
                        actor.Anatomy.BloodLoss = 2; //Start bleeding. Silly idiot
                        return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.DarkRed, "The potion scrapes your insides as you drink it. You start bleeding") };
                    case Enums.PotionType.BLINDING:
                        {
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 10;
                            effect.MinutesLeft = 3;
                            effect.Name = EffectName.BLIND;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.Blue, "You can see again");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect }, new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.DarkRed, "Your eyes cloud over.") };
                        }
                    case Enums.PotionType.BRAWN:
                        {
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 2;
                            effect.MinutesLeft = 5;
                            effect.Name = EffectName.BRAWN;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.Blue, "The effect wears off");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect }, new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You feel strong") };
                        }
                    case Enums.PotionType.CHARISMA:
                        { //AKA Alcohol :)
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 2;
                            effect.MinutesLeft = 5;
                            effect.Name = EffectName.CHAR;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.Blue, "The effect wears off");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect }, new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You feel more confident") };
                        }
                    case Enums.PotionType.DEFENCE:
                        return new ActionFeedback[0] { };
                        //TODO later
                    case Enums.PotionType.DEFENSIVE_KNOWLEDGE:
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                actor.Attributes.IncreaseSkill(SkillName.DODGER);
                            }
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You become more knowledgable about not getting hit") };
                        }
                    case Enums.PotionType.FEEDING:
                        {
                            //Fill em up
                            actor.FeedingLevel = FeedingLevel.STUFFED;
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You feel rather full") };
                        }
                    case Enums.PotionType.FIRE:
                        {
                            actor.Anatomy.Chest -= 2;
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.DarkRed, "The potion tastes like fire. That burns a bit...") };
                        }
                    case Enums.PotionType.HEALING:
                        {
                            //Be healed! This is not strictly speaking an effect, but that should be fine
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 5;
                            effect.MinutesLeft = 0;
                            effect.Name = EffectName.HEAL;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "Your wounds heal...");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect } , new LogFeedback(InterfaceSpriteName.POTION_ICON,Color.ForestGreen,"Your wounds close") };
                        }
                    case Enums.PotionType.ICE:
                        {
                            actor.Anatomy.Chest -= 2;
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.DarkRed, "The potion freezes your insides.") };
                        }
                    case Enums.PotionType.INTELLIGENCE:
                        {
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 2;
                            effect.MinutesLeft = 5;
                            effect.Name = EffectName.INTEL;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.Blue, "The effect wears off");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect }, new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You feel smarter") };
                        }
                    case Enums.PotionType.OFFENSIVE_KNOWLEDGE:
                        {
                            for (int i = 0; i < 20; i++)
                            {
                                actor.Attributes.IncreaseSkill(SkillName.FIGHTER);
                            }
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "You become more knowledgable about hitting things") };
                        }
                    case Enums.PotionType.PARALYSIS:
                        {
                            actor.Anatomy.StunAmount = 3;
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.DarkRed, "You can't move...") };
                        }
                    case Enums.PotionType.PERCEPTION:
                        {
                            Effect effect = new Effect();
                            effect.Actor = actor;
                            effect.EffectAmount = 2;
                            effect.MinutesLeft = 5;
                            effect.Name = EffectName.PERC;
                            effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.Blue, "The effect wears off");
                            return new ActionFeedback[2] { new ReceiveEffectFeedback() { Effect = effect }, new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.ForestGreen, "Your vision feels sharper") };
                        }
                    case Enums.PotionType.POISON:
                        {
                            actor.Anatomy.Chest -= 2;
                            return new ActionFeedback[1] { new LogFeedback(InterfaceSpriteName.POTION_ICON, Color.DarkRed, "This potion tastes horrible. You feel sick.") };
                        }
                    default:
                        throw new NotImplementedException("No code for " + this.PotionType);
                }

                return null;
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }
    }
}
