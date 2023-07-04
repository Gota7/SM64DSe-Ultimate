using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SM64DSe
{
	public static class FileHeaderGenerator
	{
        private static readonly string[] fileHeaderStart =
{
            "#pragma once",
            "",
            "consteval u16 GetFileIdFromName(const char* name, bool ov0 = true)",
            "{",
            "\tstruct FileInfo",
            "\t{",
            "\t\tu16 fileID;",
            "\t\tu16 ov0ID;",
            "\t\tconst char* name;",
            "\t};",
            "\t",
            "\tconstexpr FileInfo files[] =",
            "\t{",
        };

        private static readonly string[] fileHeaderEnd =
{
            "\t};",
            "\t",
            "\tconst FileInfo* begin = std::begin(files);",
            "\tconst FileInfo* end = std::end(files);",
            "\tconst FileInfo* prevMid = nullptr;",
            "\t",
            "\twhile (true)",
            "\t{",
            "\t\tconst FileInfo* mid = begin + (end - begin >> 1);",
            "\t\tif (void FileNotFound(); mid == prevMid) FileNotFound();",
            "\t\tprevMid = mid;",
            "\t\t",
            "\t\tconst char* c0 = name;",
            "\t\tconst char* c1 = mid->name;",
            "\t\t",
            "\t\tfor(; *c0 == *c1; ++c0, ++c1)",
            "\t\t\tif (*c0 == '\\0') return ov0 ? mid->ov0ID : mid-> fileID;",
            "\t\t",
            "\t\tif (*c0 < *c1) end = mid; else begin = mid;",
            "\t}",
            "}",
            "",
            "consteval u16 operator\"\"fid(const char* name, std::size_t)",
            "{",
            "\treturn GetFileIdFromName(name, false);",
            "}",
            "",
            "consteval u16 operator\"\"ov0(const char* name, std::size_t)",
            "{",
            "\treturn GetFileIdFromName(name, true);",
            "}",
        };

        private static string ToHex(ushort num)
        {
            return "0x" + Convert.ToString(num, 16).PadLeft(4, '0').ToLower();
        }

        public static void Generate(string headerFilePath)
		{
            List<NitroROM.FileEntry> sortedFiles = Program.m_ROM.m_FileEntries.ToList();
            sortedFiles.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));

            List<string> lines = new List<string>();
            lines.Capacity = fileHeaderStart.Length + fileHeaderEnd.Length + Program.m_ROM.m_FileEntries.Length;
            lines.AddRange(fileHeaderStart);

            foreach (NitroROM.FileEntry file in sortedFiles) if (file.InternalID != 0xffff)
                    lines.Add("\t\t{ " + ToHex(file.ID) + ", " + ToHex(file.InternalID) + ", " + '"' + file.FullName + '"' + " },");

            lines.AddRange(fileHeaderEnd);

            File.WriteAllLines(headerFilePath, lines);
        }
	}
}
