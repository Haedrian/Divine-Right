using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Database;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Local;
using DivineRightGame.ItemFactory.ItemFactoryManagers;
using DRObjects.ActorHandling.Enums;
using DRObjects;
using DRObjects.Graphics;

namespace DivineRightGame.ActorHandling
{
    /// <summary>
    /// Class for generating actor attributes
    /// </summary>
    public static class ActorGeneration
    {
        private static Random random = new Random();
        private static List<RaceData> database = null;

        /// <summary>
        /// Gets enemy data for a particular enemy with a particular ID
        /// Will also calculate any random effects as required, so this will not be deterministic if you run it more than once.
        /// </summary>
        /// <param name="enemyID"></param>
        /// <returns></returns>
        public static EnemyData GetEnemyData(int enemyID)
        {
            List<string> properties = DatabaseHandling.GetItemProperties(Archetype.ENEMIES, enemyID);

            if (properties == null || properties.Count == 0)
            {
                throw new Exception("Enemy with ID " + enemyID + " not found");
            }

            //Hard coded sadly
            return new EnemyData()
            {
                EnemyID = enemyID,
                EnemyLineOfSight = Int32.Parse(properties[6]),
                EnemyName = properties[1],
                EnemyType = properties[4],
                Intelligent = Boolean.Parse(properties[7]),
                Profession = Boolean.Parse(properties[8]) ? ActorProfession.WARRIOR: ActorProfession.CIVILIAN //Keep it simple for now
            };

        }

        /// <summary>
        /// Picks a random enemy type which is either intelligent or unintelligent
        /// </summary>
        /// <param name="intelligent"></param>
        /// <returns></returns>
        public static string GetEnemyType(bool intelligent)
        {
            var dictionary = DatabaseHandling.GetDatabase(Archetype.ENEMIES);

            //Get all unique enemy types which are either intelligent or unintelligent
            var possibleMatches = dictionary.Values.Where(v => Boolean.Parse(v[7]).Equals(intelligent)).Select(v => v[4]).Distinct();

            //How many are there? pick one at random

            return possibleMatches.ToArray()[random.Next(possibleMatches.Count() - 1)];
        }

        /// <summary>
        /// Creates an Enemy of a paticular type (optional), having a particular tag (optional) and having a particular intelligence (optional).
        /// Will return the enemy ID so that you can get the actual item from the crea
        /// </summary>
        /// <param name="enemyType"></param>
        /// <param name="enemyTag"></param>
        /// <param name="intelligent"></param>
        /// <param name="level">The skill level of this enemy</param>
        /// <param name="gearCost">The total cost of this unit's equipped items</param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static DRObjects.Actor CreateActor(string enemyType, string enemyTag, bool? intelligent, int level, int gearCost, Gender? gender, out int enemyID, ActorProfession? profession = null)
        {
            //Get all the data from the database and we'll make our own filtering
            var dictionary = DatabaseHandling.GetDatabase(Archetype.ENEMIES);

            var possibleMatches = dictionary.Values.AsQueryable();

            if (!String.IsNullOrEmpty(enemyType))
            {
                possibleMatches = possibleMatches.Where(v => v[4].Equals(enemyType));
            }

            if (!String.IsNullOrEmpty(enemyTag))
            {
                possibleMatches = possibleMatches.Where(v => v[5].Equals(enemyTag));
            }

            if (intelligent.HasValue)
            {
                possibleMatches = possibleMatches.Where(v => Boolean.Parse(v[7]).Equals(intelligent.Value));
            }

            if (gender.HasValue)
            {
                possibleMatches = possibleMatches.Where(v =>(v[12]).Equals(gender.Value.ToString()));
            }

            //Put the possible matches and pick one at random
            if (possibleMatches.Count() == 0)
            {
                enemyID = -1;
                return null; //No match
            }

            //Pick one at random
            var selected = possibleMatches.ToArray()[random.Next(possibleMatches.Count())];

            int aggressivity = Int32.Parse(selected[10]);

            //Create the details
            enemyID = Int32.Parse(selected[0]);
            EnemyData data = GetEnemyData(enemyID);

            DRObjects.Actor actor = new DRObjects.Actor();
            actor.EnemyData = data;
            actor.IsPlayerCharacter = false;
            actor.LineOfSight = data.EnemyLineOfSight;
            actor.UniqueId = Guid.NewGuid();
            actor.IsAggressive = aggressivity > 0;
            actor.Gender = (Gender) Enum.Parse(typeof(Gender), selected[12]);

            actor.Name = ActorNameGenerator.GenerateName(enemyType, actor.Gender);

            //Give him attributes 
            actor.Attributes = GenerateAttributes(data.EnemyType, data.Profession, level,actor);

            //Set his anatomy too
            actor.Anatomy = GenerateAnatomy(data.EnemyType);

            //link one to another
            actor.Attributes.Health = actor.Anatomy;

            //Create the equipped item inventory
            actor.Inventory.EquippedItems = GenerateEquippedItems(gearCost);

            //And create the inventory
            foreach (var item in actor.Inventory.EquippedItems.Values)
            {
                actor.Inventory.Inventory.Add(item.Category, item);
            }

            return actor;
        }

        public static SkillsAndAttributes GenerateAttributes(string race, ActorProfession profession, int level,Actor actor)
        {
            //Roll 6 3d3s and put the results in an array
            int[] results = new int[6];

            //We'll throw away the smallest one

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = random.Next(3) + 1 + random.Next(3) + 1 + random.Next(3) + 1; 
            }

            //sort the array by size so top one goes first
            results = results.OrderByDescending(r => r).ToArray();

            //Create a new ActorAttribute
            SkillsAndAttributes att = new SkillsAndAttributes();

            if (profession == ActorProfession.WARRIOR)
            {
                //Prefer Brawn, Agil, Perc, Intel then Dex
                att.Brawn = results[0];
                att.Agil = results[1];
                att.Perc = results[2];
                att.Intel = results[3];
                att.Char = results[4];
                
                //Combat skills - give him evasion and attack in an amount equal to level
                att.Evasion = level;
                att.HandToHand = level;

            }
            else if (profession == ActorProfession.CIVILIAN)
            {
                //Prefer things randomly
                results = results.OrderByDescending(r => random.Next(10)).ToArray();

                att.Brawn = results[0];
                att.Agil = results[1];
                att.Perc = results[2];
                att.Intel = results[3];
                att.Char = results[4];

                //No combat skills
            }
            else if (profession == ActorProfession.CRAFTER)
            {
                //Prefer Dex, Intel, Perc, Brawn, Agil
                att.Char = results[0];
                att.Intel = results[1];
                att.Perc = results[2];
                att.Brawn = results[3];
                att.Agil = results[4];

                //No combat skills
            }
            else if (profession == ActorProfession.HARVESTER)
            {
                //Prefer Brawn, Perc, Dex, Intel, Agil
                att.Brawn = results[0];
                att.Perc = results[1];
                att.Char = results[2];
                att.Intel = results[3];
                att.Agil = results[4];

                //Give some combat skills, a third of the level
                att.HandToHand = level / 3;
                att.Evasion = level / 3;
            }
            else
            {
                throw new NotImplementedException("No code for profession " + profession);
            }

            //Add the racial bonuses
            RaceData data = ReadRaceData(race);

            att.Agil = att.BaseAgil + data.AgilModifier;
            att.Brawn = att.BaseBrawn + data.BrawnModifier;
            att.Char = att.BaseChar + data.DexModifier;
            att.Intel = att.BaseIntel + data.IntelModifier;
            att.Perc = att.BasePerc + data.PercModifier;

            //Make sure all attributes are larger than 0
            att.Agil = att.BaseAgil > 0 ? att.BaseAgil : 0;
            att.Brawn = att.BaseBrawn > 0 ? att.BaseBrawn : 0;
            att.Char = att.BaseChar > 0 ? att.BaseChar : 0;
            att.Intel = att.BaseIntel > 0 ? att.BaseIntel : 0;
            att.Perc = att.BasePerc > 0 ? att.BasePerc : 0;

            att.Actor = actor;
            att.Health = actor.Anatomy;

            return att;
        }

        /// <summary>
        /// Generates a set of equipped items, not exceeding the total cost. For now will only work on warriors
        /// </summary>
        /// <returns></returns>
        public static Dictionary<EquipmentLocation,InventoryItem> GenerateEquippedItems(int totalCost)
        {
            Dictionary<EquipmentLocation, InventoryItem> equipped = new Dictionary<EquipmentLocation, InventoryItem>();

            //Decide what percentage to spend on the weapon, the rest will be armour
            int weaponPercentage = 3 + GameState.Random.Next(8);

            InventoryItemManager mgr = new InventoryItemManager();

            int moneyLeft = totalCost;

            int moneyForWeapon = moneyLeft * weaponPercentage / 10;

            //Buy the weapon
            var weapon = mgr.GetBestCanAfford("WEAPON", moneyForWeapon);

            if (weapon != null)
            {
                //Equip it
                equipped.Add(EquipmentLocation.WEAPON, weapon);
                //And reduce the value
                moneyLeft -= weapon.BaseValue;
            }

            //Now let's buy the rest of the items. For now let's divide everything equally and try to get everyything at least
            int moneyForEachPiece = moneyLeft / 4;

            //Try to buy an armour piece for each part
            //Shield
            var shield = mgr.GetBestCanAfford("SHIELD", moneyForEachPiece);

            if (shield != null)
            {
                //Equip it
                equipped.Add(EquipmentLocation.SHIELD, shield);
                //And reduce the value
                moneyLeft -= shield.BaseValue;
            }

            //Helm
            var helm = mgr.GetBestCanAfford("HELM", moneyForEachPiece);

            if (helm != null)
            {
                equipped.Add(EquipmentLocation.HEAD, helm);

                moneyLeft -= helm.BaseValue;
            }

            var bodyArmour = mgr.GetBestCanAfford("BODY ARMOUR", moneyForEachPiece);

            if (bodyArmour != null)
            {
                equipped.Add(EquipmentLocation.BODY, bodyArmour);
                moneyLeft -= bodyArmour.BaseValue;
            }

            var legs = mgr.GetBestCanAfford("LEGS", moneyForEachPiece);

            if (legs != null)
            {
                equipped.Add(EquipmentLocation.LEGS, legs);
                moneyLeft -= legs.BaseValue;
            }

            //Now that we presumably have one of each, let's buy something better

            //To do this, let's start with a divisor of 3.
            //We see if we can buy three better items.
            //Then we look at what we can do with the rest divided by 2
            //Then we look at what we can do with the rest divided by 1
            //And if that doesn't work, we 'lose' the rest

            for (int divisor = 3; divisor > 0; divisor--)
            {
                int moneyToSpend = moneyLeft / divisor;

                for (int i = 0; i < divisor; i++)
                {
                    var item = mgr.GetBestCanAfford("Armour", moneyToSpend);

                    if (item == null)
                    {
                        continue;
                    }

                    InventoryItem previousItem = null;

                    //Do we have something else already?
                    if (equipped.ContainsKey(item.EquippableLocation.Value))
                    {
                        previousItem = equipped[item.EquippableLocation.Value];

                        //Does one cost more than the other ?
                        if (item.BaseValue >= previousItem.BaseValue)
                        {
                            //Swap them
                            equipped.Remove(item.EquippableLocation.Value);
                            equipped.Add(item.EquippableLocation.Value, item);

                            //And fix the total amount of money
                            moneyLeft += previousItem.BaseValue;
                            moneyLeft -= item.BaseValue;

                            moneyToSpend = moneyLeft / divisor;
                        }
                    }
                    else
                    {
                        equipped.Add(item.EquippableLocation.Value, item);
                        moneyLeft -= item.BaseValue;

                        moneyToSpend = moneyLeft / divisor;
                    }

                }
            }

            //Do we still have any cash?
            if (moneyLeft > 0)
            {
                //Pick up some form of jewelry
                var item = mgr.GetBestCanAfford("ring", moneyLeft);

                if (item != null)
                {
                    equipped.Add(EquipmentLocation.RING1, item);
                }

            }

            //Equip them
            foreach (var value in equipped.Values)
            {
                value.IsEquipped = true;
                value.InInventory = true;
            }

            return equipped;

        }

        public static HumanoidAnatomy GenerateAnatomy(string race)
        {
            //This is a dummmy for now. Just generate the standard humanoid anatomy
            HumanoidAnatomy anatomy = new HumanoidAnatomy()
            {
                    Head = 4,
                    HeadMax = 4,
                    LeftArm = 6,
                    LeftArmMax = 6,
                    RightArm = 6,
                    RightArmMax = 6,
                    Chest = 12,
                    ChestMax = 12,
                    Legs = 8,
                    LegsMax = 8,

                    BloodLoss = 0, 
                    BloodTotal = HumanoidAnatomy.BLOODTOTAL,
                    BodyTimer = 0, 
                    StunAmount = 0
            };

            return anatomy;

        }

        /// <summary>
        /// Reads the race data for a particular race
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        private static RaceData ReadRaceData(string race)
        {
            if (database == null)
            {
                database = new List<RaceData>();

                //Get the entirety of the race database
                var raceDB = DatabaseHandling.GetDatabase(Archetype.RACE);

                //Parse it and shove it in the array
                foreach (List<string> dbRace in raceDB.Values)
                {
                    RaceData datum = new RaceData()
                    {
                        RaceName = dbRace[1],
                        IsIntelligent = bool.Parse(dbRace[2]),
                        BrawnModifier = Int32.Parse(dbRace[3]),
                        AgilModifier = Int32.Parse(dbRace[4]),
                        DexModifier = Int32.Parse(dbRace[5]),
                        PercModifier = Int32.Parse(dbRace[6]),
                        IntelModifier = Int32.Parse(dbRace[7])
                    };

                    database.Add(datum);
                }
            }

            return database.Where(d => d.RaceName.Equals(race, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Regenerates the stats of an orc such that it retains any missions it has, however will be of a particular level with a particular gear cost
        /// Will also pick the correct orc icon depending on level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="gearCost"></param>
        /// <param name="enemyID"></param>
        /// <returns></returns>
        public static void RegenerateOrc(Actor actor,int level, int gearCost)
        {
            Random random = new Random();

            //This should give us a random from 0.75 to 1.25
            double multiplier = (random.NextDouble()/2)-0.25 + 1;

            //Start by regenerating the actor's stats and gear
            actor.Attributes = GenerateAttributes("orc", level == 5 ? ActorProfession.CIVILIAN : ActorProfession.WARRIOR, (int)(level * multiplier), actor);

            multiplier = (random.NextDouble()/2)-0.25 + 1;

            //And the gear
            actor.Inventory.EquippedItems = GenerateEquippedItems((int)(gearCost * multiplier));

            foreach (var item in actor.Inventory.EquippedItems.Values)
            {
                actor.Inventory.Inventory.Add(item.Category, item);
            }

            //And finally, we need to pick the right graphic
            string graphic = String.Empty;

            if (level <= 5)
            {
                actor.MapCharacter.Graphic = SpriteManager.GetSprite(LocalSpriteName.ENEMY_ORC_CIV);
            }
            else if (level <= 10)
            {
                actor.MapCharacter.Graphic = SpriteManager.GetSprite(LocalSpriteName.ENEMY_ORC_LIGHT);
            }
            else
            {
                actor.MapCharacter.Graphic = SpriteManager.GetSprite(LocalSpriteName.ENEMY_ORC_HEAVY);
            }

            //Done
        }

    }
}
