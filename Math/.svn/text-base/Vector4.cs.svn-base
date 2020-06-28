using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanMatchers.Math
{

    [Serializable]
    public class Vector4: Vector
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

        public double R
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

        public double T
        {
            get
            {
                return base[3];
            }
            set
            {
                base[3] = value;
            }
        }

        public Vector4(Vector source) : base(source)
        {
        }

        public Vector4() : base(4)
        {
        }

        public Vector4(double x, double y, double r, double t)
            : base(4)
        {
            base[0] = x;
            base[1] = y;
            base[2] = r;
            base[3] = t;
        }

    } // class Vector4

}
