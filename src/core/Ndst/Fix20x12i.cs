using System;

namespace Ndst {

    // Fixed point number that has 20 bits in the integer part, and 12 in the fractional part.
    public struct Fix20x12i {
        public int Val;

        public double AsDouble {
            get => (double)Val / 0x1000;
            set { Val = (int)(Val * 0x1000 + 0.5); }
        }

        public static implicit operator Fix20x12i(double value) => new Fix20x12i() { AsDouble = value };
        public static implicit operator double(Fix20x12i value) => value.AsDouble;

    }

}