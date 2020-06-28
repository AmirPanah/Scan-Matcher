using System;
using System.Text;
using System.Collections.Generic;
using ScanMatchers.Math;

namespace ScanMatchers.ScanMatcher.GSlamBase
{

    public class Line
    {
        public Vector2 head;
        public Vector2 tail;
        public float a;
        public float b;
        public float err;
        public int laser_start;
        public int laser_end;

        public Line()
        {

        }

        public Line(Line t)
        {
            head = new Vector2(t.head);
            tail = new Vector2(t.tail);
            a = t.a;
            b = t.b;
            err = t.err;
            laser_start = t.laser_start;
            laser_end = t.laser_end;
        }

        public float AngleBetween(Line l)
        {
            Vector2 t1 = head - tail, t2 = l.head - l.tail;
            return MathHelper.AngleBetweenLines(t1, t2);
        }

    }

}
