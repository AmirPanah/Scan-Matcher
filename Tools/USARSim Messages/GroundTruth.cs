using System;
using System.Text;
using System.Collections.Generic;

using ScanMatchers.Math;

namespace ScanMatchers.Tools.USARSimMessages
{

    public class GroundTruth : ICloneable
    {
        //SEN {Time 111.6857} {Type GroundTruth} {Name GroundTruth} {Location 5.69,4.82,-0.26} {Orientation 0.00,6.28,0.00}

        public float fTime = 0.0f;
        public Vector3 p3Loc = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 p3Ori = new Vector3(0.0f, 0.0f, 0.0f);

        public GroundTruth()
        {
        }

        public GroundTruth(GroundTruth t)
        {
            if (t == null) return;
            this.fTime = t.fTime;
            this.p3Loc = new Vector3(t.p3Loc);
            this.p3Ori = new Vector3(t.p3Ori);
        }

        public GroundTruth(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            fTime = float.Parse(msg.getSegment("Time").Get("Time"));

            float[] curLocation = USARParser.parseFloats(msg.getSegment("Location").Get("Location"), ",");
            float[] curRotation = USARParser.parseFloats(msg.getSegment("Orientation").Get("Orientation"), ",");
            normalRotation(curRotation);

            this.p3Loc = new Vector3(curLocation[0], curLocation[1], curLocation[2]);
            this.p3Ori = new Vector3(curRotation[0], curRotation[1], curRotation[2]);
        }

        private void normalRotation(float[] r)
        {
            float pi = (float)System.Math.PI;
            for (int i = 0; i < r.Length; i++)
            {
                if (r[i] > 2 * pi || r[i] < (-2) * pi)
                    r[i] = r[i] % (2 * pi);
                if (r[i] >= pi)
                    r[i] -= 2 * pi;
                if (r[i] < -pi)
                    r[i] += 2 * pi;
            }
        }

        public object Clone()
        {
            GroundTruth obj = new GroundTruth();
            obj.fTime = this.fTime;
            obj.p3Loc = new Vector3(this.p3Loc);
            obj.p3Ori = new Vector3(this.p3Ori);
            return obj;
        }
    }

}
