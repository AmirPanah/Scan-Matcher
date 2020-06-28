using System;

namespace ScanMatchers.Math
{

    [Serializable]
    public class Pose2D
    {
        protected const double PI = System.Math.PI;

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
            double radians = Rotation;
            
            while (radians > PI)
                radians -= 2 * PI;
            
            while (radians <= -PI)
                radians += 2 * PI;

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
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}} / {{2:f{1}}}", decimalsInPosition, decimalsInRotation), X / 1000.0, Y / 1000.0, Rotation);
        }

        public string ToString(int decimalsInPosition)
        {
            return String.Format(String.Format("{{0:f{0}}} , {{1:f{0}}}", decimalsInPosition), X / 1000.0, Y / 1000.0);
        }

        public override string ToString()
        {
            return this.ToString(5, 5);
        }

    } // class Pose2D

}

