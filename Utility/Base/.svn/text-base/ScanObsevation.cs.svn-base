using System;

using ScanMatchers.Tools.Structures;

namespace ScanMatchers.ScanMatcher.Base
{
    [Serializable]
    public class ScanObservation
    {
        // Fields
        private float _Factor;
        private ILaserRangeData _RangeData;

        // Methods
        public ScanObservation(float factor, ILaserRangeData rangeData)
        {
            this._Factor = factor;
            this._RangeData = rangeData;
        }

        // Properties
        public float Factor
        {
            get
            {
                return this._Factor;
            }
        }

        public double FieldOfView
        {
            get
            {
                return this._RangeData.FieldOfView;
            }
        }

        public int Length
        {
            get
            {
                return this.RangeScanner.Range.Length;
            }
        }

        public double MaxRange
        {
            get
            {
                return (double)this._RangeData.MaxRange;
            }
        }

        public double MinRange
        {
            get
            {
                return (double)this._RangeData.MinRange;
            }
        }

        public float OffsetX
        {
            get
            {
                return this._RangeData.OffsetX;
            }
        }

        public float OffsetY
        {
            get
            {
                return this._RangeData.OffsetY;
            }
        }

        public double Resolution
        {
            get
            {
                return this._RangeData.Resolution;
            }
        }

        public ILaserRangeData RangeScanner
        {
            get
            {
                return this._RangeData;
            }
        }
    }

}
