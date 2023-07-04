using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SM64DSe
{
	public static class SoundHeaderGenerator
	{
        class SeqArcListItem
        {
            public string Name;
            public ushort ID;
            public ushort SeqArcID;
        }

        private static readonly string[] soundHeaderStart =
        {
            "#pragma once",
            "",
            "#include \"SM64DS_Common.h\"",
            "#include \"Sound.h\"",
            "",
            "struct SoundIDs",
            "{",
            "\tu16 seqArcID;",
            "\tu16 seqID;",
            "};",
            "",
            "consteval SoundIDs GetSoundInfo(const char* name)",
            "{",
            "\tstruct SoundInfo",
            "\t{",
            "\t\tSoundIDs ids;",
            "\t\tconst char* name;",
            "\t};",
            "\t",
            "\tconstexpr SoundInfo files[] =",
            "\t{",
        };

        private static readonly string[] soundHeaderEnd =
{
            "\t};",
            "\t",
            "\tconst SoundInfo* begin = std::begin(files);",
            "\tconst SoundInfo* end = std::end(files);",
            "\tconst SoundInfo* prevMid = nullptr;",
            "\t",
            "\twhile (true)",
            "\t{",
            "\t\tconst SoundInfo* mid = begin + (end - begin >> 1);",
            "\t\tif (void SoundNotFound(); mid == prevMid) SoundNotFound();",
            "\t\tprevMid = mid;",
            "\t\t",
            "\t\tconst char* c0 = name;",
            "\t\tconst char* c1 = mid->name;",
            "\t\t",
            "\t\tfor(; *c0 == *c1; ++c0, ++c1)",
            "\t\t\tif (*c0 == '\\0') return mid->ids;",
            "\t\t",
            "\t\tif (*c0 < *c1) end = mid; else begin = mid;",
            "\t}",
            "}",
            "",
            "namespace Sound",
            "{",
            "\t[[gnu::always_inline]]",
            "\tinline void Play(SoundIDs ids, Vector3 camSpacePos)",
            "\t{",
            "\t\tPlay(ids.seqArcID, ids.seqID, camSpacePos);",
            "\t}",
            "\t",
            "\t[[gnu::always_inline]]",
            "\tinline void Play2D(SoundIDs ids)",
            "\t{",
            "\t\tPlay2D(ids.seqArcID, ids.seqID);",
            "\t}",
            "\t",
            "\t[[gnu::always_inline]]",
            "\tinline void PlayLong(u32& soundID, SoundIDs ids, Vector3 camSpacePos, u32 arg4)",
            "\t{",
            "\t\tsoundID = PlayLong(soundID, ids.seqArcID, ids.seqID, camSpacePos, arg4);",
            "\t}",
            "}",
            "",
            "consteval SoundIDs operator\"\"sfx(const char* name, std::size_t)",
            "{",
            "\treturn GetSoundInfo(name);",
            "}",
        };

        private static string ToHex(ushort num)
        {
            return "0x" + Convert.ToString(num, 16).PadLeft(4, '0').ToLower();
        }

        public static void Generate(string headerFilePath)
		{
            SM64DSFormats.SDAT soundData = new SM64DSFormats.SDAT(Program.m_ROM.GetFileFromName("data/sound_data.sdat"));
            List<SeqArcListItem> sequences = new List<SeqArcListItem>();
            ushort seqArcID = 0;
            ushort seqID = 0;

            foreach (SM64DSFormats.SDAT.SequenceArchive seqArc in soundData.m_SeqArcs)
            {
                foreach (SM64DSFormats.SDAT.SequenceArchive.Sequence seq in seqArc.m_Sequences)
                {
                    sequences.Add(new SeqArcListItem { Name = seq.m_Filename, ID = seqID, SeqArcID = seqArcID });
                    seqID++;
                }

                seqID = 0;
                seqArcID++;
            }

            sequences.Sort((a, b) => string.CompareOrdinal(a.Name, b.Name));

            List<string> lines = new List<string>();
            lines.Capacity = soundHeaderStart.Length + soundHeaderEnd.Length + sequences.Count;
            lines.AddRange(soundHeaderStart);

            foreach (SeqArcListItem sequence in sequences) if (sequence.Name != "SYMB²")
                    lines.Add("\t\t{ " + ToHex(sequence.SeqArcID) + ", " + ToHex(sequence.ID) + ", " + '"' + sequence.Name + '"' + " },");

            lines.AddRange(soundHeaderEnd);

            File.WriteAllLines(headerFilePath, lines);
        }
	}
}
