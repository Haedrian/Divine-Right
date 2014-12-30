using System;
namespace DRObjects.ActorHandling.CharacterSheet
{
    public interface IAnatomy
    {
        int BloodLoss { get; set; }
        int BloodTotal { get; set; }
        int BodyTimer { get; set; }
        int Chest { get; set; }
        int ChestMax { get; set; }
        int Head { get; set; }
        int HeadMax { get; set; }
        int LeftArm { get; set; }
        int LeftArmMax { get; set; }
        int Legs { get; set; }
        int LegsMax { get; set; }
        int RightArm { get; set; }
        int RightArmMax { get; set; }
        int StunAmount { get; set; }
    }
}
