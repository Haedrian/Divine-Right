using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.EventHandling.MultiEvents;
using DRObjects.Graphics;

namespace DRObjects.MultiEvents
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
                    ChoiceName = "Gen",
                    Text = "Little is known about their early life",
                });
            choices.Add(new MultiEventChoice()
            {
                ChoiceName = "Random",
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
                    Text = "as their Leader of men"
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

            return gme;
        }
    }
}
