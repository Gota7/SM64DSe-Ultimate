using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SM64DS_SCRIPT_LIB
{

    /// <summary>
    /// Command File Binary.
    /// </summary>
    public class CommandBinary {

        public int[] saveInfo = new int[2];

        //Command Binary.
        public CommandBinary(byte[] file = null) {
            if (file != null) { load(file, ref saveInfo); }
        }

        /// <summary>
        /// Commands.
        /// </summary>
        public List<command> commands;

        /// <summary>
        /// Commands.
        /// </summary>
        public struct command {

            public byte length;
            public byte instruction;
            public Int16 minFrame;
            public Int16 maxFrame;
            public byte cameraCommand; //Only if commandType = 0x4;
            public byte[] parameters; //Take up the rest of the size.

        }


        /// <summary>
        /// Load from binary.
        /// </summary>
        /// <param name="b"></param>
        public void load(byte[] b, ref int[] saveInfo) {

            commands = new List<command>();

            MemoryStream src = new MemoryStream(b);
            BinaryReader br = new BinaryReader(src);

            bool foundEnd = false;

            //Read the commands.
            while (src.Position < b.Length && !foundEnd) {

                long backPos = src.Position;

                try
                {

                    command c = new command();
                    c.length = br.ReadByte();
                    if (c.length != 0)
                    {
                        c.instruction = br.ReadByte();
                        c.minFrame = br.ReadInt16();
                        c.maxFrame = br.ReadInt16();
                        if (c.instruction == 0x04) { c.cameraCommand = br.ReadByte(); c.parameters = br.ReadBytes(c.length - 7); } else { c.parameters = br.ReadBytes(c.length - 6); }
                    }
                    else {
                        foundEnd = true;
                        if (src.Position <= (src.Length - 2))
                        {
                            saveInfo[0] = br.ReadByte();
                            saveInfo[1] = br.ReadByte();
                        }
                        else {
                            saveInfo[0] = 0xFF;
                        }
                    }
                    commands.Add(c);

                }
                catch {

                    MessageBox.Show("Found bad instruction at: 0x" + backPos.ToString("X") + ". Ignoring.");
                    src.Position = backPos;
                    byte x = br.ReadByte();
                    if (x > 1) { x -= 1; }
                    src.Position += x;

                }

            }

        }


        /// <summary>
        /// Convert to bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] toBytes(bool writeSaveInfo = false) {

            MemoryStream o = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(o);

            foreach (command c in commands)
            {

                if (c.length != 0)
                {
                    if (c.instruction == 0x4) { bw.Write((byte)(7 + c.parameters.Length)); } else { bw.Write((byte)(6 + c.parameters.Length)); }
                    bw.Write(c.instruction);
                    bw.Write(c.minFrame);
                    bw.Write(c.maxFrame);
                    if (c.instruction == 0x4) { bw.Write(c.cameraCommand); }
                    bw.Write(c.parameters);
                }
                else { bw.Write((byte)0x00); }

            }

            if (saveInfo[0] != 0xFF && writeSaveInfo) {
                bw.Write((byte)saveInfo[0]);
                bw.Write((byte)saveInfo[1]);
            }

            while ((o.Length % 4) != 0) {
                bw.Write((byte)0xFF);
            }

            return o.ToArray();

        }

    }


    /// <summary>
    /// Kuppa File Script.
    /// </summary>
    public static class KuppaFactory {


        /// <summary>
        /// Generate Kuppa script from binary.
        /// </summary>
        /// <returns></returns>
        public static string[] CommandBinaryToKPS(CommandBinary cb, KuppaLib kl) {

            List<string> commands = new List<string>();

            if (cb.saveInfo[0] != 0xFF) {

                string startInfoS = "#save_info " + cb.saveInfo[0] + ", ";
                int bit = cb.saveInfo[1];
                if ((bit & 0b1) == 0b1) { startInfoS += 0 + ""; }
                else if ((bit & 0b10) == 0b10) { startInfoS += 1 + ""; }
                else if ((bit & 0b100) == 0b100) { startInfoS += 2 + ""; }
                else if ((bit & 0b1000) == 0b1000) { startInfoS += 3 + ""; }
                else if ((bit & 0b10000) == 0b10000) { startInfoS += 4 + ""; }
                else if ((bit & 0b100000) == 0b100000) { startInfoS += 5 + ""; }
                else if ((bit & 0b1000000) == 0b1000000) { startInfoS += 6 + ""; }
                else if ((bit & 0b10000000) == 0b10000000) { startInfoS += 7 + ""; }
                else if ((bit & 0b100000000) == 0b100000000) { startInfoS += 8 + ""; }
                else if ((bit & 0b1000000000) == 0b1000000000) { startInfoS += 9 + ""; }
                else if ((bit & 0b10000000000) == 0b10000000000) { startInfoS += 10 + ""; }
                else if ((bit & 0b100000000000) == 0b100000000000) { startInfoS += 11 + ""; }
                else if ((bit & 0b1000000000000) == 0b1000000000000) { startInfoS += 12 + ""; }
                else if ((bit & 0b10000000000000) == 0b10000000000000) { startInfoS += 13 + ""; }
                else if ((bit & 0b100000000000000) == 0b100000000000000) { startInfoS += 14 + ""; }
                else if ((bit & 0b1000000000000000) == 0b1000000000000000) { startInfoS += 15 + ""; }

                commands.Add(startInfoS);

            }

            //Read commands.
            foreach (CommandBinary.command c in cb.commands) {
                commands.Add(kl.FetchCommand(c));
            }

            return commands.ToArray();

        }


        /// <summary>
        /// Generate Command Binary From Kuppa Script.
        /// </summary>
        /// <param name="kps"></param>
        /// <param name="kl"></param>
        /// <returns></returns>
        public static CommandBinary KPSToCommandBinary(string[] kps, KuppaLib kl, ref bool success) {

            CommandBinary cb = new CommandBinary();
            cb.commands = new List<CommandBinary.command>();
            cb.saveInfo = new int[2];
            cb.saveInfo[0] = 0xFF;

            for (int i = 0; i < kps.Length; i++) {

                if (kps[i].StartsWith("#save_info")) {

                    kps[i] = kps[i].Remove(0, 11);
                    kps[i].Replace(" ", "");
                    cb.saveInfo[0] = byte.Parse(NumberInterpreter.ParseValue(kps[i].Split(',')[0]));
                    cb.saveInfo[1] = (1 << byte.Parse(NumberInterpreter.ParseValue(kps[i].Split(',')[1])));
                    kps[i] = "";

                }

            }

            bool commentMode = false;
            foreach (string k in kps) {
                string s = k;
                s = s.Replace("\t", "");
                s = s.Replace(" ", "");
                s = s.Replace("\r", "");

                try
                {
                    if (s.Length > 0 && !s.StartsWith("//") && !s.StartsWith("/*") && !commentMode)
                    {
                        cb.commands.Add(kl.GenerateCommand(s));
                        if (cb.commands[cb.commands.Count() - 1].parameters == null && cb.commands[cb.commands.Count() - 1].length != 0) { throw new Exception(); }
                    }
                }
                catch {
                    success = false;
                    MessageBox.Show(s + " - THIS IS INVALID!!!");
                    cb.commands = new List<CommandBinary.command>();
                }
                if (s.StartsWith("/*")) { commentMode = true; }
                if (s.EndsWith("*/")) { commentMode = false; }
            }

            return cb;

        }

    }

}
