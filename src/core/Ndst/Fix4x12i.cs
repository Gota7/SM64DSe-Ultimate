using System;

namespace Ndst {

    // Fixed point number that has 20 bits in the integer part, and 12 in the fractional part.
    public struct Fix4x12i {
        public short Val;

        public double AsDouble {
            get => (double)Val / 0x1000;
            set { Val = (short)(Val * 0x1000 + 0.5); }
        }

        public static implicit operator Fix4x12i(double value) => new Fix4x12i() { AsDouble = value };
        public static implicit operator double(Fix4x12i value) => value.AsDouble;

    }

}