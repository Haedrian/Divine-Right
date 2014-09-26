using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using System.Globalization;

namespace DRObjects.ActorHandling.CharacterSheet
{
    [Serializable]
    public class ActorSkill
    {
        //These repersents how many LearnSkills are required in order to level up.
        //They are divided in batches of five.
        private const int FIRST_FIVE_COST = 10;
        private const int SECOND_FIVE_COST = 20;
        private const int THIRD_FIVE_COST = 40;
        private const int FOURTH_FIVE_COST = 80;

        private static readonly TextInfo myTI = new CultureInfo("en-gb", false).TextInfo;

        /// <summary>
        /// The levels to display
        /// </summary>
        public static readonly string[] DisplayLevels = new string[20] 
        {
            "Terrible","Pathetic","Pitiful","Incompetant","Dabbling",
            "Amateur","Practicing","Below Average","Average","Journeyman",
            "Competant","Practiced","Professional","Gifted","Expert",
            "Renowned","Master","Awesome","Legendary","Epic"
        };

        /// <summary>
        /// The name of the skill
        /// </summary>
        public SkillName SkillName { get; set; }

        /// <summary>
        /// The level of the skill
        /// </summary>
        public double SkillLevel { get; set; }

        /// <summary>
        /// A displayable representation of the skill name
        /// </summary>
        public string SkillNameString
        {
            get
            {
              return   myTI.ToTitleCase(this.SkillName.ToString().Replace("_", " ").ToLower());
            }
        }

        /// <summary>
        /// A textual representation of the skill level
        /// </summary>
       public string SkillLevelString
        {
           get
            {
               return DisplayLevels[(int)Math.Floor(SkillLevel)];
            }
        }

        public ActorSkill(SkillName name)
        {
            this.SkillName = name;
            this.SkillLevel = 0;
        }

        /// <summary>
        /// Adds a single bit of knowledge to the skill.
        /// </summary>
        /// <returns></returns>
        public void LearnSkill()
        {
            if (this.SkillLevel < 5)
            {
                this.SkillLevel += 1 / FIRST_FIVE_COST;
            }
            else if (this.SkillLevel < 10)
            {
                this.SkillLevel += 1 / SECOND_FIVE_COST;
            }
            else if (this.SkillLevel < 15)
            {
                this.SkillLevel += 1 / THIRD_FIVE_COST;
            }
            else
            {
                this.SkillLevel += 1 / FOURTH_FIVE_COST;
            }

            if (this.SkillLevel > 19)
            {
                this.SkillLevel += 19; //too big
            }

        }
    }
}
