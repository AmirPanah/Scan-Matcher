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

namespace TestScanMatch
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dlgOpen.FileName = "";

            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dlgOpen.FileName))
                {
                    linkLabel1.Text = dlgOpen.FileName;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IScanMatchers sc = new IdcScanMatcher();

            string[] lines = File.ReadAllLines(linkLabel1.Text);

            string cmdLaser1 = "SEN {Time 6.42} {Type RangeScanner} {Name Scanner1} {Resolution 0.0174} {FOV 3.1415} {Range " + lines[0] + "}";
            string cmdLaser2 = "SEN {Time 6.43} {Type RangeScanner} {Name Scanner1} {Resolution 0.0174} {FOV 3.1415} {Range " + lines[1] + "}";

            USARParser p1 = new USARParser(cmdLaser1);
            USARParser p2 = new USARParser(cmdLaser2);

            Laser m1 = new Laser(p1);
            Laser m2 = new Laser(p2);

            ScanObservation so1 = new ScanObservation(1000, m1);
            ScanObservation so2 = new ScanObservation(1000, m2);

            string [] parts = lines[2].Split("/".ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);

            float[] p = USARParser.parseFloats(parts[0], ",");
            float t = float.Parse(parts[1]);

            Pose2D pSeed = new Pose2D(p[0], p[1], t);

            MatchResult r = sc.Match(so1, so2, pSeed);

            textBox1.Text += "My Estm. :" + r.EstimatedOdometry.ToString() + "\r\n";
            textBox1.Text += "UvA Estm. :" + lines[3] + "\r\n";

        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
