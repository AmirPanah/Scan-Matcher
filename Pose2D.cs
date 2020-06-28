using System;

namespace UvARescue.Math
{

    [Serializable]
    public class Pose2D
    {

        private Vector2 _Position;
        private double _Rotation;

        public Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
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
                _Rotation = value;
            }
        }

        public double X
        {
            get
            {
                return _Position.X;
            }
            set
            {
                _Position.X = value;
            }
        }

        public double Y
        {
            get
            {
                return _Position.Y;
            }
            set
            {
                _Position.Y = value;
            }
        }

        public Pose2D() : this(new Vector2(0.0, 0.0), 0.0)
        {
        }

        public Pose2D(double rotation) : this(new Vector2(0.0, 0.0), rotation)
        {
        }

        public Pose2D(double x, double y, double rotation) : this(new Vector2(x, y), rotation)
        {
        }

        public Pose2D(Vector2 position, double rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public double GetNormalizedRotation()
        {
            bool flag;

            double radians = Rotation;
            while (flag)
            {
                radians -= 6.28318530717959;
                flag = radians > 3.14159265358979;
            }
            while (flag)
            {
                radians += 6.28318530717959;
                flag = radians <= -3.14159265358979;
            }
            return radians;
        }

        public Pose2D ToGlobal(Pose2D currentOrigin)
        {
            TMatrix2D rotmx = currentOrigin.ToGlobalMatrix();
            return rotmx * this;
        }

        public TMatrix2D ToGlobalMatrix()
        {
            return new TMatrix2D(Position, Rotation);
        }

        public Pose2D ToLocal(Pose2D targetOrigin)
        {
            TMatrix2D rotmx = targetOrigin.ToLocalMatrix();
            return rotmx * this;
        }

        public TMatrix2D ToLocalMatrix()
        {
            TMatrix2D mx = new TMatrix2D(-Rotation);
            Pose2D merotated = mx * this;
            mx.Translation = -merotated.Position;
            return mx;
        }

        public string ToString(int decimalsInPosition, int decimalsInRotation)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}} / {{2:f{1}}}\uFFFD", decimalsInPosition, decimalsInRotation), X / 1000.0, Y / 1000.0, Rotation);
        }

        public string ToString(int decimalsInPosition)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}}\uFFFD", decimalsInPosition), X / 1000.0, Y / 1000.0);
        }

        public override string ToString()
        {
            return this.ToString(5, 5);
        }

    } // class Pose2D

}

