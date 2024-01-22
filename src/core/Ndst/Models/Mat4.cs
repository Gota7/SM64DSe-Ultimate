namespace Ndst.Models {

    // A 4x4 Matrix.
    public unsafe struct Mat4 {
        fixed float Values[16];

        // Identity matrix.
        public static Mat4 Identity {
            get {
                Mat4 ret;
                ret[0, 0] = 1;
                ret[1, 1] = 1;
                ret[2, 2] = 1;
                ret[3, 3] = 1;
                return ret;
            }
        }

        // 1-Dimensional indexer.
        public float this[int i] {
            get => Values[i];
            set { Values[i] = value; }
        } 

        // 2-Dimensional indexer. Values are stored in columns, left to right. These indices are like X, Y or N, M.
        public float this[int i, int j] {
            get => Values[i * 4 + j];
            set { Values[i * 4 + j] = value; }
        }

        public override string ToString() => this[0, 0].ToString() + ", " + this[1, 0].ToString() + ", " + this[2, 0].ToString() + ", " + this[3, 0].ToString()
                                    + "\n" + this[0, 1].ToString() + ", " + this[1, 1].ToString() + ", " + this[2, 1].ToString() + ", " + this[3, 1].ToString()
                                    + "\n" + this[0, 2].ToString() + ", " + this[1, 2].ToString() + ", " + this[2, 2].ToString() + ", " + this[3, 2].ToString()
                                    + "\n" + this[0, 3].ToString() + ", " + this[1, 3].ToString() + ", " + this[2, 3].ToString() + ", " + this[3, 3].ToString();

        // Add two matrices.
        public static Mat4 operator +(Mat4 a, Mat4 b) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = a[i, j] + b[i, j];
                }
            }
            return ret;
        }

        // Sub two matrices.
        public static Mat4 operator -(Mat4 a, Mat4 b) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = a[i, j] - b[i, j];
                }
            }
            return ret;
        }

        // Scale a matrix.
        public static Mat4 operator *(Mat4 a, float b) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = a[i, j] * b;
                }
            }
            return ret;
        }

        // Scale a matrix.
        public static Mat4 operator *(float b, Mat4 a) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = a[i, j] * b;
                }
            }
            return ret;
        }

        // Scale a matrix.
        public static Mat4 operator /(Mat4 a, float b) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = a[i, j] / b;
                }
            }
            return ret;
        }
        
        // Scale a matrix.
        public static Mat4 operator /(float b, Mat4 a) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = b / a[i, j];
                }
            }
            return ret;
        }

        // Multiply a matrix.
        public static Mat4 operator *(Mat4 a, Mat4 b) {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = a[i, 0] * b[0, j] + a[i, 1] * b[1, j] + a[i, 2] * b[2, j] + a[i, 3] * b[3, j];
                }
            }
            return ret;
        }

        // Transpose a matrix.
        public Mat4 Transpose() {
            Mat4 ret;
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    ret[i, j] = this[j, i];
                }
            }
            return ret;
        }

    }

}