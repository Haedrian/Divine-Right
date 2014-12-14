using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;

namespace DRObjects.Items.Tiles.Global
{
    [Serializable]
    /// <summary>
    /// This is a special kind of map item tile which can be used for all global tiles.
    /// </summary>
    public class GlobalTile :
        MapItem
    {
        #region Constants

        private SpriteData BIGTREE = SpriteManager.GetSprite(GlobalSpriteName.BIGTREE);
        private SpriteData DEADTREE = SpriteManager.GetSprite(GlobalSpriteName.DEADTREE);
        private SpriteData DESERTTILE = SpriteManager.GetSprite(LocalSpriteName.SAND);
        private SpriteData FORESTTILE = SpriteManager.GetSprite(GlobalSpriteName.FORESTTILE);
        private SpriteData GARIGUETILE = SpriteManager.GetSprite(LocalSpriteName.GARRIGUE);
        private SpriteData GRASSTILE = SpriteManager.GetSprite(LocalSpriteName.GRASS_TILE);
        private SpriteData HILLSLOPE = SpriteManager.GetSprite(GlobalSpriteName.HILLSLOPE);
        private SpriteData MOUNTAIN = SpriteManager.GetSprite(GlobalSpriteName.MOUNTAIN);
        private SpriteData RIVER = SpriteManager.GetSprite(GlobalSpriteName.RIVER);
        private SpriteData SNOWTILE = SpriteManager.GetSprite(LocalSpriteName.SNOW);
        private SpriteData SWAMPTILE = SpriteManager.GetSprite(LocalSpriteName.SWAMP);
        private SpriteData TREE = SpriteManager.GetSprite(GlobalSpriteName.TREE);
        private SpriteData TROPICALTREE = SpriteManager.GetSprite(GlobalSpriteName.TROPICALTREE);
        private SpriteData WATERTILE = SpriteManager.GetSprite(GlobalSpriteName.WATERTILE);
        private SpriteData TROPICALTILE = SpriteManager.GetSprite(LocalSpriteName.JUNGLE_TILE);

        private SpriteData BROWN = SpriteManager.GetSprite(ColourSpriteName.BROWN);
        private SpriteData GREEN = SpriteManager.GetSprite(ColourSpriteName.GREEN);
        private SpriteData INDIGO = SpriteManager.GetSprite(ColourSpriteName.INDIGO);
        private SpriteData MARBLEBLUE = SpriteManager.GetSprite(ColourSpriteName.MARBLEBLUE);
        private SpriteData ORANGE = SpriteManager.GetSprite(ColourSpriteName.ORANGE);
        private SpriteData PINK = SpriteManager.GetSprite(ColourSpriteName.PINK);
        private SpriteData PURPLE = SpriteManager.GetSprite(ColourSpriteName.PURPLE);
        private SpriteData RED = SpriteManager.GetSprite(ColourSpriteName.RED);
        private SpriteData WHITE = SpriteManager.GetSprite(ColourSpriteName.WHITE);
        private SpriteData YELLOW = SpriteManager.GetSprite(ColourSpriteName.YELLOW);

        #endregion

        #region members

        /// <summary>
        /// This represents the elevation of the tile, from -500 to 500.
        /// Anything less than 0, is underwater
        /// Anything less than 80 is Plain
        /// 250 or less is hilly
        /// anything above 250 is mountainous
        /// </summary>
        public int Elevation { get; set; }
        /// <summary>
        /// This represents the region this tile belongs in. Currently this is used by the world generation code only.
        /// </summary>
        public int Region { get; set; }

        /// <summary>
        /// Does it have a river?
        /// </summary>
        public bool HasRiver { get; set; }

        /// <summary>
        /// Do we draw a contour on this tile?
        /// </summary>
        public bool HasHillSlope { get; set; }

        /// <summary>
        /// Temperature of the tile in Celcius
        /// </summary>
        public decimal ClimateTemperature { get; set; }

        /// <summary>
        /// The amount of rainfall this tile receives
        /// </summary>
        public decimal Rainfall { get; set; }

        /// <summary>
        /// The biome of the tile, as determined by its traits
        /// </summary>
        public GlobalBiome? Biome { get; set; }

        /// <summary>
        /// Determines how desirable the tile is, based on its temperature, elevation, and resources
        /// </summary>
        public int BaseDesirability { get; set; }

        /// <summary>
        /// If set to true, it will not be possible to colonise on this tile
        /// </summary>
        public bool IsBlockedForColonisation { get; set; }

        /// <summary>
        /// Whether this tile has a road. Later we'll have proper directions and everything
        /// </summary>
        public bool HasRoad { get; set; }

        /// <summary>
        /// Which civilisation owns the tile - if any. Other civilisations will not colonise this tile.
        /// </summary>
        public int? Owner
        {
            get
            {
                if (owners == null || owners.Count == 0)
                {
                    return null;
                }
                else
                {
                    return owners.Peek();
                }

            }
            set
            {
                if (owners == null)
                {
                    owners = new Stack<int>();
                }

                owners.Push(value.Value);
            }
        }

        /// <summary>
        /// Holds the owners. It's a stack so in case the owner changes, we can still keep note of who the owner could also be. This is to be used mostly when clearing resources from being bandit-owned.
        /// </summary>
        private Stack<int> owners { get; set; }

        /// <summary>
        /// Whether it contains a resource (to save us time looking)
        /// </summary>
        public bool HasResource { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Determine how difficult it is to hunt successfully. Hunting will be d10 + explorer skill
        /// </summary>
        /// <returns></returns>
        public int HuntDifficulty()
        {
            //What biome do we have?
            switch (this.Biome.Value)
            {
                case GlobalBiome.ARID_DESERT:
                    return 20;
                case GlobalBiome.DENSE_FOREST:
                    return 3;
                case GlobalBiome.GARIGUE:
                    return 10;
                case GlobalBiome.GRASSLAND:
                    return 5;
                case GlobalBiome.POLAR_DESERT:
                    return 20;
                case GlobalBiome.POLAR_FOREST:
                    return 18;
                case GlobalBiome.RAINFOREST:
                    return 3;
                case GlobalBiome.WETLAND:
                    return 10;
                case GlobalBiome.WOODLAND:
                    return 4;
            }

            throw new NotImplementedException("Type " + this.Biome.Value + " not implemented");
        }


        /// <summary>
        /// Returns the total amount of time needed to traverse into this tile.
        /// Takes into consideration the actor's skill and the way they're travelling
        /// </summary>
        public int TraverseTimeInMinutes(Actor actor)
        {
            int totalTime = 0;

            if (this.Elevation > 250)
            {
                totalTime += 250;
            }

            if (!this.Biome.HasValue)
            {
                return 100;
            }

            //What biome do we have?
            switch (this.Biome.Value)
            {
                case GlobalBiome.ARID_DESERT:
                    totalTime += 100;
                    break;
                case GlobalBiome.DENSE_FOREST:
                    totalTime += 200;
                    break;
                case GlobalBiome.GARIGUE:
                    totalTime += 75;
                    break;
                case GlobalBiome.GRASSLAND:
                    totalTime += 50;
                    break;
                case GlobalBiome.POLAR_DESERT:
                    totalTime += 100;
                    break;
                case GlobalBiome.POLAR_FOREST:
                    totalTime += 150;
                    break;
                case GlobalBiome.RAINFOREST:
                    totalTime += 200;
                    break;
                case GlobalBiome.WETLAND:
                    totalTime += 100;
                    break;
                case GlobalBiome.WOODLAND:
                    totalTime += 100;
                    break;
            }

            //Take the skill into consideration
            double skillEffect = 1;
            if (actor.Attributes.Skills.ContainsKey(SkillName.EXPLORER))
            {
                skillEffect = 20 - actor.Attributes.Skills[SkillName.EXPLORER].SkillLevel;

                skillEffect = 0.5 + ((0.5 / 20) * skillEffect);
            }

            totalTime = (int)(totalTime * skillEffect);

            //Do we have a road?

            if (this.HasRoad)
            {
                //One third of the time
                totalTime /= 3;
            }

            //Increase skill
            actor.Attributes.IncreaseSkill(SkillName.EXPLORER);

            return totalTime;

        }

        public override bool MayContainItems
        {
            get
            {
                //Is it underwater? or a mountain?
                if (this.Elevation < 0 || this.Elevation > 250)
                {
                    //Nope
                    return false;
                }

                //Is there a river?
                if (this.HasRiver)
                {
                    return false;
                }

                return true; //Yeah we can move

            }
            set
            {
                base.MayContainItems = value;
            }
        }

        public override string Name
        {
            get
            {
                if (HasRiver)
                {
                    return "River";
                }
                else if (this.Elevation > 250)
                {
                    return "Mountain";
                }
                else if (this.Elevation < 0)
                {
                    return "Sea";
                }
                else if (!this.Biome.HasValue)
                {
                    return "??";
                }
                else
                { //the name of the biome
                    string text = this.Biome.Value.ToString().ToLower().Replace("_", " ");

                    return char.ToUpper(text[0]) + text.Substring(1);
                }

            }
            set
            {
                base.Name = value;
            }
        }

        public override string Description
        {
            get
            {

                if (HasRiver)
                {
                    return "a river";
                }
                else if (this.Elevation < 0)
                {
                    return "the sea";
                }
                else if (this.Elevation > 250)
                {
                    return "a mountain";
                }

                return "a " + (this.Biome.HasValue ? this.Biome.Value.ToString().ToLower().Replace("_", " ") : "?") + " with an elevation of " + this.Elevation;
            }
            set
            {
                base.Description = value;
            }
        }

        public override List<SpriteData> Graphics
        {
            get
            {
                //we need to determine what kind of items we have on the tile
                List<SpriteData> graphics = new List<SpriteData>();

                //check the elevation and determine which graphic we'll show

                if (Elevation < 0)
                {
                    graphics.Add(RIVER);
                }
                else if (Biome.HasValue)
                {
                    //The graphic will depend on the biome
                    switch (this.Biome)
                    {
                        case GlobalBiome.ARID_DESERT:
                            graphics.Add(DESERTTILE);
                            break;
                        case GlobalBiome.DENSE_FOREST:
                            graphics.Add(BIGTREE);
                            graphics.Add(GRASSTILE);
                            break;
                        case GlobalBiome.GARIGUE:
                            graphics.Add(GARIGUETILE);
                            break;
                        case GlobalBiome.GRASSLAND:
                            graphics.Add(GRASSTILE);
                            break;
                        case GlobalBiome.POLAR_DESERT:
                            graphics.Add(SNOWTILE);
                            break;
                        case GlobalBiome.RAINFOREST:
                            graphics.Add(TROPICALTREE);
                            graphics.Add(TROPICALTILE);
                            break;
                        case GlobalBiome.WETLAND:
                            graphics.Add(SWAMPTILE);
                            break;
                        case GlobalBiome.WOODLAND:
                            graphics.Add(TREE);
                            graphics.Add(GRASSTILE);
                            break;
                        case GlobalBiome.POLAR_FOREST:
                            graphics.Add(DEADTREE);
                            graphics.Add(SNOWTILE);
                            break;

                    }
                }
                else
                {
                    //default
                    graphics.Add(GRASSTILE);
                }

                //Do we have a slope?

                if (HasHillSlope)
                {
                    graphics.Insert(0, HILLSLOPE);
                }

                //Do we have a river?

                if (HasRiver)
                {
                    graphics.Insert(0, RIVER);
                }

                //do we have a mountain?

                if (Elevation > 250)
                {
                    graphics.Insert(0, MOUNTAIN);
                }

                //Do we have a road?

                if (HasRoad)
                {
                    graphics.Insert(0,SpriteManager.GetSprite(LocalSpriteName.PAVEMENT_TILE_1));
                }

                return graphics;
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();

            actions.AddRange(base.GetPossibleActions(actor));

            if (actor.MapCharacter.Coordinate - this.Coordinate < 2)
            {
                //Allow a hunt
                actions.Add(ActionType.HUNT);
            }

            return actions.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.HUNT)
            {
                //User is going to try to hunt.

                actor.Attributes.IncreaseSkill(SkillName.EXPLORER);

                //Depending on his skill and how plentiful animals are, time will pass. But we'll always be successful

                TimePassFeedback tps = new TimePassFeedback() { TimePassInMinutes = 100 }; //An IG Hour

                Random random = new Random();

                int diceRoll = random.Next(10) + 1; //Roll a d10

                if (diceRoll + actor.Attributes.Skills[SkillName.EXPLORER].SkillLevel + (actor.Attributes.Perc - 5) >= HuntDifficulty())
                {
                    LocationChangeFeedback lcf = new LocationChangeFeedback() { RandomEncounter = this.Biome };

                    return new ActionFeedback[] { tps, lcf }; //Success!

                }

                return new ActionFeedback[] { tps, new CurrentLogFeedback(InterfaceSpriteName.HUNT, Color.Orange, "You fail to find anything") };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }

        #endregion

        #region Functions

        public SpriteData GetGraphicsByOverlay(GlobalOverlay overlay)
        {
            if (overlay.Equals(GlobalOverlay.NONE))
            {
                return null;
            }

            #region Region Overlay
            else if (overlay.Equals(GlobalOverlay.REGION))
            {
                //if the elevation is underwater, nothing
                if (this.Elevation < 0)
                {
                    return null;
                }

                //otherwise, depending on the region number, colour them
                switch (Region % 7)
                {
                    case 0: return BROWN;
                    case 1: return GREEN;
                    case 2: return ORANGE;
                    case 3: return PINK;
                    case 4: return PURPLE;
                    case 5: return RED;
                    case 6: return YELLOW;
                }

                return null;

            }
            #endregion
            #region Temperature Overlay
            else if (overlay.Equals(GlobalOverlay.TEMPERATURE))
            {

                //Don't overlay the sea
                if (this.Elevation < 0)
                {
                    return null;
                }

                if (this.ClimateTemperature > 30)
                {
                    return RED;
                }
                else if (this.ClimateTemperature > 20)
                {
                    return ORANGE;
                }
                else if (this.ClimateTemperature > 10)
                {
                    return YELLOW;
                }
                else if (this.ClimateTemperature > 0)
                {
                    return INDIGO;
                }
                else if (this.ClimateTemperature > -10)
                {
                    return MARBLEBLUE;
                }
                else if (this.ClimateTemperature < -10)
                {
                    return WHITE;
                }

                return null;
            }
            #endregion
            #region Rainfall Overlay
            else if (overlay.Equals(GlobalOverlay.RAINFALL))
            {

                if (Elevation < 0)
                {
                    return null;
                }

                if (Rainfall > 8)
                {
                    //really wet
                    return RED;

                }
                else if (Rainfall > 6)
                {
                    //wet
                    return ORANGE;
                }
                else if (Rainfall > 4)
                {
                    return YELLOW;
                }
                else if (Rainfall > 2)
                {
                    return INDIGO;
                }
                else if (Rainfall < 2)
                {
                    return null;
                }

            }
            #endregion
            #region Elevation Overlay
            else if (overlay == GlobalOverlay.ELEVATION)
            {
                if (Elevation > 250)
                {
                    //mountain
                    return RED;
                }
                else if (Elevation > 100)
                {
                    return ORANGE;
                }
                else if (Elevation > 50)
                {
                    return YELLOW;
                }
                else if (Elevation > 25)
                {
                    return INDIGO;
                }
                else if (Elevation > 0)
                {
                    return WHITE;
                }
                else
                {
                    return null;
                }
            }
            #endregion
            #region Desirability Overlay
            else if (overlay == GlobalOverlay.DESIRABILITY)
            {
                if (BaseDesirability > 10)
                {
                    return GREEN;
                }
                else if (BaseDesirability > 5)
                {
                    return YELLOW;
                }
                else if (BaseDesirability > 0)
                {
                    return ORANGE;
                }
                else if (BaseDesirability <= 0)
                {
                    return RED;
                }
            }
            #endregion
            #region Owner
            else if (overlay == GlobalOverlay.OWNER)
            {
                if (Owner == null)
                {
                    return null;
                }

                switch (Owner.Value)
                {
                    case 0: return BROWN;
                    case 1: return GREEN;
                    case 2: return ORANGE;
                    case 3: return PINK;
                    case 4: return PURPLE;
                    case 5: return RED;
                    case 6: return YELLOW;

                    case 100: return INDIGO; //orc
                    case 50: return WHITE; //banditS
                }
            }
            #endregion
            return null;

        }

        #endregion

        #region Constructor

        public GlobalTile()
        {
            Elevation = 0;
            Region = -1;
            base.MayContainItems = true;
            IsBlockedForColonisation = false;
            HasResource = false;
            HasRoad = false;
        }

        #endregion
    }
}
