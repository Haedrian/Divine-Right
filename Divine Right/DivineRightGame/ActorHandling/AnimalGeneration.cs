using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.Enums;
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

            actor.EnemyData = new EnemyData();
            actor.EnemyData.EnemyLineOfSight = data.LineOfSight;
            actor.EnemyData.Intelligent = false;
            actor.EnemyData.Profession = ActorProfession.WARRIOR;

            actor.Gender = Gender.U;
            actor.Inventory = new ActorInventory();

            //TODO: GENERATE THE FOOD AND HIDE DROPS

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
