using System;
using System.Text;
using System.Collections.Generic;

using ScanMatchers.Math;

namespace ScanMatchers.Tools.USARSimMessages
{

    public class INS
    {
        //SEN {Type INS} {Name INS} {Location 4.50,1.90,1.80} {Orientation 0.00,0.00,0.00}
        public Vector3 p3Ori = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 p3Loc = new Vector3(0.0f, 0.0f, 0.0f);

        public INS(INS t)
        {
            if (t == null) return;

            this.p3Ori = new Vector3(t.p3Ori);
            this.p3Loc = new Vector3(t.p3Loc);
        }

        public INS(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            float[] curLocation = USARParser.parseFloats(msg.getSegment("Location").Get("Location"), ",");
            float[] curRotation = USARParser.parseFloats(msg.getSegment("Orientation").Get("Orientation"), ",");
            normalRotation(curRotation);

            p3Loc = new Vector3(curLocation[0], curLocation[1], curLocation[2]);
            p3Ori = new Vector3(curRotation[0], curRotation[1], curRotation[2]);
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
    }

}
