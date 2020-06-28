using System;
using System.Collections.Generic;

using ScanMatchers.Math;
using ScanMatchers.ScanMatcher.Base;
using System.IO;

namespace ScanMatchers.ScanMatcher
{

    public class IdcScanMatcher : IcpScanMatcher
    {
        // Methods
        protected internal override MatchResult MatchPoints(Vector2[] points1, Vector2[] points2, Pose2D seed, double dangle, bool[] filter1, bool[] filter2)
        {
            int startTime = Environment.TickCount;

            Pose2D rawdpose = seed;
            Pose2D curdpose = rawdpose;
            Vector2[] curpoints2 = null;
            Correlation<Vector2>[] curpairs = null;
            Queue<double> results = new Queue<double>();
            int nconverged = 0;
            Vector2 uavg = new Vector2();
            double sumerror = 0.0;
            double sumangcov = 0.0;

            int n = 1;
            while (n <= base.MAX_ITERATIONS)
            {
                curpoints2 = this.ToLocal(points2, curdpose);
                
                curpairs = this.CorrelatePointsDM(points1, curpoints2, filter1, filter2);


                if (curpairs.Length > 0)
                {
                    Vector2 sumpoint1 = new Vector2();
                    Vector2 sumpoint2 = new Vector2();
                    foreach (Correlation<Vector2> pair in curpairs)
                    {
                        sumpoint1 += pair.Point1;
                        sumpoint2 += pair.Point2;
                    }
                    Vector2 u1 = sumpoint1 / curpairs.GetLength(0);
                    Vector2 u2 = sumpoint2 / curpairs.GetLength(0);
                    uavg = (u1 + u2) / 2;
                    sumpoint1 = new Vector2();
                    sumpoint2 = new Vector2();
                    sumerror = 0.0;
                    sumangcov = 0.0;
                    foreach (Correlation<Vector2> pair in curpairs)
                    {
                        Vector2 point1 = pair.Point1;
                        Vector2 point2 = pair.Point2;
                        sumpoint1 += new Vector2((point1[0] - u1[0]) * (point2[0] - u2[0]), (point1[1] - u1[1]) * (point2[1] - u2[1]));
                        sumpoint2 += new Vector2((point1[0] - u1[0]) * (point2[1] - u2[0]), (point1[1] - u1[1]) * (point2[0] - u2[1]));
                        Vector2 dpoint = point1 - point2;
                        Vector2 mpoint = (point1 + point2) / 2;
                        sumerror += dpoint * dpoint;
                        sumangcov += mpoint * mpoint;
                    }
                    double rotest = System.Math.Atan2(sumpoint2[0] - sumpoint2[1], sumpoint1[0] + sumpoint1[1]);
                    TMatrix2D rotmx = new TMatrix2D(rotest);
                    Vector2 transest = u2 - (rotmx * u1);
                    curdpose = new Pose2D(curdpose.Position - transest, curdpose.Rotation - rotest);
                    rotmx = new TMatrix2D(-rotest);
                    curdpose = (Pose2D) (rotmx * curdpose);

                    bool converged = false;
                    foreach (double result in results)
                    {
                        if (sumerror == 0.0)
                        {
                            converged = converged || (result < base.RESULTS_MAXERROR);
                        }
                        else
                        {
                            converged = converged || (System.Math.Abs((double) ((result / sumerror) - 1.0)) < base.RESULTS_MAXERROR);
                        }
                    }
                    results.Enqueue(sumerror);
                    while (results.Count > base.RESULTS_CONSIDERED)
                    {
                        results.Dequeue();
                    }
                    if (converged)
                    {
                        nconverged++;
                    }
                }
                if (nconverged >= base.RESULTS_CONVERGED)
                {
                    break;
                }
                n++;
            }

            Matrix3 mcov = new Matrix3();

            bool inverseError = false;
            double mdist = 0.0;
            int num = 0;

            if (curpairs.Length > 0)
            {
                foreach (Correlation<Vector2> pair in curpairs)
                {
                    Vector2 dpoint = pair.Point2 - pair.Point1;
                    mdist += System.Math.Pow(dpoint.X, 2.0) + System.Math.Pow(dpoint.Y, 2.0);
                }
                mdist = System.Math.Pow(mdist, 0.5);
                num = curpairs.GetLength(0);

                MathNet.Numerics.LinearAlgebra.Matrix cov = new MathNet.Numerics.LinearAlgebra.Matrix(3, 3);
                cov[0, 0] = num;
                cov[0, 1] = 0.0;
                cov[0, 2] = -num * uavg[1];
                cov[1, 0] = 0.0;
                cov[1, 1] = num;
                cov[1, 2] = num * uavg[0];
                cov[2, 0] = -num * uavg[1];
                cov[2, 1] = num * uavg[0];
                cov[2, 2] = sumangcov;

                try
                {
                    cov = (sumerror / (2 * num - 3)) * cov.Inverse();
                }
                catch 
                {
                    inverseError = true;
                }

                mcov[0, 0] = (float)cov[0, 0];
                mcov[0, 1] = (float)cov[0, 1];
                mcov[0, 2] = (float)cov[0, 2];
                mcov[1, 0] = (float)cov[1, 0];
                mcov[1, 1] = (float)cov[1, 1];
                mcov[1, 2] = (float)cov[1, 2];
                mcov[2, 0] = (float)cov[2, 0];
                mcov[2, 1] = (float)cov[2, 1];
                mcov[2, 2] = (float)cov[2, 2];
            }

            double duration = (Environment.TickCount - startTime) / 1000.0;
            return new MatchResult(rawdpose, curdpose, n, duration, this.HasConverged(points1.Length, num, n) && !inverseError);
        }
    }
}
