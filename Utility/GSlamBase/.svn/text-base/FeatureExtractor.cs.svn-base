using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ScanMatchers.Math;
using ScanMatchers.Tools.USARSimMessages;

namespace ScanMatchers.ScanMatcher.GSlamBase
{

    public class FeatureExtractor
    {
        private const float VERY_LARGE_A = float.PositiveInfinity;
        private const float VERY_SMALL_A = 0.001f;
        private const float DISTANCE_TO_CONSIDER_NEW_PART = 0.5f;
        private const float DISTANCE_TO_CONSIDER_NEW_PART_FOR_CORNER = 0.1f;
        private const float MAX_ERROR_TOLERANCE_FOR_LINE = 0.01f;
        private const float MIN_LENGTH_OF_LINE_TO_CONSIDER_NEW_LINE = 0.2f;
        private float SIN1, COS1;
        private float D_THRESH = 0.05f;
        private const float MIN_ANGLE_OF_CORNER = 10.0f;
        public const float MAX_DISTANCE_TO_ADD_NEW_EDGE = 0.5f;
        private const float MAXIMUME_VALID_EDGE_DISTANCE_FROM_LINES = 1F;

        public const float MIN_DISTANCE_BETWEEN_EDGES = 0.05f;
        public const float MAX_DIST_TO_CONSIDER_POINTS_EQUAL_UPDATE = 0.05f;

        private Laser LaserData;
        //private Point3D RobotPos;
        //private Point3D RobotRot;
        private Pose2D RobotPose;

        public List<Vector2> vLasers;
        private List<Edge> NewEdges;
        private List<Line> NewLines;
        private int timeCycle;
        private EdgeTrimmer edgeTrimmer;
        public FeatureExtractor()
        {
            SIN1 = (float)System.Math.Sin(MathHelper.DegToRad(1f));
            COS1 = (float)System.Math.Cos(MathHelper.DegToRad(1f));
            edgeTrimmer = new EdgeTrimmer();
        }

        public void updateValues(Pose2D robot, Laser LaserData, int timeCycle)
        {
            this.LaserData = LaserData;
            this.RobotPose = new Pose2D(robot.Position, robot.Rotation);
            GenerateLaserVectors();
            this.timeCycle = timeCycle;
        }

        public static float DistanceOfPointToLine(Vector2 p, Line l)
        {
            Vector2 imgP = new Vector2(p);

            if (l.a.CompareTo(VERY_LARGE_A) == 0)
            {
                imgP.X = l.b;
                imgP.Y = p.Y;
            }
            else
            {
                imgP.X = (l.a * p.Y + p.X - l.a * l.b) / (l.a * l.a + 1);
                imgP.Y = l.a * imgP.X + l.b;
            }

            return MathHelper.GetDistance(p, imgP); //p.getDistance2D(imgP);
        }

        private void GenerateLaserVectors()
        {
            vLasers = new List<Vector2>();

            Matrix2 R = new Matrix2(System.Math.Cos(RobotPose.Rotation), -System.Math.Sin(RobotPose.Rotation),
                                    System.Math.Sin(RobotPose.Rotation), System.Math.Cos(RobotPose.Rotation));

            List<Vector4> points = new List<Vector4>();

            Vector2 tmpC;

            int len = LaserData.fRanges.Count - 1;
            for (int i = 0; i <= len; i++)
            {
                double dist = LaserData.fRanges[i];
                double angle = LaserData.fTheta[i];

                if (!LaserData.bFilters[i])
                {
                    tmpC = new Vector2(System.Math.Cos(angle) * dist, System.Math.Sin(angle) * dist);
                    tmpC = R * tmpC;
                    tmpC.X += RobotPose.X;
                    tmpC.Y += RobotPose.Y;

                    vLasers.Add(tmpC);
                }
            }
        }

        private bool IsPointBetweenTwoPoints(Vector2 center, Vector2 p1, Vector2 p2)
        {
            Vector2 pMid = new Vector2();

            List<Vector2> plist = new List<Vector2>();
            plist.Add(p1);
            plist.Add(p2);

            Line tmpLine = MatchLineToPoints(plist);

            if (tmpLine.a.CompareTo(VERY_LARGE_A) == 0)
            {
                pMid.X = tmpLine.b;
                pMid.Y = center.Y;
            }
            else
            {
                pMid.X = (tmpLine.a * center.Y + center.X - tmpLine.a * tmpLine.b) / (tmpLine.a * tmpLine.a + 1);
                pMid.Y = tmpLine.a * pMid.X + tmpLine.b;
            }

            float dline = MathHelper.GetDistance(p1, p2);
            float d1 = MathHelper.GetDistance(center, p1);
            float d2 = MathHelper.GetDistance(center, p2);

            return ((d1 < dline) & (d2 < dline));
        }

        public List<Line> ExtractParts()
        {
            List<Line> lines = new List<Line>();
            int iStart = 0;
            for (int i = 0; i < vLasers.Count - 1; i++)
            {
                if (MathHelper.GetDistance(vLasers[i], vLasers[i + 1]) > DISTANCE_TO_CONSIDER_NEW_PART) // //maxDist
                {
                    List<Vector2> pSubL = new List<Vector2>();
                    pSubL = vLasers.GetRange(iStart, i - iStart + 1);
                    //tl.Add(pSubL);
                    if (pSubL.Count >= 2)
                        lines.AddRange(GenerateLines(pSubL, iStart));
                    iStart = i + 1;
                }
            }
            if (iStart < vLasers.Count - 1)
            {
                List<Vector2> pSubL = new List<Vector2>();
                pSubL = vLasers.GetRange(iStart, vLasers.Count - iStart - 1);
                if (pSubL.Count >= 2)
                    lines.AddRange(GenerateLines(pSubL, iStart));
            }
            NewLines = lines;
            return lines;
        }

        public List<Line> GenerateLines(List<Vector2> pLasers, int StartIndex)
        {
            List<Line> lines = new List<Line>();
            List<Vector2> pSubL = new List<Vector2>();
            int iStart, iEnd;

            iStart = 0;
            iEnd = pLasers.Count - 1;
            while (true)
            {
                pSubL = pLasers.GetRange(iStart, iEnd - iStart + 1);
                Line l = MatchLineToPoints(pSubL);
                if (l.err < MAX_ERROR_TOLERANCE_FOR_LINE)
                {
                    l = CalculateSE(l, pLasers, ref iStart, iEnd);
                    l.laser_start += StartIndex;
                    l.laser_end += StartIndex;

                    if (MathHelper.GetDistance(l.head, l.tail) > MIN_LENGTH_OF_LINE_TO_CONSIDER_NEW_LINE)
                    {
                        lines.Add(l);
                    }

                    iEnd = pLasers.Count - 1;
                    if (iStart == iEnd) break;
                }
                else
                {
                    iEnd = (int)System.Math.Ceiling((float)((iStart + iEnd) / 2));
                }
            }
            return lines;
        }

        private Line CalculateSE(Line l, List<Vector2> pLasers, ref int iStart, int iEnd)
        {
            Line retLine = l;
            int i = iEnd;

            for (; ; )
            {
                i++;
                if (i == pLasers.Count)
                {
                    List<Vector2> pSubL = new List<Vector2>();
                    pSubL = pLasers.GetRange(iStart, i - iStart);
                    Line tmpL = MatchLineToPoints(pSubL);

                    Vector2 imgStart = new Vector2(RobotPose.Position);
                    Vector2 imgEnd = new Vector2(RobotPose.Position);

                    retLine = tmpL;
                    if (retLine.a.CompareTo(VERY_LARGE_A) == 0)
                    {
                        imgStart.X = tmpL.b;
                        imgStart.Y = pLasers[iStart].Y;
                        imgEnd.X = tmpL.b;
                        imgEnd.Y = pLasers[i - 1].Y;
                    }
                    else
                    {
                        imgStart.X = (tmpL.a * pLasers[iStart].Y + pLasers[iStart].X - tmpL.a * tmpL.b) / (tmpL.a * tmpL.a + 1);
                        imgStart.Y = tmpL.a * imgStart.X + tmpL.b;

                        imgEnd.X = (tmpL.a * pLasers[i - 1].Y + pLasers[i - 1].X - tmpL.a * tmpL.b) / (tmpL.a * tmpL.a + 1);
                        imgEnd.Y = tmpL.a * imgEnd.X + tmpL.b;
                    }
                    retLine.laser_start = iStart;
                    retLine.laser_end = i - 1;

                    iStart = i - 1;

                    retLine.head = imgStart;
                    retLine.tail = imgEnd;

                    break;
                }

                float e = ErrorOfPToL(pLasers[i], l);

                // i Nabayad Bashe Toosh
                if (e > MAX_ERROR_TOLERANCE_FOR_LINE)
                {
                    List<Vector2> pSubL = new List<Vector2>();
                    pSubL = pLasers.GetRange(iStart, i - iStart);
                    Line tmpL = MatchLineToPoints(pSubL);

                    Vector2 imgStart = new Vector2(RobotPose.Position);
                    Vector2 imgEnd = new Vector2(RobotPose.Position);
                    retLine = tmpL;

                    if (retLine.a == VERY_LARGE_A)
                    {
                        imgStart.X = tmpL.b;
                        imgStart.Y = pLasers[iStart].Y;
                        imgEnd.X = tmpL.b;
                        imgEnd.Y = pLasers[i - 1].Y;
                    }
                    else
                    {
                        imgStart.X = (tmpL.a * pLasers[iStart].Y + pLasers[iStart].X - tmpL.a * tmpL.b) / (tmpL.a * tmpL.a + 1);
                        imgStart.Y = tmpL.a * imgStart.X + tmpL.b;

                        imgEnd.X = (tmpL.a * pLasers[i - 1].Y + pLasers[i - 1].X - tmpL.a * tmpL.b) / (tmpL.a * tmpL.a + 1);
                        imgEnd.Y = tmpL.a * imgEnd.X + tmpL.b;
                    }
                    retLine.laser_start = iStart;
                    retLine.laser_end = i - 1;

                    iStart = i;

                    retLine.head = imgStart;
                    retLine.tail = imgEnd;

                    break;
                }
            }

            return retLine;
        }


        public List<Edge> FindEdge()
        {
            List<Edge> res = FindEdge_OLD();
            edgeTrimmer.trimNewEdges(res);
            return res;

        }

        private List<Edge> FindEdge_OLD()
        {
            List<Edge> retEdge = new List<Edge>();
            float[] vals;
            Vector2 point = new Vector2();
            Edge edge;

            for (int i = 0; i < NewLines.Count - 1; i++)
            {
                vals = getThreshHold(NewLines[i].tail, NewLines[i + 1].head);
                float lt1 = vals[0];
                float lt2 = vals[1];

                float mid = (lt1 + lt2) * (0.01745f) / 5f;
                D_THRESH = mid;

                float d = vals[3];
                float dist = vals[2];
                float distPTL1 = FeatureExtractor.DistanceOfPointToLine(NewLines[i + 1].head, NewLines[i]);
                float distPTL2 = FeatureExtractor.DistanceOfPointToLine(NewLines[i].tail, NewLines[i + 1]);
                float distPTL = (float)System.Math.Min(distPTL1, distPTL2);

                point = calculateCollision(NewLines[i], NewLines[i + 1]);
                if (point != null)
                {
                    if (System.Math.Abs(NewLines[i].laser_end - NewLines[i + 1].laser_start) == 1)
                    {
                        List<Line> res = new List<Line>();
                        res.Add(NewLines[i]);
                        res.Add(NewLines[i + 1]);

                        if (IsEdgeValid(point, NewLines[i].tail, NewLines[i + 1].head) && (dist < DISTANCE_TO_CONSIDER_NEW_PART)
                            && ((2f * (lt2 - lt1) / (lt2 + lt1)) < 0.1f))
                        {


                            edge = new Edge(point, RobotPose.Position, EdgeType.Corner, res, timeCycle);
                            if (System.Math.Abs(edge.angleBetweenLines) > MathHelper.DegToRad(MIN_ANGLE_OF_CORNER) &&
                                System.Math.Abs(edge.angleBetweenLines) < MathHelper.DegToRad(180 - MIN_ANGLE_OF_CORNER))
                            {
                                retEdge.Add(edge);
                                continue;
                            }
                        }
                        else
                        {
                            edge = new Edge(point, RobotPose.Position, EdgeType.Intersection, res, timeCycle);

                            float distToTail = MathHelper.GetDistance(edge.point, res[0].tail); //edge.point.getDistance3D(res[0].tail);
                            float distToHead = MathHelper.GetDistance(edge.point, res[1].head); //edge.point.getDistance3D(res[1].head);
                            float distI = System.Math.Min(distToHead, distToTail);

                            if (System.Math.Abs(edge.angleBetweenLines) > MathHelper.DegToRad(4.5f * MIN_ANGLE_OF_CORNER) &&
                                System.Math.Abs(edge.angleBetweenLines) < MathHelper.DegToRad(180 - 4.5f * MIN_ANGLE_OF_CORNER) &&
                                distI >= 0.9f)
                            {
                                retEdge.Add(edge);
                            }
                        }
                    }
                }

                if (dist > DISTANCE_TO_CONSIDER_NEW_PART && distPTL > dist * (float)System.Math.Sin(MathHelper.DegToRad(10f)))
                {
                    List<Line> res = new List<Line>();

                    if (!IsPointBetweenTwoPoints(RobotPose.Position, NewLines[i].tail, NewLines[i + 1].head) &&
                        System.Math.Abs(NewLines[i].laser_end - NewLines[i + 1].laser_start) == 1)
                    {
                        if (lt1 > lt2)
                        {
                            res.Add(NewLines[i + 1]);
                            point = NewLines[i + 1].head;
                        }
                        else
                        {
                            res.Add(NewLines[i]);
                            point = NewLines[i].tail;
                        }
                        edge = new Edge(point, RobotPose.Position, EdgeType.EndOfWall, res, timeCycle);
                        retEdge.Add(edge);
                    }
                }

            }

            List<Edge> refEdges = new List<Edge>();
            int interest_count = 0;

            for (int i = 0; i < retEdge.Count; i++)
            {
                float dist = 0.0f;
                float best = 0.0f;
                Edge bestEdge = null;

                float distToNE;

                Edge ne = retEdge[i];

                if (ne.type == EdgeType.Intersection)
                {
                    best = float.MaxValue;
                    distToNE = MathHelper.GetDistance(RobotPose.Position, ne.point); //RobotPose.Position.getDistance2D(ne.point);
                    distToNE *= MathHelper.DegToRad(5f);
                    float MAX_DISTANCE_TO_ADD_NEW = System.Math.Max(distToNE, MAX_DISTANCE_TO_ADD_NEW_EDGE);

                    for (int j = 0; j < retEdge.Count; j++)
                    {
                        if (j != i)
                        {
                            Edge oe = retEdge[j];

                            dist = MathHelper.GetDistance(ne.point, oe.point); //ne.point.getDistance2D(oe.point);
                            if (dist < MAX_DISTANCE_TO_ADD_NEW)
                            {
                                if (dist < best)
                                {
                                    best = dist;
                                    bestEdge = oe;
                                }
                            }
                        }
                    }

                    if (interest_count != 0)
                    {
                        if (best == float.MaxValue)
                        {
                            refEdges.Add(ne);
                            interest_count++;
                        }
                    }
                    else
                    {
                        refEdges.Add(ne);
                        interest_count++;
                    }
                }
                else
                {
                    refEdges.Add(ne);
                }
            }
            NewEdges = refEdges;
            return refEdges;
        }

        private Line MatchLineToPoints(List<Vector2> pL)
        {
            Line l = new Line();
            Line vl = new Line();

            float sigmaX, sigmaX2, sigmaY, sigmaXY;
            sigmaX = 0f; sigmaX2 = 0f; sigmaY = 0f; sigmaXY = 0f;

            for (int i = 0; i < pL.Count; i++)
            {
                sigmaX += (float)pL[i].X;
                sigmaX2 += (float)pL[i].X * (float)pL[i].X;
                sigmaY += (float)pL[i].Y;
                sigmaXY += (float)pL[i].X * (float)pL[i].Y;
            }

            float temp = (float)System.Math.Abs((pL.Count * sigmaX2) - sigmaX * sigmaX);

            if (temp < VERY_SMALL_A)
            {
                l.a = VERY_LARGE_A;
                l.b = sigmaX / pL.Count;
            }
            else
            {
                l.a = ((pL.Count * sigmaXY) - (sigmaY * sigmaX)) / ((pL.Count * sigmaX2) - sigmaX * sigmaX);
                l.b = (((-1f) * sigmaXY * sigmaX) + (sigmaY * sigmaX2)) / ((pL.Count * sigmaX2) - sigmaX * sigmaX);
            }

            l.err = computeError(pL, l);

            vl.a = VERY_LARGE_A;
            vl.b = sigmaX / pL.Count;

            vl.err = computeError(pL, vl);

            if (vl.err <= l.err)
                return vl;
            else
                return l;
        }

        private float ErrorOfPToL(Vector2 pL, Line l)
        {
            return DistanceOfPointToLine(pL, l);
        }

        private float computeError(List<Vector2> pL, Line l)
        {
            int i = 0;
            float e = 0;
            while (i < pL.Count)
            {
                double d = ErrorOfPToL(pL[i], l);
                e += (float)System.Math.Pow(d, 2);
                i++;
            }
            e /= (pL.Count);
            return (float)System.Math.Sqrt(e);
        }

        private Vector2 calculateCollision(Line line1, Line line2)
        {
            Vector2 temp = new Vector2();

            if (line1.a.CompareTo(VERY_LARGE_A) == 0)
            {
                if (line2.a.CompareTo(VERY_LARGE_A) == 0)
                {
                    return null;
                }
                temp.X = line1.b;
                temp.Y = line2.a * temp.X + line2.b;
            }
            else if (line2.a.CompareTo(VERY_LARGE_A) == 0)
            {
                temp.X = line2.b;
                temp.Y = line1.a * temp.X + line1.b;
            }
            else
            {
                temp.X = (line2.b - line1.b) / (line1.a - line2.a);
                temp.Y = line1.a * temp.X + line1.b;
            }

            return temp;
        }

        private float[] getThreshHold(Vector2 p1, Vector2 p2) // DP1ToR, DP2ToR, DP1P2, ThreshHold
        {
            float lt1 = MathHelper.GetDistance(p1, RobotPose.Position);  //TODO: for what center?
            float lt2 = MathHelper.GetDistance(p2, RobotPose.Position);
            float l2 = (float)System.Math.Max(lt1, lt2);
            float l1 = (l2 == lt1 ? lt2 : lt1);
            float h = l1 * SIN1;
            float lp = l1 * COS1;
            float d = (float)System.Math.Sqrt((l2 - lp) * (l2 - lp) + h * h);
            float dist = MathHelper.GetDistance(p1, p2);

            float[] res = new float[4];
            res[0] = lt1;
            res[1] = lt2;
            res[2] = dist;
            res[3] = d;
            return res;
        }

        private bool IsEdgeValid(Vector2 pEdge, Vector2 pHead, Vector2 pTail)
        {
            float dest1 = MathHelper.GetDistance(pHead, pEdge);
            float dest2 = MathHelper.GetDistance(pTail, pEdge);

            float MinDistance = System.Math.Min(dest1, dest2);

            return MinDistance < MAXIMUME_VALID_EDGE_DISTANCE_FROM_LINES;
        }

    }

}
