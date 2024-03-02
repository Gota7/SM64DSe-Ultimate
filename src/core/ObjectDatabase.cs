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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;
using Serilog;

namespace SM64DSe
{
    public static class ObjectDatabase
    {
        public class ObjectInfo
        {
            public struct ParamInfo
            {
                [JsonProperty("name")]
                public string m_Name;
                [JsonProperty("offset")]
                public int m_Offset;
                [JsonProperty("length")]
                public int m_Length;
                [JsonProperty("length")]
                public string m_Type;
                [JsonProperty("values")]
                public string m_Values;
                [JsonProperty("description")]
                public string m_Description;
            }

            public string GetBasicInfo()
            {
                return (m_Name + "\n" + m_InternalName + "\n" + m_Description);
            }

            [JsonProperty("category")]
            public int m_Category;

            [JsonProperty("id")]
            public ushort m_ID;
            
            [JsonProperty("actorid")]
            public ushort m_ActorID;
            
            [JsonProperty("name")]
            public string m_Name;
            
            [JsonProperty("internalname")]
            public string m_InternalName;
            
            [JsonProperty("description")]
            public string m_Description;
            
            [JsonProperty("bankreq")]
            public int m_BankRequirement;
            
            [JsonProperty("dlreq")]
            public string m_DynamicLibraryRequirement;

            [JsonProperty("params")]
            public ParamInfo[] m_ParamInfo;
            
            [JsonIgnore]
            public int m_NumBank;
            
            [JsonIgnore]
            public int m_BankSetting;

            [JsonIgnore]
            public string m_IsCustomModelPath;
        }

        public static ObjectInfo[] m_ObjectInfo;
        public static uint m_Timestamp;
        public static WebClient m_WebClient;
        
        public static void Initialize()
        {
            m_ObjectInfo = new ObjectInfo[65536];
            for (int i = 0; i < 65536; i++)
                m_ObjectInfo[i] = new ObjectInfo();

            m_WebClient = new WebClient();
        }

        public static void LoadFromFile(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                List<ObjectInfo> items = JsonConvert.DeserializeObject<List<ObjectInfo>>(json);
                
                // This would overwrite any previously defined objects
                foreach (ObjectInfo objectInfo in items)
                {
                    if (objectInfo.m_InternalName != null && objectInfo.m_InternalName.StartsWith("@CUSTOM%"))
                    {
                        objectInfo.m_IsCustomModelPath = objectInfo.m_InternalName.Substring(8);
                    }
                    if (objectInfo.m_DynamicLibraryRequirement != null && objectInfo.m_DynamicLibraryRequirement.StartsWith("@CUSTOM%"))
                    {
                        objectInfo.m_DynamicLibraryRequirement = objectInfo.m_DynamicLibraryRequirement.Substring(8);
                    }
                    m_ObjectInfo[objectInfo.m_ID] = objectInfo;
                }
                Log.Information($"{items.Count} has been loaded.");
            }
        }

        public static void LoadProjectSpecific()
        {
            if (Program.m_ROM == null)
                return;
            
            string parentDirectory = Directory.GetParent(Program.m_ROM.m_Path).FullName;
            string projectObjects = Path.Combine(parentDirectory, "objects.json");
            if (!File.Exists(projectObjects))
                return;
            
            Console.WriteLine("We found an objects.json aside the ROM, loading it as additonal objects");
            LoadFromFile(projectObjects);
        }

        public static void Load()
        {
            FileStream fs = null; XmlReader xr = null;
            try 
            { 
                fs = File.OpenRead("assets/objectdb.xml"); 
                xr = XmlReader.Create(fs);

                xr.ReadToFollowing("database");
                xr.MoveToAttribute("timestamp");
                m_Timestamp = uint.Parse(xr.Value);
            }
            catch
            {
                if (xr != null) xr.Close();
                if (fs != null) fs.Close();

                m_Timestamp = 1;
                throw new Exception("Failed to open objectdb.xml");
            }

            while (xr.ReadToFollowing("object"))
            {
                string temp;

                xr.MoveToAttribute("id");
                int id = 0; int.TryParse(xr.Value, out id);
                if ((id < 0) || (id > m_ObjectInfo.Length))
                    continue;

                ObjectInfo oinfo = m_ObjectInfo[id];
                oinfo.m_ID = (ushort)id;

                xr.ReadToFollowing("category");
                temp = xr.ReadElementContentAsString();
                int.TryParse(temp, out oinfo.m_Category);

                xr.ReadToFollowing("name");
                oinfo.m_Name = xr.ReadElementContentAsString();
                xr.ReadToFollowing("internalname");
                oinfo.m_InternalName = xr.ReadElementContentAsString();
                if (oinfo.m_InternalName.StartsWith("@CUSTOM%")) { oinfo.m_IsCustomModelPath = oinfo.m_InternalName.Substring(8); } else { oinfo.m_IsCustomModelPath = null; }

                if (oinfo.m_Name == "")
                    oinfo.m_Name = oinfo.m_InternalName;

                xr.ReadToFollowing("actorid");
                temp = xr.ReadElementContentAsString();
                ushort.TryParse(temp, out oinfo.m_ActorID);

                xr.ReadToFollowing("description");
                oinfo.m_Description = xr.ReadElementContentAsString();

                xr.ReadToFollowing("bankreq");
                temp = xr.ReadElementContentAsString();
                if (temp == "none")
                    oinfo.m_BankRequirement = 0;
                else
                {
                    oinfo.m_BankRequirement = 1;
                    try
                    {
                        oinfo.m_NumBank = int.Parse(temp.Substring(0, temp.IndexOf('=')));
                        oinfo.m_BankSetting = int.Parse(temp.Substring(temp.IndexOf('=') + 1));
                    }
                    catch { oinfo.m_BankRequirement = 2; }
                }
                
                xr.ReadToFollowing("dlreq");
                temp = xr.ReadElementContentAsString();
                if (temp != null && temp != "none")
                {
                    if (oinfo.m_DynamicLibraryRequirement.StartsWith("@CUSTOM%"))
                    {
                        temp = temp.Substring(8);
                    }
                    oinfo.m_DynamicLibraryRequirement = temp;
                }

                List<ObjectInfo.ParamInfo> paramlist = new List<ObjectInfo.ParamInfo>();
                while (xr.ReadToNextSibling("param"))
                {
                    ObjectInfo.ParamInfo pinfo = new ObjectInfo.ParamInfo();

                    xr.ReadToFollowing("name");
                    pinfo.m_Name = xr.ReadElementContentAsString();

                    xr.ReadToFollowing("offset");
                    temp = xr.ReadElementContentAsString();
                    int.TryParse(temp, out pinfo.m_Offset);
                    xr.ReadToFollowing("length");
                    temp = xr.ReadElementContentAsString();
                    int.TryParse(temp, out pinfo.m_Length);

                    xr.ReadToFollowing("type");
                    pinfo.m_Type = xr.ReadElementContentAsString();
                    xr.ReadToFollowing("values");
                    pinfo.m_Values = xr.ReadElementContentAsString();

                    xr.ReadToFollowing("description");
                    pinfo.m_Description = xr.ReadElementContentAsString();

                    paramlist.Add(pinfo);
                }
                oinfo.m_ParamInfo = paramlist.ToArray();
            }

            xr.Close();
            fs.Close();

            // Loading potential project specific objects from user defined file
            try
            {
                LoadProjectSpecific();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                MessageBox.Show($"Cannot load objects from `objects.json` file: {e.Message}");
            }
        } 

        public static void LoadFallback()
        {
            StringReader sr = new StringReader(Properties.Resources.obj_list);

            String curline;
            Regex lineregex = new Regex("0x([\\dabcdef]+) == (.*?) \\(0x([\\dabcdef]+)\\)");
            
            while ((curline = sr.ReadLine()) != null)
            {
                Match stuff = lineregex.Match(curline);

                int id = int.Parse(stuff.Groups[1].Value, NumberStyles.HexNumber);
                ObjectInfo oinfo = m_ObjectInfo[id];

                oinfo.m_ID = (ushort)id;
                oinfo.m_Name = stuff.Groups[2].Value;
                oinfo.m_InternalName = stuff.Groups[2].Value;
                oinfo.m_ActorID = ushort.Parse(stuff.Groups[3].Value, NumberStyles.HexNumber);

                oinfo.m_Category = 0;
                oinfo.m_Description = "";
                oinfo.m_BankRequirement = 2;
                oinfo.m_ParamInfo = new ObjectInfo.ParamInfo[0];
            }

            sr.Close();
        }

        public static void Update(bool force)
        {
            string ts = force ? "" : "?ts=" + m_Timestamp.ToString();
            m_WebClient.DownloadStringAsync(new Uri(Program.ServerURL + "download_objdb.php" + ts));
        }
    }
}
