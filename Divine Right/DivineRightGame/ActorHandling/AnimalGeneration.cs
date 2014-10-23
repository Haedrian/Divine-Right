using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.Enums;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.Items.Archetypes.Local;

namespace DivineRightGame.ActorHandling
{
    public static class AnimalGeneration
    {

        /// <summary>
        /// Generates an animal actor, together with it's Map Character from it's parameters
        /// </summary>
        /// <returns></returns>
        public static Actor GenerateAnimal(AnimalData data)
        {
            //Load the race data
            data.RaceData = ActorGeneration.ReadRaceData(data.RaceID);

            Actor actor = new Actor();

            actor.Anatomy = ActorGeneration.GenerateAnatomy("human");
            actor.Attributes = new SkillsAndAttributes();
            actor.Attributes.Actor = actor;
            ActorGeneration.GenerateAttributes(data.RaceData.RaceName,ActorProfession.WARRIOR,GameState.Random.Next(data.MinLevel,data.MaxLevel),actor);

            actor.Attributes.Health = actor.Anatomy;
            actor.Attributes.HandToHand = GameState.Random.Next(data.MinLevel, data.MaxLevel);
            actor.Attributes.Evasion = GameState.Random.Next(data.MinLevel, data.MaxLevel);

            actor.EnemyData = new EnemyData();
            actor.EnemyData.EnemyLineOfSight = data.LineOfSight;
            actor.EnemyData.Intelligent = false;
            actor.EnemyData.Profession = ActorProfession.WARRIOR;
            actor.EnemyData.EnemyName = data.Name;

            actor.Gender = Gender.U;
            actor.Inventory = new ActorInventory();

            //TODO: GENERATE THE FOOD AND HIDE DROPS

            //Meat.
            if (data.MeatAmount > 0)
            {
                //Create some meat
                ConsumableItem meat = new ConsumableItem();
                meat.ArmourRating = 0;
                meat.BaseValue = 50;
                meat.Category = InventoryCategory.SUPPLY;
                meat.Description = "The meat of a " + data.Name;
                meat.EffectPower = 1;
                meat.Effects = ConsumableEffect.FEED;
                meat.Graphic = SpriteManager.GetSprite(LocalSpriteName.STEAK);
                meat.InternalName = "meat";
                meat.IsEquippable = false;
                meat.InInventory = true;
                meat.IsEquipped = false;
                meat.MayContainItems = true;
                meat.Name = data.Name + " meat";
                meat.Stackable = true;
                meat.TotalAmount = data.MeatAmount;

                actor.Inventory.Inventory.Add(meat.Category, meat);
            }

            //Hide
            if (data.HideAmount > 0)
            {
                //Create a hide
                InventoryItem hide = new InventoryItem();
                hide.ArmourRating = 0;
                hide.BaseValue = data.HideValue;
                hide.Category = InventoryCategory.LOOT;
                hide.Description = "The hide of a " + data.Name;
                hide.Graphic = SpriteManager.GetSprite(LocalSpriteName.HIDE);
                hide.InInventory = true;
                hide.InternalName = "hide";
                hide.IsEquippable = false;
                hide.IsEquipped = false;
                hide.MayContainItems = true;
                hide.Name = data.Name + " hide";
                hide.Stackable = true;
                hide.TotalAmount = data.HideAmount;

                actor.Inventory.Inventory.Add(hide.Category, hide);
            }

            actor.IsAggressive = data.IsAggressive;
            actor.IsAlive = true;
            actor.IsAnimal = true;
            actor.IsDomesticatedAnimal = data.Domesticated;
            actor.IsPlayerCharacter = false;
            actor.IsStunned = false;
            actor.LineOfSight = data.LineOfSight;
            actor.Name = data.Name;
            actor.UnarmedDamageDice = data.DamageDice;
            actor.UniqueId = Guid.NewGuid();

            LocalCharacter lc = new LocalCharacter();
            lc.Actor = actor;
            lc.Description = data.Name;

            //Pick a graphic
            string graphic = data.GraphicsList[GameState.Random.Next(data.GraphicsList.Length)];

            lc.Graphic = (SpriteManager.GetSprite((LocalSpriteName) Enum.Parse(typeof(LocalSpriteName),graphic)));

            lc.InternalName = data.Name;
            lc.IsStunned = false;
            lc.LineOfSightRange = data.LineOfSight;
            lc.MayContainItems = false;
            lc.Name = data.Name;

            actor.MapCharacter = lc;

            return actor;
        }

    }
}
