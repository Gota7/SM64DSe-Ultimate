using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static SM64DS_SCRIPT_LIB.NumberInterpreter;

namespace SM64DS_SCRIPT_LIB
{
    public class KuppaLib
    {

        public List<kuppaChange> kuppaChanges; //Changes.

        /// <summary>
        /// Kuppa Changes.
        /// </summary>
        public struct kuppaChange {

            public bool isCamera; //If camera.
            public bool isPlayer; //If player.
            public int id; //Id of type.
            public string textShortcut; //Shortcut of text.
            public int[] paramSizes; //Byte sizes of parameters.
            public NumberFormats[] paramFormats; //Parameter formats.

        }


        /// <summary>
        /// New Lib.
        /// </summary>
        public KuppaLib(ref bool success, string[] file = null) {

            kuppaChanges = new List<kuppaChange>();
            if (file != null) Load(file, ref success);

        }


        /// <summary>
        /// Load from file.
        /// </summary>
        /// <param name="file"></param>
        public void Load(string[] file, ref bool success) {

            bool isCamera = false;
            bool isPlayer = false;
            bool commentMode = false;
            foreach (string s2 in file)
            {

                string s = s2;
                s = s.Replace(" ", string.Empty);
                s = s.Replace("\t", string.Empty);
                s = s.Replace("\r", string.Empty);

                if (s.StartsWith("/*")) { commentMode = true; }
                if (!(s.StartsWith("//") || commentMode))
                {
 
                    if (s.Contains("Camera:")) { isCamera = true; isPlayer = false; }
                    if (s.Contains("Instruction:")) { isCamera = false; isPlayer = false; }
                    if (s.Contains("Player:")) { isCamera = false;  isPlayer = true; }

                    kuppaChange k = new kuppaChange();
                    if (s.Length > 0 && !s.Contains("Camera:") && !s.Contains("Instruction:") && !s.Contains("Player:"))
                    {

                        try
                        {

                            k.id = int.Parse(s.Split('-')[0]);
                            k.textShortcut = s.Split('-')[1].Split('[')[0];
                            string[] pams = s.Split('[')[1].Split(']')[0].Split(',');
                            k.paramSizes = new int[pams.Length];
                            k.paramFormats = new NumberFormats[pams.Length];
                            int pamCount = 0;
                            if (pams[0] != "")
                            {
                                foreach (string pam in pams)
                                {
                                    string num = pam.ToLower();
                                    k.paramFormats[pamCount] = NumberFormats.Decimal;
                                    if (num.Contains(("h"))) { num = num.Replace("h", ""); k.paramFormats[pamCount] = NumberFormats.Hex; }
                                    if (num.Contains(("b"))) { num = num.Replace("b", ""); k.paramFormats[pamCount] = NumberFormats.Bin; }
                                    if (num.Contains(("i"))) { num = num.Replace("i", ""); k.paramFormats[pamCount] = NumberFormats.Input; }
                                    k.paramSizes[pamCount] = int.Parse(num);
                                    pamCount += 1;
                                }
                            }
                            else
                            {
                                k.paramSizes = new int[0];
                            }
                            k.isCamera = isCamera;
                            k.isPlayer = isPlayer;
                            kuppaChanges.Add(k);

                        }
                        catch {

                            success = false;
                            MessageBox.Show(s + " - THIS IS INVALID!!!");

                        }
                    }

                }
                if (s.EndsWith("*/")) { commentMode = false; }

            }

        }


        /// <summary>
        /// Get text command from binary command.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public string FetchCommand(CommandBinary.command c) {

            string command = "";

            //End Command.
            if (c.length == 0)
            {
                command = "End;";
            }

            //Something Else.
            else
            {

                //Camera-Type.
                if (c.instruction == 0x4)
                {

                    //See if shortcut exists.
                    bool shortcutFound = false;
                    foreach (kuppaChange k in kuppaChanges) {

                        if (k.id == c.cameraCommand && k.isCamera && !shortcutFound) {

                            shortcutFound = true;
                            command = k.textShortcut;
                            if (k.paramSizes.Length == 0)
                            {
                                if (c.minFrame != c.maxFrame)
                                {
                                    command += " : (" + c.minFrame + ", " + c.maxFrame + ");";
                                }
                                else {
                                    command += " : (" + c.minFrame + ");";
                                }
                            }
                            else {
                                command += "[";
                                MemoryStream src = new MemoryStream(c.parameters);
                                BinaryReader br = new BinaryReader(src);
                                int count = 0;
                                foreach (int size in k.paramSizes) {

                                    long num;
                                    string realNum = "";
                                    switch (size) {

                                        case 1:
                                            num = br.ReadByte();
                                            break;

                                        case 2:
                                            num = br.ReadInt16();
                                            break;

                                        case 4:
                                            num = br.ReadInt32();
                                            break;

                                        case 8:
                                            num = br.ReadInt64();
                                            break;

                                        default:
                                            throw new Exception("Unsupported Parameter Size!");
                                            break;

                                    }

                                    switch (k.paramFormats[count]) {

                                        case NumberFormats.Decimal:
                                            realNum = num.ToString();
                                            break;

                                        case NumberFormats.Hex:
                                            realNum = IntToHex(num);
                                            break;

                                        case NumberFormats.Bin:
                                            realNum = IntToBinary(num);
                                            break;

                                        case NumberFormats.Input:
                                            realNum = IntToInput(num);
                                            break;

                                    }
                                    command += realNum + ", ";
                                    count += 1;

                                }
                                char[] trimChars = { ',', ' ' };
                                command = command.TrimEnd(trimChars);
                                if (c.minFrame != c.maxFrame)
                                {
                                    command += "] : (" + c.minFrame + ", " + c.maxFrame + ");";
                                }
                                else
                                {
                                    command += "] : (" + c.minFrame + ");";
                                }                       

                            }

                        }

                    }

                    //No shortcut available.
                    if (!shortcutFound) {

                        command = "Cam_Instruction(" + c.cameraCommand + ")[";
                        foreach (byte b in c.parameters)
                        {
                            command += "" + String.Format("0x{0:X}", b) + ", ";
                        }
                        char[] trimChars = { ',', ' ' };
                        command = command.TrimEnd(trimChars);
                        if (c.minFrame != c.maxFrame)
                        {
                            command += "] : (" + c.minFrame + ", " + c.maxFrame + ");";
                        }
                        else
                        {
                            command += "] : (" + c.minFrame + ");";
                        }

                    }

                }

                //Player
                else if (c.instruction < 4)
                {

                    //See if shortcut exists.
                    bool shortcutFound = false;
                    foreach (kuppaChange k in kuppaChanges)
                    {

                        if (k.id == c.parameters[0] && k.isPlayer && !shortcutFound)
                        {

                            shortcutFound = true;
                            command = k.textShortcut;
                            string player = "Mario";
                            switch (c.instruction) {

                                case 1:
                                    player = "Luigi";
                                    break;

                                case 2:
                                    player = "Wario";
                                    break;

                                case 3:
                                    player = "Yoshi";
                                    break;

                            }
                            if (k.paramSizes.Length == 0)
                            {
                                if (c.minFrame != c.maxFrame)
                                {
                                    command += "{" + player + "} : (" + c.minFrame + ", " + c.maxFrame + ");";
                                }
                                else
                                {
                                    command += "{" + player + "} : (" + c.minFrame + ");";
                                }
                            }
                            else
                            {
                                command += "{" + player + "} [";
                                MemoryStream src = new MemoryStream(c.parameters);
                                BinaryReader br = new BinaryReader(src);
                                br.ReadByte();
                                int count = 0;
                                foreach (int size in k.paramSizes)
                                {

                                    long num;
                                    string realNum = "";
                                    switch (size)
                                    {

                                        case 1:
                                            num = br.ReadByte();
                                            break;

                                        case 2:
                                            num = br.ReadInt16();
                                            break;

                                        case 4:
                                            num = br.ReadInt32();
                                            break;

                                        case 8:
                                            num = br.ReadInt64();
                                            break;

                                        default:
                                            throw new Exception("Unsupported Parameter Size!");
                                            break;

                                    }

                                    switch (k.paramFormats[count])
                                    {

                                        case NumberFormats.Decimal:
                                            realNum = num.ToString();
                                            break;

                                        case NumberFormats.Hex:
                                            realNum = IntToHex(num);
                                            break;

                                        case NumberFormats.Bin:
                                            realNum = IntToBinary(num);
                                            break;

                                        case NumberFormats.Input:
                                            realNum = IntToInput(num);
                                            break;

                                    }
                                    command += realNum + ", ";
                                    count += 1;

                                }
                                char[] trimChars = { ',', ' ' };
                                command = command.TrimEnd(trimChars);
                                if (c.minFrame != c.maxFrame)
                                {
                                    command += "] : (" + c.minFrame + ", " + c.maxFrame + ");";
                                }
                                else
                                {
                                    command += "] : (" + c.minFrame + ");";
                                }

                            }

                        }

                    }

                    //No shortcut available.
                    if (!shortcutFound)
                    {

                        command = "Instruction(" + c.instruction + ")[";
                        foreach (byte b in c.parameters)
                        {
                            command += "" + String.Format("0x{0:X}", b) + ", ";
                        }
                        char[] trimChars = { ',', ' ' };
                        command = command.TrimEnd(trimChars);
                        if (c.minFrame != c.maxFrame)
                        {
                            command += "] : (" + c.minFrame + ", " + c.maxFrame + ");";
                        }
                        else
                        {
                            command += "] : (" + c.minFrame + ");";
                        }

                    }

                }

                //Other Type.
                else
                {

                    //See if shortcut exists.
                    bool shortcutFound = false;
                    foreach (kuppaChange k in kuppaChanges)
                    {

                        if (k.id == c.instruction && !k.isCamera && !shortcutFound)
                        {

                            shortcutFound = true;
                            command = k.textShortcut;
                            if (k.paramSizes.Length == 0)
                            {
                                if (c.minFrame != c.maxFrame)
                                {
                                    command += " : (" + c.minFrame + ", " + c.maxFrame + ");";
                                }
                                else
                                {
                                    command += " : (" + c.minFrame + ");";
                                }
                            }
                            else
                            {
                                command += "[";
                                MemoryStream src = new MemoryStream(c.parameters);
                                BinaryReader br = new BinaryReader(src);
                                int count = 0;
                                foreach (int size in k.paramSizes)
                                {

                                    long num;
                                    string realNum = "";
                                    switch (size)
                                    {

                                        case 1:
                                            num = br.ReadByte();
                                            break;

                                        case 2:
                                            num = br.ReadInt16();
                                            break;

                                        case 4:
                                            num = br.ReadInt32();
                                            break;

                                        case 8:
                                            num = br.ReadInt64();
                                            break;

                                        default:
                                            throw new Exception("Unsupported Parameter Size!");
                                            break;

                                    }

                                    switch (k.paramFormats[count])
                                    {

                                        case NumberFormats.Decimal:
                                            realNum = num.ToString();
                                            break;

                                        case NumberFormats.Hex:
                                            realNum = IntToHex(num);
                                            break;

                                        case NumberFormats.Bin:
                                            realNum = IntToBinary(num);
                                            break;

                                        case NumberFormats.Input:
                                            realNum = IntToInput(num);
                                            break;

                                    }
                                    command += realNum + ", ";
                                    count += 1;

                                }
                                char[] trimChars = { ',', ' ' };
                                command = command.TrimEnd(trimChars);
                                if (c.minFrame != c.maxFrame)
                                {
                                    command += "] : (" + c.minFrame + ", " + c.maxFrame + ");";
                                }
                                else
                                {
                                    command += "] : (" + c.minFrame + ");";
                                }

                            }

                        }

                    }

                    //No shortcut available.
                    if (!shortcutFound)
                    {

                        command = "Instruction(" + c.instruction + ")[";
                        foreach (byte b in c.parameters)
                        {
                            command += "" + String.Format("0x{0:X}", b) + ", ";
                        }
                        char[] trimChars = { ',', ' ' };
                        command = command.TrimEnd(trimChars);
                        command += "] : (" + c.minFrame + ", " + c.maxFrame + ");";

                    }

                }

            }

            return command;

        }


        /// <summary>
        /// Generate a command for a binary.
        /// </summary>
        /// <param name="s2"></param>
        /// <returns></returns>
        public CommandBinary.command GenerateCommand(string s) {

            CommandBinary.command c = new CommandBinary.command();
            c.length = 1;

            //Text ID.
            if (s.Contains("End;")) {

                c.length = 0;
                return c;

            }
            string id = s.Split(':')[0];
            if (s.Contains("[")) { id = id.Split('[')[0]; }
            if (id.Contains("(")) { id = id.Split('(')[0]; }
            if (id.Contains("{")) { id = id.Split('{')[0]; }

            //Cam Instruction.
            if (id == "Cam_Instruction")
            {
                c.instruction = 0x4;
                c.cameraCommand = byte.Parse(NumberInterpreter.ParseValue(s.Split('(')[1].Split(')')[0]));

                List<byte> param = new List<byte>();
                string[] paramS = s.Split('[')[1].Split(']')[0].Split(',');
                foreach (string s2 in paramS) {
                    if (s2 != "") param.Add(byte.Parse(NumberInterpreter.ParseValue(s2)));
                }
                c.parameters = param.ToArray();

            }

            //Instruction.
            else if (id == "Instruction")
            {
                c.instruction = byte.Parse(NumberInterpreter.ParseValue(s.Split('(')[1].Split(')')[0]));

                List<byte> param = new List<byte>();
                string[] paramS = s.Split('[')[1].Split(']')[0].Split(',');
                foreach (string s2 in paramS)
                {
                    if (s2 != "") param.Add(byte.Parse(NumberInterpreter.ParseValue(s2)));
                }
                c.parameters = param.ToArray();
            }

            //Custom.
            else
            {
                for (int i = 0; i < kuppaChanges.Count(); i++) {

                    if (id == kuppaChanges[i].textShortcut) {

                        if (kuppaChanges[i].isCamera)
                        {

                            c.instruction = 0x4;
                            c.cameraCommand = (byte)kuppaChanges[i].id;

                        }

                        else if (kuppaChanges[i].isPlayer) {

                            string player = s.Split('{')[1].Split('}')[0];
                            switch (player.ToLower()) {

                                case "0":
                                case "mario":
                                    c.instruction = 0;
                                    break;

                                case "1":
                                case "luigi":
                                    c.instruction = 1;
                                    break;

                                case "2":
                                case "wario":
                                    c.instruction = 2;
                                    break;

                                case "3":
                                case "yoshi":
                                    c.instruction = 3;
                                    break;

                            }

                        }

                        else
                        {
                            c.instruction = (byte)kuppaChanges[i].id;
                        }

                        List<int> param = new List<int>();
                        if (kuppaChanges[i].paramSizes.Length > 0)
                        {
                            string[] paramS = s.Split('[')[1].Split(']')[0].Split(',');
                            foreach (string s2 in paramS)
                            {
                                param.Add(int.Parse(NumberInterpreter.ParseValue(s2)));
                            }
                        }

                        int pos = 0;
                        MemoryStream o = new MemoryStream();
                        BinaryWriter bw = new BinaryWriter(o);
                        foreach (int num in kuppaChanges[i].paramSizes) {

                            switch (num) {

                                case 1:
                                    bw.Write((byte)param[pos]);
                                    pos += 1;
                                    break;

                                case 2:
                                    bw.Write((UInt16)param[pos]);
                                    pos += 1;
                                    break;

                                case 4:
                                    bw.Write((UInt32)param[pos]);
                                    pos += 1;
                                    break;

                            }

                        }
                        c.parameters = o.ToArray();

                        if (kuppaChanges[i].isPlayer) {

                            List<byte> newPar = new List<byte>();
                            newPar.Add((byte)kuppaChanges[i].id);
                            foreach (byte b in o.ToArray()) {
                                newPar.Add(b);
                            }
                            c.parameters = newPar.ToArray();

                        }

                    }

                }
            }

            if (s.Split(':')[1].Split('(')[1].Split(')')[0].Contains(','))
            {
                c.minFrame = Int16.Parse(NumberInterpreter.ParseValue(s.Split(':')[1].Split('(')[1].Split(')')[0].Split(',')[0]));
                c.maxFrame = Int16.Parse(NumberInterpreter.ParseValue(s.Split(':')[1].Split('(')[1].Split(')')[0].Split(',')[1]));
            }
            else {
                c.minFrame = Int16.Parse(NumberInterpreter.ParseValue(s.Split(':')[1].Split('(')[1].Split(')')[0]));
                c.maxFrame = Int16.Parse(NumberInterpreter.ParseValue(s.Split(':')[1].Split('(')[1].Split(')')[0]));
            }
            return c;

        }

    }
}
