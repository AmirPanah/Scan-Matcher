using System;
using System.Text;
using System.Collections.Generic;

using ScanMatchers.Math;

namespace ScanMatchers.ScanMatcher.GSlamBase
{
    public class LineMatchResult
    {
        public float corrolation;
        public float count;
        public Pose2D changes;
        public Dictionary<Edge, Edge> matchedEdges; // = new Dictionary<Edge, Edge>();// New Edge , Map Edge
        public List<Edge> newEdges;

        public LineMatchResult(float corrolation, Dictionary<Edge, Edge> matchedEdges, Pose2D changes, List<Edge> newEdges)
        {
            this.corrolation = corrolation;
            this.matchedEdges = matchedEdges;
            this.count = matchedEdges.Count;
            this.changes = changes;
            this.newEdges = newEdges;
        }
    }
}
