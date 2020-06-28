using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;

using ScanMatchers.Math;

namespace ScanMatchers.Tools.USARSimMessages
{

    public class Odometry
    {
        //SEN {Type Odometry} {Name Odometry} {Pose 0.2415,0.0029,-0.5157}
        public float x = 0.0f;
        public float y = 0.0f;
        public float theta = 0.0f;

        public static implicit operator PointF(Odometry p)
        {
            return new PointF(p.x, p.y);
        }

        public static implicit operator Vector3(Odometry p)
        {
            return new Vector3(p.x, p.y, p.theta);
        }

        public Odometry(Odometry t)
        {
            if (t == null) return;

            this.x = t.x;
            this.y = t.y;
            this.theta = t.theta;
        }

        public Odometry(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            float[] curPose = USARParser.parseFloats(msg.getSegment("Pose").Get("Pose"), ",");
            x = curPose[0];
            y = curPose[1];
            theta = curPose[2];
        }

    }

}
