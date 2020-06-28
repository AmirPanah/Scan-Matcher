using System;
using System.Collections.Generic;

namespace ScanMatchers.Math
{

    public class MathHelper
    {

        public static float GetDistance(Vector2 first, Vector2 next)
        {
            double dx = next[0] - first[0];
            double dy = next[1] - first[1];

            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2 TranformVector(Vector2 local, Vector2 center, Pose2D q)
        {
            Matrix2 R = new Matrix2(System.Math.Cos(q.Rotation), -System.Math.Sin(q.Rotation),
                                    System.Math.Sin(q.Rotation), System.Math.Cos(q.Rotation));

            Vector2 tmpL = local - center;
            tmpL = R * tmpL + center;
            tmpL += q.Position;

            return tmpL;
        }

        public static float VectorLength(Vector2 v)
        {
            double dx2 = v[0] * v[0];
            double dy2 = v[1] * v[1];
            return (float)System.Math.Sqrt(dx2 + dy2);
        }

        public static float AngleBetweenVectors(Vector2 vFrom, Vector2 vTo)
        {
            //double ang1 = System.Math.Atan2(first[1], first[0]);
            //double ang2 = System.Math.Atan2(next[1], next[0]);

            //return (float)(ang2 - ang1);
            float dp = DotProduct(vFrom, vTo),
            vfl = VectorLength(vFrom),
            vtl = VectorLength(vTo);

            float th = dp / (vfl * vtl);
            float fAlpha = (float)System.Math.Acos(th);

            Vector2 vT = new Vector2();
            vT.X = vTo.X * vFrom.X + vTo.Y * vFrom.Y;
            vT.Y = vTo.X * (-vFrom.Y) + vTo.Y * vFrom.X;

            if (vT.Y < 0) { fAlpha = -fAlpha; }

            return fAlpha;

        }

        public static float AngleBetweenLines(Vector2 l1, Vector2 l2)
        {
            float angle = System.Math.Abs(AngleBetweenVectors(l1, l2));
            if (angle > System.Math.PI / 2)
                angle = (float)System.Math.PI - angle;
            return angle;
        }

        public static float DegToRad(float deg)
        {
            return deg * ((float)System.Math.PI / 180.0f);
        }

        public static float RadToDeg(float rad)
        {
            return rad * (180.0f / (float)System.Math.PI);
        }

        public static float DotProduct(Vector2 vBase, Vector2 vTo)
        {
            return (float)(vBase.X * vTo.X) + (float)(vBase.Y * vTo.Y);
        }

    }
}
