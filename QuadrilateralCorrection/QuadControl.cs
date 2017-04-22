using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace QuadControl
{
    internal class QuadControl : PictureBox
    {
        public QuadControl()
        {
            this.BackgroundImage = global::QuadrilateralCorrectionEffect.Resources.CheckerBoard;
            //this.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.TabStop = false;

            using (var memoryStream = new MemoryStream(QuadrilateralCorrectionEffect.Resources.HandOpen))
                handOpen = new Cursor(memoryStream);
            using (var memoryStream = new MemoryStream(QuadrilateralCorrectionEffect.Resources.HandGrab))
                handGrab = new Cursor(memoryStream);
        }

        #region Variables
        bool MouseIsDown = false; // True if mouse button is down
        Point MouseFromNub = new Point();
        int Radius = 3; // nb Radius * 2 + 1 = size
        Cursor handOpen;
        Cursor handGrab;
        const int DeadZone = 30;
        #endregion

        #region Properties
        // four Nubs to store coordinates and activation states
        Nub nubTL, nubTR, nubBR, nubBL;

        // four publicly accessible get/sets which map the internal location variables
        public Point NubTL
        {
            get => nubTL.Location;
            set
            {
                nubTL.Location = value;
                OnValueChanged();
                this.Refresh();
            }
        }
        public Point NubTR
        {
            get => nubTR.Location;
            set
            {
                nubTR.Location = value;
                OnValueChanged();
                this.Refresh();
            }
        }
        public Point NubBR
        {
            get => nubBR.Location;
            set
            {
                nubBR.Location = value;
                OnValueChanged();
                this.Refresh();
            }
        }
        public Point NubBL
        {
            get => nubBL.Location;
            set
            {
                nubBL.Location = value;
                OnValueChanged();
                this.Refresh();
            }
        }

        public byte SelectedNub
        {
            get
            {
                byte nub = 0;

                if (nubTL.Selected)
                    nub = 1;
                else if (nubTR.Selected)
                    nub = 2;
                else if (nubBR.Selected)
                    nub = 3;
                else if (nubBL.Selected)
                    nub = 4;

                return nub;
            }
        }
        #endregion

        #region Event handler
        // delegate event handler
        public delegate void ValueChangedEventHandler(object sender);
        public event ValueChangedEventHandler ValueChanged;

        protected void OnValueChanged()
        {
            this.ValueChanged?.Invoke(this);
        }
        #endregion

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;

            // Draw quadrilateral
            using (Pen outlinePen = new Pen(Color.Black))
            {
                outlinePen.Color = Color.White;
                outlinePen.DashStyle = DashStyle.Dash;
                pe.Graphics.DrawLine(outlinePen, nubTL.Location, nubTR.Location);
                pe.Graphics.DrawLine(outlinePen, nubTR.Location, nubBR.Location);
                pe.Graphics.DrawLine(outlinePen, nubBR.Location, nubBL.Location);
                pe.Graphics.DrawLine(outlinePen, nubBL.Location, nubTL.Location);

                outlinePen.Color = Color.Black;
                outlinePen.DashStyle = DashStyle.Dot;
                pe.Graphics.DrawLine(outlinePen, nubTL.Location, nubTR.Location);
                pe.Graphics.DrawLine(outlinePen, nubTR.Location, nubBR.Location);
                pe.Graphics.DrawLine(outlinePen, nubBR.Location, nubBL.Location);
                pe.Graphics.DrawLine(outlinePen, nubBL.Location, nubTL.Location);
            }

            // Draw Nubs
            using (Pen nubPen = new Pen(Color.White, 4))
            using (Pen nubStatePen = new Pen(Color.Black, 1.6f))
            {
                // Top Left control nub
                Radius = (nubTL.Hovered || nubTL.Selected) ? 5 : 3;
                pe.Graphics.DrawEllipse(nubPen, nubTL.X - Radius, nubTL.Y - Radius, Radius * 2 + 1, Radius * 2 + 1);
                nubStatePen.Color = (nubTL.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubTL.X - Radius, nubTL.Y - Radius, Radius * 2 + 1, Radius * 2 + 1);

                // Top Right control nub
                Radius = (nubTR.Hovered || nubTR.Selected) ? 5 : 3;
                pe.Graphics.DrawEllipse(nubPen, nubTR.X - Radius - 1, nubTR.Y - Radius, Radius * 2 + 1, Radius * 2 + 1);
                nubStatePen.Color = (nubTR.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubTR.X - Radius - 1, nubTR.Y - Radius, Radius * 2 + 1, Radius * 2 + 1);

                // Bottom Right control nub
                Radius = (nubBR.Hovered || nubBR.Selected) ? 5 : 3;
                pe.Graphics.DrawEllipse(nubPen, nubBR.X - Radius - 1, nubBR.Y - Radius - 1, Radius * 2 + 1, Radius * 2 + 1);
                nubStatePen.Color = (nubBR.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubBR.X - Radius - 1, nubBR.Y - Radius - 1, Radius * 2 + 1, Radius * 2 + 1);

                // Bottom Left control nub
                Radius = (nubBL.Hovered || nubBL.Selected) ? 5 : 3;
                pe.Graphics.DrawEllipse(nubPen, nubBL.X - Radius, nubBL.Y - Radius - 1, Radius * 2 + 1, Radius * 2 + 1);
                nubStatePen.Color = (nubBL.Selected) ? Color.DodgerBlue : Color.Black;
                pe.Graphics.DrawEllipse(nubStatePen, nubBL.X - Radius, nubBL.Y - Radius - 1, Radius * 2 + 1, Radius * 2 + 1);
            }
        }

        #region Mouse events
        protected override void OnMouseDown(MouseEventArgs e)
        {
            MouseIsDown = true; // because the mouse button is down
            //MouseDownStart = e.Location; // has the location of the mouse pointer when the button is pressed

            // find which control nub is being activated (if any)
            if (NearNub(e.Location, nubTL))
            {
                if (e.Button == MouseButtons.Right)
                {
                    SelectNub(nubTL);
                }
                else
                {
                    GrabNub(nubTL, e.Location);
                }
            }
            else if (NearNub(e.Location, nubTR))
            {
                if (e.Button == MouseButtons.Right)
                {
                    SelectNub(nubTR);
                }
                else
                {
                    GrabNub(nubTR, e.Location);
                }
            }
            else if (NearNub(e.Location, nubBR))
            {
                if (e.Button == MouseButtons.Right)
                {
                    SelectNub(nubBR);
                }
                else
                {
                    GrabNub(nubBR, e.Location);
                }
            }
            else if (NearNub(e.Location, nubBL))
            {
                if (e.Button == MouseButtons.Right)
                {
                    SelectNub(nubBL);
                }
                else
                {
                    GrabNub(nubBL, e.Location);
                }
            }

            this.Refresh();

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            nubTL.Grabbed = false;
            nubTR.Grabbed = false;
            nubBR.Grabbed = false;
            nubBL.Grabbed = false;
            MouseIsDown = false;

            this.Refresh();
            OnValueChanged();

            base.OnMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!MouseIsDown)
            {
                if (NearNub(e.Location, nubTL))
                {
                    HoverNub(nubTL);
                }
                else if (NearNub(e.Location, nubTR))
                {
                    HoverNub(nubTR);
                }
                else if (NearNub(e.Location, nubBR))
                {
                    HoverNub(nubBR);
                }
                else if (NearNub(e.Location, nubBL))
                {
                    HoverNub(nubBL);
                }
                else
                {
                    UnHoverNubs();
                }
            }
            else if (nubTL.Grabbed)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    if (e.X <= nubTL.X - DeadZone)
                        nubTL.X = ClampToWidth(e.X + DeadZone);
                    else if (e.X >= nubTL.X + DeadZone)
                        nubTL.X = ClampToWidth(e.X - DeadZone);

                    if (e.Y <= nubTL.Y - DeadZone)
                        nubTL.Y = ClampToHeight(e.Y + DeadZone);
                    else if (e.Y >= nubTL.Y + DeadZone)
                        nubTL.Y = ClampToHeight(e.Y - DeadZone);
                }
                else
                {
                    nubTL.X = ClampToWidth(e.X - MouseFromNub.X);
                    nubTL.Y = ClampToHeight(e.Y - MouseFromNub.Y);
                }
            }
            else if (nubTR.Grabbed)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    if (e.X <= nubTR.X - DeadZone)
                        nubTR.X = ClampToWidth(e.X + DeadZone);
                    else if (e.X >= nubTR.X + DeadZone)
                        nubTR.X = ClampToWidth(e.X - DeadZone);

                    if (e.Y <= nubTR.Y - DeadZone)
                        nubTR.Y = ClampToHeight(e.Y + DeadZone);
                    else if (e.Y >= nubTR.Y + DeadZone)
                        nubTR.Y = ClampToHeight(e.Y - DeadZone);
                }
                else
                {
                    nubTR.X = ClampToWidth(e.X - MouseFromNub.X);
                    nubTR.Y = ClampToHeight(e.Y - MouseFromNub.Y);
                }
            }
            else if (nubBR.Grabbed)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    if (e.X <= nubBR.X - DeadZone)
                        nubBR.X = ClampToWidth(e.X + DeadZone);
                    else if (e.X >= nubBR.X + DeadZone)
                        nubBR.X = ClampToWidth(e.X - DeadZone);

                    if (e.Y <= nubBR.Y - DeadZone)
                        nubBR.Y = ClampToHeight(e.Y + DeadZone);
                    else if (e.Y >= nubBR.Y + DeadZone)
                        nubBR.Y = ClampToHeight(e.Y - DeadZone);
                }
                else
                {
                    nubBR.X = ClampToWidth(e.X - MouseFromNub.X);
                    nubBR.Y = ClampToHeight(e.Y - MouseFromNub.Y);
                }
            }
            else if (nubBL.Grabbed)
            {
                if (e.Button == MouseButtons.Middle)
                {
                    if (e.X <= nubBL.X - DeadZone)
                        nubBL.X = ClampToWidth(e.X + DeadZone);
                    else if (e.X >= nubBL.X + DeadZone)
                        nubBL.X = ClampToWidth(e.X - DeadZone);

                    if (e.Y <= nubBL.Y - DeadZone)
                        nubBL.Y = ClampToHeight(e.Y + DeadZone);
                    else if (e.Y >= nubBL.Y + DeadZone)
                        nubBL.Y = ClampToHeight(e.Y - DeadZone);
                }
                else
                {
                    nubBL.X = ClampToWidth(e.X - MouseFromNub.X);
                    nubBL.Y = ClampToHeight(e.Y - MouseFromNub.Y);
                }
            }
            this.Refresh();
            if (MouseIsDown)
                OnValueChanged();

            base.OnMouseMove(e);
        }
        #endregion

        #region Nub functions
        private void SelectNub(Nub nub)
        {
            nubTL.Selected = false;
            nubTR.Selected = false;
            nubBR.Selected = false;
            nubBL.Selected = false;

            if (nub.Location == nubTL.Location)
            {
                nubTL.Selected = !nub.Selected;
            }
            else if (nub.Location == nubTR.Location)
            {
                nubTR.Selected = !nub.Selected;
            }
            else if (nub.Location == nubBR.Location)
            {
                nubBR.Selected = !nub.Selected;
            }
            else if (nub.Location == nubBL.Location)
            {
                nubBL.Selected = !nub.Selected;
            }

            nubTL.Grabbed = false;
            nubTR.Grabbed = false;
            nubBR.Grabbed = false;
            nubBL.Grabbed = false;

            this.Cursor = Cursors.Default;
        }

        private void GrabNub(Nub nub, Point mouseLocation)
        {
            nubTL.Grabbed = false;
            nubTR.Grabbed = false;
            nubBR.Grabbed = false;
            nubBL.Grabbed = false;

            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            if (nub.Location == nubTL.Location)
            {
                nubTL.Grabbed = true;
                nubTL.Hovered = true;
            }
            else if (nub.Location == nubTR.Location)
            {
                nubTR.Grabbed = true;
                nubTR.Hovered = true;
            }
            else if (nub.Location == nubBR.Location)
            {
                nubBR.Grabbed = true;
                nubBR.Hovered = true;
            }
            else if (nub.Location == nubBL.Location)
            {
                nubBL.Grabbed = true;
                nubBL.Hovered = true;
            }

            nubTL.Selected = false;
            nubTR.Selected = false;
            nubBR.Selected = false;
            nubBL.Selected = false;

            MouseFromNub.X = mouseLocation.X - nub.X;
            MouseFromNub.Y = mouseLocation.Y - nub.Y;

            this.Cursor = handGrab;
        }

        private void UnHoverNubs()
        {
            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            this.Cursor = Cursors.Default;
        }

        private void HoverNub(Nub nub)
        {
            nubTL.Hovered = false;
            nubTR.Hovered = false;
            nubBR.Hovered = false;
            nubBL.Hovered = false;

            if (nub.Location == nubTL.Location)
            {
                nubTL.Hovered = true;
            }
            else if (nub.Location == nubTR.Location)
            {
                nubTR.Hovered = true;
            }
            else if (nub.Location == nubBR.Location)
            {
                nubBR.Hovered = true;
            }
            else if (nub.Location == nubBL.Location)
            {
                nubBL.Hovered = true;
            }

            this.Cursor = handOpen;
        }

        private bool NearNub(Point mouseLocation, Nub nub)
        {
            return ((Math.Abs(mouseLocation.X - nub.X) <= Radius + 10) && (Math.Abs(mouseLocation.Y - nub.Y) <= Radius + 10));
        }
        #endregion

        #region Utility routines
        private int ClampToWidth(int x)
        {
            int y = (x < 0) ? 0 : (x > this.ClientSize.Width - 1) ? this.ClientSize.Width - 1 : x;
            return y;
        }

        private int ClampToHeight(int x)
        {
            int y = (x < 0) ? 0 : (x > this.ClientSize.Height - 1) ? this.ClientSize.Height - 1 : x;
            return y;
        }
        #endregion

        private struct Nub
        {
            private Point location;
            internal Point Location
            {
                get => location;
                set => location = value;
            }
            internal int X
            {
                get => location.X;
                set => location.X = value;
            }
            internal int Y
            {
                get => location.Y;
                set => location.Y = value;
            }
            internal bool Grabbed { get; set; }
            internal bool Hovered { get; set; }
            internal bool Selected { get; set; }
        }
    }
}