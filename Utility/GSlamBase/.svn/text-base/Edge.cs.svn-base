using System;
using System.Text;
using System.Collections.Generic;

using ScanMatchers.Math;

namespace ScanMatchers.ScanMatcher.GSlamBase
{
    public enum EdgeType
    {
        Corner,
        EndOfWall,
        Intersection
    };

    public class Edge
    {
        public Vector2 point;
        public EdgeType type;
        public List<Line> lines;
        public float angleBetweenLines = -1f;

        ///////////////////////////////
        public float corrolation = 0f;
        ///////////////////////////////
        public int lastUpdatedTimeCycle = 0;
        public int timeCycleCreated = 0;

        private Vector2 firstObservedRobotPos = null;

        public Edge(Vector2 point, Vector2 robotPos, EdgeType type, List<Line> connectedLines, int timeCycle)
        {
            this.type = type;
            this.point = point;
            this.lines = connectedLines;
            this.firstObservedRobotPos = new Vector2(robotPos);
            updateAngleBetweenLines();
            this.nearestPointRobotObserved = new Vector2(firstObservedRobotPos);
            nearestDistanceUpdated = MathHelper.GetDistance(firstObservedRobotPos, point); //firstObservedRobotPos.getDistance2D(point);
            newObservationAccuracyP = getAccuracyDistance(nearestDistanceUpdated);
            updateTime++;
            updateMaxDistanceToMatch();
            timeCycleCreated = timeCycle;
            lastUpdatedTimeCycle = timeCycle;
        }

        public Edge(Vector2 point, EdgeType type, List<Line> lines, float accuracyP)
        {
            this.lines = null;
            this.type = type;
            this.point = point;
            this.newObservationAccuracyP = accuracyP;
            this.lines = lines;
            updateAngleBetweenLines();
        }

        private void updateAngleBetweenLines()
        {
            if (lines != null && lines.Count == 2)
            {
                Vector2 t1 = lines[0].head - lines[0].tail, t2 = lines[1].head - lines[1].tail;
                angleBetweenLines = MathHelper.AngleBetweenLines(t1, t2);
            }
            else
                angleBetweenLines = -1f;
        }

        public int updateTime = 0;
        private float nearestDistanceUpdated = float.MaxValue;
        private Vector2 nearestPointRobotObserved = null;
        private float oldAccuracyDist = 0;
        private float oldAccuracyP = 0.1f;
        private float oldAccuracyUT = 0;
        private float newObservationAccuracyP = 0;
        private float maxDistanceToMatch = 0;
        private float getAccuracyDistance(float dist)
        {
            return -0.4f * (float)System.Math.Atan(0.2f * dist - 0.2f) + 0.6f;
        }

        public float AccuracyOfEdgeP
        {
            get { return oldAccuracyP; }
        }

        public float AccuracyOfNewObservation
        {
            get { return newObservationAccuracyP; }
        }

        public float NearestDistanceRobotObserved
        {
            get { return nearestDistanceUpdated; }
        }
        public void updateObservation(Vector2 robotPos, Vector2 newPosition, int timeCycle)
        {
            updatePosition(newPosition);
            updateTime++;
            float newDist = MathHelper.GetDistance(robotPos, point); //robotPos.getDistance2D(point);
            if (newDist < nearestDistanceUpdated)
            {
                nearestPointRobotObserved = new Vector2(robotPos);
                nearestDistanceUpdated = newDist;
                oldAccuracyDist = getAccuracyDistance(newDist);
            }

            oldAccuracyUT = getAccuracyUpdateTime(updateTime);
            oldAccuracyP = (float)System.Math.Sqrt(oldAccuracyP * oldAccuracyUT);

            updateMaxDistanceToMatch();
            lastUpdatedTimeCycle = timeCycle;
        }

        private float getAccuracyUpdateTime(int updateTime)
        {
            return 1f - (float)System.Math.Pow(1.1, -(float)updateTime);
        }

        private void updatePosition(Vector2 newPos)
        {
            float x = ((float)point.X * (float)updateTime + (float)newPos.X) / (float)(updateTime + 1);
            float y = ((float)point.Y * (float)updateTime + (float)newPos.Y) / (float)(updateTime + 1);
            point.X = x;
            point.Y = y;
        }

        public float MaxDistanceToMatch
        {
            get { return maxDistanceToMatch; }
        }

        private const float ERR_TOLERANCE_MATCH = 0.1f;
        private void updateMaxDistanceToMatch() // 20 M Laser -> 20 CM Err, 2 ObS
        {
            maxDistanceToMatch = nearestDistanceUpdated / 50f + ERR_TOLERANCE_MATCH;
        }

        public static Edge applyRotateShift(Edge edge, Pose2D Changes, Vector2 rotateCenter)
        {
            Vector2 newP, newT, newH;
            newP = MathHelper.TranformVector(edge.point, rotateCenter, Changes);
            Line newLine;
            List<Line> lines = null;
            if (edge.lines == null)
            {
                throw new Exception("WTF");
            }
            else
            {
                lines = new List<Line>();

                foreach (Line line in edge.lines)
                {
                    newH = MathHelper.TranformVector(line.head, rotateCenter, Changes); //Point3D.applyRotateShift(line.head, Changes.Rotation, rotateCenter, Changes.Position);
                    newT = MathHelper.TranformVector(line.tail, rotateCenter, Changes); //Point3D.applyRotateShift(line.tail, Changes.Rotation, rotateCenter, Changes.Position);
                    newLine = new Line();
                    newLine.head = newH;
                    newLine.tail = newT;
                    lines.Add(newLine);
                }
            }

            return new Edge(newP, edge.type, lines, edge.AccuracyOfNewObservation);
        }

        private static float getMinAngleBetweenLines(Edge e1, Edge e2)
        {
            float a, best = float.MaxValue;
            foreach (Line l1 in e1.lines)
            {
                foreach (Line l2 in e2.lines)
                {
                    a = l1.AngleBetween(l2);
                    if (a < best)
                        best = a;
                }
            }
            return best;
        }

        private static float getAvgAngleBetweenLines(Edge e1, Edge e2)
        {
            float a11, a12, a21, a22, r1, r2;
            a11 = e1.lines[0].AngleBetween(e2.lines[0]);
            a12 = e1.lines[1].AngleBetween(e2.lines[1]);

            a21 = e1.lines[0].AngleBetween(e2.lines[1]);
            a22 = e1.lines[1].AngleBetween(e2.lines[0]);

            r1 = (a11 + a12) / 2f;
            r2 = (a21 + a22) / 2f;

            return System.Math.Min(r1, r2);
        }

        public static float getAngleBetweenLines(Edge e1, Edge e2)
        {
            if (e1.lines.Count == 2 && e2.lines.Count == 2)
            {
                return getAvgAngleBetweenLines(e1, e2);
            }
            else
            {
                return getMinAngleBetweenLines(e1, e2);
            }
        }

    }

}
