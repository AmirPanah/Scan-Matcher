using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;

using ScanMatchers.Math;
using ScanMatchers.Tools.Structures;

namespace ScanMatchers.Tools.USARSimMessages
{

    public class Laser : ICloneable, ILaserRangeData
    {
        //SEN {Time 2202.98} {Type RangeScanner} {Name Scanner1} {Resolution 0.0174} {FOV 3.1415} {Range 2.1951,2.1962,2.1956,2.1956,2.1993,2.2015,2.2067,2.2097,2.2144,2.2203,2.2294,2.2334,2.2413,2.2541,2.2613,2.2699,2.2846,2.2954,2.3065,2.3206,2.3368,2.3494,2.3688,2.3849,2.4041,2.4215,2.4408,2.4648,2.4878,2.5092,2.5368,2.5589,2.5872,2.6163,2.6484,2.6780,1.4325,1.4145,1.3820,1.3527,1.3227,1.2958,1.2705,1.2473,1.2244,1.2025,1.1810,1.1626,1.1425,1.1242,1.1087,1.0923,1.0774,1.0629,1.0485,1.0343,1.0219,1.0097,0.9982,0.9879,0.9782,0.9684,0.9586,0.9509,0.9417,0.9334,0.9259,0.9187,0.9130,0.9070,0.9007,0.8949,0.8889,0.8851,0.8797,0.8752,0.8715,0.8681,0.8554,0.8513,0.9473,1.0547,1.1911,1.3619,1.6718,1.8432,1.8444,1.8429,1.8407,1.8433,1.8417,1.8410,1.8438,1.8451,1.8445,1.8496,1.8535,1.8543,1.8614,1.8627,1.8689,1.8747,1.8845,1.8907,1.8967,1.9074,1.9146,1.9261,1.9387,1.9499,1.9607,1.9737,1.9861,2.0013,2.0177,2.0338,2.0519,2.0693,2.0870,2.1078,2.1294,2.1485,2.1733,2.1978,2.2253,2.2507,2.2809,2.3089,2.3404,2.2683,2.3286,2.3899,2.4588,2.5207,2.5459,2.6038,2.6365,2.6646,2.7001,1.6763,1.6999,1.7366,1.7747,1.8151,1.8604,1.9063,1.9553,2.0100,2.0627,2.1237,2.1878,2.2837,2.3634,2.4398,2.4830,4.3756,4.5520,4.7475,4.9563,4.9250,4.7569,4.6105,4.4652,4.3296,4.2058,4.0943,3.9799,4.8007,4.7817,4.7704,4.7562,4.7421,4.7276,4.7165,4.7069,4.6971,4.6928,4.6877,4.6875,4.6913,4.6849}
        public List<double> fTheta;
        public List<double> fRanges;
        public List<bool> bFilters;

        public string fName = "";
        public float fResolution = 0.0f;
        public float fFov = 0.0f;
        public float fTime = -1.0f;
        public float fAng0 = 0.0f;

        /// <summary>
        /// laser position in global frame
        /// </summary>
        public Pose2D pos = new Pose2D(); //global frame

        /// <summary>
        /// robot position in global frame
        /// </summary>
        public Pose2D posRobot = new Pose2D(); //global frame

        public static float min_range = 0.0f;
        public static float max_range = 10f;
        public sbyte scan_mask;


        private string validateString = "";
        private string thetaString = "";
        private string rangesString = "";

        public Laser(Laser t)
        {
            if (t == null) return;

            fRanges = new List<double>();
            fRanges.AddRange(t.fRanges);

            fTheta = new List<double>();
            fTheta.AddRange(t.fTheta);

            bFilters = new List<bool>();
            bFilters.AddRange(t.bFilters);

            this.rangesString = t.rangesString;
            this.fName = t.fName;
            this.fAng0 = t.fAng0;
            this.fTime = t.fTime;
            this.fFov = t.fFov;
            this.fResolution = t.fResolution;
            this.pos = new Pose2D(t.pos.Position, t.pos.Rotation);
            this.posRobot = new Pose2D(t.posRobot.Position, t.posRobot.Rotation);
            this.scan_mask = t.scan_mask;
        }

        public Laser(USARParser msg)
        {
            if (msg.size == 0 || msg.segments == null) return;
            ParseState(msg);
        }

        private void ParseState(USARParser msg)
        {
            fName = msg.getSegment("Name").Get("Name");
            fFov = float.Parse(msg.getSegment("FOV").Get("FOV"));
            fResolution = float.Parse(msg.getSegment("Resolution").Get("Resolution"));

            rangesString = msg.getSegment("Range").Get("Range");
            fRanges = new List<double>();
            fRanges.AddRange(USARParser.parseDoubles(rangesString, ","));

            validateString = msg.getString("Valid");
            if (!string.IsNullOrEmpty(validateString))
            {
                bFilters = new List<bool>();
                bFilters.AddRange(USARParser.parseBools(validateString, ",", true));
            }
            else
            {
                bFilters = new List<bool>();
                foreach (float range in fRanges)
                    bFilters.Add(range < min_range || range > max_range);
            }

            if (!string.IsNullOrEmpty(thetaString))
                fAng0 = (float)fTheta[0];
            else
                fAng0 = ((float)System.Math.PI / 2 - fFov / 2);

            thetaString = msg.getString("Theta");
            if (!string.IsNullOrEmpty(thetaString))
            {
                fTheta = new List<double>();
                fTheta.AddRange(USARParser.parseDoubles(thetaString, ","));
            }
            else
            {
                fTheta = new List<double>();
                for (int i = 0; i < fRanges.Count; i++)
                    fTheta.Add(fAng0 + i * fResolution);
            }

            if (msg.getSegment("Time") != null)
                fTime = float.Parse(msg.getSegment("Time").Get("Time"));

            scan_mask = 1;
        }

        public float GetMaxBeam()
        {
            float maxBeam = float.MinValue;
            foreach (float d in fRanges)
            {
                maxBeam = System.Math.Max(maxBeam, d);
            }
            return maxBeam;
        }

        public object Clone()
        {
            Laser obj = new Laser(this);
            return obj;
        }

        public override string ToString()
        {
            string ret = "";

            ret = string.Format("Range {{FOV {0}}} {{RES {1}}} {{POS {2}}} {{POSR {3}}} {{RANGES {4}}}",
                                fFov.ToString(), fResolution.ToString(), pos.ToString().Replace(" ", "#"),
                                posRobot.ToString().Replace(" ", "#"), rangesString);
            return ret;
        }

        #region ILaserRangeData Members

        float ILaserRangeData.MaxRange
        {
            get { return max_range; }
        }

        float ILaserRangeData.MinRange
        {
            get { return min_range; }
        }

        float ILaserRangeData.OffsetX
        {
            get { return (float)pos.X; }
        }

        float ILaserRangeData.OffsetY
        {
            get { return (float)pos.Y; }
        }

        double ILaserRangeData.FieldOfView
        {
            get { return fFov; }
        }

        double[] ILaserRangeData.Range
        {
            get { return fRanges.ToArray(); }
        }

        double[] ILaserRangeData.RangeTheta
        {
            get { return fTheta.ToArray(); }
        }

        bool[] ILaserRangeData.RangeFilters
        {
            get { return bFilters.ToArray(); }
        }

        double ILaserRangeData.Resolution
        {
            get { return fResolution; }
        }

        #endregion
    }

}
