using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SlimDX.Direct2D;
using SlimDX;

namespace LogViewer
{
    public partial class DXRenderWindow : UserControl
    {
        public delegate void UserPaints(WindowRenderTarget target);
        public event UserPaints UserPaint = null;

        public WindowRenderTarget renderTarget = null;
        public Matrix3x2 renderMatrix;
        public Vector2 renderTrans;
        public float renderScale = 1f;

        private Point curMouse;

        public DXRenderWindow()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);

            UpdateStyles();

            renderTarget = new WindowRenderTarget(new Factory(), new WindowRenderTargetProperties()
            {
                Handle = this.Handle,
                PixelSize = this.ClientSize
            });

            this.ClientSizeChanged += new EventHandler(DXRenderWindow_ClientSizeChanged);
            this.MouseWheel += new MouseEventHandler(DXRenderWindow_MouseWheel);
            this.MouseDown += new MouseEventHandler(DXRenderWindow_MouseDown);
            this.MouseMove += new MouseEventHandler(DXRenderWindow_MouseMove);

            renderTrans = Vector2.Zero;
        }

        private void DXRenderWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                curMouse = e.Location;
            }            
        }

        private void DXRenderWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Vector2 trans = new Vector2(e.X - curMouse.X, e.Y - curMouse.Y);

                renderTrans.X += trans.X;
                renderTrans.Y += trans.Y;

                curMouse = e.Location;

                this.Invalidate();
            }
        }

        private void DXRenderWindow_MouseWheel(object sender, MouseEventArgs e)
        {
            renderScale += e.Delta * 0.001f;
            if (renderScale < 0.1f)
                renderScale = 0.1f;

            this.Invalidate();
        }

        private void DXRenderWindow_ClientSizeChanged(object sender, EventArgs e)
        {
            renderTarget.Resize(this.ClientSize);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (!renderTarget.IsOccluded)
            {
                renderTarget.BeginDraw();
                renderTarget.Transform = Matrix3x2.Identity;
                renderTarget.Clear(this.BackColor);

                if (UserPaint != null)
                    UserPaint(renderTarget);

                renderTarget.EndDraw();
            }
        }

        public void Transform(Vector2 trans, double theta)
        {
            Matrix m = Matrix.Transformation2D(Vector2.Zero, 0, new Vector2(renderScale, renderScale), 
                                               Vector2.Zero, (float)theta, trans + renderTrans);

            renderMatrix = Matrix3x2.Identity;

            renderMatrix.M11 = m.M11;
            renderMatrix.M12 = m.M12;
            renderMatrix.M21 = m.M21;
            renderMatrix.M22 = m.M22;
            renderMatrix.M31 = m.M41;
            renderMatrix.M32 = m.M42;
        }
    }
}
