using System;

namespace ScanMatchers.Math
{
    [Serializable]
    public class Matrix3 : Matrix
    {

        public static Matrix3 operator -(Matrix3 right)
        {
            return new Matrix3(right.Negative());
        }

        public static Matrix3 operator +(Matrix3 left, Matrix3 right)
        {
            return new Matrix3(left.Add(right));
        }

        public static Matrix3 operator -(Matrix3 left, Matrix3 right)
        {
            return new Matrix3(left.Subtract(right));
        }

        public static Matrix3 operator *(double factor, Matrix3 matrix)
        {
            return new Matrix3(matrix.Multiply(factor));
        }

        public static Matrix3 operator *(Matrix3 matrix, double factor)
        {
            return new Matrix3(matrix.Multiply(factor));
        }

        public static Matrix3 operator *(Matrix3 left, Matrix3 right)
        {
            return new Matrix3(left.Multiply(right));
        }

        public static Vector3 operator *(Matrix3 matrix, Vector3 vector)
        {
            return new Vector3(matrix.Multiply(vector));
        }

        public Matrix3(Matrix source) : base(source)
        {
        }

        public Matrix3() : base(3)
        {
        }

    } // class Matrix3
}

