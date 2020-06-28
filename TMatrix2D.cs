using System;

namespace UvARescue.Math
{

    [Serializable]
    public class TMatrix2D : Matrix3
    {

        private double _Rotation;

        public double DeltaX
        {
            get
            {
                return base[0, 2];
            }
            set
            {
                base[0, 2] = value;
            }
        }

        public double DeltaY
        {
            get
            {
                return base[1, 2];
            }
            set
            {
                base[1, 2] = value;
            }
        }

        public double Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                bool flag = value != _Rotation;
                if (flag)
                {
                    _Rotation = value;
                    double s = Math.Sin(value);
                    double c = Math.Cos(value);
                    base[0, 0] = c;
                    base[0, 1] = -s;
                    base[1, 0] = s;
                    base[1, 1] = c;
                }
            }
        }

        public Vector2 Translation
        {
            get
            {
                return new Vector2(DeltaX, DeltaY);
            }
            set
            {
                DeltaX = value.X;
                DeltaY = value.Y;
            }
        }

        public static TMatrix2D operator -(TMatrix2D right)
        {
            return new TMatrix2D(-right.DeltaX, -right.DeltaY, -right.Rotation);
        }

        public static Vector3 operator *(TMatrix2D matrix, Vector3 vector)
        {
            return new Vector3(matrix.Multiply(vector));
        }

        public static Vector2 operator *(TMatrix2D matrix, Vector2 vector)
        {
            return matrix.Multiply(vector);
        }

        public static Pose2D operator *(TMatrix2D matrix, Pose2D pose)
        {
            Vector3 vector = new Vector3(pose.X, pose.Y, 1.0);
            vector = matrix * vector;
            return new Pose2D(vector.X, vector.Y, pose.Rotation + matrix.Rotation);
        }

        public TMatrix2D() : this(0.0, 0.0, 0.0)
        {
        }

        public TMatrix2D(double rotation) : this(0.0, 0.0, rotation)
        {
        }

        public TMatrix2D(Vector2 translation) : this(translation.X, translation.Y, 0.0)
        {
        }

        public TMatrix2D(Vector2 translation, double rotation) : this(translation.X, translation.Y, rotation)
        {
        }

        public TMatrix2D(double dx, double dy) : this(dx, dy, 0.0)
        {
        }

        public TMatrix2D(double dx, double dy, double rotation)
        {
            LoadIdentity();
            DeltaX = dx;
            DeltaY = dy;
            Rotation = rotation;
        }

        public Vector2 Multiply(Vector2 vector)
        {
            Vector2 result = new Vector2();
            result.X = (vector.X * base[0, 0]) + (vector.Y * base[0, 1]) + base[0, 2];
            result.Y = (vector.X * base[1, 0]) + (vector.Y * base[1, 1]) + base[1, 2];
            return result;
        }

        public void Rotate(double angle)
        {
            Rotation += angle;
        }

        public void Translate(Vector2 translation)
        {
            Translate(translation.X, translation.Y);
        }

        public void Translate(double x, double y)
        {
            DeltaX += x;
            DeltaY += y;
        }

    } // class TMatrix2D

}

