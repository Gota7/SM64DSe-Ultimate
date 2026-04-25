using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SM64DSe.ImportExport.LevelImportExport
{
    class LevelImporterV2 : LevelImporterV1
    {
        public LevelImporterV2() 
            : base() { }

        protected override void ReadCLPSDataFromXML(XmlReader reader, Level level)
        {
            CLPS.Entry entry = new CLPS.Entry();

            while (reader.Read())
            {
                reader.MoveToContent();

                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    switch (reader.LocalName)
                    {
                        case "Entry":
                            entry = new CLPS.Entry();
                            break;
                        case "TerrainType":
                            entry.m_Texture = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Water":
                            entry.m_Water = (ulong)(reader.ReadElementContentAsBoolean() ? 1 : 0);
                            break;
                        case "ViewID":
                            entry.m_ViewID = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Traction":
                            entry.m_Traction = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "CameraBehaviour":
                            entry.m_CamBehav = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Behaviour":
                            entry.m_Behav = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "TransparentToCamera":
                            entry.m_CamThrough = (ulong)(reader.ReadElementContentAsBoolean() ? 1 : 0);
                            break;
                        case "Toxic":
                            entry.m_Toxic = (ulong)(reader.ReadElementContentAsBoolean() ? 1 : 0);
                            break;
                        case "Unknown26":
                            entry.m_Unk26 = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Padding1":
                            entry.m_Pad1 = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "WindID":
                            entry.m_WindID = (ulong)reader.ReadElementContentAsLong();
                            break;
                        case "Padding2":
                            entry.m_Pad2 = (ulong)reader.ReadElementContentAsLong();
                            break;
                    }
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement))
                {
                    if (reader.LocalName.Equals("Entry"))
                    {
                        level.m_CLPS.Add(entry);
                    }
                    else if (reader.LocalName.Equals("CLPS"))
                    {
                        return;
                    }
                }
            }
        }

        protected override List<float> ReadTextureAnimationTranslationTable(XmlReader reader)
        {
            return ReadIntArray(reader, "TranslationTable")
                .ConvertAll(x => x / 4096f);
        }

        protected override List<float> ReadTextureAnimationRotationTable(XmlReader reader)
        {
            return ReadShortArray(reader, "RotationTable")
                .ConvertAll(x => x / 4096.0f * 360.0f);
        }

        protected override List<float> ReadTextureAnimationScaleTable(XmlReader reader)
        {
            return ReadIntArray(reader, "ScaleTable")
                .ConvertAll(x => x / 4096f);
        }

        private List<int> ReadIntArray(XmlReader reader, string element)
        {
            int length = int.Parse(reader.GetAttribute("length"));

            List<int> values = new List<int>(length);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("value"))
                {
                    values.Add(reader.ReadElementContentAsInt());
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals(element))
                {
                    return values;
                }
            }

            return values;
        }

        private List<short> ReadShortArray(XmlReader reader, string element)
        {
            int length = int.Parse(reader.GetAttribute("length"));

            List<short> values = new List<short>(length);

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("value"))
                {
                    values.Add(short.Parse(reader.ReadElementContentAsString()));
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals(element))
                {
                    return values;
                }
            }

            return values;
        }

        protected override LevelTexAnim.Def ReadTextureAnimation(XmlReader reader,
            List<float> scales, List<float> rotations, List<float> translations)
        {
            LevelTexAnim.Def texAnim = new LevelTexAnim.Def();

            int scaleXStart = 0, scaleXLength = 0;
            int scaleYStart = 0, scaleYLength = 0;
            int rotationStart = 0, rotationLength = 0;
            int translationXStart = 0, translationXLength = 0;
            int translationYStart = 0, translationYLength = 0;

            while (reader.Read())
            {
                reader.MoveToContent();
                if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("MaterialName"))
                {
                    texAnim.m_MaterialName = reader.ReadElementContentAsString();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("DefaultScaleValue"))
                {
                    texAnim.m_DefaultScale = uint.Parse(reader.ReadElementContentAsString()) / 4096f;
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleXStartIndex"))
                {
                    scaleXStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("ScaleXLength"))
                {
                    scaleXLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && (reader.LocalName.Equals("ScaleYStartIndex") || reader.LocalName.Equals("ScaleStartIndex")))
                {
                    scaleYStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && (reader.LocalName.Equals("ScaleYLength") || reader.LocalName.Equals("ScaleLength")))
                {
                    scaleYLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationStartIndex"))
                {
                    rotationStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("RotationLength"))
                {
                    rotationLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationXStartIndex"))
                {
                    translationXStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationXLength"))
                {
                    translationXLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationYStartIndex"))
                {
                    translationYStart = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.Element) && reader.LocalName.Equals("TranslationYLength"))
                {
                    translationYLength = reader.ReadElementContentAsInt();
                }
                else if (reader.NodeType.Equals(XmlNodeType.EndElement) && reader.LocalName.Equals("TextureAnimation"))
                {
                    texAnim.m_ScaleXValues = scales.GetRange(scaleXStart, scaleXLength);
                    texAnim.m_ScaleYValues = scales.GetRange(scaleYStart, scaleYLength);
                    texAnim.m_RotationValues = rotations.GetRange(rotationStart, rotationLength);
                    texAnim.m_TranslationXValues = translations.GetRange(translationXStart, translationXLength);
                    texAnim.m_TranslationYValues = translations.GetRange(translationYStart, translationYLength);
                    return texAnim;
                }
            }

            return texAnim;
        }
    }
}
