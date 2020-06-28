using System;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace UvARescue.Math
{

    [Serializable]
    public class Vector
    {

        private int _length;
        private double[] _v;

        public double this[int idx]
        {
            get
            {
                return _v[idx];
            }
            set
            {
                _v[idx] = value;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public static Vector operator -(Vector right)
        {
            return right.Negative();
        }

        public static Vector operator +(Vector left, Vector right)
        {
            return left.Add(right);
        }

        public static Vector operator -(Vector left, Vector right)
        {
            return left.Subtract(right);
        }

        public static Vector operator *(double factor, Vector vector)
        {
            return vector.Multiply(factor);
        }

        public static Vector operator *(Vector vector, double factor)
        {
            return vector.Multiply(factor);
        }

        public static double operator *(Vector left, Vector right)
        {
            return left.Multiply(right);
        }

        public static Vector operator *(Vector left, Matrix right)
        {
            return left.Multiply(right);
        }

        public static Vector operator /(Vector vector, double divisor)
        {
            return vector.Divide(divisor);
        }

        public Vector(Vector source) : this(source.Length)
        {
            int i2;

            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                this[i] = source[i];
                i = checked(i + 1);
                i2 = i1;
            }
        }

        public Vector(int length)
        {
            _length = length;
            _v = new double[checked(checked(length - 1) + 1)];
        }

        public Vector Add(Vector right)
        {
            int i2;

            bool flag = right.Length != Length;
            if (flag)
                throw new InvalidOperationException("Vectors must be of equal length\uFFFD");
            Vector result = new Vector(Length);
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                result[i] = this[i] + right[i];
                i = checked(i + 1);
                i2 = i1;
            }
            return result;
        }

        public Vector Divide(double divisor)
        {
            int i2;

            Vector result = new Vector(Length);
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                result[i] = this[i] / divisor;
                i = checked(i + 1);
                i2 = i1;
            }
            return result;
        }

        public double Multiply(Vector right)
        {
            int i2;

            bool flag = right.Length != Length;
            if (flag)
                throw new InvalidOperationException("Vectors must be of equal length\uFFFD");
            double result = 0.0;
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                result += this[i] * right[i];
                i = checked(i + 1);
                i2 = i1;
            }
            return result;
        }

        public Vector Multiply(Matrix matrix)
        {
            int i3;

            bool flag = Length != matrix.Rows;
            if (flag)
                throw new InvalidOperationException("Dimensions must agree\uFFFD");
            Vector result = new Vector(matrix.Cols);
            int i1 = checked(matrix.Cols - 1);
            int col = 0;
            while (col <= i3)
            {
                double sum = 0.0;
                int i2 = checked(matrix.Rows - 1);
                int row = 0;
                while (row <= i3)
                {
                    sum += this[row] * matrix[row, col];
                    row = checked(row + 1);
                    i3 = i2;
                }
                result[col] = sum;
                col = checked(col + 1);
                i3 = i1;
            }
            return result;
        }

        public Vector Multiply(double factor)
        {
            int i2;

            Vector result = new Vector(Length);
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                result[i] = factor * this[i];
                i = checked(i + 1);
                i2 = i1;
            }
            return result;
        }

        public Vector Negative()
        {
            int i2;

            Vector result = new Vector(Length);
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                result[i] = -this[i];
                i = checked(i + 1);
                i2 = i1;
            }
            return result;
        }

        public Vector Subtract(Vector right)
        {
            int i2;

            bool flag = right.Length != Length;
            if (flag)
                throw new InvalidOperationException("Vectors must be of equal length\uFFFD");
            Vector result = new Vector(Length);
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                result[i] = this[i] - right[i];
                i = checked(i + 1);
                i2 = i1;
            }
            return result;
        }

        public override string ToString()
        {
            int i2;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Vector of size {0}\uFFFD", Length).AppendLine();
            builder.Append(" [ \uFFFD");
            int i1 = checked(Length - 1);
            int i = 0;
            while (i <= i2)
            {
                builder.Append(Conversions.ToString(this[i]) + " \uFFFD");
                i = checked(i + 1);
                i2 = i1;
            }
            builder.Append("]\uFFFD").AppendLine();
            return builder.ToString();
        }

        public static Vector Add(Vector left, Vector right)
        {
            return left.Add(right);
        }

        public static Vector Divide(double divisor, Vector vector)
        {
            return vector.Divide(divisor);
        }

        public static Vector Multiply(double factor, Vector vector)
        {
            return vector.Multiply(factor);
        }

        public static double Multiply(Vector left, Vector right)
        {
            return left.Multiply(right);
        }

        public static Vector Multiply(Vector left, Matrix right)
        {
            return left.Multiply(right);
        }

        public static Vector Negative(Vector vector)
        {
            return vector.Negative();
        }

        public static Vector Ones(int length)
        {
            int i2;

            Vector vector = new Vector(length);
            int i1 = checked(length - 1);
            int i = 0;
            while (i <= i2)
            {
                vector[i] = 1.0;
                i = checked(i + 1);
                i2 = i1;
            }
            return vector;
        }

        public static Vector Subtract(Vector left, Vector right)
        {
            return left.Subtract(right);
        }

        public static Vector Zeros(int length)
        {
            int i2;

            Vector vector = new Vector(length);
            int i1 = checked(length - 1);
            int i = 0;
            while (i <= i2)
            {
                vector[i] = 0.0;
                i = checked(i + 1);
                i2 = i1;
            }
            return vector;
        }

    } // class Vector

}

