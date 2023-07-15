using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Ndst.Graphics {

    // Palette.
    public class Palette {
        public List<RGB5> Colors = new List<RGB5>();
        public int IndexSize = 0x10;

        public void Read(BinaryReader r, int len) {
            Colors = new List<RGB5>();
            for (int i = 0; i < len / 2; i++) {
                Colors.Add(new RGB5(r.ReadUInt16()));
            }
        }

        public void Write(BinaryWriter w) {
            foreach (var c in Colors) {
                w.Write(c.Val);
            }
        }

        // Limit the color palette.
        public static List<RGB5> LimitColorPalette(Image<Argb32> img, int maxColors, Argb32? firstColor, out int[,] newGraphic) {
            
            // Get each pixel.
            List<RGB5> pixels = new List<RGB5>();
            List<RGB5> uniquePixels = new List<RGB5>();
            List<int> reservedIndices = new List<int>();
            int colOffset = 0;
            if (firstColor != null) colOffset = 1;
            for (int i = 0; i < img.Width; i++) {
                for (int j = 0; j < img.Height; j++) {
                    if (firstColor != null && firstColor.Value.Argb == img[i, j].Argb) {
                        reservedIndices.Add(img.Height * i + j);
                    } else {
                        pixels.Add(Argb32ToRGB5(img[i, j]));
                        if (uniquePixels.Where(x => x.Val == pixels.Last().Val).Count() == 0) {
                            uniquePixels.Add(pixels.Last());
                        }
                    }
                }
            }
            int currNum = 0;
            if (uniquePixels.Count <= maxColors - (firstColor == null ? 0 : 1)) {
                newGraphic = new int[img.Width, img.Height];
                for (int i = 0; i < img.Width; i++) {
                    for (int j = 0; j < img.Height; j++) {
                        if (reservedIndices.Contains(img.Height * i + j)) {
                            newGraphic[i, j] = 0;
                        } else {
                            newGraphic[i, j] = FindColor(uniquePixels, Argb32ToRGB5(img[i, j])) + colOffset;
                        }
                    }
                }
                return uniquePixels; // We have enough colors.
            }

            // Shrink the color palette.
            List<int> newIndices;
            List<RGB5> newColors = ShrinkColorPalette(maxColors - (firstColor == null ? 0 : 1), pixels, out newIndices);

            // Recreate the graphic.
            newGraphic = new int[img.Width, img.Height];
            for (int i = 0; i < img.Width; i++) {
                for (int j = 0; j < img.Height; j++) {
                    if (reservedIndices.Contains(img.Height * i + j)) {
                        newGraphic[i, j] = 0;
                    } else {
                        newGraphic[i, j] = newIndices[currNum++] + colOffset;
                    }
                }
            }
            return newColors;

            // Argb32 to RGB5.
            RGB5 Argb32ToRGB5(Argb32 a) {
                return new RGB5() { R8 = a.R, G8 = a.G, B8 = a.B };
            }

            // Find color.
            int FindColor(List<RGB5> colors, RGB5 c) {
                for (int i = 0; i < colors.Count; i++) {
                    if (c.Val == colors[i].Val) {
                        return i;
                    }
                }
                return -1;
            }
            
        }

        // Test limiting a color palette.
        public static void TestLimitColorPalette(Image<Argb32> img, int maxColors, Argb32? firstColor) {

            // Get each pixel.
            List<RGB5> pixels = new List<RGB5>();
            List<RGB5> uniquePixels = new List<RGB5>();
            List<int> reservedIndices = new List<int>();
            for (int i = 0; i < img.Width; i++) {
                for (int j = 0; j < img.Height; j++) {
                    if (firstColor != null && firstColor.Value.Argb == img[i, j].Argb) {
                        reservedIndices.Add(img.Height * i + j);
                    } else {
                        pixels.Add(Argb32ToRGB5(img[i, j]));
                        if (uniquePixels.Where(x => x.Val == pixels.Last().Val).Count() == 0) {
                            uniquePixels.Add(pixels.Last());
                        }
                    }
                }
            }
            if (uniquePixels.Count <= maxColors - (firstColor == null ? 0 : 1)) {
                return; // We have enough colors.
            }

            // Shrink the color palette.
            List<int> newIndices;
            List<RGB5> newPal = ShrinkColorPalette(maxColors - (firstColor == null ? 0 : 1), pixels, out newIndices);

            // Set new pixels in the image.
            int currNum = 0;
            for (int i = 0; i < img.Width; i++) {
                for (int j = 0; j < img.Height; j++) {
                    if (reservedIndices.Contains(img.Height * i + j)) {
                        img[i, j] = firstColor.Value;
                    } else {
                        img[i, j] = RGB5ToArgb32(newPal[newIndices[currNum++]]);
                    }
                }
            }

            // Argb32 to RGB5.
            RGB5 Argb32ToRGB5(Argb32 a) {
                return new RGB5() { R8 = a.R, G8 = a.G, B8 = a.B };
            }

            // RGB5 to Argb32.
            Argb32 RGB5ToArgb32(RGB5 r) {
                return new Argb32(r.R8, r.G8, r.B8);
            }

        }

        // Shrink the color palette.
        public static List<RGB5> ShrinkColorPalette(int numCols, List<RGB5> colors, out List<int> closestIndices) {

            //Run the algorithm.
            int bucketsFilled = 1;

            // We must convert to CIELAB first as RGB sucks at representing what the human eye sees.
            List<CIELAB> cols = new List<CIELAB>();
            foreach (var c in colors) {
                cols.Add(new CIELAB(c));
            }
            List<List<CIELAB>> buckets = new List<List<CIELAB>>();
            buckets.Add(cols);

            // Run until we get the desired number of colors. What we do here is split the bucket with the most variance.
            while (bucketsFilled < numCols) {
                List<CIELAB> largestBucketVar = null;
                double largestVar = double.MinValue;
                foreach (var b in buckets) {
                    double variance = CIELAB.CalculateVariance(b);
                    if (variance >= largestVar) {
                        largestBucketVar = b;
                        largestVar = variance;
                    }
                }
                buckets.Remove(largestBucketVar);
                List<CIELAB> v1, v2;
                CIELAB.SplitBucket(largestBucketVar, out v1, out v2);
                buckets.Add(v1);
                buckets.Add(v2);
                bucketsFilled++;
            }

            // Convert each bucket into RGB5.
            List<RGB5> outputRGB = new List<RGB5>();
            List<CIELAB> outputCIE = new List<CIELAB>();
            foreach (var b in buckets) {
                var avg = CIELAB.AverageColor(b);
                outputCIE.Add(avg);
                outputRGB.Add(avg.ToRGB5());
            }

            // Get closest value for RGB indices.
            closestIndices = new List<int>();
            for (int i = 0; i < cols.Count; i++) {
                closestIndices.Add(cols[i].ClosestColorIndex(outputCIE));
            }

            // Return the final buckets.
            return outputRGB;
            
        }

    }

}