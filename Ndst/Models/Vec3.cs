using System;

namespace Ndst.Models {

    // 3D Vector.
    public struct Vec3 {
        public float X;
        public float Y;
        public float Z;

        public float MagnitudeSquared => X * X + Y * Y + Z * Z;
        public float Magnitude => (float)Math.Sqrt(MagnitudeSquared);

        public Vec3(float val) {
            X = Y = Z = val;
        }

        public Vec3(float x, float y, float z) {
            X = x;
            Y = y;
            Z = z;
        }

        // Add two vectors.
        public static Vec3 operator +(Vec3 a, Vec3 b) {
            return new Vec3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        // Sub two vectors.
        public static Vec3 operator -(Vec3 a, Vec3 b) {
            return new Vec3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        // Dot two vectors.
        public static float operator *(Vec3 a, Vec3 b) {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        // Scale a vector.
        public static Vec3 operator *(Vec3 a, float b) {
            return new Vec3(a.X * b, a.Y * b, a.Z * b);
        }

        // Scale a vector.
        public static Vec3 operator *(float b, Vec3 a) {
            return new Vec3(a.X * b, a.Y * b, a.Z * b);
        }

        // Scale a vector.
        public static Vec3 operator /(Vec3 a, float b) {
            return new Vec3(a.X / b, a.Y / b, a.Z / b);
        }

        // Scale a vector.
        public static Vec3 operator /(float b, Vec3 a) {
            return new Vec3(b / a.X, b / a.Y, b / a.Z);
        }

        // Cross a vector.
        public static Vec3 operator %(Vec3 a, Vec3 b) {
            float x = a.Y * b.Z - a.Z * b.Y;
            float y = -(a.X * b.Z - a.Z * b.X);
            float z = a.X * b.Y - a.Y * b.X;
            return new Vec3(x, y, z);
        }

        // Normalize.
        public Vec3 Normalize() {
            return this / Magnitude;
        }

    }

}