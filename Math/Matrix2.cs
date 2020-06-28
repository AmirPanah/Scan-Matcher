using System;

namespace ScanMatchers.Math
{
    [Serializable]
    public class Matrix2 : Matrix
    {

        public static Matrix2 operator -(Matrix2 right)
        {
            return new Matrix2(right.Negative());
        }

        public static Matrix2 operator +(Matrix2 left, Matrix2 right)
        {
            return new Matrix2(left.Add(right));
        }

        public static Matrix2 operator -(Matrix2 left, Matrix2 right)
        {
            return new Matrix2(left.Subtract(right));
        }

        public static Matrix2 operator *(double factor, Matrix2 matrix)
        {
            return new Matrix2(matrix.Multiply(factor));
        }

        public static Matrix2 operator *(Matrix2 matrix, double factor)
        {
            return new Matrix2(matrix.Multiply(factor));
        }

        public static Matrix2 operator *(Matrix2 left, Matrix2 right)
        {
            return new Matrix2(left.Multiply(right));
        }

        public static Vector2 operator *(Matrix2 matrix, Vector2 vector)
        {
            return new Vector2(matrix.Multiply(vector));
        }

        public Matrix2(Matrix source) : base(source)
        {
        }

        public Matrix2() : base(2)
        {
        }

        public Matrix2(double v00, double v01, double v10, double v11) : base(2)
        {
            base[0, 0] = v00;
            base[0, 1] = v01;
            base[1, 0] = v10;
            base[1, 1] = v11;
        }

        public Matrix2 Invert()
        {
            Matrix2 answer = new Matrix2();
            double det = Determinant();
            answer[0, 0] = 1.0 / det * base[1, 1];
            answer[0, 1] = 1.0 / det * -base[0, 1];
            answer[1, 0] = 1.0 / det * -base[1, 0];
            answer[1, 1] = 1.0 / det * base[0, 0];
            return answer;
        }

        public new Matrix2 Transpose()
        {
            return new Matrix2(base.Transpose());
        }

        public override double Determinant()
        {
            return (base[0, 0] * base[1, 1]) - (base[0, 1] * base[1, 0]);
        }

    } // class Matrix2
}

