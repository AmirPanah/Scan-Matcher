using System;
using System.Text;

namespace ScanMatchers.Math
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

        public Vector(int length)
        {
            _length = length;
            _v = new double[length];
        }

        public Vector(Vector source)
            : this(source.Length)
        {
            int len = this.Length - 1;

            for (int i = 0; i <= len; i++)
                this[i] = source[i];
        }

        public Vector Add(Vector right)
        {
            if (right.Length != this.Length)
                throw new InvalidOperationException("Vectors must be of equal length");

            Vector result = new Vector(this.Length);

            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
                result[i] = this[i] + right[i];

            return result;
        }

        public Vector Divide(double divisor)
        {
            Vector result = new Vector(this.Length);
            
            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
                result[i] = this[i] / divisor;

            return result;
        }

        public double Multiply(Vector right)
        {
            if (right.Length != this.Length)
                throw new InvalidOperationException("Vectors must be of equal length");

            double result = 0.0;
            
            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
                result += this[i] * right[i];

            return result;
        }

        public Vector Multiply(Matrix matrix)
        {
            if (this.Length != matrix.Rows)
                throw new InvalidOperationException("Dimensions must agree");

            Vector result = new Vector(matrix.Cols);

            int colsCount = matrix.Cols - 1;
            int rowsCount = matrix.Rows - 1;

            for (int col = 0; col <= colsCount; col++)
            {
                double sum = 0.0;
                
                for (int row = 0; row <= rowsCount; row++)
                    sum += this[row] * matrix[row, col];
            
                result[col] = sum;
            }
            return result;
        }

        public Vector Multiply(double factor)
        {
            Vector result = new Vector(this.Length);
            
            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
                result[i] = factor * this[i];

            return result;
        }

        public Vector Negative()
        {
            Vector result = new Vector(this.Length);

            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
            {
                result[i] = -this[i];
            }

            return result;
        }

        public Vector Subtract(Vector right)
        {
            if (right.Length != this.Length)
                throw new InvalidOperationException("Vectors must be of equal length");

            Vector result = new Vector(this.Length);

            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
                result[i] = this[i] - right[i];

            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            
            builder.AppendFormat("Vector of size {0}", this.Length).AppendLine();
            builder.Append(" [ ");

            int len = this.Length - 1;
            for (int i = 0; i <= len; i++)
                builder.Append(this[i] + " ");

            builder.Append("]").AppendLine();
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

        public static Vector Subtract(Vector left, Vector right)
        {
            return left.Subtract(right);
        }

        public static Vector Ones(int length)
        {
            Vector vector = new Vector(length);

            int len = length - 1;
            for (int i = 0; i <= len; i++)
                vector[i] = 1.0;

            return vector;
        }

        public static Vector Zeros(int length)
        {
            Vector vector = new Vector(length);

            int len = length - 1;
            for (int i = 0; i <= len; i++)
                vector[i] = 0.0;

            return vector;
        }

    } // class Vector

}

