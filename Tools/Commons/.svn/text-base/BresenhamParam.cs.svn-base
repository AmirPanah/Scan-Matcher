using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;


namespace ScanMatchers.Tools.Commons
{

    public class BresenhamParam
    {
        internal int X1, Y1;
        internal int X2, Y2;
        internal int Increment;
        internal bool UsingYIndex;
        internal int DeltaX, DeltaY;
        internal int DTerm;
        internal int IncrE, IncrNE;
        internal int XIndex, YIndex;
        internal bool Flipped;

        public Point CurrentPoint
        {
            get
            {
                Point currentPoint = new Point(0, 0);
                if (UsingYIndex)
                {
                    currentPoint.Y = XIndex;
                    currentPoint.X = YIndex;
                    if (Flipped)
                        currentPoint.X = -currentPoint.X;
                }
                else
                {
                    currentPoint.X = XIndex;
                    currentPoint.Y = YIndex;
                    if (Flipped)
                        currentPoint.Y = -currentPoint.Y;
                }

                return currentPoint;
            }

        }

        public BresenhamParam(int p1x, int p1y, int p2x, int p2y)
        {
            this.UsingYIndex = false;

            if (System.Math.Abs((double)(p2y - p1y) / (double)(p2x - p1x)) > 1)
                this.UsingYIndex = true;

            if (this.UsingYIndex)
            {
                this.Y1 = p1x;
                this.X1 = p1y;
                this.Y2 = p2x;
                this.X2 = p2y;
            }
            else
            {
                this.X1 = p1x;
                this.Y1 = p1y;
                this.X2 = p2x;
                this.Y2 = p2y;
            }

            if ((p2x - p1x) * (p2y - p1y) < 0)
            {
                this.Flipped = true;
                this.Y1 = -this.Y1;
                this.Y2 = -this.Y2;
            }
            else
                this.Flipped = false;

            if (this.X2 > this.X1)
                this.Increment = 1;
            else
                this.Increment = -1;

            this.DeltaX = this.X2 - this.X1;
            this.DeltaY = this.Y2 - this.Y1;

            this.IncrE = 2 * this.DeltaY * this.Increment;
            this.IncrNE = 2 * (this.DeltaY - this.DeltaX) * this.Increment;
            this.DTerm = (2 * this.DeltaY - this.DeltaX) * this.Increment;

            this.XIndex = this.X1;
            this.YIndex = this.Y1;
        }

        public bool nextPoint()
        {
            if (XIndex == X2)
                return false;

            XIndex += Increment;
            if (DTerm < 0 || (Increment < 0 && DTerm <= 0))
                DTerm += IncrE;
            else
            {
                DTerm += IncrNE;
                YIndex += Increment;
            }
            return true;
        }
    }

}
