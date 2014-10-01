using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;

namespace DRObjects.ActorHandling
{
    [Serializable]
    /// <summary>
    /// The attributes and skills of the particular actor
    /// </summary>
    public class SkillsAndAttributes
    {
        #region Members

        private int brawn;
        private int charisma;
        private int agil;
        private int perc;
        private int intel;

        /// <summary>
        /// For enemies, use this as hand to hand damage
        /// </summary>
        private int enemyHandToHand;
        /// <summary>
        /// For enemies, use this as evasion
        /// </summary>
        private int enemyEvasion;

        #region Links
        public HumanoidAnatomy Health { get; set; }

        public Actor Actor { get; set; }

        public SkillsAndAttributes()
        {
            this.TempAgil = 0;
            this.TempBrawn = 0;
            this.TempChar = 0;
            this.TempIntel = 0;
            this.TempPerc = 0;
        }
        #endregion

        #endregion


        #region Properties

        #region Character Attributes

        public Dictionary<SkillName, ActorSkill> Skills = new Dictionary<SkillName, ActorSkill>();

        public int BaseBrawn { get { return this.brawn; } }
        public int BaseChar { get { return this.charisma; } }
        public int BaseAgil { get { return this.agil; } }
        public int BasePerc { get { return this.perc; } }
        public int BaseIntel { get { return this.intel; } }

        public int Brawn { get { int total = (TempBrawn ?? 0) + (int)((double)brawn * Health.RightArm / Health.RightArmMax); return total > 1 ? total : 1; } set { brawn = value; } }
        public int Char { get { int total = (TempChar ?? 0) + (int)((double)charisma * Health.RightArm / Health.RightArmMax); return total > 1 ? total : 1; } set { charisma = value; } }
        public int Agil { get { int total = (TempAgil ?? 0) + (int)((double)agil * Health.Legs / Health.LegsMax); return total > 1 ? total : 1; } set { agil = value; } }
        public int Perc { get { int total = perc + (TempPerc ?? 0); return total > 1 ? total : 1; } set { perc = value; } }
        public int Intel { get { int total = intel + (TempIntel ?? 0); return total > 1 ? total : 1; } set { intel = value; } }

        public int? TempBrawn { get; set; }
        public int? TempChar { get; set; }
        public int? TempAgil { get; set; }
        public int? TempPerc { get; set; }
        public int? TempIntel { get; set; }

        public int HandToHand
        {
            get
            {
                int totalH2H = 0;

                if (Actor.IsPlayerCharacter)
                {
                    if (this.Skills.ContainsKey(SkillName.FIGHTER))
                    {
                        totalH2H += (int)(this.Skills[SkillName.FIGHTER].SkillLevel);
                    }

                    //What weapon are we using?

                    if (Actor.Inventory.EquippedItems.ContainsKey(EquipmentLocation.WEAPON))
                    {
                        var weapon = Actor.Inventory.EquippedItems[EquipmentLocation.WEAPON];

                        if (weapon.WeaponType == "SWORD")
                        {
                            //Use sword damage
                            if (this.Skills.ContainsKey(SkillName.SWORDFIGHTER))
                            {
                                totalH2H += (int)this.Skills[SkillName.SWORDFIGHTER].SkillLevel;
                            }
                        }
                        else if (weapon.WeaponType == "AXE")
                        {
                            //Use axe damage
                            if (this.Skills.ContainsKey(SkillName.AXEFIGHTER))
                            {
                                totalH2H += (int)this.Skills[SkillName.AXEFIGHTER].SkillLevel;
                            }
                        }
                        else
                        {
                            throw new NotImplementedException("No code for weapon with damage type " + weapon.WeaponType);
                        }
                    }
                    else
                    {
                        //Hand to hand
                        //Use wrestling
                        if (this.Skills.ContainsKey(SkillName.BRAWLER))
                        {
                            totalH2H += (int)this.Skills[SkillName.BRAWLER].SkillLevel;
                        }
                    }

                    //Take the average
                    totalH2H /= 2;

                }
                else
                {
                    totalH2H = enemyHandToHand;
                }

                return totalH2H;
            }
            set
            {
                if (this.Actor == null || !this.Actor.IsPlayerCharacter)
                {
                    this.enemyHandToHand = value;
                }
                //otherwise do nothing
            }
        }
        public int Evasion
        {
            get
            {
                if (this.Actor.IsPlayerCharacter)
                {
                    //If it's the player character, use his dodge skill
                    int totalSkill = 0;
                    int divAmount = 1;

                    if (this.Skills.ContainsKey(SkillName.DODGER))
                    {
                        totalSkill += (int)this.Skills[SkillName.DODGER].SkillLevel;
                    }

                    //Is he wearing armour ?
                    if (this.Actor.Inventory.EquippedItems.ContainsKey(EquipmentLocation.BODY))
                    {
                        //Add the armour skill
                        totalSkill += (int)this.Skills[SkillName.ARMOUR_USER].SkillLevel;
                        divAmount++;
                    }

                    //Is he using a shield ?
                    if (this.Actor.Inventory.EquippedItems.ContainsKey(EquipmentLocation.SHIELD))
                    {
                        //Add the armour skill
                        totalSkill += (int)this.Skills[SkillName.BLOCKER].SkillLevel;
                        divAmount++;
                    }

                    return totalSkill / divAmount;
                }
                else
                {
                    return enemyHandToHand;
                }
            }
            set
            {
                if (this.Actor == null || !this.Actor.IsPlayerCharacter)
                {
                    this.enemyEvasion = value;
                }
            }
        }

        #endregion

        public int Dodge
        {
            get
            {
                return Evasion + Agil;
            }
        }

        public int WoundResist
        {
            get
            {
                return Brawn - 5;
            }
        }

        #endregion

        #region Skills

        /// <summary>
        /// Increases skill in a particular skill, or creates a new one if there is no such skill
        /// </summary>
        /// <param name="name"></param>
        public void IncreaseSkill(SkillName name)
        {
            if (this.Skills.ContainsKey(name))
            {
                this.Skills[name].LearnSkill();
            }
            else
            {
                this.Skills.Add(name, new ActorSkill(name));
            }
        }

        #endregion
    }
}
