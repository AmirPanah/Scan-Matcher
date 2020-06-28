using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScanMatchers.Math;
using System.Threading;

namespace ScanMatchers.ScanMatcher.Base
{
    public class Debug
    {
        public delegate void DrawVectors(Vector[] v, int id);
        public delegate void DrawRelations(Correlation<Vector2>[] r);

        public static DrawVectors DrawVectorFunction;
        public static DrawRelations DrawRelationFunction;

        public static volatile int SLEEP_ON_DEBUG = 10;

        public static void DrawPoints(Vector[] v, int id)
        {
            if (DrawVectorFunction != null)
                DrawVectorFunction(v, id);
        }

        public static void DrawPointRelations(Correlation<Vector2>[] r)
        {
            if (DrawRelationFunction != null)
            {
                DrawRelationFunction(r);
                Thread.Sleep(SLEEP_ON_DEBUG);
            }


        }

    }
}
