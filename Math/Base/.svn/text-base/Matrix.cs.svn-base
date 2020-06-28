using System;
using System.Text;

namespace ScanMatchers.Math
{
    [Serializable]
    public class Matrix
    {
        private int _cols;
        private int _rows;
        private double[,] _m;

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

        public Matrix(int size)
            : this(size, size)
        {
        }

        public Matrix(Matrix source)
            : this(source.Rows, source.Cols)
        {
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    this[row, col] = source[row, col];
        }

        public Matrix(int rows, int cols)
        {
            this._rows = rows;
            this._cols = cols;
            this._m = new double[rows, cols];
        }

        public Matrix Add(Matrix right)
        {
            if ((((right.Rows != this.Rows) || (right.Cols != this.Cols)) ? 1 : 0) != 0)
                throw new InvalidOperationException("Matrixes must be of equal dimensions");

            Matrix result = new Matrix(this.Rows, this.Cols);
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    result[row, col] = this[row, col] + right[row, col];

            return result;
        }

        public void LoadIdentity()
        {
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    if (row == col)
                        this[row, col] = 1.0;
                    else
                        this[row, col] = 0.0;
        }

        public Matrix Multiply(Matrix right)
        {
            if (this.Cols != right.Rows)
                throw new InvalidOperationException("Inner Matrix dimensions must agree");

            Matrix result = new Matrix(this.Rows, right.Cols);

            int rowsCount = this.Rows - 1;
            int colsCount = right.Cols - 1;
            int this_ColsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
            {
                for (int col = 0; col <= colsCount; col++)
                {
                    double sum = 0.0;
                    for (int idx = 0; idx <= this_ColsCount; idx++)
                    {
                        sum += this[row, idx] * right[idx, col];
                    }
                    result[row, col] = sum;
                }
            }
            return result;
        }

        public Vector Multiply(Vector vector)
        {
            if (this.Cols != vector.Length)
                throw new InvalidOperationException("Dimensions must agree");

            Vector result = new Vector(this.Rows);

            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
            {
                double sum = 0.0;
                for (int col = 0; col <= colsCount; col++)
                {
                    sum += this[row, col] * vector[col];
                }
                result[row] = sum;
            }
            return result;
        }

        public Matrix Multiply(double factor)
        {
            Matrix result = new Matrix(this.Rows, this.Cols);
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
            {
                for (int col = 0; col <= colsCount; col++)
                {
                    result[row, col] = factor * this[row, col];
                }
            }
            return result;
        }

        public Matrix Negative()
        {
            Matrix result = new Matrix(this.Rows, this.Cols);
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    result[row, col] = -this[row, col];

            return result;
        }

        public Matrix Subtract(Matrix right)
        {
            if ((right.Rows != this.Rows) || (right.Cols != this.Cols))
                throw new InvalidOperationException("Matrixes must be of equal dimensions");

            Matrix result = new Matrix(this.Rows, this.Cols);
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    result[row, col] = this[row, col] - right[row, col];
            
            return result;
        }

        public double Trace()
        {
            if (this.Rows != this.Cols)
                throw new InvalidOperationException("Cannot compute Trace for non-square matrixes");

            double result = 0.0;
            int rowsCount = this.Rows - 1;

            for (int idx = 0; idx <= rowsCount; idx++)
                result += this[idx, idx];
            
            return result;
        }

        public Matrix Transpose()
        {
            Matrix result = new Matrix(this.Cols, this.Rows);
            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    result[col, row] = this[row, col];

            return result;
        }

        public virtual double Determinant()
        {
            throw new NotSupportedException("Determinant computation not supported on Matrix\uFFFD");
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("Matrix of size {0}x{1}", this.Rows, this.Cols).AppendLine();
            builder.Append(" [ ");

            int rowsCount = this.Rows - 1;
            int colsCount = this.Cols - 1;

            for (int i = 0; i <= rowsCount; i++)
            {
                for (int j = 0; j <= colsCount; j++)
                    builder.Append(this[i, j] + " ");
                
                if (i < rowsCount)
                    builder.AppendLine();
                else
                    builder.Append("]").AppendLine();
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
            Matrix matrix = new Matrix(rows, cols);
            int rowsCount = rows - 1;
            int colsCount = cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    matrix[row, col] = 1.0;

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
            Matrix matrix = new Matrix(rows, cols);
            int rowsCount = rows - 1;
            int colsCount = cols - 1;

            for (int row = 0; row <= rowsCount; row++)
                for (int col = 0; col <= colsCount; col++)
                    matrix[row, col] = 0.0;

            return matrix;
        }

        public static Matrix Zeros(int size)
        {
            return Matrix.Zeros(size, size);
        }

    } // class Matrix
}

