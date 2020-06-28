using System;

namespace ScanMatchers.Math
{

    [Serializable]
    public class Vector2 : Vector
    {
        public static Vector2 Zero = new Vector2();

        public double X
        {
            get
            {
                return base[0];
            }
            set
            {
                base[0] = value;
            }
        }

        public double Y
        {
            get
            {
                return base[1];
            }
            set
            {
                base[1] = value;
            }
        }

        public static Vector2 operator -(Vector2 right)
        {
            return new Vector2(right.Negative());
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.Add(right));
        }

        public static Vector2 operator -(Vector2 left, Vector2 right)
        {
            return new Vector2(left.Subtract(right));
        }

        public static Vector2 operator *(double factor, Vector2 vector)
        {
            return new Vector2(vector.Multiply(factor));
        }

        public static Vector2 operator *(Vector2 vector, double factor)
        {
            return new Vector2(vector.Multiply(factor));
        }

        public static double operator *(Vector2 left, Vector2 right)
        {
            return left.Multiply(right);
        }

        public static Vector2 operator *(Vector2 left, Matrix2 right)
        {
            return new Vector2(left.Multiply(right));
        }

        public static Vector2 operator /(Vector2 vector, double divisor)
        {
            return new Vector2(vector.Divide(divisor));
        }

        public Vector2(Vector source) : base(source)
        {
        }

        public Vector2() : base(2)
        {
        }

        public Vector2(double x, double y)
            : base(2)
        {
            base[0] = x;
            base[1] = y;
        }

    } // class Vector2

}

