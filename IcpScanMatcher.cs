using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using UvARescue.Agent;
using UvARescue.Math;
using UvARescue.Tools;

namespace UvARescue.Slam
{

    public abstract class IcpScanMatcher : IScanMatcher
    {

        public const double PI_2 = 1.5707963267949;

        protected float MAX_CORRELATIONDISTANCE;
        protected int MAX_ITERATIONS;
        protected int RESULTS_CONSIDERED;
        protected int RESULTS_CONVERGED;
        protected double RESULTS_MAXERROR;

        public IcpScanMatcher()
        {
            MAX_ITERATIONS = 80;
            RESULTS_CONSIDERED = 10;
            RESULTS_CONVERGED = 8;
            RESULTS_MAXERROR = 0.01;
            MAX_CORRELATIONDISTANCE = 500.0F;
        }

        protected Correlation<Vector2>[] CorrelatePointsDM(Manifold manifold, Vector2[] points1, Vector2[] points2, bool[] filter1, bool[] filter2)
        {
            bool flag;
            int i7;

            double maxdist = Math.Pow((double)MAX_CORRELATIONDISTANCE, 2.0);
            double[,] dists = new double[checked(checked(checked((int)points1.Length) - 1) + 1), checked(checked(checked((int)points2.Length) - 1) + 1)];
            int[] marks1to2 = new int[checked(checked(checked((int)points1.Length) - 1) + 1)];
            int i2 = checked(checked((int)points1.Length) - 1);
            int i = 0;
            while (i <= i7)
            {
                double curdist = maxdist + 1.0;
                int curmark = -1;
                int i3 = checked(checked((int)points2.Length) - 1);
                int j = 0;
                while (j <= i7)
                {
                    double dist = ComputeSquaredDistance(points1[i], points2[j]);
                    dists[i, j] = dist;
                    if (filter1[i] || filter2[j] || (dist >= maxdist))
                        goto label_0;
                    flag = dist < curdist ? true : false;
                    if (flag)
                    {
                        curdist = dists[i, j];
                        curmark = j;
                    }
                    j = checked(j + 1);
                    i7 = i3;
                }
                marks1to2[i] = curmark;
                i = checked(i + 1);
                i7 = i2;
            }
            int matches = 0;
            List<Correlation<Vector2>> correlations = new List<Correlation<Vector2>>();
            int[] marks2to1 = new int[checked(checked(checked((int)points2.Length) - 1) + 1)];
            int i4 = checked(checked((int)points2.Length) - 1);
            int j1 = 0;
            while (j1 <= i7)
            {
                double curdist1 = maxdist + 1.0;
                int curmark1 = -1;
                int i5 = checked(checked((int)points1.Length) - 1);
                int i1 = 0;
                while (i1 <= i7)
                {
                    flag = marks1to2[i1] == j1;
                    if (flag)
                    {
                        double dist1 = dists[i1, j1];
                        if (dist1 >= maxdist)
                            goto label_0;
                        flag = dist1 < curdist1 ? true : false;
                        if (flag)
                        {
                            curdist1 = dist1;
                            curmark1 = i1;
                        }
                    }
                    i1 = checked(i1 + 1);
                    i7 = i5;
                }
                marks2to1[j1] = curmark1;
                flag = curdist1 < maxdist;
                if (flag)
                {
                    Correlation<Vector2> correlation = new Correlation<Vector2>(points1[curmark1], points2[j1]);
                    correlations.Add(correlation);
                    matches = checked(matches + 1);
                }
                j1 = checked(j1 + 1);
                i7 = i4;
            }
            int[,] pairs = new int[checked(checked(matches - 1) + 1), 2];
            int p = 0;
            int i6 = checked(checked((int)points2.Length) - 1);
            int j2 = 0;
            while (j2 <= i7)
            {
                flag = marks2to1[j2] < 0;
                if (flag)
                {
                }
                else
                {
                    pairs[p, 0] = marks2to1[j2];
                    pairs[p, 1] = j2;
                    p = checked(p + 1);
                }
                j2 = checked(j2 + 1);
                i7 = i6;
            }
            return correlations.ToArray();
        }

        protected Correlation<Vector2>[] CorrelatePointsQT(Manifold manifold, Vector2[] points1, Vector2[] points2, bool[] filter1, bool[] filter2)
        {
            bool flag;
            int i5;

            QuadTree<Vector2> qtree1 = new QuadTree<Vector2>();
            int i2 = checked(checked((int)points1.Length) - 1);
            int i = 0;
            while (i <= i5)
            {
                flag = !filter1[i];
                if (flag)
                    qtree1.Insert(points1[i]);
                i = checked(i + 1);
                i5 = i2;
            }
            QuadTree<Vector2> qtree2 = new QuadTree<Vector2>();
            int i3 = checked(checked((int)points2.Length) - 1);
            int i1 = 0;
            while (i1 <= i5)
            {
                flag = !filter2[i1];
                if (flag)
                    qtree2.Insert(points2[i1]);
                i1 = checked(i1 + 1);
                i5 = i3;
            }
            List<Correlation<Vector2>> correlations = new List<Correlation<Vector2>>();
            Vector2[] vector2Arr = points2;
            int i4 = 0;
            while (flag)
            {
                Vector2 point = vector2Arr[i4];
                Vector2 point1 = qtree1.FindNearestNeighbour(point, MAX_CORRELATIONDISTANCE);
                flag = Information.IsNothing(point1);
                if (flag)
                {
                }
                else
                {
                    Vector2 point2 = qtree2.FindNearestNeighbour(point1, MAX_CORRELATIONDISTANCE);
                    flag = Information.IsNothing(point2);
                    if (flag)
                    {
                    }
                    else
                    {
                        flag = point2 == point;
                        if (flag)
                        {
                            Correlation<Vector2> correlation = new Correlation<Vector2>(point1, point2);
                            correlations.Add(correlation);
                        }
                    }
                }
                i4 = checked(i4 + 1);
                flag = i4 < checked((int)vector2Arr.Length);
            }
            return correlations.ToArray();
        }

        public MatchResult Match(Manifold manifold, ScanObservation scan1, ScanObservation scan2, Pose2D seed)
        {
            Vector2[] points1, points2;

            bool flag = (scan1.Length >= 2) && (scan2.Length >= 2) ? true : false;
            if (flag)
                throw new InvalidOperationException("Cannot match scans with fewer than 2 scanpoints\uFFFD");
            if (scan1.FieldOfView != scan2.FieldOfView)
                goto label_0;
            flag = scan1.Resolution == scan2.Resolution ? true : false;
            if (flag)
            {
                double[] angles = ComputeScanAngles(scan1);
                points1 = ToPoints(scan1, angles);
                points2 = ToPoints(scan2, angles);
            }
            else
            {
                points1 = ToPoints(scan1, ComputeScanAngles(scan1));
                points2 = ToPoints(scan2, ComputeScanAngles(scan2));
            }
            bool[] filter1 = ToFilter(scan1);
            bool[] filter2 = ToFilter(scan2);
            double dangle = scan1.FieldOfView / (double)scan1.Length;
            return MatchPoints(manifold, points1, points2, seed, dangle, filter1, filter2);
        }

        protected internal abstract MatchResult MatchPoints(Manifold manifold, Vector2[] points1, Vector2[] points2, Pose2D seed, double dangle, bool[] filter1, bool[] filter2);

        void IScanMatcher.ApplyConfig(Config config)
        {
            RESULTS_MAXERROR = Double.Parse(config.GetConfig("slam\uFFFD", "error-threshold\uFFFD", Conversions.ToString(RESULTS_MAXERROR)));
            MAX_CORRELATIONDISTANCE = Single.Parse(config.GetConfig("slam\uFFFD", "correlation-maxdistance\uFFFD", Conversions.ToString(MAX_CORRELATIONDISTANCE)));
        }

        protected virtual double[] ComputeScanAngles(ScanObservation scan)
        {
            int i2;

            double fov = scan.FieldOfView;
            double res = scan.Resolution;
            int length = checked(checked((int)Math.Round(fov / res)) + 1);
            double fromAngle = -fov / 2.0;
            double[] angles = new double[checked(checked(length - 1) + 1)];
            int i1 = checked(length - 1);
            int i = 0;
            while (i <= i2)
            {
                angles[i] = fromAngle + (res * (double)i);
                i = checked(i + 1);
                i2 = i1;
            }
            return angles;
        }

        protected virtual double ComputeSquaredDistance(Vector2 point1, Vector2 point2)
        {
            return Math.Pow(point2.X - point1.X, 2.0) + Math.Pow(point2.Y - point1.Y, 2.0);
        }

        protected virtual bool HasConverged(int numPoints, int numCorrespondencies, int numIterations)
        {
            return numIterations < MAX_ITERATIONS;
        }

        MatchResult IScanMatcher.Match(Manifold manifold, Patch patch1, Patch patch2, Pose2D seed)
        {
            return this.Match(manifold, patch1.Scan, patch2.Scan, seed);
        }

        protected virtual bool[] ToFilter(ScanObservation scan)
        {
            int i2;

            ScanObservation scanObservation = scan;
            bool[] filter = new bool[checked(checked(checked((int)scanObservation.Range.Length) - 1) + 1)];
            int i1 = checked(checked((int)scanObservation.Range.Length) - 1);
            int i = 0;
            while (i <= i2)
            {
                filter[i] = (scanObservation.Range[i] >= scanObservation.MinRange) && (scanObservation.Range[i] <= scanObservation.MaxRange) ? true : false;
                i = checked(i + 1);
                i2 = i1;
            }
            return filter;
            scanObservation = null;
            return ToFilter;
        }

        protected virtual Vector2[] ToLocal(Vector2[] points, Pose2D target)
        {
            int i2;

            TMatrix2D rotmx = target.ToGlobalMatrix();
            Vector2[] local = new Vector2[checked(checked(checked((int)points.Length) - 1) + 1)];
            int i1 = checked(checked((int)points.Length) - 1);
            int i = 0;
            while (i <= i2)
            {
                local[i] = rotmx * points[i];
                i = checked(i + 1);
                i2 = i1;
            }
            return local;
        }

        protected virtual Vector2[] ToPoints(ScanObservation scan, double[] angles)
        {
            int i2;

            ScanObservation scanObservation = scan;
            Vector2[] points = new Vector2[checked(checked(checked((int)scanObservation.Range.Length) - 1) + 1)];
            int i1 = checked(checked((int)scanObservation.Range.Length) - 1);
            int i = 0;
            while (i <= i2)
            {
                double dist = scanObservation.Range[i] * (double)scanObservation.Factor;
                double angle = angles[i];
                points[i] = new Vector2(Math.Cos(angle) * dist, Math.Sin(angle) * dist);
                i = checked(i + 1);
                i2 = i1;
            }
            return points;
            scanObservation = null;
            return ToPoints;
        }

    } // class IcpScanMatcher

}

