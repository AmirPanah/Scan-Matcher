using ScanMatchers.Math;

namespace ScanMatchers.ScanMatcher.Base
{
    public class MatchResult
    {
        private float _CovarianceThreshold = 10000.0f;
        private float CHANGE_MAXTRANSLATION = 1000; //mm
        private float CHANGE_MAXROTATION = 100;     //degrees

        // Fields
        private bool _Converged;
        private Matrix3 _Covariance;
        private double _Distance;
        private Pose2D _EstimatedOdometry;
        private int _NumCorrespondencies;
        private int _NumIterations;
        private int _NumMilliseconds;
        private double _Duration;
        private Pose2D _RawOdometry;

        // Methods
        public MatchResult(Pose2D rawOdometry, Pose2D estimatedOdometry, int numIterations, bool converged)
        {
            this._RawOdometry = rawOdometry;
            this._EstimatedOdometry = estimatedOdometry;
            this._NumIterations = numIterations;
            this._Converged = converged;
        }

        public MatchResult(Pose2D rawOdometry, Pose2D estimatedOdometry, int numIterations, double duration, bool converged)
        {
            this._RawOdometry = rawOdometry;
            this._EstimatedOdometry = estimatedOdometry;
            this._NumIterations = numIterations;
            this._Duration = duration;
            this._Converged = converged;
        }

        public MatchResult(Pose2D rawOdometry, Pose2D estimatedOdometry, Matrix3 covariance, double distance, int numIterations, int numCorrespondencies, int numMilliseconds, bool converged)
        {
            this._RawOdometry = rawOdometry;
            this._EstimatedOdometry = estimatedOdometry;
            this._Covariance = covariance;
            this._Distance = distance;
            this._NumIterations = numIterations;
            this._NumCorrespondencies = numCorrespondencies;
            this._NumMilliseconds = numMilliseconds;
            this._Converged = converged;

            if (numCorrespondencies == 0)
                this._Converged = false;

            if (System.Math.Abs(covariance[0, 0]) > _CovarianceThreshold ||
                System.Math.Abs(covariance[1, 1]) > _CovarianceThreshold ||
                System.Math.Abs(covariance[2, 2]) > _CovarianceThreshold)
            {
                this._Converged = false;
            }

            this._Converged = !ExceedDelta(estimatedOdometry);
        }

        private bool ExceedDelta(Pose2D dpose)
        {
            bool extend = false;
            extend = extend || System.Math.Abs(dpose.X) > CHANGE_MAXTRANSLATION;
            extend = extend || System.Math.Abs(dpose.Y) > CHANGE_MAXTRANSLATION;

            double dradians = dpose.GetNormalizedRotation();
            double dangle = dradians / System.Math.PI * 180;
            extend = extend || System.Math.Abs(dangle) > CHANGE_MAXROTATION;

            return extend;
        }


        // Properties
        public bool Converged
        {
            get
            {
                return this._Converged;
            }
        }

        public Matrix3 Covariance
        {
            get
            {
                return this._Covariance;
            }
        }

        public double Distance
        {
            get
            {
                return this._Distance;
            }
        }

        public Pose2D EstimatedOdometry
        {
            get
            {
                return this._EstimatedOdometry;
            }
        }

        public int NumCorrespondencies
        {
            get
            {
                return this._NumCorrespondencies;
            }
        }

        public int NumIterations
        {
            get
            {
                return this._NumIterations;
            }
        }

        public int NumMilliseconds
        {
            get
            {
                return this._NumMilliseconds;
            }
        }

        public double Duration
        {
            get
            {
                return this._Duration;
            }
        }

        public Pose2D RawOdometry
        {
            get
            {
                return this._RawOdometry;
            }
        }
    }
}
