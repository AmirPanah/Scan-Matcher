using System;
using System.Text;
using System.Collections.Generic;

namespace ScanMatchers.ScanMatcher.GSlamBase
{
    public class EdgeTrimmer
    {
        public EdgeTrimmer()
        {
        }

        private const bool REMOVE_UNACCURATE_EDGES = true;
        private const bool REMOVE_VERY_OLD_HIGH_EDGES = true;


        private const float TRIM_MIN_ACCURACY_TO_KEEP_UNACCURATE_EDGE = 0.2f;
        private const float TRIM_MIN_ACCURACY_VERY_OLD_HIGH = 0.8f;
       
        private const int TRIM_MIN_UPDATE_CYCLE_UNACCURATE_EDGES = 10;
        private const int TRIM_MIN_PASSED_CYCLE_FROM_CREATION = 100;
        
        private const int TRIM_NUM_EDGES_UB = 150;
        
        private const int TRIM_MIN_PASSED_TIME_LAST_UPDATE_VERY_OLD_EDGES = 250;
        private const int TRIM_MIN_UPDATE_CYCLE_VERY_OLD_HIGH = 100;

        private int totalRemovedUnaccurateEdges = 0;
        private int totalRemovedVeryOldHighEdges = 0;

        public void trimMapEdges(List<Edge> MapEdges, int timeCycle)
        {
            int age, lastUpdateAge;
            List<Edge> toRemoveVeryOldHigh = new List<Edge>();
            List<Edge> toRemoveUnaccurate = new List<Edge>();
            foreach (Edge e in MapEdges)
            {
                age = timeCycle - e.timeCycleCreated;
                if (age < TRIM_MIN_PASSED_CYCLE_FROM_CREATION)
                    continue;

                if(REMOVE_UNACCURATE_EDGES)
                {
                    if (e.AccuracyOfEdgeP < TRIM_MIN_ACCURACY_TO_KEEP_UNACCURATE_EDGE ||
                        e.updateTime < TRIM_MIN_UPDATE_CYCLE_UNACCURATE_EDGES)
                    {
                        toRemoveUnaccurate.Add(e);
                        continue;
                    }
                }

                lastUpdateAge = timeCycle - e.lastUpdatedTimeCycle;

                if (REMOVE_VERY_OLD_HIGH_EDGES)
                {
                    if (MapEdges.Count < TRIM_NUM_EDGES_UB)
                        continue;

                    if (lastUpdateAge < TRIM_MIN_PASSED_TIME_LAST_UPDATE_VERY_OLD_EDGES)
                        continue;

                    if (e.AccuracyOfEdgeP < TRIM_MIN_ACCURACY_VERY_OLD_HIGH ||
                        e.updateTime < TRIM_MIN_UPDATE_CYCLE_VERY_OLD_HIGH)
                    {
                        toRemoveVeryOldHigh.Add(e);
                        continue;
                    }
                }


            }
            
            totalRemovedVeryOldHighEdges += toRemoveVeryOldHigh.Count;
            totalRemovedUnaccurateEdges += toRemoveUnaccurate.Count;
            
            List<Edge> toRemove = new List<Edge>();
            
            toRemove.AddRange(toRemoveVeryOldHigh);
            toRemove.AddRange(toRemoveUnaccurate);
            foreach (Edge e in toRemove)
            {
                MapEdges.Remove(e);
            }
            //Console.WriteLine("TOTAL MAP EDGES : " + MapEdges.Count);
            //Console.WriteLine("TOTAL REMOVED UNACCURATE EDGES : " + totalRemovedUnaccurateEdges);
            //Console.WriteLine("TOTAL REMOVED Very OLD High EDGES : " + totalRemovedVeryOldHighEdges);

        }


        private const int NEW_EDGES_LB = 15;
        private const int NEW_EDGES_MB = 20;
        private const int NEW_EDGES_UB = 30;
        public void trimNewEdges(List<Edge> orig)
        {
            if (orig.Count <= NEW_EDGES_UB)
                return;
            List<Edge> res = orig;
            //if (orig.Count > NEW_EDGES_UB)
            {
                //res = trimEdgesIterative(orig, NEW_EDGES_UB, NEW_EDGES_MB);
            }
            //if (res.Count > NEW_EDGES_MB)
            {
                res = trimToSize(res, NEW_EDGES_UB);
                //Console.WriteLine("Orig Size : " + orig.Count + " Trimmed Size : " + res.Count);
            }

            return;
        }

        private List<Edge> trimEdges(List<Edge> orig, float minAccuracy)
        {
            List<Edge> res = new List<Edge>();
            foreach (Edge e in orig)
            {
                if (e.AccuracyOfNewObservation > minAccuracy)
                {
                    res.Add(e);
                }
            }
            return res;
        }

        private const float TRIM_IT_STEP = 0.05f;
        private List<Edge> trimEdgesIterative(List<Edge> orig, int max, int min)
        {
            List<Edge> res = orig;
            float maxA = 0, minA = 1;
            foreach (Edge e in orig)
            {
                if (e.AccuracyOfNewObservation > maxA)
                    maxA = e.AccuracyOfNewObservation;
                if (e.AccuracyOfNewObservation < minA)
                    minA = e.AccuracyOfNewObservation;
            }

            for (float ac = maxA; ac >= 0.1f; ac -= TRIM_IT_STEP)
            {
                res = trimEdges(orig, ac);
                if (res.Count <= max && res.Count >= min)
                {
                    Console.WriteLine("Orig Size : " + orig.Count + " Accuracy : " + ac + " size : " + res.Count);
                    return res;
                }
            }
            return orig;
        }

        private List<Edge> trimToSize(List<Edge> orig, int size)
        {
            List<Edge> sorted = sortByAccuracy(orig);
            Edge e;
            List<Edge> res = new List<Edge>();
            for (int i = 0; i < size; i++)
            {
                e = sorted[i];
                res.Add(e);
            }

            return res;
        }

        private List<Edge> sortByAccuracy(List<Edge> orig)
        {
            Edge[] oe = orig.ToArray();
            Edge e, ne, t;
            int len = orig.Count - 1;
            for (int i = 0; i < len; i++)
            {
                for (int j = i + 1; j <= len; j++)
                {
                    e = oe[i];
                    ne = oe[j];
                    if (e.AccuracyOfNewObservation < ne.AccuracyOfNewObservation)
                    {
                        oe[i] = ne;
                        oe[j] = e;
                    }
                }
            }
            List<Edge> res = new List<Edge>(oe);
            return res;
        }
    }
}
