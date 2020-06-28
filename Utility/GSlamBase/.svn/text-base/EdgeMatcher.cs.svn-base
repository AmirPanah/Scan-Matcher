using System;
using System.Text;
using System.Collections.Generic;
using ScanMatchers.Math;

namespace ScanMatchers.ScanMatcher.GSlamBase
{
    public class EdgeMatcher
    {
        private const float ADDITIONAL_ERR_TOLERANCE_MATCH_CORROLATION = 0.2f;
        public const float MAX_ANGLE_TO_MATCH = 20f * 0.0175f;

        public EdgeMatcher()
        {

        }

        private float getMaxDistanceToMatchCorrolation(Edge newEdge, Edge oldEdge)
        {
            return newEdge.MaxDistanceToMatch + oldEdge.MaxDistanceToMatch + ADDITIONAL_ERR_TOLERANCE_MATCH_CORROLATION;
        }

        private Dictionary<Edge, Edge> calculateCurrentMatchesUnsorted(List<Edge> NewEdges, List<Edge> MapEdges)
        {
            float dist = 0.0f;
            float best = 0.0f;
            Edge bestEdge = null;

            float maxDistanceToMatch;
            bool found;
            List<Edge> calculatedMapEdges = new List<Edge>();
            Dictionary<Edge, Edge> res = new Dictionary<Edge, Edge>();
            foreach (Edge ne in NewEdges)
            {
                best = float.MaxValue;
                found = false;
                foreach (Edge oe in MapEdges)
                {


                    //dist = ne.point.getDistance2D(oe.point);
                    maxDistanceToMatch = getMaxDistanceToMatchCorrolation(ne, oe);
                    if (!canEdgesMatch(ne, oe, maxDistanceToMatch))
                        continue;
                    if (dist < best && !calculatedMapEdges.Contains(oe))
                    {
                        best = dist;
                        bestEdge = oe;
                        found = true;
                    }
                }

                if (found)
                {
                    res.Add(ne, bestEdge);
                    calculatedMapEdges.Add(bestEdge);

                }
            }

            return res;
        }

        private Dictionary<Edge, Edge> calculateCurrentMatchesSorted(List<Edge> NewEdges, List<Edge> MapEdges)
        {
            float dist = 0.0f;
            float best = 0.0f;
            Edge bestEdge = null;

            float maxDistanceToMatch;
            bool found;
            List<Edge> calculatedMapEdges = new List<Edge>();
            Dictionary<Edge, Edge> res = new Dictionary<Edge, Edge>();
            List<Edge> sortedEdges = sortEdgesByDistance(NewEdges, MapEdges);
            foreach (Edge ne in sortedEdges)
            {
                best = float.MaxValue;
                found = false;
                foreach (Edge oe in MapEdges)
                {
                    dist = MathHelper.GetDistance(ne.point, oe.point); //ne.point.getDistance2D(oe.point);
                    maxDistanceToMatch = getMaxDistanceToMatchCorrolation(ne, oe);
                    if (dist < maxDistanceToMatch)
                    {
                        if (dist < best && !calculatedMapEdges.Contains(oe))
                        {
                            best = dist;
                            bestEdge = oe;
                            found = true;
                        }
                    }
                }

                if (found)
                {
                    res.Add(ne, bestEdge);
                    calculatedMapEdges.Add(bestEdge);

                }
            }

            return res;
        }

        private Dictionary<Edge, Edge> calculateCurrentMatches(List<Edge> NewEdges, List<Edge> MapEdges)
        {
            return calculateCurrentMatchesUnsorted(NewEdges, MapEdges);
        }

        public LineMatchResult getCorrelation_NEW(Pose2D Changes, List<Edge> NewEdges, List<Edge> MapEdges, Vector2 RobotPos)
        {
            List<Edge> newEdges = convertEdges(NewEdges, Changes, RobotPos);
            Dictionary<Edge, Edge> matchedEdges = calculateCurrentMatches(newEdges, MapEdges);
            float count = matchedEdges.Count;
            float P = 0, sigmaP = 0;
            float res = 0, dist;
            Edge ne, oe;
            foreach (KeyValuePair<Edge, Edge> kv in matchedEdges)
            {
                ne = kv.Key;
                oe = kv.Value;
                dist = MathHelper.GetDistance(ne.point, oe.point); //ne.point.getDistance2D(oe.point);
                //lineAng = Edge.getAngleBetweenLines(ne, oe);
                P = (float)System.Math.Sqrt(ne.AccuracyOfNewObservation * oe.AccuracyOfEdgeP);
                //P = ne.AccuracyOfNewObservation * bestEdge.AccuracyOfEdgeP;
                //res += (dist + lineAng)*P;
                res += (dist) * P;
                sigmaP += P;
            }
            res /= ((float)System.Math.Pow(count, 2) * sigmaP);

            LineMatchResult lmres = new LineMatchResult(res, matchedEdges, Changes, newEdges);

            return lmres;
        }

        private List<Edge> sortEdgesByDistance(List<Edge> NewEdges, List<Edge> MapEdges)
        {
            int count = NewEdges.Count;
            Edge[] se = new Edge[count];
            float[] sev = new float[count];
            float best, dist;
            int cnt = 0;
            foreach (Edge n in NewEdges)
            {
                best = float.MaxValue;
                foreach (Edge oe in MapEdges)
                {
                    dist = MathHelper.GetDistance(n.point, oe.point); //n.point.getDistance2D(oe.point);
                    if (dist < best)
                    {
                        best = dist;
                    }
                }
                se[cnt] = n;
                sev[cnt] = best;
                cnt++;
            }

            Edge ce, ne, t;
            float cev, nev, tv;
            for (int i = 0; i < count; i++) // BUBBLE SORT
            {
                ce = se[i];
                cev = sev[i];
                for (int j = i + 1; j < count; j++)
                {
                    ne = se[j];
                    nev = sev[j];
                    if (cev > nev)
                    {
                        t = ce;
                        tv = cev;
                        se[i] = ne;
                        sev[i] = nev;
                        se[j] = t;
                        sev[j] = tv;
                        ce = ne;
                        cev = nev;

                    }
                }
            }
            List<Edge> res = new List<Edge>(se);
            return res;
        }

        public static List<Edge> convertEdges(List<Edge> Edges, Pose2D Changes, Vector2 RobotPos) // Should Convert Lines
        {
            Edge nEdge;
            List<Edge> res = new List<Edge>();

            foreach (Edge edge in Edges)
            {

                nEdge = Edge.applyRotateShift(edge, Changes, RobotPos);
                res.Add(nEdge);
            }
            return res;
        }

        public bool canEdgeTypesMatch(Edge e1, Edge e2)
        {
            return !(e1.type == EdgeType.Intersection ^ e2.type == EdgeType.Intersection);
        }

        public bool canEdgesMatch(Edge e1, Edge e2, float maxDistance)
        {
            if (!canEdgeTypesMatch(e1, e2))
                return false;

            if ((e1.type == EdgeType.Corner && e2.type == EdgeType.Corner) ||
                (e1.type == EdgeType.Intersection && e2.type == EdgeType.Intersection))
            {
                if (System.Math.Abs(e1.angleBetweenLines - e2.angleBetweenLines) > MAX_ANGLE_TO_MATCH)
                    return false;
            }

            float dist = MathHelper.GetDistance(e1.point, e2.point); //e1.point.getDistance2D(e2.point);
            if (dist > maxDistance)
                return false;
            return true;
        }

    }
}
