using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SM64DS_SCRIPT_LIB
{
    public static class NumberInterpreter
    {

        /// <summary>
        /// Convert int to hex.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToHex(Int64 value) {

            return String.Format("0x{0:X}", value);

        }


        /// <summary>
        /// Convert int to binary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToBinary(Int64 value)
        {

            return "0b" + Convert.ToString(value, 2);

        }


        /// <summary>
        /// Convert int to input.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string IntToInput(Int64 value)
        {

            string o = "";

            if ((value & Input.A) == Input.A) { o += "A"; }
            if ((value & Input.B) == Input.B) { o += "B"; }
            if ((value & Input.SELECT) == Input.SELECT) { o += "^"; }
            if ((value & Input.START) == Input.START) { o += "S"; }
            if ((value & Input.RIGHT) == Input.RIGHT) { o += ">"; }
            if ((value & Input.LEFT) == Input.LEFT) { o += "<"; }
            if ((value & Input.UP) == Input.UP) { o += "U"; }
            if ((value & Input.DOWN) == Input.DOWN) { o += "D"; }
            if ((value & Input.CAM_RIGHT) == Input.CAM_RIGHT) { o += "+"; }
            if ((value & Input.CAM_LEFT) == Input.CAM_LEFT) { o += "-"; }
            if ((value & Input.R) == Input.R) { o += "R"; }
            if ((value & Input.Y) == Input.Y) { o += "Y"; }
            if ((value & Input.L) == Input.L) { o += "L"; }
            if ((value & Input.X) == Input.X) { o += "X"; }

            if (o.Length < 1) { return value.ToString(); }

            return o;

        }


        /// <summary>
        /// Convert hex to int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string HexToInt(string value)
        {

            // strip the leading 0x
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }
            return Int64.Parse(value, NumberStyles.HexNumber).ToString();

        }


        /// <summary>
        /// Convert binary to int.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string BinaryToInt(string value)
        {

            // strip the leading 0x
            if (value.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Substring(2);
            }
            return Convert.ToInt64(value, 2).ToString();

        }


        /// <summary>
        /// Parse a string as a number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ParseValue(string value) {

            if (value.StartsWith("0x")) { return HexToInt(value); }
            if (value.StartsWith("0b")) { return BinaryToInt(value); }
            int x;
            if (!int.TryParse(value, out x)) { return InputToInt(value); };

            return value;

        }


        /// <summary>
        /// Inputs to int.
        /// </summary>
        /// <returns>The to int.</returns>
        /// <param name="value">Value.</param>
        public static string InputToInt(string value) {

            value = value.ToUpper();
            int val = 0;
            if (value.Contains("A")) { val += Input.A; }
            if (value.Contains("B")) { val += Input.B; }
            if (value.Contains("^")) { val += Input.SELECT; }
            if (value.Contains("S")) { val += Input.START; }
            if (value.Contains(">")) { val += Input.RIGHT; }
            if (value.Contains("<")) { val += Input.LEFT; }
            if (value.Contains("U")) { val += Input.UP; }
            if (value.Contains("D")) { val += Input.DOWN; }
            if (value.Contains("+")) { val += Input.CAM_RIGHT; }
            if (value.Contains("-")) { val += Input.CAM_LEFT; }
            if (value.Contains("R")) { val += Input.R; }
            if (value.Contains("Y")) { val += Input.Y; }
            if (value.Contains("L")) { val += Input.L; }
            if (value.Contains("X")) { val += Input.X; }

            foreach (char c in value)
            {
                if (!(c == 'A' || c == 'B' || c == '^' || c == 'S' || c == '>' || c == '<' || c == 'U' || c == 'D' || c == '+' || c == '-' || c == 'R' || c == 'Y' || c == 'L' || c == 'X')) { throw new Exception(); }
            }
            return val.ToString();

        }


        /// <summary>
        /// Input buttons.
        /// </summary>
        public static class Input
        {

            public const int A = 1 << 0;         //A
            public const int B = 1 << 1;         //B
            public const int SELECT = 1 << 2;    //^
            public const int START = 1 << 3;     //S
            public const int RIGHT = 1 << 4;     //>
            public const int LEFT = 1 << 5;      //<
            public const int UP = 1 << 6;        //U
            public const int DOWN = 1 << 7;      //D
            public const int CAM_RIGHT = 1 << 8; //+
            public const int CAM_LEFT = 1 << 9;  //-
            public const int R = 1 << 10;        //R
            public const int Y = 1 << 11;        //Y
            public const int L = 1 << 14;        //L
            public const int X = 1 << 15;        //X
        }
         
        /// <summary>
        /// Number formats.
        /// </summary>
        public enum NumberFormats {

            Decimal, Hex, Bin, Input

        }

    }
}
