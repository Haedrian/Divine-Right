using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.EventHandling.MultiEvents;
using DRObjects.Graphics;
using DRObjects;
using DRObjects.ActorHandling.Enums;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.CharacterSheet;
using DivineRightGame.ItemFactory.ItemFactoryManagers;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Local;

namespace DivineRightGame.CharacterCreation
{
    /// <summary>
    /// Character Creation MultiDecision Component
    /// </summary>
    public static class CharacterCreation
    {
        /// <summary>
        /// Generates the particular Game Multi Event for character creation 
        /// </summary>
        /// <returns></returns>
        public static GameMultiEvent GenerateCharacterCreation()
        {
            var book = SpriteManager.GetSprite(InterfaceSpriteName.BOOK);

            GameMultiEvent gme = new GameMultiEvent();
            gme.EventName = "GENERATE_CHARACTER";
            gme.Image = book;
            gme.Title = "The Avatar is born";
            gme.Text = "And thus, world was done\nHumans forget their maker.\nAvatar will come";

            List<MultiEventChoice> choices = new List<MultiEventChoice>();
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "GEN",
                    Text = "Little is known about their early life",
                });
            choices.Add(new MultiEventChoice()
            {
                ChoiceName = "RANDOM",
                Text = "Nothing is known about their early life"
            });

            gme.Choices = choices.ToArray();

            GameMultiEvent next = new GameMultiEvent();

            gme.Choices[0].NextChoice = next;

            next.Image = book;
            next.Title = "The form of the Avatar";
            next.Text = "Avatar takes form\nAs a human has been born\nA perfect human";

            choices = new List<MultiEventChoice>();
            choices.Add(new MultiEventChoice()
            {
                ChoiceName = "MALE",
                Text = "The Male form was Chosen"
            });
            choices.Add(new MultiEventChoice()
            {
                ChoiceName = "FEMALE",
                Text = "The Female form was Chosen"
            }
            );

            next.Choices = choices.ToArray();

            next = new GameMultiEvent();

            foreach (var choice in choices)
            {
                choice.NextChoice = next;
            }

            next.Image = book;
            next.Title = "The Family of the Avatar";
            next.Text = "Common Family\nFew riches,little greatness\nCommon Beginnings";

            choices = new List<MultiEventChoice>();
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "MERCHANTS",
                    Text = "A family of Merchants"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "CLERICS",
                    Text = "A family of Clerics"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "EXPLORERS",
                    Text = "A family of Explorers"
                });
            next.Choices = choices.ToArray();

            next = new GameMultiEvent();

            foreach (var choice in choices)
            {
                choice.NextChoice = next;
            }

            next.Image = book;
            next.Title = "The Avatar's Nature";
            next.Text = "And Avatar grew.\nA special little human\nAnd brilliance was theirs";

            choices = new List<MultiEventChoice>();
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "INTEL",
                    Text = "As the Wisdom of Dragon's was theirs"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "AGIL",
                    Text = "As Grace and Speed was theirs"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "CHAR",
                    Text = "As a tongue of gold was theirs"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "BRAWN",
                    Text = "As the strenght of many men was theirs"
                });

            next.Choices = choices.ToArray();

            next = new GameMultiEvent();

            foreach (var choice in choices)
            {
                choice.NextChoice = next;
            }

            next.Image = book;
            next.Title = "The Avatar fights";
            next.Text = "And Avatar Grew\nIn militia was drafted\nExcelled amongst other men";

            choices = new List<MultiEventChoice>();
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "LEADER",
                    Text = "as the Leader of men"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "HEALING",
                    Text = "as their healer"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "PERSUATION",
                    Text = "as their persuader"
                });

            next.Choices = choices.ToArray();
            next = new GameMultiEvent();

            foreach (var choice in choices)
            {
                choice.NextChoice = next;
            }

            next.Image = book;
            next.Title = "And it begins";
            next.EventName = "GENERATE_CHARACTER";
            next.Text = "True god spoke to them\nAnd they left their parents' house\nWith blessing and gift";

            choices = new List<MultiEventChoice>();

            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "SWORD",
                    Text = "A sword to smite the nonbelievers"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "ARMOUR",
                    Text = "Armour to protect against the heretic"
                });
            choices.Add(new MultiEventChoice()
                {
                    ChoiceName = "MONEY",
                    Text = "Money to sway the hearts of lesser men"
                });

            next.Choices = choices.ToArray();

            //Give them the items everyone starts with
            InventoryItemManager iim = new InventoryItemManager();

            var food = iim.GetItemsWithAMaxValue("SUPPLY", 150);

            foreach (var foodItem in food)
            {
                GameState.PlayerCharacter.Inventory.Inventory.Add(InventoryCategory.SUPPLY, foodItem as InventoryItem);
                (foodItem as InventoryItem).InInventory = true;
            }
            return gme;
        }

        /// <summary>
        /// Processes the Parameters and updates the main character in the right manner
        /// </summary>
        /// <param name="mainCharacter"></param>
        /// <param name="choicesMade"></param>
        public static void ProcessParameters(List<string> choicesMade)
        {
            Actor mainCharacter = GameState.PlayerCharacter;

            foreach (var choice in choicesMade)
            {
                if (String.Equals(choice, "RANDOM", StringComparison.InvariantCultureIgnoreCase))
                {
                    ProcessParameters(GenerateRandom());
                    return; //Generate a random set instead
                }
                else if (String.Equals(choice, "GEN", StringComparison.InvariantCultureIgnoreCase))
                {
                    //do nothing
                }
                else if (String.Equals(choice, "MALE", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Gender = Gender.M;
                    mainCharacter.MapCharacter.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
                }
                else if (String.Equals(choice, "FEMALE", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Gender = Gender.F;
                    mainCharacter.MapCharacter.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_FEMALE);
                }
                else if (String.Equals(choice, "MERCHANTS", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.HAGGLER, new ActorSkill(SkillName.HAGGLER) { SkillLevel = 5 });
                }
                else if (String.Equals(choice, "CLERICS", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.RITUALIST, new ActorSkill(SkillName.RITUALIST) { SkillLevel = 5 });
                }
                else if (String.Equals(choice, "EXPLORERS", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.EXPLORER, new ActorSkill(SkillName.EXPLORER) { SkillLevel = 5 });
                }
                else if (String.Equals(choice, "INTEL", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Intel = mainCharacter.Attributes.Intel + 2;
                }
                else if (String.Equals(choice, "AGIL", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Agil = mainCharacter.Attributes.Agil + 2;
                }
                else if (String.Equals(choice, "BRAWN", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Brawn = mainCharacter.Attributes.Brawn + 2;
                }
                else if (String.Equals(choice, "CHAR", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Char = mainCharacter.Attributes.Char + 2;
                }
                else if (String.Equals(choice, "LEADER", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.LEADER, new ActorSkill(SkillName.LEADER) { SkillLevel = 5 });
                }
                else if (String.Equals(choice, "HEALING", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.HEALER, new ActorSkill(SkillName.HEALER) { SkillLevel = 5 });
                }
                else if (String.Equals(choice, "PERSUATION", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.PERSUADER, new ActorSkill(SkillName.PERSUADER) { SkillLevel = 5 });
                }

                else if (String.Equals(choice, "SWORD", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.FIGHTER, new ActorSkill(SkillName.FIGHTER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.DODGER, new ActorSkill(SkillName.DODGER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.BLOCKER, new ActorSkill(SkillName.BLOCKER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.SWORDFIGHTER, new ActorSkill(SkillName.SWORDFIGHTER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.ARMOUR_USER, new ActorSkill(SkillName.ARMOUR_USER) { SkillLevel = 5 });

                    //Give him a nice sword
                    InventoryItemManager iim = new InventoryItemManager();

                    var sword = iim.GetBestCanAfford("Sword", 500);

                    mainCharacter.Inventory.Inventory.Add(sword.Category, sword);
                    sword.InInventory = true;
                }
                else if (String.Equals(choice, "ARMOUR", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.FIGHTER, new ActorSkill(SkillName.FIGHTER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.DODGER, new ActorSkill(SkillName.DODGER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.BLOCKER, new ActorSkill(SkillName.BLOCKER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.ARMOUR_USER, new ActorSkill(SkillName.ARMOUR_USER) { SkillLevel = 7 });

                    //Give him nice chest armour
                    InventoryItemManager iim = new InventoryItemManager();

                    var armour = iim.GetBestCanAfford("Body Armour", 500);

                    mainCharacter.Inventory.Inventory.Add(armour.Category, armour);
                    armour.InInventory = true;
                }
                else if (String.Equals(choice, "MONEY", StringComparison.InvariantCultureIgnoreCase))
                {
                    mainCharacter.Attributes.Skills.Add(SkillName.FIGHTER, new ActorSkill(SkillName.FIGHTER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.DODGER, new ActorSkill(SkillName.DODGER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.BLOCKER, new ActorSkill(SkillName.BLOCKER) { SkillLevel = 5 });
                    mainCharacter.Attributes.Skills.Add(SkillName.ARMOUR_USER, new ActorSkill(SkillName.ARMOUR_USER) { SkillLevel = 5 });

                    //Give him moneh!
                    mainCharacter.Inventory.TotalMoney += 700;
                }
                else
                {
                    throw new NotImplementedException("No code for choice " + choice);
                }
            }
        }

        /// <summary>
        /// Creates a number of random choices 
        /// </summary>
        public static List<string> GenerateRandom()
        {
            GameMultiEvent mic = GenerateCharacterCreation();

            List<string> randomChoices = new List<string>();

            //Pick the first one
            GameMultiEvent curr = mic.Choices[0].NextChoice;

            while (curr != null)
            {
                var choice = curr.Choices[GameState.Random.Next(curr.Choices.Length)];

                randomChoices.Add(choice.ChoiceName);

                curr = choice.NextChoice;
            }

            return randomChoices;

        }

    }
}
