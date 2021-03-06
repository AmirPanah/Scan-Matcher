﻿using System;
using System.Drawing;
using System.Windows.Forms;

using SlimDX.Direct2D;
using SlimDX;
using System.Threading;
using System.IO;
using SlimDX.DirectWrite;

using ScanMatchers.Math;
using ScanMatchers.Tools.USARSimMessages;
using ScanMatchers.ScanMatcher.Base;
using ScanMatchers.ScanMatcher;
using System.Collections;
using System.Collections.Generic;
using ScanMatchers.Tools.Commons;

namespace LogViewer
{
    public partial class frmViewer : Form
    {
        delegate void LogParser(string line);

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

        LogParser evtLogParser;
        Thread mLogParser;


        volatile int mReadingLogLine_Delay = 10;
        volatile int mReadedLineCount = 0;
        volatile bool bCarmen = false;
        volatile bool bPause = false;
        volatile float count;
        volatile float sumIter;
        volatile float avgIter;

        volatile float mLaserParsingTime = 0;
        volatile float mLaserParsingTime_MAX = float.MinValue;
        volatile float mLaserParsingTime_MIN = float.MaxValue;

        volatile eSimulationMessageType mSeedMode = eSimulationMessageType.INS;


        Pose2D mInitPose = null;
        Pose2D mPrevPose = null;
        Pose2D mSeedPose = null;
        bool bInitiated = false;

        Pose2D mPrevDeltaPose = new Pose2D();
        Pose2D mDeltaPose = new Pose2D();

        Pose2D mPrevGTH = null;
        Pose2D mGroundTH = new Pose2D();

        Pose2D mEstimated = new Pose2D();
        Pose2D mCorrected = new Pose2D();
        Pose2D mLastPosition = new Pose2D();

        Pose2D mCurOdo = null;
        Pose2D mSeedOdo = null;
        Pose2D mDeltaOdo = new Pose2D();

        float mFactor = 1000f;
        float m2DFactor = 20f;
        double mDuration = 0;
        int mNumIteration = 0;
        ScanObservation mPrevScan = null;
        IScanMatchers mScanMatcher = null;
        GSLAM mMRLSlam = new GSLAM();
        bool GSLAMMode = false;
        volatile int GSLAMResult = 0;

        //Drawing Objects
        float robotScaleFactor = 5f;
        TextFormat textFormat = null;
        SolidColorBrush defaultBrush = null;
        SolidColorBrush redBrush = null;
        SolidColorBrush magentaBrush = null;
        SolidColorBrush greenBrush = null;
        SolidColorBrush purpleBrush = null;
        SolidColorBrush cyanBrush = null;
        SolidColorBrush robotBrush = null;
        SolidColorBrush robotDirBrush = null;

        MessageParser parser = new MessageParser();

        HashSet<PT_MAP> mMapPoints = new HashSet<PT_MAP>();
        HashSet<Pose2D> mRobotPath = new HashSet<Pose2D>();


        public frmViewer()
        {
            InitializeComponent();
        }

        private void frmViewer_Load(object sender, EventArgs e)
        {
            cmbLogFileType.SelectedIndex = 0;
            cmbScanMatcher.SelectedIndex = 2;
            cmdScanSeed.SelectedIndex = 0;

            textFormat = new TextFormat(new SlimDX.DirectWrite.Factory(), "Tahoma",
                                        SlimDX.DirectWrite.FontWeight.Normal,
                                        SlimDX.DirectWrite.FontStyle.Normal,
                                        SlimDX.DirectWrite.FontStretch.Normal, 10f, "en-us");

            defaultBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Black);
            redBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Red);
            magentaBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Magenta);
            greenBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Green);
            purpleBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Purple);
            cyanBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Cyan);
            robotBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Yellow);
            robotDirBrush = new SolidColorBrush(renderWindow.renderTarget, Color.Red);

            renderWindow.UserPaint += new DXRenderWindow.UserPaints(renderWindow_UserPaint);
        }
        private void lblLogFilePath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dlgOpenFile.FileName = "";

            if (dlgOpenFile.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dlgOpenFile.FileName))
                {
                    lblLogFilePath.Text = dlgOpenFile.FileName;
                }
            }
        }
        private void cmbLogFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (cmbLogFileType.SelectedIndex == 1)
            //{
            //    cmdScanSeed.SelectedIndex = 1;
            //    cmdScanSeed.Enabled = false;
            //}
            //else
            //    cmdScanSeed.Enabled = true;
        }


        private void frmViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var item in ObjectTable.Objects)
                item.Dispose();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            Debug.DrawVectorFunction = SetDebugDrawList;
            Debug.DrawRelationFunction = SetRelationList;

            bCarmen = false;
            GSLAMMode = false;
            
            mInitPose = null;
            mPrevPose = null;
            mSeedPose = null;
            bInitiated = false;

            mPrevDeltaPose = new Pose2D();
            mDeltaPose = new Pose2D();

            mPrevGTH = null;
            mGroundTH = new Pose2D();

            mEstimated = new Pose2D();
            mCorrected = new Pose2D();
            mLastPosition = new Pose2D();

            mCurOdo = null;
            mSeedOdo = null;
            mDeltaOdo = new Pose2D();
            
            mPrevScan = null;
            mScanMatcher = null;

            switch (cmbLogFileType.SelectedIndex)
            {
                case 0: //USARSim
                    evtLogParser = USARSimLogParser;
                    break;
                case 1: //Journal
                    bCarmen = true;
                    evtLogParser = CarmenLogParser;
                    break;
                case 2: //Old Carmen
                    evtLogParser = CarmenTRUELogParser;
                    break;
                case 3: //Rescue Robot
                    evtLogParser = RescueRealLogParser;
                    break;
            }

            switch (cmdScanSeed.SelectedIndex)
            {
                case 0: //None
                    mSeedMode = eSimulationMessageType.None;
                    break;

                case 1: //INS
                    mSeedMode = eSimulationMessageType.INS;
                    break;

                case 2: //Odometry
                    mSeedMode = eSimulationMessageType.Odometdy;
                    break;

                case 3: //GTH
                    mSeedMode = eSimulationMessageType.GroundTruth;
                    break;
            }

            switch (cmbScanMatcher.SelectedIndex)
            {
                case 0: //None
                    mScanMatcher = null;
                    break;
                case 1: //WSM
                    mScanMatcher = new WeightedScanMatcher();
                    break;
                case 2: //IDC
                    mScanMatcher = new IdcScanMatcher();
                    break;
                case 3: //MbICP
                    mScanMatcher = new MbIcpScanMatcher();
                    break;
                case 4:
                    mScanMatcher = new MbIcpScanMatcher();
                    GSLAMMode = true;
                    break;
            }

            mLogParser = new Thread(new ThreadStart(RunLogLoader));
            mLogParser.IsBackground = true;
            mLogParser.Start();

            btnStart.Enabled = false;
            btnStop.Enabled = btnPauseResume.Enabled = true;

            renderWindow.Invalidate();
        }
        private void btnPause_Click(object sender, EventArgs e)
        {
            if (mLogParser == null) return;

            if (mLogParser.ThreadState == (ThreadState.WaitSleepJoin | ThreadState.Background))
            {
                mLogParser.Suspend();
                btnPauseResume.Text = "Resume";
            }
            else
            {
                mLogParser.Resume();
                btnPauseResume.Text = "Pause";
            }

            renderWindow.Invalidate();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            mInitPose = null;
            mPrevPose = null;
            mSeedPose = null;
            mDeltaPose = new Pose2D();

            mPrevGTH = null;
            mGroundTH = new Pose2D();

            mEstimated = new Pose2D();
            mCorrected = new Pose2D();

            mPrevScan = null;
            mScanMatcher = null;

            if (mLogParser != null)
            {
                mLogParser.Abort();
                mLogParser.Join();
                mLogParser = null;

                mReadedLineCount = 0;
                lock (((ICollection)mScans).SyncRoot)
                {
                    mScans.Clear();
                    mMapPoints.Clear();
                    mRobotPath.Clear();
                }

                btnStart.Enabled = true;
                btnStop.Enabled = btnPauseResume.Enabled = false;
            }

            renderWindow.Invalidate();
        }


        private void RunLogLoader()
        {
            if (!File.Exists(lblLogFilePath.Text))
                return;

            string[] mAllLines = File.ReadAllLines(lblLogFilePath.Text);
            mReadedLineCount = 0;

            while (mLogParser.IsAlive)
            {
                if (bPause)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (evtLogParser != null)
                    evtLogParser(mAllLines[mReadedLineCount++]);

                if (mReadedLineCount == mAllLines.Length)
                    break;

                Thread.Sleep(mReadingLogLine_Delay);
            }

            renderWindow.Invalidate();
        }

        private void RescueRealLogParser(string l)
        {
            string[] parts = l.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts[0].Contains("FLASER"))
            {
                int count = int.Parse(parts[1]);
                int maxData = count + 2 + 6;

                //ODOMETRY
                string odo = parts[maxData - 3] + "," +
                             parts[maxData - 2] + "," +
                             parts[maxData - 1];
                odo = "SEN {Type Odometry} {Name Odometry} {Pose " + odo + "}";

                parser.SimulationMessage = odo;

                Odometry mOdo = (Odometry)parser.MessageData;

                Pose2D mNewPose = new Pose2D(mOdo.x * mFactor, mOdo.y * mFactor, MathHelper.DegToRad(mOdo.theta));

                mPrevPose = mSeedPose;
                mSeedPose = mNewPose;

                if (mPrevPose != null)
                {
                    mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                            mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));

                    mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                            mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                }
                else
                {
                    mEstimated = mNewPose;
                    mCorrected = mNewPose;
                }

                //LASER
                string lsr = "";
                int rc = 0;
                int step = 10;
                for (int i = 0; i < count; i += step)
                {
                    lsr += double.Parse(parts[i + 2]) + ",";
                    rc++;
                }
                lsr = lsr.TrimEnd(",".ToCharArray());

                double fov = MathHelper.DegToRad(270);
                double res = step * (fov / count);
                string range = "SEN {Time 0.0} {Type RangeScanner} {Name Scanner1} {Resolution " + res + "} {FOV " + fov + "} {Range " + lsr + "} ";

                int StartTime = Environment.TickCount;
                parser.SimulationMessage = range;
                mLaserParsingTime = (float)(Environment.TickCount - StartTime) / 1000F;
                mLaserParsingTime_MAX = System.Math.Max(mLaserParsingTime_MAX, mLaserParsingTime);
                mLaserParsingTime_MIN = System.Math.Min(mLaserParsingTime_MIN, mLaserParsingTime);
                Laser mLaser = (Laser)parser.MessageData;

                if (mScanMatcher != null)
                {
                    if (mPrevScan == null)
                    {
                        mPrevScan = new ScanObservation(mFactor, mLaser);
                        AddScansForRender2(mLaser, mEstimated);
                    }
                    else
                    {
                        //if (IcpScanMatcher.ExceedDelta(mDeltaPose))
                        {
                            ScanObservation mScan = new ScanObservation(mFactor, mLaser);

                            MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);
                            mResult = mScanMatcher.Match(mPrevScan, mScan, mResult.EstimatedOdometry);
                            mDuration = mResult.Duration;
                            mNumIteration = mResult.NumIterations;

                            if (mResult.Converged)
                            {
                                mDeltaPose = new Pose2D(mResult.EstimatedOdometry.Y,
                                                        mResult.EstimatedOdometry.X,
                                                        mResult.EstimatedOdometry.Rotation);

                                mCorrected = IcpScanMatcher.CompoundPoses(mCorrected, mDeltaPose);

                                AddScansForRender2(mLaser, mCorrected);

                                mPrevScan = mScan;
                                mDeltaPose = new Pose2D();
                            }
                        }
                    }
                }
                else
                    AddScansForRender2(mLaser, mEstimated);
            }
        }
        private void CarmenTRUELogParser(string l)
        {
            string[] parts = l.Split(" \t".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (parts[0].Contains("FLASER"))
            {
                int count = int.Parse(parts[1]);
                int maxData = count + 2 + 6;

                //ODOMETRY
                string odo = parts[maxData - 3] + "," + 
                             parts[maxData - 2] + "," +
                             parts[maxData - 1];
                odo = "SEN {Type Odometry} {Name Odometry} {Pose " + odo + "}";

                parser.SimulationMessage = odo;

                Odometry mOdo = (Odometry)parser.MessageData;

                Pose2D mNewPose = new Pose2D(mOdo.x * mFactor, mOdo.y * mFactor, mOdo.theta);

                mPrevPose = mSeedPose;
                mSeedPose = mNewPose;

                if (mPrevPose != null)
                {
                    mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                            mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));

                    mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                            mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                }
                else
                {
                    mEstimated = mNewPose;
                    mCorrected = mNewPose;
                }

                //LASER
                string lsr = "";
                for (int i = 0; i < count; i++)
                {
                    lsr += double.Parse(parts[i + 2]) + ",";
                }
                lsr = lsr.TrimEnd(",".ToCharArray());

                ///TODO : we must change this res
                double res = MathHelper.DegToRad(270) / count;
                string range = "SEN {Time 0.0} {Type RangeScanner} {Name Scanner1} {Resolution " + res + "} {FOV " + MathHelper.DegToRad(270) + "} {Range " + lsr + "} ";

                int StartTime = Environment.TickCount;
                parser.SimulationMessage = range;
                mLaserParsingTime = (float)(Environment.TickCount - StartTime) / 1000F;
                mLaserParsingTime_MAX = System.Math.Max(mLaserParsingTime_MAX, mLaserParsingTime);
                mLaserParsingTime_MIN = System.Math.Min(mLaserParsingTime_MIN, mLaserParsingTime);
                Laser mLaser = (Laser)parser.MessageData;

                if (mScanMatcher != null)
                {
                    if (mPrevScan == null)
                    {
                        mPrevScan = new ScanObservation(mFactor, mLaser);
                        AddScansForRender2(mLaser, mEstimated);
                    }
                    else
                    {
                        //if (IcpScanMatcher.ExceedDelta(mDeltaPose))
                        {
                            ScanObservation mScan = new ScanObservation(mFactor, mLaser);

                            MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);

                           
                            mDuration = mResult.Duration;
                            mNumIteration = mResult.NumIterations;
                           

                            if (mResult.Converged)
                            {
                                mDeltaPose = new Pose2D(mResult.EstimatedOdometry.Y,
                                                        mResult.EstimatedOdometry.X,
                                                        mResult.EstimatedOdometry.Rotation);

                                mCorrected = IcpScanMatcher.CompoundPoses(mCorrected, mDeltaPose);

                                AddScansForRender2(mLaser, mCorrected);

                                mPrevScan = mScan;
                                mDeltaPose = new Pose2D();
                            }
                        }
                    }
                }
                else
                    AddScansForRender2(mLaser, mEstimated);
            }
        }
        private void CarmenLogParser(string l)
        {
            USARParser p = new USARParser(l, "\":", true);

            string gth = p.getString("estimate").Replace(" ", "");
            gth = "SEN {Time 111.6857} {Type GroundTruth} {Name GroundTruth} {Location " + gth + "} {Orientation " + gth + "}";

            parser.SimulationMessage = gth;

            GroundTruth mGTH = (GroundTruth)parser.MessageData;
            Pose2D mNewGTH = new Pose2D(mGTH.p3Loc.X * mFactor, mGTH.p3Loc.Y * mFactor, mGTH.p3Ori.Z);
            mPrevGTH = mGroundTH;
            mGroundTH = mNewGTH;

            if (mSeedMode == eSimulationMessageType.GroundTruth)
            {
                Pose2D mNewPose = new Pose2D(mGTH.p3Loc.X * mFactor, mGTH.p3Loc.Y * mFactor, mGTH.p3Ori.Z);

                //if (mInitPose == null)
                //    mInitPose = new Pose2D(mGTH.p3Loc.X, mGTH.p3Loc.Y, mGTH.p3Ori.Z);

                mPrevPose = mSeedPose;
                mSeedPose = mNewPose;

                if (mScanMatcher == null)
                {
                    if (mPrevPose != null)
                        mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                    else
                        mEstimated = mNewPose;
                }
                else
                {
                    if (mPrevPose != null)
                    {
                        mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                                mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));

                        mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                    }
                    else
                        mEstimated = mNewPose;
                }
            }



            string odo = p.getString("odometry").Replace(" ", "");
            odo = "SEN {Type Odometry} {Name Odometry} {Pose " + odo + "}";

            parser.SimulationMessage = odo;

            Odometry mOdo = (Odometry)parser.MessageData;
            if (mSeedMode == eSimulationMessageType.Odometdy || mSeedMode == eSimulationMessageType.INS)
            {
                Pose2D mNewPose = new Pose2D(mOdo.x * mFactor, mOdo.y * mFactor, mOdo.theta);

                //if (mInitPose == null)
                //    mInitPose = new Pose2D(mOdo.x, mOdo.y, mOdo.theta);

                mPrevPose = mSeedPose;
                mSeedPose = mNewPose;

                if (mScanMatcher == null)
                {
                    if (mPrevPose != null)
                        mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                    else
                        mEstimated = mNewPose;
                }
                else
                {
                    if (mPrevPose != null)
                    {
                        mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                                mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));

                        mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                    }
                    else
                    {
                        mEstimated = mNewPose;
                        mCorrected = mNewPose;
                    }
                }
            }

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

            int StartTime = Environment.TickCount;
            parser.SimulationMessage = range;
            mLaserParsingTime = (float)(Environment.TickCount - StartTime) / 1000F;
            mLaserParsingTime_MAX = System.Math.Max(mLaserParsingTime_MAX, mLaserParsingTime);
            mLaserParsingTime_MIN = System.Math.Min(mLaserParsingTime_MIN, mLaserParsingTime);

            Laser mLaser = (Laser)parser.MessageData;

            if (mScanMatcher != null)
            {
                if (mPrevScan == null)
                {
                    mPrevScan = new ScanObservation(mFactor, mLaser);
                    AddScansForRender2(mLaser, mEstimated);
                }
                else
                {
                    ScanObservation mScan = new ScanObservation(mFactor, mLaser);

                    //new++++


                    //mScanMatcher = new WeightedScanMatcher();
                    //MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);

                    //mScanMatcher = new MbIcpScanMatcher();
                    //mResult = mScanMatcher.Match(mPrevScan, mScan, mResult.EstimatedOdometry);

                    //************************* OLD *************
                    //mScanMatcher = new MbIcpScanMatcher();
                    //MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);

                    mScanMatcher = new WeightedScanMatcher();
                    MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);

                    //****************** OLD ********************


                    //************** NEW

                    //mScanMatcher = new WeightedScanMatcher();
                    //MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan,mDeltaPose );

                    //mScanMatcher = new MbIcpScanMatcher();
                    // mResult = mScanMatcher.Match(mPrevScan, mScan,mResult.EstimatedOdometry );

                   

                    //****************Neww********

                    //MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);

                    count++;
                    mDuration = mResult.Duration;
                    mNumIteration = mResult.NumIterations;
                    sumIter = sumIter + mNumIteration;
                    avgIter = sumIter / count;

                    mDeltaPose = mResult.EstimatedOdometry;
                    mCorrected = IcpScanMatcher.CompoundPoses(mCorrected, mDeltaPose);
                    //mCorrected.Position += mDeltaPose.Position;
                    //mCorrected.Rotation += mDeltaPose.Rotation;

                    AddScansForRender2(mLaser, mCorrected);

                    mPrevScan = mScan;
                    mDeltaPose = new Pose2D();
                }
            }
            else
                AddScansForRender2(mLaser, mEstimated);
        }
        private void USARSimLogParser(string l)
        {
            if (l.Contains("RangeScanner"))
            {
                int StartTime = Environment.TickCount;
                parser.SimulationMessage = l;
                mLaserParsingTime = (float)(Environment.TickCount - StartTime) / 1000F;
                mLaserParsingTime_MAX = System.Math.Max(mLaserParsingTime_MAX, mLaserParsingTime);
                mLaserParsingTime_MIN = System.Math.Min(mLaserParsingTime_MIN, mLaserParsingTime);

                Laser lp = (Laser)parser.MessageData;
                string lpSTR = "FLASER " + lp.fRanges.Count + " ";

                foreach(double d in lp.fRanges)
                {
                    lpSTR += d.ToString("#.####") + " ";
                }

                lpSTR += "\r\n";
                File.AppendAllText(@"C:\Change.log", lpSTR);
            }
            else
            {
                parser.SimulationMessage = l;
            }

            switch (parser.MessageType)
            {
                case (int)eSimulationMessageType.INS:
                    INS mINS = (INS)parser.MessageData;
                    
                    if (mSeedMode == eSimulationMessageType.INS)
                    {
                        Pose2D mNewPose = new Pose2D(mINS.p3Loc.X * mFactor, mINS.p3Loc.Y * mFactor, mINS.p3Ori.Z);
                        mNewPose = ConvertToCarmen(mNewPose);

                        mPrevPose = mSeedPose;
                        mSeedPose = mNewPose;

                        if (mScanMatcher == null)
                        {
                            if (mPrevPose != null)
                                mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                        mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                            else
                                mEstimated = mNewPose;
                        }
                        else
                        {
                            if (mPrevPose != null)
                            {
                                mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                                        mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));

                                mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                        mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                            }
                            else
                            {
                                mEstimated = mNewPose;
                                //mCorrected = mNewPose;
                            }
                        }

                        if (!bInitiated)
                            bInitiated = true;
                    }


                    break;
                
                case (int)eSimulationMessageType.GroundTruth:
                    GroundTruth mGTH = (GroundTruth)parser.MessageData;

                    Pose2D mNewGTH = new Pose2D(mGTH.p3Loc.X * mFactor, mGTH.p3Loc.Y * mFactor, mGTH.p3Ori.Z);
                    mNewGTH = ConvertToCarmen(mNewGTH);

                    if (mPrevGTH == null)
                        mPrevGTH = mNewGTH;
                    else
                    {
                        mGroundTH = new Pose2D(mGroundTH.Position + (mNewGTH.Position - mPrevGTH.Position),
                                               mGroundTH.Rotation + (mNewGTH.Rotation - mPrevGTH.Rotation));
                        mPrevGTH = mNewGTH;
                    }

                    if (mSeedMode == eSimulationMessageType.GroundTruth)
                    {
                        mPrevPose = mSeedPose;
                        mSeedPose = mNewGTH;

                        if (mScanMatcher == null)
                        {
                            if (mPrevPose != null)
                                mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                        mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                            else
                                mEstimated = mNewGTH;
                        }
                        else
                        {
                            if (mPrevPose != null)
                            {
                                mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                                        mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));

                                mEstimated = new Pose2D(mEstimated.Position + (mSeedPose.Position - mPrevPose.Position),
                                                        mEstimated.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                            }
                            else
                            {
                                mEstimated = mNewGTH;
                                mCorrected = mNewGTH;
                            }
                        }

                        if (!bInitiated)
                            bInitiated = true;
                    }

                    break;
                
                case (int)eSimulationMessageType.Odometdy:
                    Odometry mOdo = (Odometry)parser.MessageData;

                    Pose2D mNewOdo = new Pose2D(mOdo.x * mFactor, mOdo.y * mFactor, mOdo.theta);
                    mNewOdo = ConvertToCarmen(mNewOdo);

                    mCurOdo = mSeedOdo;
                    mSeedOdo = mNewOdo;

                    if (mPrevPose != null)
                    {
                        mDeltaOdo = new Pose2D(mDeltaOdo.Position + (mSeedOdo.Position - mCurOdo.Position),
                                               mDeltaOdo.Rotation + (mSeedOdo.Rotation - mCurOdo.Rotation));
                    }


                    if (mSeedMode == eSimulationMessageType.Odometdy)
                    {
                        mPrevPose = mSeedPose;
                        mSeedPose = mNewOdo;

                        mEstimated = mNewOdo;
                         
                        if (mPrevPose != null)
                        {
                            mDeltaPose = new Pose2D(mDeltaPose.Position + (mSeedPose.Position - mPrevPose.Position),
                                                    mDeltaPose.Rotation + (mSeedPose.Rotation - mPrevPose.Rotation));
                        }

                        if (!bInitiated)
                            bInitiated = true;
                    }

                    break;

                case (int)eSimulationMessageType.Laser:
                    Laser mLaser = (Laser)parser.MessageData;

                    if (GSLAMMode)
                    {
                        if (mPrevScan == null)
                        {
                            mPrevScan = new ScanObservation(mFactor, mLaser);
                            
                            mMRLSlam.updateValues(mLaser, mCorrected);
                            mMRLSlam.DoSLAM();
                        }
                        else
                        {
                            mCorrected.X += mDeltaPose.Y;
                            mCorrected.Y -= mDeltaPose.X;
                            mCorrected.Rotation -= mDeltaPose.Rotation;

                            mMRLSlam.updateValues(mLaser, mCorrected);
                            GSLAMResult = mMRLSlam.DoSLAM();

                            if (GSLAMResult == 0)
                            {
                                Pose2D dif = mMRLSlam.Dif;

                                mCorrected.Position += dif.Position;
                                mCorrected.Rotation += dif.Rotation;

                            }

                            mDeltaOdo = new Pose2D();
                            mDeltaPose = new Pose2D();

                            AddScansForRender2(mLaser, mCorrected);
                        }
                        break;
                    }


                    if (mScanMatcher != null)
                    {
                        if (mPrevScan == null)
                        {
                            mPrevScan = new ScanObservation(mFactor, mLaser);
                            //AddScansForRender2(mLaser, mEstimated);
                        }
                        else
                        {
                            ScanObservation mScan = new ScanObservation(mFactor, mLaser);

                            {
                                MatchResult mResult = mScanMatcher.Match(mPrevScan, mScan, mDeltaPose);
                                mDuration = mResult.Duration;
                                mNumIteration = mResult.NumIterations;

                                if (mResult.Converged)
                                {
                                    Pose2D es = mResult.EstimatedOdometry;
                                    es = CorrectAngle(es);

                                    mCorrected = IcpScanMatcher.CompoundPoses(mCorrected, es);

                                    AddScansForRender2(mLaser, mCorrected);

                                    mDeltaOdo = new Pose2D();
                                    mDeltaPose = new Pose2D();
                                }

                                mPrevScan = mScan;
                            }
                        }
                    }
                    else
                    {
                        if (bInitiated)
                            AddScansForRender2(mLaser, mEstimated);
                    }

                    break;
            }
        }

        private Pose2D ConvertToCarmen(Pose2D es)
        {
            return new Pose2D(es.X, -es.Y, -es.GetNormalizedRotation());
        }
        private Pose2D CorrectAngle(Pose2D l)
        {
            double correction_angle = -Math.PI / 2;
            double X = l.X * Math.Cos(correction_angle) - l.Y * Math.Sin(correction_angle);
            double Y = l.X * Math.Sin(correction_angle) + l.Y * Math.Cos(correction_angle);
            l.X = X;
            l.Y = Y;
            return l;
        }


        List<Laser> mScans = new List<Laser>(1000);
        private const int ADDING_CYCLE = 3;
        private int CYCLE_COUNT = 0;

        double resolution = 0.05;
        double wall_thickness = 0.01;
        private void AddScansForRender2(Laser mLaser, Pose2D mEst)
        {
            mEst = new Pose2D(mEst.X / mFactor, mEst.Y / mFactor, mEst.Rotation);

            int i;
            int x1int, y1int, x2int, y2int;
            double theta, delta_theta, temp_laser_x, temp_laser_y;
            BresenhamParam b_params;

            double correction_angle = 0;

            if (mInitPose == null)
            {
                mInitPose = new Pose2D(mEst.X / resolution,
                                       mEst.Y / resolution,
                                       mEst.Rotation);
            }

            double laser_x = mEst.X;
            double laser_y = mEst.Y;
            double laser_theta = mEst.Rotation;

            correction_angle = 0;

            laser_theta = laser_theta + correction_angle;
            temp_laser_x = laser_x * Math.Cos(correction_angle) - laser_y * Math.Sin(correction_angle);
            temp_laser_y = laser_x * Math.Sin(correction_angle) + laser_y * Math.Cos(correction_angle);

            laser_x = (temp_laser_x / this.resolution - mInitPose.X);
            laser_y = (temp_laser_y / this.resolution - mInitPose.Y);

            x1int = (int)Math.Floor(laser_x);
            y1int = (int)Math.Floor(laser_y);

            theta = laser_theta - Math.PI / 2.0;
            delta_theta = mLaser.fResolution;

            mLaser.posRobot = new Pose2D(laser_x, laser_y, laser_theta);
            mLastPosition = mLaser.posRobot;

            lock (mRobotPath)
            {
                mRobotPath.Add(mLastPosition);
            }

            CYCLE_COUNT++;
            if (CYCLE_COUNT % ADDING_CYCLE == 0)
                CYCLE_COUNT = 0;
            else
                return;

            lock (((ICollection)mScans).SyncRoot)
            {
                mScans.Add(mLaser);

                for (i = 0; i < mLaser.fRanges.Count; i++)
                {
                    if (!mLaser.bFilters[i])
                    {
                        x1int = (int)(laser_x + (mLaser.fRanges[i]) * Math.Cos(theta) / resolution);
                        y1int = (int)(laser_y + (mLaser.fRanges[i]) * Math.Sin(theta) / resolution);

                        x2int = (int)(laser_x + (mLaser.fRanges[i] + wall_thickness) * Math.Cos(theta) / resolution);
                        y2int = (int)(laser_y + (mLaser.fRanges[i] + wall_thickness) * Math.Sin(theta) / resolution);

                        b_params = new BresenhamParam(x1int, y1int, x2int, y2int);
                        do
                        {
                            Point curP = b_params.CurrentPoint;
                            {
                                PT_MAP mT = new PT_MAP(0, curP);

                                if (!mMapPoints.Contains(mT))
                                {
                                    mMapPoints.Add(mT);
                                }
                            }
                        }
                        while (b_params.nextPoint());
                    }
                    theta += delta_theta;
                }
            }
        }
        
        class TMAPPOINTS
        {
            public Mutex mux = new Mutex();
            public Vector[] points;
        }
        TMAPPOINTS p0 = new TMAPPOINTS();
        TMAPPOINTS p1 = new TMAPPOINTS();
        TMAPPOINTS p2 = new TMAPPOINTS();
        private int pointW = 1;
        private void SetDebugDrawList(Vector[] v, int id)
        {
            switch(id){
                case 0:
                    p0.mux.WaitOne();
                    p0.points = v;
                    p0.mux.ReleaseMutex();
                    break;
                case 1:
                    p1.mux.WaitOne();
                    p1.points = v;
                    p1.mux.ReleaseMutex();
                    break;
                case 2:
                    p2.mux.WaitOne();
                    p2.points = v;
                    p2.mux.ReleaseMutex();
                    break;
            }
        }

        class TMAPRELATION
        {
            public Mutex mux = new Mutex();
            public Correlation<ScanMatchers.Math.Vector2>[] relations;
        }
        TMAPRELATION r = new TMAPRELATION();
        private void SetRelationList(Correlation<ScanMatchers.Math.Vector2>[] v)
        {
            r.mux.WaitOne();
            r.relations = v;
            r.mux.ReleaseMutex();
        }

        Rectangle tmpRectangle = new Rectangle();
        private Rectangle setTmpRectangle(int x, int y, int w, int h)
        {
            tmpRectangle.X = x;
            tmpRectangle.Y = y;
            tmpRectangle.Width = w;
            tmpRectangle.Height = h;

            return tmpRectangle;
        }

        /// <summary>
        /// user paints here
        /// </summary>
        /// <param name="target"></param>
        private void renderWindow_UserPaint(WindowRenderTarget target)
        {
            string logreaderState = "";
            if (mLogParser != null)
                logreaderState = mLogParser.ThreadState.ToString();
            else
                logreaderState = "Not Started";

            if (textFormat != null)
            {
                target.DrawText("Log Reader Thread State : " + logreaderState, textFormat, renderWindow.ClientRectangle, defaultBrush);
                target.DrawText("Readed Lines Count : " + mReadedLineCount + ", Mode = " + (bPause ? "Paused" : "Running"), textFormat, new Rectangle(0, 12, 1000, 0), defaultBrush);

                target.DrawText("GTH. Pos : " + mGroundTH.ToString(), textFormat, setTmpRectangle(0, 24, 1000, 0), defaultBrush);

                target.DrawText("Estm. Pos : " + mEstimated.ToString(), textFormat, setTmpRectangle(0, 36, 1000, 0), defaultBrush);

                target.DrawText("Corr. Pos : " + mCorrected.ToString(), textFormat, setTmpRectangle(0, 48, 1000, 0), defaultBrush);

                target.DrawText("Duration : " + mDuration.ToString("#0.0000") + ",  Num.ITER : " + mNumIteration + ", Avg.ITER : " + avgIter, 
                                textFormat, setTmpRectangle(0, 60, 1000, 0), defaultBrush);

                target.DrawText("Parsing Time : " + mLaserParsingTime + ", Max : " + mLaserParsingTime_MAX + ", Min : " + mLaserParsingTime_MIN,
                                textFormat, setTmpRectangle(0, 72, 1000, 0), defaultBrush);

                if (GSLAMMode)
                    target.DrawText("GSLAM Result : " + GSLAMResult, textFormat, setTmpRectangle(0, 84, 1000, 0), defaultBrush);
            }

            renderWindow.Transform(new SlimDX.Vector2(renderWindow.Width / 2, renderWindow.Height / 2), 0);
            target.Transform = renderWindow.renderMatrix;

            lock (((ICollection)mScans).SyncRoot)
            {
                if (mScans.Count != 0)
                {
                    foreach (PT_MAP mPtm in mMapPoints)
                    {
                        Point mPt = mPtm.p;
                        Rectangle ptRect;
                        if (bCarmen)
                            ptRect = setTmpRectangle(mPt.X, -mPt.Y, 1, 1);
                        else
                            ptRect = setTmpRectangle(mPt.X, mPt.Y, 1, 1);

                        if (mPtm.scanID == 0)
                            target.FillRectangle(defaultBrush, ptRect);
                        else
                            target.FillRectangle(redBrush, ptRect);
                    }

                    Ellipse e = new Ellipse();
                    if (bCarmen)
                        e.Center = new PointF((float)mLastPosition.X,  /// mFactor * m2DFactor,
                                              (float)-mLastPosition.Y); /// mFactor * m2DFactor);
                    else
                        e.Center = new PointF((float)mLastPosition.X,  /// mFactor * m2DFactor,
                                              (float)mLastPosition.Y); /// mFactor * m2DFactor);
                                                                       
                    e.RadiusX = e.RadiusY = robotScaleFactor;

                    target.FillEllipse(robotBrush, e);

                    PointF endDir = e.Center;
                    double dir = mLastPosition.GetNormalizedRotation();

                    if (bCarmen)
                    {
                        endDir.X += (float)Math.Cos(dir) * robotScaleFactor;
                        endDir.Y -= (float)Math.Sin(dir) * robotScaleFactor;
                    }
                    else
                    {
                        endDir.X += (float)Math.Cos(dir) * robotScaleFactor;
                        endDir.Y += (float)Math.Sin(dir) * robotScaleFactor;
                    }

                    target.DrawLine(robotDirBrush, e.Center, endDir);
                }
            }

            if (p0.points != null)
            {
                p0.mux.WaitOne();
                    foreach (Vector v in p0.points)
                    {
                        Point mPt = new Point((int)(v[0] / mFactor * m2DFactor), (int)(v[1] / mFactor * m2DFactor));
                        Rectangle ptRect;
                        if (bCarmen)
                            ptRect = setTmpRectangle(mPt.X, -mPt.Y, pointW, pointW);
                        else
                            ptRect = setTmpRectangle(mPt.X, mPt.Y, pointW, pointW);

                        target.FillRectangle(redBrush, ptRect);
                    }
                p0.mux.ReleaseMutex();
            }
            if (p1.points != null)
            {
                p1.mux.WaitOne();
                foreach (Vector v in p1.points)
                {
                    Point mPt = new Point((int)(v[0] / mFactor * m2DFactor), (int)(v[1] / mFactor * m2DFactor));
                    Rectangle ptRect;
                    if (bCarmen)
                        ptRect = setTmpRectangle(mPt.X, -mPt.Y, pointW, pointW);
                    else
                        ptRect = setTmpRectangle(mPt.X, mPt.Y, pointW, pointW);

                    target.FillRectangle(magentaBrush, ptRect);
                }
                p1.mux.ReleaseMutex();
            }
            if (p2.points != null)
            {
                p2.mux.WaitOne();
                foreach (Vector v in p2.points)
                {
                    Point mPt = new Point((int)(v[0] / mFactor * m2DFactor), (int)(v[1] / mFactor * m2DFactor));
                    Rectangle ptRect;
                    if (bCarmen)
                        ptRect = setTmpRectangle(mPt.X, -mPt.Y, pointW, pointW);
                    else
                        ptRect = setTmpRectangle(mPt.X, mPt.Y, pointW, pointW);

                    target.FillRectangle(greenBrush, ptRect);
                }
                p2.mux.ReleaseMutex();
            }
            if (r.relations != null)
            {
                r.mux.WaitOne();
                foreach (Correlation<ScanMatchers.Math.Vector2> c in r.relations)
                {
                    Point mPt0 = new Point((int)(c.Point1[0] / mFactor * m2DFactor), (int)(c.Point1[1] / mFactor * m2DFactor));
                    Point mPt1 = new Point((int)(c.Point2[0] / mFactor * m2DFactor), (int)(c.Point2[1] / mFactor * m2DFactor));
                    if (bCarmen)
                    {
                        mPt0.Y = -mPt0.Y;
                        mPt1.Y = -mPt1.Y;
                    }
                    target.DrawLine(purpleBrush, mPt0, mPt1);
                }
                r.mux.ReleaseMutex();
            }

            lock (mRobotPath)
            {
                PointF m2DFirst = new PointF();

                bool mFirst = true;

                foreach (Pose2D mPt in mRobotPath)
                {
                    PointF m2DPose;
                    if (bCarmen)
                        m2DPose = new PointF((float)mPt.X,  /// mFactor * m2DFactor,
                                             (float)-mPt.Y);  /// mFactor * m2DFactor);
                    else
                        m2DPose = new PointF((float)mPt.X,  /// mFactor * m2DFactor,
                                             (float)mPt.Y);  /// mFactor * m2DFactor);

                    if (mFirst)
                        mFirst = false;
                    else
                    {
                        target.DrawLine(cyanBrush, m2DFirst, m2DPose);
                    }

                    m2DFirst = m2DPose;
                }
            }
        }
        private void tmrUpdateWindow_Tick(object sender, EventArgs e)
        {
            renderWindow.Invalidate();
        }
        private void trkReadDelay_Scroll(object sender, EventArgs e)
        {
            lblDelay.Text = string.Format("Delay : {0} ms", trkReadDelay.Value);
            mReadingLogLine_Delay = trkReadDelay.Value;
            Debug.SLEEP_ON_DEBUG = mReadingLogLine_Delay;
        }

        private void renderWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
                bPause = !bPause;
        }

    }
}
