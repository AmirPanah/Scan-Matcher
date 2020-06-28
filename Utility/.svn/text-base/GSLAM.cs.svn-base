using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using ScanMatchers.Math;
using ScanMatchers.Tools.USARSimMessages;
using ScanMatchers.ScanMatcher.GSlamBase;
using ScanMatchers.ScanMatcher.Base;

namespace ScanMatchers.ScanMatcher
{

    public class GSLAM : IDisposable
    {
        private const float MAX_HEIGHT_TO_COSIDER_LASER_VALID = 0.16f;
        private const float MAX_DETECTED_TRANSFORMATION_TO_CONSIDER_VALID = 0.4f;
        //change 0.9 to 0.5
        private const float MAX_DETECTED_ROTATION_TO_CONSIDER_VALID = 0.5f;
        private const float MAX_DISTANCE_TO_COUNT_FOR_CORROLATION = 0.5f;

        private const float ADDITIONAL_ERR_TOLERANCE_NEIGHBOR_CORROLATION = 0f;
        private const float PI = (float)System.Math.PI;

        private const bool USE_NEW_CORROLATION = true;
        private const bool UPDATE_EDGES = true;

        private const float MAX_CORROLATION_TO_CONSIDER_MATCH = USE_NEW_CORROLATION ? 0.1f : 0.05f;
        //Set To true,testing getEdgesInNeighbourNew
        private const bool EDGE_IN_NEIGHBOUR_FINDING_NEW = true;

        private float MAX_DISTANCE_TO_MATCH = 1f;

        //private Point3D RobotRot;
        //private Point3D RobotPos;
        private Pose2D RobotCURState;

        private Laser LaserData;
        private List<Edge> NewEdges;
        private List<Edge> MapEdges;
        private List<Line> NewLines;
        private List<Line> MapLines = new List<Line>();

        private bool isInit;
        private bool ReservedValidData;
        private bool hasValidOdometryForEvaluation = false;

        //private Vector3 center = new Point3D(0, 0, 0);
        //private Point3D[] Changes;
        private Pose2D DeltaPose;

        private LineMatchResult currentMatch;

        private FeatureExtractor featureExtractor = new FeatureExtractor();
        private EdgeMatcher edgeMatcher;
        private EdgeTrimmer edgeTrimmer;

        private int timeCycle = 0;
        public GSLAM()
        {
            LaserData = null;
            NewEdges = new List<Edge>();
            MapEdges = new List<Edge>();
            NewLines = new List<Line>();
            RobotCURState = new Pose2D();

            ReservedValidData = false;
            isInit = false;

            edgeMatcher = new EdgeMatcher();
            edgeTrimmer = new EdgeTrimmer();
        }

        public void Dispose()
        {
            NewLines.Clear();
            NewLines = null;

            NewEdges.Clear();
            NewEdges = null;

            GC.Collect();
        }

        public void updateValues(Laser l, Pose2D curPose)
        {
            //RobotPos = new Point3D(pos);
            //RobotRot = new Point3D(rot);
            ////Add For KF
            //KRobotPos = new Point3D(pos);
            //KRobotRot = new Point3D(rot);
            RobotCURState = new Pose2D(curPose.Position, curPose.Rotation);

            ReservedValidData = (l != null);
            LaserData = l;

            currentMatch = null;
            timeCycle++;
        }

        public int slamFailed = 0;
        public int continuousSlamFailureCycles = 0;
        private const int TRIMING_PERIOD = 50;

        private const bool ADD_FEATURES_IF_SLAM_FAILED = true;
        private const bool RETRY_IF_SLAM_FAILED = true;
        private const bool RETRY_IF_NON_VALID_TRANS = true;

        public int DoSLAM()
        {
            if (!ReservedValidData) return -4;

            //featureExtractor.updateValues(RobotPos, RobotRot, LaserData, timeCycle);
            featureExtractor.updateValues(RobotCURState, LaserData, timeCycle);

            NewLines = featureExtractor.ExtractParts();
            NewEdges = featureExtractor.FindEdge();

            // Calculate Odometry
            if (!isInit)
            {
                MapEdges.Clear();
                float d;
                foreach (Edge e in NewEdges)
                {
                    d = MathHelper.GetDistance(e.point, RobotCURState.Position); //e.point.getDistance2D(RobotPos);
                    if (d < MAX_DIST_EDGE_ROBOT_TO_ADD_NEW_EDGE)
                        MapEdges.Add(e);
                }
                isInit = !isInit;

                return 1;
            }
            else
            {
                if (timeCycle % TRIMING_PERIOD == 0)
                    edgeTrimmer.trimMapEdges(MapEdges, timeCycle);

                Debug.DrawPoints(GetEdgeVector(MapEdges), 1);
                Debug.DrawPoints(GetEdgeVector(NewEdges), 2);

                currentMatch = GetMovementCorrection(MapEdges, NewEdges);

                if (currentMatch != null)
                    DeltaPose = currentMatch.changes;
                else
                    DeltaPose = null;

                hasValidOdometryForEvaluation = false;
                continuousSlamFailureCycles++;

                if (DeltaPose != null)
                {
                    if (NewEdges.Count > 1)
                    {
                        if (isChangeValid(DeltaPose))
                        {
                            RobotCURState.Position += DeltaPose.Position;
                            RobotCURState.Rotation += DeltaPose.Rotation;

                            hasValidOdometryForEvaluation = true;
                            getNewEdgesRegardingNewEstimatedChanges();
                            UpdateEdges(NewEdges, currentMatch.matchedEdges);// Age BEkhaym Taghiri Roosh BEdim In Update Edges Kharab Mishe.
                            continuousSlamFailureCycles = 0;

                            return 0;
                        }
                        else
                        {
                            if (RETRY_IF_SLAM_FAILED && RETRY_IF_NON_VALID_TRANS && retryIfSlamFailed())
                            {
                                hasValidOdometryForEvaluation = true;
                                return 0;
                            }
                            else
                                return -1;
                        }
                    }
                    else
                    {
                        return -2;
                    }
                }
                else
                {
                    return -1;
                }
            }
        }

        private Vector[] GetEdgeVector(List<Edge> el)
        {
            List<Vector> edges = new List<Vector>();
            
            foreach (Edge e in el)
            {
                edges.Add(e.point);
            }

            return edges.ToArray();
        }


        private const float MAX_DIST_EDGE_ROBOT_TO_ADD_NEW_EDGE = 15f;
        private void UpdateEdges(List<Edge> newEdges, Dictionary<Edge, Edge> matchedEdges)
        {
            List<Edge> unmatchedEdges = new List<Edge>();
            Edge ne, oe = null;
            float dist, best;
            bool found;
            KeyValuePair<Edge, Edge> match;
            float distRobot;
            foreach (Edge e in newEdges)
            {
                found = false;
                best = float.MaxValue;
                distRobot = MathHelper.GetDistance(RobotCURState.Position, e.point); //RobotPos.getDistance2D(e.point);

                if (distRobot >= MAX_DIST_EDGE_ROBOT_TO_ADD_NEW_EDGE)
                    continue;

                foreach (KeyValuePair<Edge, Edge> kv in matchedEdges) // NEW , OLD
                {
                    ne = kv.Key;
                    oe = kv.Value;
                    dist = MathHelper.GetDistance(ne.point, e.point); //ne.point.getDistance2D(e.point);
                    if (dist < best)
                        best = dist;
                    if (dist < FeatureExtractor.MAX_DIST_TO_CONSIDER_POINTS_EQUAL_UPDATE)
                    {
                        match = kv;
                        match.Value.updateObservation(RobotCURState.Position, match.Key.point, timeCycle);
                        found = true;
                        break;
                    }
                }

                if (!found)
                    unmatchedEdges.Add(e);
            }

            foreach (Edge ue in unmatchedEdges)
            {
                found = false;
                foreach (Edge med in MapEdges)
                {
                    if (edgeMatcher.canEdgesMatch(ue, med, FeatureExtractor.MIN_DISTANCE_BETWEEN_EDGES))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    MapEdges.Add(ue);
                }
            }
        }

        private List<Edge> getEdgesInNeighbourNew(Edge newEdge, float maxDistance)
        {
            List<Edge> res = new List<Edge>();
            foreach (Edge edge in MapEdges)
            {
                if (!edgeMatcher.canEdgesMatch(newEdge, edge, maxDistance))
                    continue;
                res.Add(edge);
            }
            return res;
        }

        private List<Edge> getEdgesInNeighbourOld(Edge newEdge, float distance)
        {
            MAX_DISTANCE_TO_MATCH = LaserData.GetMaxBeam() * EdgeMatcher.MAX_ANGLE_TO_MATCH;
            List<Edge> res = new List<Edge>();
            float dist;
            foreach (Edge edge in MapEdges)
            {
                dist = MathHelper.GetDistance(edge.point, newEdge.point); //edge.point.getDistance2D(newEdge.point);
                if (dist < distance)
                {
                    if (System.Math.Abs(newEdge.angleBetweenLines - edge.angleBetweenLines) <
                        EdgeMatcher.MAX_ANGLE_TO_MATCH)
                        res.Add(edge);
                }
            }
            return res;
        }

        private Pose2D getConversions(Edge oldEdge, Edge newEdge) // Rotation , Location
        {
            Pose2D res = new Pose2D();

            if (newEdge.type == EdgeType.EndOfWall || oldEdge.type == EdgeType.EndOfWall)
            {
                if (newEdge.type != oldEdge.type)
                {
                    return null;
                }
            }

            if (newEdge.type == EdgeType.EndOfWall)
            {
                Vector2 vo, vn;
                if (newEdge.point == newEdge.lines[0].head)
                {
                    vo = oldEdge.lines[0].tail - oldEdge.lines[0].head;
                    vn = newEdge.lines[0].tail - newEdge.lines[0].head;
                }
                else // == tail
                {
                    vo = oldEdge.lines[0].head - oldEdge.lines[0].tail;
                    vn = newEdge.lines[0].head - newEdge.lines[0].tail;
                }

                float angle = MathHelper.AngleBetweenVectors(vo, vn);
                if (angle > 2 * PI || angle < -2 * PI || float.IsNaN(angle) || float.IsInfinity(angle))
                {
                    angle = 0;
                }
                res.Rotation = -angle; // angle I Ke Tooye Res E Bayad Inghadr Charkhoonde Beshe 

                Vector2 nt = MathHelper.TranformVector(newEdge.point, Vector2.Zero, new Pose2D(RobotCURState.Position, res.GetNormalizedRotation()));
                Vector2 Trans = oldEdge.point - nt;

                res.Position = Trans;
            }
            else
            {
                Vector2 vo1, vo2, vn1, vn2;

                float dph0 = MathHelper.GetDistance(newEdge.point, newEdge.lines[0].head), //newEdge.point.getDistance2D(newEdge.lines[0].head),
                      dpt0 = MathHelper.GetDistance(newEdge.point, newEdge.lines[0].tail), //newEdge.point.getDistance2D(newEdge.lines[0].tail),
                      dph1 = MathHelper.GetDistance(newEdge.point, newEdge.lines[1].head), //newEdge.point.getDistance2D(newEdge.lines[1].head),
                      dpt1 = MathHelper.GetDistance(newEdge.point, newEdge.lines[1].tail); //newEdge.point.getDistance2D(newEdge.lines[1].tail);

                if (dph0 < dpt0)
                {
                    vo1 = oldEdge.lines[0].tail - oldEdge.lines[0].head;
                    vn1 = newEdge.lines[0].tail - newEdge.lines[0].head;
                }
                else // == tail
                {
                    vo1 = oldEdge.lines[0].head - oldEdge.lines[0].tail;
                    vn1 = newEdge.lines[0].head - newEdge.lines[0].tail;
                }

                if (dph1 < dpt1)
                {
                    vo2 = oldEdge.lines[1].tail - oldEdge.lines[1].head;
                    vn2 = newEdge.lines[1].tail - newEdge.lines[1].head;
                }
                else // == tail
                {
                    vo2 = oldEdge.lines[1].head - oldEdge.lines[1].tail;
                    vn2 = newEdge.lines[1].head - newEdge.lines[1].tail;
                }

                float angle1 = MathHelper.AngleBetweenVectors(vo1, vn1), angle2 = MathHelper.AngleBetweenVectors(vo2, vn2);
                float angle = (angle1 + angle2) / 2f;
                res.Rotation = -angle;

                Vector2 nt = MathHelper.TranformVector(newEdge.point, Vector2.Zero, new Pose2D(RobotCURState.Position,  res.Rotation));
                Vector2 Trans = oldEdge.point - nt;

                res.Position = Trans;
            }

            return res;
        }

        private LineMatchResult GetMovementCorrection(List<Edge> MapEdges, List<Edge> NewEdges)
        {
            List<Edge> possibleMatches;

            Pose2D conv;
            float best = float.MaxValue;
            List<LineMatchResult> matches = new List<LineMatchResult>();
            LineMatchResult lmt;
            float maxDisttoMatch;
            foreach (Edge edge in NewEdges)
            {
                if (EDGE_IN_NEIGHBOUR_FINDING_NEW)
                {
                    maxDisttoMatch = getMaxDistanceToMatchNeighbourNewEdges(edge);
                    possibleMatches = getEdgesInNeighbourNew(edge, maxDisttoMatch);
                }
                else
                {
                    possibleMatches = getEdgesInNeighbourOld(edge, MAX_DISTANCE_TO_MATCH);
                }

                foreach (Edge ted in possibleMatches)
                {
                    conv = getConversions(ted, edge);
                    if (conv == null)
                        continue;

                    lmt = edgeMatcher.getCorrelation_NEW(conv, NewEdges, MapEdges, RobotCURState.Position);

                    if (lmt.corrolation == float.NaN)
                    {
                        continue;
                    }

                    matches.Add(lmt);
                }
            }

            if (matches.Count == 0)
                return null;

            float curCount = NewEdges.Count;
            List<LineMatchResult> tt = new List<LineMatchResult>();
            do
            {
                foreach (LineMatchResult ch in matches)
                {
                    if (ch.count == curCount && ch.corrolation < MAX_CORROLATION_TO_CONSIDER_MATCH) // 
                    {
                        if (isChangeValid(ch.changes))
                            tt.Add(ch);
                    }
                }
                curCount--;
            } while (curCount > 0 && tt.Count == 0); // FAQAT Ta Vaghty Ke Ba Maximum Te'dad Javab Begire.
            curCount++;

            if (tt.Count == 0)
                return null;

            best = float.MaxValue;
            LineMatchResult reslm = null;
            foreach (LineMatchResult mt in tt)
            {

                if (mt.corrolation < best)
                {
                    best = mt.corrolation;
                    reslm = mt;
                }
            }

            return reslm;
        }

        private void getNewEdgesRegardingNewEstimatedPositions()
        {
            featureExtractor.updateValues(RobotCURState, LaserData, timeCycle);
            //GenerateLaserVectors();
            NewLines = featureExtractor.ExtractParts();
            NewEdges = featureExtractor.FindEdge();
            MapLines.AddRange(NewLines);
        }

        private void getNewEdgesRegardingNewEstimatedChanges()
        {
            if (currentMatch != null) NewEdges = currentMatch.newEdges;
        }

        private const float ADDITIONAL_NEIGHBOUR_SEARCHING_EDGES_IF_FAILURE = 0.5f;
        private const float MAX_NEIGHBOUR_SEARCHING_RADIUS_FAILURE = 1.5f;
        private float getMaxDistanceToMatchNeighbourNewEdges(Edge edge)
        {
            float errDist = edge.MaxDistanceToMatch;
            float robotDist = MathHelper.GetDistance(RobotCURState.Position, edge.point); //RobotPos.getDistance2D(edge.point);
            float distRot = robotDist * MAX_DETECTED_ROTATION_TO_CONSIDER_VALID; // ESTIMATE OF SIN Around 0 up To 20 Degrees!
            float res = MAX_DETECTED_TRANSFORMATION_TO_CONSIDER_VALID + distRot + errDist + ADDITIONAL_ERR_TOLERANCE_NEIGHBOR_CORROLATION;
            //res = Math.Max(1f, res);
            int fail = continuousSlamFailureCycles + retryCycle;
            if (fail > 0)
            {
                res += ADDITIONAL_NEIGHBOUR_SEARCHING_EDGES_IF_FAILURE;
                res += (float)System.Math.Min(EACH_CYCLE_TRANS_FAILURE_TOLERANCE * fail, MAX_NEIGHBOUR_SEARCHING_RADIUS_FAILURE);
            }

            return res;
        }

        private Dictionary<Edge, Edge> getMatchesBetween(List<Edge> MapEdges, List<Edge> NewEdges)
        {
            return null;
        }


        // CHANGES Related To New Version

        public void resetDif()
        {
            DeltaPose = new Pose2D();
        }

        public Pose2D Dif
        {
            get
            {
                if (hasValidOdometryForEvaluation)
                {
                    return DeltaPose;
                }
                else
                {
                    return null;
                }
            }
        }

        // CHANGES Related To New Version



        // Version 1.8

        private const float EACH_CYCLE_TRANS_FAILURE_TOLERANCE = 0.1f;
        private const float EACH_CYCLE_ROT_FAILURE_TOLERANCE = 0.1f;
        private const float MAX_TRANS_FAILURE_TOLERANCE = 0.5f;
        private const float MAX_ROT_FAILURE_TOLERANCE = 0.5f;
        private float[] getMaxDetectedValidTransformation() // 1 = Transformation , 0 = Rotation
        {
            float[] res = new float[2];
            res[0] = MAX_DETECTED_ROTATION_TO_CONSIDER_VALID;
            res[1] = MAX_DETECTED_TRANSFORMATION_TO_CONSIDER_VALID;

            int fail = continuousSlamFailureCycles + retryCycle;
            if (fail > 1) // Failed Previous Cycle(s)
            {
                res[0] += (float)System.Math.Min(EACH_CYCLE_ROT_FAILURE_TOLERANCE * fail,
                                                 MAX_ROT_FAILURE_TOLERANCE);

                res[1] += (float)System.Math.Min(EACH_CYCLE_TRANS_FAILURE_TOLERANCE * fail,
                                                 MAX_TRANS_FAILURE_TOLERANCE);
            }

            if (res[0] > 0.87)
                res[0] = MAX_DETECTED_ROTATION_TO_CONSIDER_VALID;

            return res;
        }

        private const float RETRY_MODE_MAX_DETECTED_VALID_TRANS = 1f;
        private const float RETRY_MODE_MAX_DETECTED_VALID_ROT = 1f;
        private bool isChangeValid(Pose2D Changes)
        {
            float[] validChanges;

            validChanges = getMaxDetectedValidTransformation();

            bool changesValid = (MathHelper.VectorLength(Changes.Position) < validChanges[1]) &&
                                (System.Math.Abs(Changes.GetNormalizedRotation()) < validChanges[0]);

            return changesValid;
        }

        private const int MIN_NUM_EDGES_TO_ADD_IF_SLAM_FAILED = 4;
        private const int MIN_FAILURE_CYCLES_TO_ADD_EDGES = 1;
        private const int FAILURE_CYCLES_TO_ADD_EDGES_PERIOD = 1;
        private void addFeaturesIfSlamFailed()
        {
            if (NewEdges.Count < MIN_NUM_EDGES_TO_ADD_IF_SLAM_FAILED)
                return;

            if (continuousSlamFailureCycles < MIN_FAILURE_CYCLES_TO_ADD_EDGES)
                return;

            Dictionary<Edge, Edge> matched;
            if (currentMatch != null && currentMatch.matchedEdges != null)
                matched = currentMatch.matchedEdges;
            else
                matched = new Dictionary<Edge, Edge>();

            UpdateEdges(NewEdges, matched);
        }

        private bool retryMode = false;
        private int retryCycle = 0;
        private const int MAX_RETRY_CYCLE = 2;

        private bool retryIfSlamFailed()
        {
            retryMode = true;
            for (retryCycle = 0; retryCycle < MAX_RETRY_CYCLE; retryCycle++)
            {
                LineMatchResult lm = GetMovementCorrection(MapEdges, NewEdges);
                Pose2D Changes;

                if (lm != null)
                    Changes = lm.changes;
                else
                    Changes = null;

                hasValidOdometryForEvaluation = false;
                if (Changes != null)
                {
                    if (NewEdges.Count > 1)
                    {
                        if (isChangeValid(Changes))
                        {
                            currentMatch = lm;
                     
                            RobotCURState.Position += Changes.Position;
                            RobotCURState.Rotation += Changes.Rotation;

                            hasValidOdometryForEvaluation = true;
                            getNewEdgesRegardingNewEstimatedChanges();
                            UpdateEdges(NewEdges, lm.matchedEdges);    // Age BEkhaym Taghiri Roosh BEdim In Update Edges Kharab Mishe.
                            continuousSlamFailureCycles = 0;

                            break;
                        }
                    }
                }
            }

            retryMode = false;
            return hasValidOdometryForEvaluation;
        }

    }
}


