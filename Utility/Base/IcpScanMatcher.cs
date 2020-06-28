using System;
using System.Collections.Generic;

using ScanMatchers.Math;
using ScanMatchers.Tools.Structures;
using ScanMatchers.Tools.USARSimMessages;

namespace ScanMatchers.ScanMatcher.Base
{
    public abstract class IcpScanMatcher : IScanMatchers
    {
        public const double PI_2 = 1.5707963267948966;

        // Fields
        protected float MAX_CORRELATIONDISTANCE = 2000f;
        protected int MAX_ITERATIONS = 80;
        protected int RESULTS_CONSIDERED = 10;
        protected int RESULTS_CONVERGED = 8;
        protected double RESULTS_MAXERROR = 0.007;

        protected static float MATCH_MAXTRANSLATION = 100; //mm
        protected static float MATCH_MAXROTATION = 4; //degrees

        protected static float DELTA_MAXTRANSLATION = 0.1f; //mm
        protected static float DELTA_MAXROTATION = 4; //degrees

        protected virtual double[] ComputeScanAngles(ScanObservation scan)
        {
            double fov = scan.FieldOfView;
            double res = scan.Resolution;

            int length = scan.Length;
            
            double fromAngle = -fov / 2.0;
            double[] angles = new double[length];

            int len = length - 1;
            for (int i = 0; i <= len; i++)
                angles[i] = fromAngle + (res * i);

            return angles;
        }

        protected virtual double ComputeSquaredDistance(Vector2 point1, Vector2 point2)
        {
            return (System.Math.Pow(point2.X - point1.X, 2.0) + System.Math.Pow(point2.Y - point1.Y, 2.0));
        }

        protected Correlation<Vector2>[] CorrelatePointsDM(Vector2[] points1, Vector2[] points2, bool[] filter1, bool[] filter2)
        {
            double maxdist = System.Math.Pow((double)this.MAX_CORRELATIONDISTANCE, 2.0);
            double[,] dists = new double[points1.Length, points2.Length];

            int[] marks1to2 = new int[points1.Length];

            int len1 = points1.Length - 1;
            int len2 = points2.Length - 1;

            for (int i = 0; i <= len1; i++)
            {
                double curdist = maxdist + 1.0;
                int curmark = -1;
                for (int j = 0; j <= len2; j++)
                {
                    double dist = this.ComputeSquaredDistance(points1[i], points2[j]);
                    dists[i, j] = dist;
                    if ((((!filter1[i] && !filter2[j]) && ((dist < maxdist) && (dist < curdist))) ? 1 : 0) != 0)
                    {
                        curdist = dists[i, j];
                        curmark = j;
                    }
                }
                marks1to2[i] = curmark;
            }

            int matches = 0;
            List<Correlation<Vector2>> correlations = new List<Correlation<Vector2>>();

            int[] marks2to1 = new int[points2.Length];
            for (int j = 0; j <= len2; j++)
            {
                double curdist = maxdist + 1.0;
                int curmark = -1;
                for (int i = 0; i <= len1; i++)
                {
                    if (marks1to2[i] == j)
                    {
                        double dist = dists[i, j];
                        if ((((dist < maxdist) && (dist < curdist)) ? 1 : 0) != 0)
                        {
                            curdist = dist;
                            curmark = i;
                        }
                    }
                }
                marks2to1[j] = curmark;
                if (curdist < maxdist)
                {
                    Correlation<Vector2> pair = new Correlation<Vector2>(points1[curmark], points2[j], curdist);
                    correlations.Add(pair);
                    matches++;
                }
            }

            int[,] pairs = new int[matches, 2];
            int p = 0;
            for (int j = 0; j <= len2; j++)
            {
                if (marks2to1[j] >= 0)
                {
                    pairs[p, 0] = marks2to1[j];
                    pairs[p, 1] = j;
                    p++;
                }
            }
            return correlations.ToArray();
        }

        protected Heap CorrelatePointsQT(Vector2[] points1, Vector2[] points2, bool[] filter1, bool[] filter2)
        {
            QuadTree<Vector2> qtree1 = new QuadTree<Vector2>();
            
            int len1 = points1.Length - 1;
            for (int i = 0; i <= len1; i++)
                if (!filter1[i])
                    qtree1.Insert(points1[i]);

            QuadTree<Vector2> qtree2 = new QuadTree<Vector2>();

            int len2 = points2.Length - 1;
            for (int i = 0; i <= len2; i++)
                if (!filter2[i])
                    qtree2.Insert(points2[i]);

            Heap correlations = new Heap();
            foreach (Vector2 point in points2)
            {
                Vector2 point1 = qtree1.FindNearestNeighbour(point, MAX_CORRELATIONDISTANCE);
                if (point1 != null)
                {
                    Vector2 point2 = qtree2.FindNearestNeighbour(point1, MAX_CORRELATIONDISTANCE);
                    if (!(point2 == null) && (point2 == point))
                    {
                        double d = MathHelper.GetDistance(point1, point2);
                        Correlation<Vector2> pair = new Correlation<Vector2>(point1, point2, d);
                        correlations.Add(pair);
                    }
                }
            }
            return correlations;
        }

        protected virtual bool HasConverged(int numPoints, int numCorrespondencies, int numIterations)
        {
            return (numIterations < this.MAX_ITERATIONS);
        }

        MatchResult IScanMatchers.Match(ScanObservation scan1, ScanObservation scan2, Pose2D seed)
        {
            Vector2[] points1 = null;
            Vector2[] points2 = null;

            if ((((scan1.Length < 2) || (scan2.Length < 2)) ? 1 : 0) != 0)
                throw new InvalidOperationException("Cannot match scans with fewer than 2 scanpoints");

            if ((scan1.FieldOfView == scan2.FieldOfView) && (scan1.Resolution == scan2.Resolution))
            {
                double[] angles = scan1.RangeScanner.RangeTheta;//this.ComputeScanAngles(scan1);
                points1 = this.ToPoints(scan1, angles);
                points2 = this.ToPoints(scan2, angles);
            }
            else
            {
                points1 = this.ToPoints(scan1, scan1.RangeScanner.RangeTheta);
                points2 = this.ToPoints(scan2, scan2.RangeScanner.RangeTheta);
            }

            bool[] filter1 = scan1.RangeScanner.RangeFilters; //this.ToFilter(scan1);
            bool[] filter2 = scan2.RangeScanner.RangeFilters; //this.ToFilter(scan2);
            double dangle = scan1.Resolution;
            return this.MatchPoints(points1, points2, seed, dangle, filter1, filter2);
        }

        protected internal abstract MatchResult MatchPoints(Vector2[] points1, Vector2[] points2, Pose2D seed, double dangle, bool[] filter1, bool[] filter2);
        
        protected virtual bool[] ToFilter(ScanObservation scan)
        {
            bool[] filter = new bool[scan.RangeScanner.Range.Length];
            int len = scan.RangeScanner.Range.Length - 1;
            for (int i = 0; i <= len; i++)
            {
                filter[i] = (scan.RangeScanner.Range[i] < scan.MinRange) || (scan.RangeScanner.Range[i] > scan.MaxRange);
            }
            return filter;
        }

        protected virtual Vector2[] ToLocal(Vector2[] points, Pose2D target)
        {
            TMatrix2D rotmx = target.ToGlobalMatrix();
            Vector2[] local = new Vector2[(points.Length - 1) + 1];
            int len = points.Length - 1;
            for (int i = 0; i <= len; i++)
            {
                local[i] = (Vector2) (rotmx * points[i]);
            }
            return local;
        }

        protected virtual Vector2[] ToPoints(ScanObservation scan, double[] angles)
        {
            Vector2[] points = new Vector2[scan.RangeScanner.Range.Length];

            int len = scan.RangeScanner.Range.Length - 1;
            for (int i = 0; i <= len; i++)
            {
                double dist = scan.RangeScanner.Range[i] * scan.Factor;
                double angle = angles[i];
                points[i] = new Vector2(System.Math.Cos(angle) * dist, System.Math.Sin(angle) * dist);
            }

            return points;
        }

        public static bool ExceedThreshold(Pose2D dpose)
        {
            bool extend = false;
            extend = extend || System.Math.Abs(dpose.X) > MATCH_MAXTRANSLATION;
            extend = extend || System.Math.Abs(dpose.Y) > MATCH_MAXTRANSLATION;

            double dradians = dpose.GetNormalizedRotation();
            double dangle = dradians / System.Math.PI * 180;
            extend = extend || System.Math.Abs(dangle) > MATCH_MAXROTATION;

            return extend;
        }

        public static bool ExceedDelta(Pose2D dpose)
        {
            bool extend = false;
            extend = extend || System.Math.Abs(dpose.X) > DELTA_MAXTRANSLATION;
            extend = extend || System.Math.Abs(dpose.Y) > DELTA_MAXTRANSLATION;

            double dradians = dpose.GetNormalizedRotation();
            double dangle = dradians / System.Math.PI * 180;
            extend = extend || System.Math.Abs(dangle) > DELTA_MAXROTATION;

            return extend;
        }

        public static Pose2D CompoundPoses(Pose2D t1, Pose2D t2)
        {
            Pose2D t_ret = new Pose2D();

            t_ret.X = t2.X * System.Math.Cos(t1.Rotation) - t2.Y * System.Math.Sin(t1.Rotation) + t1.X;
            t_ret.Y = t2.X * System.Math.Sin(t1.Rotation) + t2.Y * System.Math.Cos(t1.Rotation) + t1.Y;
            t_ret.Rotation = t1.Rotation + t2.Rotation;

            // Make angle [-pi,pi)
            t_ret.Rotation = t_ret.GetNormalizedRotation();

            return t_ret;
        }

    }
}
