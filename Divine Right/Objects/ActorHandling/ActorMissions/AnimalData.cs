using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    /// <summary>
    /// Represents some details about an animal
    /// </summary>
    public class AnimalData
    {
        public int AnimalID { get; set; }
        public string Name { get; set; }
        public string Tags { get; set; }
        public int MinLevel { get; set; }
        public int MaxLevel { get; set; }
        public int DamageDice { get; set; }

        public int MeatAmount { get; set; }
        public int HideAmount { get; set; }
        public int HideValue { get; set; }

        public int LineOfSight { get; set; }
        public string Biomes { private get; set; }

        private string[] _biomes;

        public string[] BiomeList
        {
            get
            {
                if (_biomes == null)
                {
                    //Generate it
                    _biomes = Biomes.Split(',');
                }

                return _biomes;
            }
        }

        public string Graphics { private get; set; }

        private string[] _graphics;

        public string[] GraphicsList
        {
            get
            {
                if (_graphics == null)
                {
                    //Generate it
                    _graphics = Graphics.Split(',');
                }

                return _graphics;
            }
        }

        public int PackSizeMin { get; set; }
        public int PackSizeMax { get; set; }

        public bool IsAggressive { get; set; }

        public bool Domesticated { get; set; }

        public int RaceID { get; set; }

        public RaceData RaceData { get; set; }

        /// <summary>
        /// Creates a new AnimalData from a list of strings as would be obtained from the database
        /// </summary>
        /// <param name="?"></param>
        public AnimalData(string[] data)
        {
            this.AnimalID = Int32.Parse(data[0]);
            this.Name = data[1];
            this.Tags = data[2];
            this.MinLevel = Int32.Parse(data[3]);
            this.MaxLevel = Int32.Parse(data[4]);
            this.DamageDice = Int32.Parse(data[5]);
            this.MeatAmount = Int32.Parse(data[6]);
            this.HideAmount = Int32.Parse(data[7]);
            this.HideValue = Int32.Parse(data[8]);
            this.LineOfSight = Int32.Parse(data[9]);
            this.Biomes = data[10];
            this.Graphics = data[11];
            this.PackSizeMin = Int32.Parse(data[12]);
            this.PackSizeMax = Int32.Parse(data[13]);
            this.IsAggressive = Boolean.Parse(data[14]);
            this.Domesticated = Boolean.Parse(data[16]);
            this.RaceID = Int32.Parse(data[17]);
        }
    }
}
