﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.LocalMapGeneratorObjects;
using System.Xml.Linq;
using DRObjects.LocalMapGeneratorObjects.Enums;
using DRObjects.ActorHandling;
using DRObjects.Enums;
using DRObjects.ActorHandling.CharacterSheet.Enums;

namespace DivineRightGame.LocalMapGenerator
{
    public class LocalMapXMLParser
    {
        private const string MAPLETTAG = "MapletTag";

        public Maplet ParseMapletFromTag(string tag,GlobalBiome? biome =  null)
        {
            return ParseMaplet(MapletDatabaseHandler.GetMapletByTag(tag),biome);
        }

        public Maplet ParseMaplet(string path)
        {
            XDocument doc = XDocument.Load(path);
            return ParseMaplet((XElement)doc.FirstNode);
        }

        /// <summary>
        /// Parses the maplet. The optional biome is for use on herds
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="biome"></param>
        /// <returns></returns>
        public Maplet ParseMaplet(XElement xml,GlobalBiome? biome = null)
        {
            //Read the first node - its going to be a maplet
            XElement element =  xml;

            if (element.Name != "Maplet")
            {
                throw new Exception("The expected node was not a maplet");
            }

            Maplet maplet = new Maplet();

            //Get the attributes
            foreach (XAttribute attr in element.Attributes())
            {
                string value = attr.Value;

                switch (attr.Name.LocalName)
                {
                    case "MapletName": maplet.MapletName = value; break;
                    case "SizeX": maplet.SizeX = Int32.Parse(value); break;
                    case "SizeY": maplet.SizeY = Int32.Parse(value); break;
                    case "SizeRange": maplet.SizeRange = Int32.Parse(value); break;
                    case "Walled": maplet.Walled = Boolean.Parse(value); break;
                    case "WindowProbability": maplet.WindowProbability = Int32.Parse(value); break;
                    case "Tiled": maplet.Tiled = Boolean.Parse(value); break;
                    case "TileID": maplet.TileID = Int32.Parse(value); break;
                    case "TileTag": maplet.TileTag = value; break;

                }
            }

            maplet.MapletContents = new List<MapletContents>();

            XElement mapletContents =element.Elements().First();
            
            //Now we go through all the children
            foreach (XElement contents in mapletContents.Elements())
            {
                //It's always going to be a MapletContents - so we can pre-populate the data
                MapletContents content = null;

                //So, what's the type of it?
                switch (contents.Name.LocalName)
                {
                    case "MapletContentsItem": content = new MapletContentsItem(); break;
                    case "MapletContentsItemTag": content = new MapletContentsItemTag(); break;
                    case "MapletContentsMaplet": content = new MapletContentsMaplet(); break;
                    case "MapletActor": content = new MapletActor(); break;
                    case "MapletHerd": content = new MapletHerd() { LocalBiome = biome.HasValue ? biome.Value : GlobalBiome.ARID_DESERT  }; break;
                    case "MapletActorWanderArea": content = new MapletActorWanderArea(); break;
                    case "MapletPatrolPoint": content = new MapletPatrolPoint(); break;
                    case "MapletFootpathNode": content = new MapletFootpathNode(); break;
                    case "MapletContentsItemSpecial": content = new MapletContentsItemSpecial(); break;
                }

                //Get the attributes
                foreach (XAttribute attr in contents.Attributes())
                {
                    string value = attr.Value;

                    switch (attr.Name.LocalName)
                    {
                        case "ProbabilityPercentage": content.ProbabilityPercentage = Int32.Parse(value); break;
                        case "MaxAmount": content.MaxAmount = Int32.Parse(value); break;
                        case "Position": content.Position = (PositionAffinity) Enum.Parse(typeof(PositionAffinity), value, true); break;
                        case "Padding": content.Padding = Int32.Parse(value); break;
                        case "AllowItemsOnTop": content.AllowItemsOnTop = Boolean.Parse(value); break;
                        case "x": content.x = Int32.Parse(value); break;
                        case "y": content.y = Int32.Parse(value); break;
                        case "ItemCategory": ((MapletContentsItem)content).ItemCategory = value; break;
                        case "ItemID": ((MapletContentsItem)content).ItemID = Int32.Parse(value); break;
                        case "Category": ((MapletContentsItemTag)content).Category = value; break;
                        case "Tag": ((MapletContentsItemTag)content).Tag = value; break;
                        case "FirstFit": ((MapletContentsMaplet)content).FirstFit = bool.Parse(value); break;
                        case "EnemyID": ((MapletActor)content).EnemyID = Int32.Parse(value); break;
                        case "EnemyType": ((MapletActor)content).EnemyType = value; break;
                        case "EnemyTag": ((MapletActor)content).EnemyTag = value; break;
                        case "UseLocalType": ((MapletActor)content).UseLocalType = bool.Parse(value); break;
                        case "EnemyMission": ((MapletActor)content).EnemyMission = (ActorMissionType)Enum.Parse(typeof(ActorMissionType), value.ToUpper()); break;
                        case "VendorType": ((MapletActor)content).VendorType = (VendorType)Enum.Parse(typeof(VendorType), value.ToUpper()); break;
                        case "VendorLevel": ((MapletActor)content).VendorLevel = Int32.Parse(value.ToString()); break;
                        case "BiomeName": ((MapletHerd)content).BiomeName = value.ToString(); break;
                        case "Domesticated": ((MapletHerd)content).Domesticated = bool.Parse(value); break;
                        case "UseLocalBiome": ((MapletHerd)content).UseLocalBiome = bool.Parse(value); break;
                        case "Owners": content.OwnerFactions = value.ToString(); break;
                        case "Profession": 
                            
                            if (content.GetType() == typeof(MapletActorWanderArea))
                            {
                                ((MapletActorWanderArea)content).Profession = (ActorProfession)Enum.Parse(typeof(ActorProfession), value.ToUpper());
                            }
                            else if (content.GetType() == typeof(MapletPatrolPoint))
                            {
                                ((MapletPatrolPoint)content).Profession = (ActorProfession)Enum.Parse(typeof(ActorProfession), value.ToUpper()); 
                            }
                            break;
                        case "MaximumAmount": content.MaxAmount = Int32.Parse(value); break;
                        case "PatrolName": ((MapletPatrolPoint)content).PatrolName = value; break;
                        case "PointRadius": ((MapletPatrolPoint)content).PointRadius = Int32.Parse(value); break;
                        case "IsPrimary": ((MapletFootpathNode)content).IsPrimary = Boolean.Parse(value); break;
                        case "HerdTag": ((MapletHerd)content).HerdTag = value; break;
                        case "Type": ((MapletContentsItemSpecial)content).Type = value; break;
                        case "Arguments": ((MapletContentsItemSpecial)content).Type = value; break;
                    }
                }

                maplet.MapletContents.Add(content);

                //Now if its a MapletContentsMaplet, it'll contain an element which is the maplet
                if (typeof(MapletContentsMaplet).Equals(content.GetType()))
                {
                    if (contents.Elements().First().Name.LocalName.Equals(MAPLETTAG))
                    {
                        //This is a maplet tag - we load it from the file instead
                        ((MapletContentsMaplet)content).Maplet = ParseMaplet(MapletDatabaseHandler.GetMapletByTag(contents.Elements().First().Attribute("Tag").Value));
                    }
                    else
                    {
                        ((MapletContentsMaplet)content).Maplet = ParseMaplet(contents.Elements().First());
                    }
                }
               
            }


            return maplet;
        }

    }
}
