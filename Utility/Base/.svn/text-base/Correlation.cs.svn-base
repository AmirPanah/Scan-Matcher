using System;

using ScanMatchers.Math;

namespace ScanMatchers.ScanMatcher.Base
{
    public struct Correlation<T> : IComparable
        where T : Vector2
    {
        public T Point1;
        public T Point2;
        public double MbDist;

        public Correlation(T point1, T point2, double dist)
        {
            this = new Correlation<T>();
            Point1 = point1;
            Point2 = point2;
            MbDist = dist;
        }

        public int CompareTo(Object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return 1;
            }

            Correlation<T> cor = (Correlation<T>)obj;

            double c1 = this.MbDist;
            double c2 = cor.MbDist;
            return -c1.CompareTo(c2);
        }

    }
}
