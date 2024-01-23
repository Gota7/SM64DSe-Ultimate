/*
    Copyright 2012 Kuribo64

    This file is part of SM64DSe.

    SM64DSe is free software: you can redistribute it and/or modify it under
    the terms of the GNU General Public License as published by the Free
    Software Foundation, either version 3 of the License, or (at your option)
    any later version.

    SM64DSe is distributed in the hope that it will be useful, but WITHOUT ANY 
    WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS 
    FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

    You should have received a copy of the GNU General Public License along 
    with SM64DSe. If not, see http://www.gnu.org/licenses/.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace SM64DSe
{
    public static class Strings
    {
        public static List<string> LevelNames()
        {
            List<string> levelNames = new List<string>();

            using (XmlReader reader = XmlReader.Create(Path.Combine(Application.StartupPath, "assets/Levels.xml")))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        if (reader.LocalName == "Name")
                        {
                            reader.MoveToContent();
                            levelNames.Add(reader.ReadElementContentAsString());
                        }
                    }
                }
            }

            return levelNames;
        }

        public static List<string> ShortLvlNames()
        {
            List<string> levelNames = new List<string>();

            using (XmlReader reader = XmlReader.Create(Path.Combine(Application.StartupPath, "assets/Levels.xml")))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    if (reader.NodeType.Equals(XmlNodeType.Element))
                    {
                        if (reader.LocalName == "ShortName")
                        {
                            reader.MoveToContent();
                            levelNames.Add(reader.ReadElementContentAsString());
                        }
                    }
                }
            }

            return levelNames;
        }

        public static string[] PaintingNames =
        {
            "Bob-omb Battlefield",
            "Whomp's Fortress",
            "Jolly Roger Bay (Bubbles)",
            "Cool Cool Mountain (Snowmen)",
            "Hazy Maze Cave (Silver Pool)",
            "Lethal Lava Land (Evil Face)",
            "Shifting Sand Land (Stone Wall)",
            "Dire, Dire Docks (Blue Shimmer)",
            "Snowman's Land (Pale Wallpaper)",
            "Wet-Dry World (Skeeter)",
            "Tall, Tall Mountain",
            "Mountain Slide (Rocky Wall)",
            "Tiny-Huge Island (Goombas)",
            "Tick Tock Clock (Clock Face)",
            "Goomboss Battle (Mario)",
            "Big Boo Battle (Luigi)",
            "Cheif Chilly Challenge (Wario)",
            "Battle Fort (Unused)",
            "Sunshine Isles"
        };

        public static string[] DoorTypes = 
        {
            "Virtual door", 
            "Standard door", 
            "Door with star (0)", 
            "Door with star (1) (WF)",                              
            "Door with star (3) (JRB)", 
            "Door with star (8) (Goomboss)", 
            "Locked door (castle basement)", 
            "Locked door (castle 2nd floor)",                              
            "Standard door", 
            "Bowser door (BitDW)", 
            "Bowser door (BitFS)", 
            "Bowser door (castle 2nd floor)",                 
            "Bowser door (BitS)", 
            "Door with star (1) (playroom)", 
            "Door with star (3) (CCM)", 
            "Old wooden door",                    
            "Rusted metal door", 
            "HMC door", 
            "BBH door", 
            "Character change (Mario)",
            "Character change (Luigi)", 
            "Character change (Wario)", 
            "Locked door (castle entrance)", 
            "Silent room door"
        };

        public static readonly string MODEL_FORMATS_FILTER = "All Supported Models|*.dae;*.imd;*.obj|" +
                "COLLADA DAE|*.dae|NITRO Intermediate Model Data|*.imd|Wavefront OBJ|*.obj";
        public static readonly string MODEL_ANIMATION_FORMATS_FILTER = "All Supported Animation Formats|*.dae;*.ica|" +
                "COLLADA DAE|*.dae|NITRO Intermediate Character Animation|*.ica";

        public static readonly string MODEL_EXPORT_FORMATS_FILTER = 
            "COLLADA DAE (.dae)|*.dae|Wavefront OBJ (.obj)|*.obj";

        public static readonly string IMAGE_EXPORT_PNG_FILTER = "PNG Image (.png)|*.png";

        public static readonly string FILTER_XML = "XML Document (.xml)|*.xml";

        public static string[] WHITESPACE = new string[] { " ", "\n", "\r\n", "\t" };

        public static bool IsBlank(string value)
        {
            return (value == null || value.Trim().Length < 1);
        }

        public static bool IsNotBlank(string value)
        {
            return !IsBlank(value);
        }
    }
}
