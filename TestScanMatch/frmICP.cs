using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScanMatchers.ScanMatcher;
using System.IO;
using ScanMatchers.Tools.USARSimMessages;
using ScanMatchers.ScanMatcher.Base;
using ScanMatchers.Math;
using SlimDX.Direct2D;
using System.Threading;
using System.Collections;

namespace TestScanMatch
{
    public partial class frmICP : Form
    {
        public struct PT_MAP
        {
            public Point p;
            public int scanID;

            public PT_MAP(int id, Point pt)
            {
                p = pt;
                scanID = id;
            }
        }

        HashSet<PT_MAP> mMapPoints = new HashSet<PT_MAP>();

        Laser mPrevLaser = null;
        Laser mCurrLaser = null;

        Pose2D mPrevPose = null;
        Pose2D mCurrPose = null;
        Pose2D mSeed = null;
        Pose2D mEstimated = new Pose2D();

        IScanMatchers mSM = new IdcScanMatcher();
        MessageParser mUSARParser = new MessageParser();

        float m3DScaleFactor = 1f;
        float m2DScaleFactor = 20;


        SolidColorBrush mRedBrush = null;
        SolidColorBrush mBlackBrush = null;

        int mIteratedLines = 0;

        public frmICP()
        {
            InitializeComponent();
        }

        private void frmICP_Load(object sender, EventArgs e)
        {
            mRedBrush = new SolidColorBrush(rwDrawer.renderTarget, Color.Red);
            mBlackBrush = new SolidColorBrush(rwDrawer.renderTarget, Color.Black);

            btnContinues.Tag = 1;
        }

        private void llLogFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dlgOpenFile.FileName = "";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dlgOpenFile.FileName))
                {
                    llLogFile.Text = dlgOpenFile.FileName;
                }
            }
        }

        private void USARSimLogParser(string l, ref bool completed)
        {
            mUSARParser.SimulationMessage = l;
            switch (mUSARParser.MessageType)
            {
                case (int)eSimulationMessageType.INS:
                    INS mINS = (INS)mUSARParser.MessageData;

                    Pose2D mNewPose = new Pose2D(mINS.p3Loc.X * m3DScaleFactor, mINS.p3Loc.Y * m3DScaleFactor, mINS.p3Ori.Z);

                    mPrevPose = mCurrPose;
                    mCurrPose = mNewPose;

                    if (mPrevPose != null)
                    {
                        mSeed = new Pose2D(mSeed.Position + (mCurrPose.Position - mPrevPose.Position),
                                           mSeed.Rotation + (mCurrPose.Rotation - mPrevPose.Rotation));
                    }

                    break;

                case (int)eSimulationMessageType.Laser:
                    Laser mLaser = (Laser)mUSARParser.MessageData;

                    if (mPrevLaser == null)
                        mPrevLaser = mLaser;
                    else
                    {
                        mCurrLaser = mLaser;
                        completed = true;
                    }

                    break;
            }
        }

        private void CarmenLogParser(string l, ref bool completed)
        {
            USARParser p = new USARParser(l, "\":", true);

            string odo = p.getString("odometry").Replace(" ", "");
            odo = "SEN {Type Odometry} {Name Odometry} {Pose " + odo + "}";

            mUSARParser.SimulationMessage = odo;

            Odometry mOdo = (Odometry)mUSARParser.MessageData;

            Pose2D mNewPose = new Pose2D(mOdo.x * m3DScaleFactor, mOdo.y * m3DScaleFactor, mOdo.theta);

            mPrevPose = mCurrPose;
            mCurrPose = mNewPose;

            if (mPrevPose != null)
                mSeed = new Pose2D(mSeed.Position + (mCurrPose.Position - mPrevPose.Position),
                                   mSeed.Rotation - (mCurrPose.Rotation - mPrevPose.Rotation));
            else
                mSeed = new Pose2D();
            

            string lsr = p.getString("readings").Replace(" ", "");
            string valid = p.getString("valid").Replace(" ", "");
            string theta = p.getString("theta").Replace(" ", "");
            float nrays = float.Parse(p.getString("nrays"));
            float min_theta = float.Parse(p.getString("min_theta"));
            float max_theta = float.Parse(p.getString("max_theta"));

            float fov = max_theta - min_theta;
            float res = fov / nrays;

            string range = "SEN {Time 0.0} {Type RangeScanner} {Name Scanner1} {Resolution " + res + "} {FOV " + fov + "} {Range " + lsr + "} {Valid " +
                           valid + "} {Theta " + theta + "}";


            mUSARParser.SimulationMessage = range;
            Laser mLaser = (Laser)mUSARParser.MessageData;

            if (mPrevLaser == null)
            {
                mPrevLaser = mLaser;
            }
            else
            {
                mCurrLaser = mLaser;
                completed = true;
            }
        }


        private void btnInitialize_Click(object sender, EventArgs e)
        {
            string[] lines = File.ReadAllLines(llLogFile.Text);

            int startIndex = int.Parse(txtPrevLaserIndex.Text);
            mIteratedLines = startIndex;

            mSeed = new Pose2D();
            mPrevLaser = null;
            mCurrLaser = null;

            mPrevPose = null;
            mCurrPose = null;
            mSeed = new Pose2D();


            bool Completed = false;
            while (!Completed)
            {
                string l = lines[startIndex++];
                USARSimLogParser(l, ref Completed);
            }

            mIteratedLines = startIndex - 1;

            //mPrevLaser.posRobot = new Pose2D(mPrevPose.Position / m3DScaleFactor, mPrevPose.Rotation);
            //mCurrLaser.posRobot = new Pose2D(mCurrPose.Position / m3DScaleFactor, mCurrPose.Rotation);

            mMapPoints.Clear();
            ComputeMapPoints(mPrevLaser, 0);
            ComputeMapPoints(mCurrLaser, 1);

            rwDrawer.Invalidate();
        }

        private void btnSeed_Click(object sender, EventArgs e)
        {
            //mCurrLaser.posRobot = new Pose2D((mPrevPose.Position + mSeed.Position) / m3DScaleFactor, 
            //                                 mPrevPose.Rotation + mSeed.Rotation);

            mCurrLaser.posRobot = new Pose2D(-mSeed.Position, -mSeed.Rotation);

            mMapPoints.Clear();
            ComputeMapPoints(mPrevLaser, 0);
            ComputeMapPoints(mCurrLaser, 1);

            rwDrawer.Invalidate();
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            ScanObservation m1 = new ScanObservation(m3DScaleFactor, mPrevLaser);
            ScanObservation m2 = new ScanObservation(m3DScaleFactor, mCurrLaser);

            MatchResult mR = null;

            mSM = new MbIcpScanMatcher();
            mEstimated = new Pose2D();

            if (chkSeed.Checked)
                mR = mSM.Match(m1, m2, mSeed);
            else
                mR = mSM.Match(m1, m2, new Pose2D());

            if (mR.Converged)
            {
                Pose2D es = mR.EstimatedOdometry;
                mEstimated = compound(mEstimated, es);

                mCurrLaser.posRobot = new Pose2D(mEstimated.Position, mEstimated.Rotation);

                mMapPoints.Clear();
                ComputeMapPoints(mPrevLaser, 0);
                ComputeMapPoints(mCurrLaser, 1);

                rwDrawer.Invalidate();
            }
        }

        private Pose2D compound(Pose2D t1, Pose2D t2)
        {
            Pose2D t_ret = new Pose2D();

            t_ret.X = t2.X * System.Math.Cos(t1.Rotation) - t2.Y * System.Math.Sin(t1.Rotation) + t1.X;
            t_ret.Y = t2.X * System.Math.Sin(t1.Rotation) + t2.Y * System.Math.Cos(t1.Rotation) + t1.Y;
            t_ret.Rotation = t1.Rotation + t2.Rotation;

            // Make angle [-pi,pi)
            t_ret.Rotation = t_ret.GetNormalizedRotation();

            return t_ret;
        }

        public void ComputeMapPoints(Laser mSc, int id)
        {
            int Len = mSc.fRanges.Count;
            for (int i = 0; i < Len; i++)
            {
                if (!mSc.bFilters[i])
                {
                    float dist = (float)mSc.fRanges[i];

                    Matrix2 R = new Matrix2(Math.Cos(mSc.posRobot.Rotation), -Math.Sin(mSc.posRobot.Rotation),
                                            Math.Sin(mSc.posRobot.Rotation), Math.Cos(mSc.posRobot.Rotation));

                    Vector2 P = new Vector2(dist * Math.Cos(mSc.fTheta[i]), dist * Math.Sin(mSc.fTheta[i]));
                    Vector2 T = new Vector2((mSc.posRobot.X) / m3DScaleFactor, (mSc.posRobot.Y) / m3DScaleFactor);

                    Vector2 xy = R * P + T;

                    //double X = dist * Math.Cos(mSc.fTheta[i] - mSc.posRobot.Rotation);
                    //double Y = dist * Math.Sin(mSc.fTheta[i] - mSc.posRobot.Rotation);

                    //Pose2D ptGlobal = new Pose2D(X - (mSc.posRobot.X / m3DScaleFactor) * m2DScaleFactor,
                    //                             Y + (mSc.posRobot.Y / m3DScaleFactor) * m2DScaleFactor, 0);

                    Point mTemp = new Point((int)(xy.Y * m2DScaleFactor), (int)(xy.X * m2DScaleFactor));
                    PT_MAP mT = new PT_MAP(id, mTemp);

                    if (!mMapPoints.Contains(mT))
                    {
                        mMapPoints.Add(mT);
                    }
                }
            }
        }

        private void rwDrawer_UserPaint(SlimDX.Direct2D.WindowRenderTarget target)
        {
            rwDrawer.Transform(new SlimDX.Vector2(rwDrawer.Width / 2, rwDrawer.Height / 2), 0);
            target.Transform = rwDrawer.renderMatrix;

            lock (mMapPoints)
            {
                foreach (PT_MAP mPtm in mMapPoints)
                {
                    Point mPt = mPtm.p;
                    Rectangle ptRect = new Rectangle(mPt.X, mPt.Y, 1, 1);
                    if (mPtm.scanID == 0)
                        target.FillRectangle(mBlackBrush, ptRect);
                    else
                        target.FillRectangle(mRedBrush, ptRect);
                }
            }
        }

        Thread mRun = null;

        private void btnContinues_Click(object sender, EventArgs e)
        {
            if ((int)btnContinues.Tag == 1)
            {
                btnContinues.Enabled = false;
                mRun = new Thread(new ThreadStart(Run));
                mRun.IsBackground = true;
                mRun.Start();
                btnContinues.Text = "Stop";
                btnContinues.Tag = 0;
                btnContinues.Enabled = true;
            }
            else
            {
                btnContinues.Enabled = false;
                mRun.Abort();
                mRun.Join();
                mRun = null;
                btnContinues.Text = "Continues";
                btnContinues.Tag = 1;
                btnContinues.Enabled = true;

                lock (mMapPoints)
                {
                    mMapPoints.Clear();
                    rwDrawer.Invalidate();
                }
            }
        }

        private void Run()
        {
            string[] lines = File.ReadAllLines(llLogFile.Text);
            int startIndex = int.Parse(txtPrevLaserIndex.Text);

            mSeed = new Pose2D();
            mPrevPose = null;
            mCurrPose = null;
            mEstimated = new Pose2D();

            //mSM = new MbIcpScanMatcher();
            mSM = new WeightedScanMatcher();

            bool Completed = false;
            while (mRun.ThreadState != ThreadState.AbortRequested)
            {
                if (startIndex >= lines.Length) break;

                string l = lines[startIndex++];
                CarmenLogParser(l, ref Completed);
                //USARSimLogParser(l, ref Completed);

                if (Completed)
                {
                    Completed = false;

                    ScanObservation m1 = new ScanObservation(m3DScaleFactor, mPrevLaser);
                    ScanObservation m2 = new ScanObservation(m3DScaleFactor, mCurrLaser);

                    MatchResult mR = null;

                    if (chkSeed.Checked)
                    {
                        //mSeed.Position *= 0.003f;
                        //mSeed.Rotation *= 0.9f;
                        mR = mSM.Match(m1, m2, mSeed);
                    }
                    else
                    {
                        //Pose2D initQ = new Pose2D(0.1, 0.1, 0.1);
                        mR = mSM.Match(m1, m2, new Pose2D());
                    }

                    if (mR.Converged)
                    {
                        Pose2D es = mR.EstimatedOdometry;

                        mEstimated = compound(mEstimated, es);

                        mCurrLaser.posRobot = new Pose2D(mEstimated.Position, mEstimated.Rotation);

                        lock (mMapPoints)
                        {
                            ComputeMapPoints(mCurrLaser, 0);
                        }

                        rwDrawer.Invalidate();

                    }

                    //if (max(abs(mSeed)) > 0.01f)
                    {
                        mPrevLaser = mCurrLaser;
                        mSeed = new Pose2D();
                    }
                }

                Thread.Sleep(10);
            }
        }

        private double max(Pose2D q)
        {
            return System.Math.Max(System.Math.Max(q.X, q.Y), q.Rotation);
        }

        private Pose2D abs(Pose2D q)
        {
            return new Pose2D(System.Math.Abs(q.X), System.Math.Abs(q.Y), System.Math.Abs(q.Rotation));
        }

    }
}
