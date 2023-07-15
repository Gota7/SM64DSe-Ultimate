using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ndst.Graphics {

    // CIE color space for having distances be closer to human perception.
    public struct CIELAB {
        public double L;
        public double A;
        public double B;

        // Create a CIE color from RGB5.
        public CIELAB(RGB5 rgb) {
            double r = rgb.R5 / (double)RGB5.MAX_COMPONENT_VALUE;
            double g = rgb.G5 / (double)RGB5.MAX_COMPONENT_VALUE;
            double b = rgb.B5 / (double)RGB5.MAX_COMPONENT_VALUE;
            double x, y, z;

            r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
            g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
            b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

            x = (r * 0.4124 + g * 0.3576 + b * 0.1805) / 0.95047;
            y = (r * 0.2126 + g * 0.7152 + b * 0.0722) / 1.00000;
            z = (r * 0.0193 + g * 0.1192 + b * 0.9505) / 1.08883;

            x = (x > 0.008856) ? Math.Pow(x, (double)1/3) : (7.787 * x) + (double)16/116;
            y = (y > 0.008856) ? Math.Pow(y, (double)1/3) : (7.787 * y) + (double)16/116;
            z = (z > 0.008856) ? Math.Pow(z, (double)1/3) : (7.787 * z) + (double)16/116;

            L = 116 * y - 16;
            A = 500 * (x - y);
            B = 200 * (y - z);
        }

        // Convert back to RGB5.
        public RGB5 ToRGB5() {
            double y = (L + 16) / 116;
            double x = A / 500 + y;
            double z = y - B / 200;
            double r, g, b;

            x = 0.95047 * ((x * x * x > 0.008856) ? x * x * x : (x - (double)16/116) / 7.787);
            y = 1.00000 * ((y * y * y > 0.008856) ? y * y * y : (y - (double)16/116) / 7.787);
            z = 1.08883 * ((z * z * z > 0.008856) ? z * z * z : (z - (double)16/116) / 7.787);

            r = x *  3.2406 + y * -1.5372 + z * -0.4986;
            g = x * -0.9689 + y *  1.8758 + z *  0.0415;
            b = x *  0.0557 + y * -0.2040 + z *  1.0570;

            r = (r > 0.0031308) ? (1.055 * Math.Pow(r, (double)1/2.4) - 0.055) : 12.92 * r;
            g = (g > 0.0031308) ? (1.055 * Math.Pow(g, (double)1/2.4) - 0.055) : 12.92 * g;
            b = (b > 0.0031308) ? (1.055 * Math.Pow(b, (double)1/2.4) - 0.055) : 12.92 * b;

            return new RGB5() {
                R5 = (byte)(Math.Max(0, Math.Min(1, r)) * RGB5.MAX_COMPONENT_VALUE),
                G5 = (byte)(Math.Max(0, Math.Min(1, g)) * RGB5.MAX_COMPONENT_VALUE),
                B5 = (byte)(Math.Max(0, Math.Min(1, b)) * RGB5.MAX_COMPONENT_VALUE)
            };

        }

        // Distance squared
        public double DeltaESquared(CIELAB other) {
            var deltaL = L - other.L;
            var deltaA = A - other.A;
            var deltaB = B - other.B;
            var c1 = Math.Sqrt(A * A + B * B);
            var c2 = Math.Sqrt(other.A * other.A + other.B * other.B);
            var deltaC = c1 - c2;
            var deltaH = deltaA * deltaA + deltaB * deltaB - deltaC * deltaC;
            deltaH = deltaH < 0 ? 0 : Math.Sqrt(deltaH);
            var sc = 1.0 + 0.045 * c1;
            var sh = 1.0 + 0.015 * c1;
            var deltaLKlsl = deltaL / (1.0);
            var deltaCkcsc = deltaC / (sc);
            var deltaHkhsh = deltaH / (sh);
            var i = deltaLKlsl * deltaLKlsl + deltaCkcsc * deltaCkcsc + deltaHkhsh * deltaHkhsh;
            return i;
        }

        // Take the average color of a group of CIELab colors.
        public static CIELAB AverageColor(List<CIELAB> colors) {
            CIELAB ret;
            ret.L = colors.Sum(x => x.L) / colors.Count;
            ret.A = colors.Sum(x => x.A) / colors.Count;
            ret.B = colors.Sum(x => x.B) / colors.Count;
            return ret;
        }

        // The variance of a group of CIELab colors.
        public static double CalculateVariance(List<CIELAB> colors) {
            double ret = 0;
            CIELAB avg = AverageColor(colors);
            foreach (var c in colors) {
                ret += c.DeltaESquared(avg); // (x - xBar)^2
            }
            return ret / (colors.Count - 1); // Use sample variance instead of population. Stats!
        }

        // Split a group of colors into best fitting groups.
        // I decided to use the virgin median-cut as I can't figure out how to use the chad Principal Component Analysis.
        public static void SplitBucket(List<CIELAB> colors, out List<CIELAB> v1, out List<CIELAB> v2) {
            double lMin = double.MaxValue;
            double lMax = double.MinValue;
            double aMin = double.MaxValue;
            double aMax = double.MinValue;
            double bMin = double.MaxValue;
            double bMax = double.MinValue;
            foreach (var c in colors) {
                if (c.L > lMax) {
                    lMax = c.L;
                }
                if (c.L < lMin) {
                    lMin = c.L;
                }
                if (c.A > aMax) {
                    aMax = c.A;
                }
                if (c.A < aMin) {
                    aMin = c.A;
                }
                if (c.B > bMax) {
                    bMax = c.B;
                }
                if (c.B < bMin) {
                    bMin = c.B;
                }
            }
            double lRange = lMax - lMin;
            double aRange = aMax - aMin;
            double bRange = bMax - bMin;
            List<CIELAB> bucket = null;
            if (lRange >= aRange && lRange >= bRange) {
                bucket = colors.OrderBy(x => x.L).ToList();
            } else if (aRange >= lRange && aRange >= bRange) {
                bucket = colors.OrderBy(x => x.A).ToList();
            } else {
                bucket = colors.OrderBy(x => x.B).ToList();
            }
            v1 = bucket.GetRange(0, bucket.Count / 2);
            v2 = bucket.GetRange(bucket.Count / 2, bucket.Count - bucket.Count / 2);
        }

        // Get the closest color index.
        public int ClosestColorIndex(List<CIELAB> colors) {
            double leastDist = double.MaxValue;
            int leastIndex = -1;
            for (int i = 0; i < colors.Count; i++) {
                double dist = DeltaESquared(colors[i]);
                if (dist < leastDist) {
                    leastDist = dist;
                    leastIndex = i;
                }
            }
            return leastIndex;
        }

    }

}