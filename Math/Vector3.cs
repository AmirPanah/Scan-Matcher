using System;

namespace ScanMatchers.Math
{

    [Serializable]
    public class Vector3 : Vector
    {

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

        public double Z
        {
            get
            {
                return base[2];
            }
            set
            {
                base[2] = value;
            }
        }

        public static Vector3 operator -(Vector3 right)
        {
            return new Vector3(right.Negative());
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.Add(right));
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.Subtract(right));
        }

        public static Vector3 operator *(double factor, Vector3 vector)
        {
            return new Vector3(vector.Multiply(factor));
        }

        public static Vector3 operator *(Vector3 vector, double factor)
        {
            return new Vector3(vector.Multiply(factor));
        }

        public static double operator *(Vector3 left, Vector3 right)
        {
            return left.Multiply(right);
        }

        public static Vector3 operator *(Vector3 left, Matrix3 right)
        {
            return new Vector3(left.Multiply(right));
        }

        public Vector3(Vector source) : base(source)
        {
        }

        public Vector3() : base(3)
        {
        }

        public Vector3(double x, double y, double z)
            : base(3)
        {
            base[0] = x;
            base[1] = y;
            base[2] = z;
        }

    } // class Vector3

}

