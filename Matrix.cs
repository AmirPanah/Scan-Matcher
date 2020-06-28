using System;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace UvARescue.Math
{

    [Serializable]
    public class Matrix
    {

        private int _cols;
        private double[,] _m;
        private int _rows;

        public double this[int row, int col]
        {
            get
            {
                return _m[row, col];
            }
            set
            {
                _m[row, col] = value;
            }
        }

        public int Cols
        {
            get
            {
                return _cols;
            }
        }

        public int Rows
        {
            get
            {
                return _rows;
            }
        }

        public static Matrix operator -(Matrix right)
        {
            return right.Negative();
        }

        public static Matrix operator +(Matrix left, Matrix right)
        {
            return left.Add(right);
        }

        public static Matrix operator -(Matrix left, Matrix right)
        {
            return left.Subtract(right);
        }

        public static Matrix operator *(double factor, Matrix matrix)
        {
            return matrix.Multiply(factor);
        }

        public static Matrix operator *(Matrix matrix, double factor)
        {
            return matrix.Multiply(factor);
        }

        public static Vector operator *(Matrix matrix, Vector vector)
        {
            return matrix.Multiply(vector);
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            return left.Multiply(right);
        }

        public Matrix(Matrix source) : this(source.Rows, source.Cols)
        {
            int i3;

            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    this[row, col] = source[row, col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
        }

        public Matrix(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _m = new double[checked(checked(rows - 1) + 1), checked(checked(cols - 1) + 1)];
        }

        public Matrix(int size) : this(size, size)
        {
        }

        public Matrix Add(Matrix right)
        {
            int i3;

            bool flag = (right.Rows == Rows) && (right.Cols == Cols) ? true : false;
            if (flag)
                throw new InvalidOperationException("Matrixes must be of equal dimensions\uFFFD");
            Matrix result = new Matrix(Rows, Cols);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    result[row, col] = this[row, col] + right[row, col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return result;
        }

        public void LoadIdentity()
        {
            int i3;

            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    bool flag = row == col;
                    if (flag)
                        this[row, col] = 1.0;
                    else
                        this[row, col] = 0.0;
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
        }

        public Matrix Multiply(Matrix right)
        {
            int i4;

            bool flag = Cols != right.Rows;
            if (flag)
                throw new InvalidOperationException("Inner Matrix dimensions must agree\uFFFD");
            Matrix result = new Matrix(Rows, right.Cols);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i4)
            {
                int i2 = checked(right.Cols - 1);
                int col = 0;
                while (col <= i4)
                {
                    double sum = 0.0;
                    int i3 = checked(Cols - 1);
                    int idx = 0;
                    while (idx <= i4)
                    {
                        sum += this[row, idx] * right[idx, col];
                        idx = checked(idx + 1);
                        i4 = i3;
                    }
                    result[row, col] = sum;
                    col = checked(col + 1);
                    i4 = i2;
                }
                row = checked(row + 1);
                i4 = i1;
            }
            return result;
        }

        public Vector Multiply(Vector vector)
        {
            int i3;

            bool flag = Cols != vector.Length;
            if (flag)
                throw new InvalidOperationException("Dimensions must agree\uFFFD");
            Vector result = new Vector(Rows);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                double sum = 0.0;
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    sum += this[row, col] * vector[col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                result[row] = sum;
                row = checked(row + 1);
                i3 = i1;
            }
            return result;
        }

        public Matrix Multiply(double factor)
        {
            int i3;

            Matrix result = new Matrix(Rows, Cols);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    result[row, col] = factor * this[row, col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return result;
        }

        public Matrix Negative()
        {
            int i3;

            Matrix result = new Matrix(Rows, Cols);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    result[row, col] = -this[row, col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return result;
        }

        public Matrix Subtract(Matrix right)
        {
            int i3;

            bool flag = (right.Rows == Rows) && (right.Cols == Cols) ? true : false;
            if (flag)
                throw new InvalidOperationException("Matrixes must be of equal dimensions\uFFFD");
            Matrix result = new Matrix(Rows, Cols);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    result[row, col] = this[row, col] - right[row, col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return result;
        }

        public double Trace()
        {
            int i2;

            bool flag = Rows != Cols;
            if (flag)
                throw new InvalidOperationException("Cannot compute Trace for non-square matrixes\uFFFD");
            double result = 0.0;
            int i1 = checked(Rows - 1);
            int idx = 0;
            while (idx <= i2)
            {
                result += this[idx, idx];
                idx = checked(idx + 1);
                i2 = i1;
            }
            return result;
        }

        public Matrix Transpose()
        {
            int i3;

            Matrix result = new Matrix(Cols, Rows);
            int i1 = checked(Rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(Cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    result[col, row] = this[row, col];
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return result;
        }

        public virtual double Determinant()
        {
            double Determinant;

            throw new NotSupportedException("Determinant computation not supported on Matrix\uFFFD");
        }

        public override string ToString()
        {
            int i3;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Matrix of size {0}x{1}\uFFFD", Rows, Cols).AppendLine();
            builder.Append(" [ \uFFFD");
            int i1 = checked(Rows - 1);
            int i = 0;
            while (i <= i3)
            {
                int i2 = checked(Cols - 1);
                int j = 0;
                while (j <= i3)
                {
                    builder.Append(Conversions.ToString(this[i, j]) + " \uFFFD");
                    j = checked(j + 1);
                    i3 = i2;
                }
                bool flag = i < checked(Rows - 1);
                if (flag)
                    builder.AppendLine();
                else
                    builder.Append("]\uFFFD").AppendLine();
                i = checked(i + 1);
                i3 = i1;
            }
            return builder.ToString();
        }

        public static Matrix Add(Matrix left, Matrix right)
        {
            return left.Add(right);
        }

        public static double Determinant(Matrix matrix)
        {
            return matrix.Determinant();
        }

        public static Matrix Identity(int size)
        {
            Matrix matrix = new Matrix(size, size);
            matrix.LoadIdentity();
            return matrix;
        }

        public static Matrix Multiply(Matrix left, Matrix right)
        {
            return left.Multiply(right);
        }

        public static Vector Multiply(Matrix left, Vector right)
        {
            return left.Multiply(right);
        }

        public static Matrix Multiply(double factor, Matrix matrix)
        {
            return matrix.Multiply(factor);
        }

        public static Matrix Negative(Matrix matrix)
        {
            return matrix.Negative();
        }

        public static Matrix Ones(int size)
        {
            return Matrix.Ones(size, size);
        }

        public static Matrix Ones(int rows, int cols)
        {
            int i3;

            Matrix matrix = new Matrix(rows, cols);
            int i1 = checked(rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    matrix[row, col] = 1.0;
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return matrix;
        }

        public static Matrix Subtract(Matrix left, Matrix right)
        {
            return left.Subtract(right);
        }

        public static double Trace(Matrix matrix)
        {
            return matrix.Trace();
        }

        public static Matrix Transpose(Matrix matrix)
        {
            return matrix.Transpose();
        }

        public static Matrix Zeros(int rows, int cols)
        {
            int i3;

            Matrix matrix = new Matrix(rows, cols);
            int i1 = checked(rows - 1);
            int row = 0;
            while (row <= i3)
            {
                int i2 = checked(cols - 1);
                int col = 0;
                while (col <= i3)
                {
                    matrix[row, col] = 0.0;
                    col = checked(col + 1);
                    i3 = i2;
                }
                row = checked(row + 1);
                i3 = i1;
            }
            return matrix;
        }

        public static Matrix Zeros(int size)
        {
            return Matrix.Zeros(size, size);
        }

    } // class Matrix

}

