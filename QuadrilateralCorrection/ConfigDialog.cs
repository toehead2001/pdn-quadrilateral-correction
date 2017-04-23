using PaintDotNet.Effects;
using System;
using System.Drawing;

namespace QuadrilateralCorrectionEffect
{
    internal partial class QuadrilateralCorrectionConfigDialog : EffectConfigDialog<QuadrilateralCorrectionEffectPlugin, QuadrilateralCorrectionConfigToken>
    {
        Rectangle uiImgBounds;
        Rectangle selection;
        Bitmap srcImage;

        public QuadrilateralCorrectionConfigDialog()
        {
            InitializeComponent();
        }

        private void QuadrilateralCorrectionConfigDialog_Load(object sender, EventArgs e)
        {
            Initializers();

            quadControl11.Width = uiImgBounds.Width + 2;
            quadControl11.Height = uiImgBounds.Height + 2;
            quadControl11.Location = new Point(quadControl11.Location.X + uiImgBounds.X, quadControl11.Location.Y + uiImgBounds.Y);

            checkBoxAutoDims.Left = label10.Left;
            checkBoxCenter.Left = label10.Left;
        }

        private void Initializers()
        {
            selection = Selection.GetBoundsInt();

            numericUpDownTopLeftX.Maximum = selection.Width - 1;
            numericUpDownTopLeftY.Maximum = selection.Height - 1;
            numericUpDownTopRightX.Maximum = selection.Width - 1;
            numericUpDownTopRightY.Maximum = selection.Height - 1;
            numericUpDownBottomRightX.Maximum = selection.Width - 1;
            numericUpDownBottomRightY.Maximum = selection.Height - 1;
            numericUpDownBottomLeftX.Maximum = selection.Width - 1;
            numericUpDownBottomLeftY.Maximum = selection.Height - 1;

            numericUpDown1.Maximum = selection.Width;
            numericUpDown2.Maximum = selection.Height;


            srcImage = EffectSourceSurface.CreateAliasedBitmap(selection);

            quadControl11.Image = srcImage;

            float quadBaseSize = this.AutoScaleDimensions.Width / 96f * 500f;
            float divisor = Math.Max(srcImage.Width, srcImage.Height) / quadBaseSize;

            uiImgBounds.Width = (int)Math.Round(srcImage.Width / divisor);
            uiImgBounds.Height = (int)Math.Round(srcImage.Height / divisor);
            uiImgBounds.X = (int)Math.Max(0, (quadBaseSize - uiImgBounds.Width) / 2f);
            uiImgBounds.Y = (int)Math.Max(0, (quadBaseSize - uiImgBounds.Height) / 2f);
        }

        #region Values-Changed events
        private void numericUpDownTopLeft_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            Point topLeft = new Point();
            topLeft.X = (int)Math.Round(numericUpDownTopLeftX.Value * (uiImgBounds.Width - 1) / (selection.Width - 1));
            topLeft.Y = (int)Math.Round(numericUpDownTopLeftY.Value * (uiImgBounds.Height - 1) / (selection.Height - 1));
            quadControl11.NubTL = topLeft;

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            FinishTokenUpdate();
        }

        private void numericUpDownTopRight_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            Point topright = new Point();
            topright.X = (int)Math.Round(numericUpDownTopRightX.Value * (uiImgBounds.Width - 1) / (selection.Width - 1));
            topright.Y = (int)Math.Round(numericUpDownTopRightY.Value * (uiImgBounds.Height - 1) / (selection.Height - 1));
            quadControl11.NubTR = topright;

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            FinishTokenUpdate();
        }

        private void numericUpDownBottomRight_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            Point bottomRight = new Point();
            bottomRight.X = (int)Math.Round(numericUpDownBottomRightX.Value * (uiImgBounds.Width - 1) / (selection.Width - 1));
            bottomRight.Y = (int)Math.Round(numericUpDownBottomRightY.Value * (uiImgBounds.Height - 1) / (selection.Height - 1));
            quadControl11.NubBR = bottomRight;

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            FinishTokenUpdate();
        }

        private void numericUpDownBottomLeft_ValueChanged(object sender, EventArgs e)
        {
            quadControl11.ValueChanged -= quadControl11_ValueChanged;

            Point bottomLeft = new Point();
            bottomLeft.X = (int)Math.Round(numericUpDownBottomLeftX.Value * (uiImgBounds.Width - 1) / (selection.Width - 1));
            bottomLeft.Y = (int)Math.Round(numericUpDownBottomLeftY.Value * (uiImgBounds.Height - 1) / (selection.Height - 1));
            quadControl11.NubBL = bottomLeft;

            quadControl11.ValueChanged += quadControl11_ValueChanged;

            FinishTokenUpdate();
        }

        private void quadControl11_ValueChanged(object sender)
        {
            numericUpDownTopLeftX.ValueChanged -= numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftY.ValueChanged -= numericUpDownTopLeft_ValueChanged;
            numericUpDownTopRightX.ValueChanged -= numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightY.ValueChanged -= numericUpDownTopRight_ValueChanged;
            numericUpDownBottomRightX.ValueChanged -= numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightY.ValueChanged -= numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomLeftX.ValueChanged -= numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftY.ValueChanged -= numericUpDownBottomLeft_ValueChanged;

            numericUpDownTopLeftX.Value = quadControl11.NubTL.X * (selection.Width - 1) / (uiImgBounds.Width - 1);
            numericUpDownTopLeftY.Value = quadControl11.NubTL.Y * (selection.Height - 1) / (uiImgBounds.Height - 1);

            numericUpDownTopRightX.Value = quadControl11.NubTR.X * (selection.Width - 1) / (uiImgBounds.Width - 1);
            numericUpDownTopRightY.Value = quadControl11.NubTR.Y * (selection.Height - 1) / (uiImgBounds.Height - 1);

            numericUpDownBottomRightX.Value = quadControl11.NubBR.X * (selection.Width - 1) / (uiImgBounds.Width - 1);
            numericUpDownBottomRightY.Value = quadControl11.NubBR.Y * (selection.Height - 1) / (uiImgBounds.Height - 1);

            numericUpDownBottomLeftX.Value = quadControl11.NubBL.X * (selection.Width - 1) / (uiImgBounds.Width - 1);
            numericUpDownBottomLeftY.Value = quadControl11.NubBL.Y * (selection.Height - 1) / (uiImgBounds.Height - 1);

            numericUpDownTopLeftX.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopLeftY.ValueChanged += numericUpDownTopLeft_ValueChanged;
            numericUpDownTopRightX.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownTopRightY.ValueChanged += numericUpDownTopRight_ValueChanged;
            numericUpDownBottomRightX.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomRightY.ValueChanged += numericUpDownBottomRight_ValueChanged;
            numericUpDownBottomLeftX.ValueChanged += numericUpDownBottomLeft_ValueChanged;
            numericUpDownBottomLeftY.ValueChanged += numericUpDownBottomLeft_ValueChanged;

            FinishTokenUpdate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAutoDims.Checked)
            {
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                label9.Enabled = false;
                label10.Enabled = false;

                numericUpDown1.Text = "-";
                numericUpDown2.Text = "-";
            }
            else
            {
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                label9.Enabled = true;
                label10.Enabled = true;
                SetDimensionValues();
            }

            FinishTokenUpdate();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            FinishTokenUpdate();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            FinishTokenUpdate();
        }

        private void checkBoxCenter_CheckedChanged(object sender, EventArgs e)
        {
            FinishTokenUpdate();
        }
        #endregion

        private void numericUpDown_Enter(object sender, EventArgs e)
        {
            (sender as System.Windows.Forms.NumericUpDown).Select(0, (sender as System.Windows.Forms.NumericUpDown).Text.Length);
        }

        #region Token Stuff
        protected override QuadrilateralCorrectionConfigToken CreateInitialToken()
        {
            return new QuadrilateralCorrectionConfigToken();
        }

        protected override void InitDialogFromToken(QuadrilateralCorrectionConfigToken effectTokenCopy)
        {
            Initializers();

            numericUpDownTopLeftX.Value = Clamp(effectTokenCopy.TopLeft.X, numericUpDownTopLeftX.Minimum, numericUpDownTopLeftX.Maximum);
            numericUpDownTopLeftY.Value = Clamp(effectTokenCopy.TopLeft.Y, numericUpDownTopLeftY.Minimum, numericUpDownTopLeftY.Maximum);
            numericUpDownTopRightX.Value = Clamp(effectTokenCopy.TopRight.X, numericUpDownTopRightX.Minimum, numericUpDownTopRightX.Maximum);
            numericUpDownTopRightY.Value = Clamp(effectTokenCopy.TopRight.Y, numericUpDownTopRightY.Minimum, numericUpDownTopRightY.Maximum);
            numericUpDownBottomRightX.Value = Clamp(effectTokenCopy.BottomRight.X, numericUpDownBottomRightX.Minimum, numericUpDownBottomRightX.Maximum);
            numericUpDownBottomRightY.Value = Clamp(effectTokenCopy.BottomRight.Y, numericUpDownBottomRightY.Minimum, numericUpDownBottomRightY.Maximum);
            numericUpDownBottomLeftX.Value = Clamp(effectTokenCopy.BottomLeft.X, numericUpDownBottomLeftX.Minimum, numericUpDownBottomLeftX.Maximum);
            numericUpDownBottomLeftY.Value = Clamp(effectTokenCopy.BottomLeft.Y, numericUpDownBottomLeftY.Minimum, numericUpDownBottomLeftY.Maximum);

            checkBoxAutoDims.Checked = effectTokenCopy.AutoDims;
            numericUpDown1.Value = Clamp(effectTokenCopy.Width, numericUpDown1.Minimum, numericUpDown1.Maximum);
            numericUpDown2.Value = Clamp(effectTokenCopy.Height, numericUpDown2.Minimum, numericUpDown2.Maximum);
            if (checkBoxAutoDims.Checked)
            {
                numericUpDown1.Text = "-";
                numericUpDown2.Text = "-";
            }
            checkBoxCenter.Checked = effectTokenCopy.Center;
        }

        protected override void LoadIntoTokenFromDialog(QuadrilateralCorrectionConfigToken writeValuesHere)
        {
            writeValuesHere.TopLeft = new Point((int)numericUpDownTopLeftX.Value, (int)numericUpDownTopLeftY.Value);
            writeValuesHere.TopRight = new Point((int)numericUpDownTopRightX.Value, (int)numericUpDownTopRightY.Value);
            writeValuesHere.BottomRight = new Point((int)numericUpDownBottomRightX.Value, (int)numericUpDownBottomRightY.Value);
            writeValuesHere.BottomLeft = new Point((int)numericUpDownBottomLeftX.Value, (int)numericUpDownBottomLeftY.Value);

            writeValuesHere.AutoDims = checkBoxAutoDims.Checked;
            writeValuesHere.Width = (int)numericUpDown1.Value;
            writeValuesHere.Height = (int)numericUpDown2.Value;
            writeValuesHere.Center = checkBoxCenter.Checked;
        }
        #endregion

        private static decimal Clamp(decimal value, decimal min, decimal max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void SetDimensionValues()
        {
            System.Collections.Generic.List<AForge.IntPoint> corners = new System.Collections.Generic.List<AForge.IntPoint>();
            corners.Add(new AForge.IntPoint((int)numericUpDownTopLeftX.Value, (int)numericUpDownTopLeftY.Value));
            corners.Add(new AForge.IntPoint((int)numericUpDownTopRightX.Value, (int)numericUpDownTopRightY.Value));
            corners.Add(new AForge.IntPoint((int)numericUpDownBottomRightX.Value, (int)numericUpDownBottomRightY.Value));
            corners.Add(new AForge.IntPoint((int)numericUpDownBottomLeftX.Value, (int)numericUpDownBottomLeftY.Value));
            AForge.Imaging.Filters.QuadrilateralTransformation quadTrans = new AForge.Imaging.Filters.QuadrilateralTransformation();
            quadTrans.SourceQuadrilateral = corners;
            Size quadTransOutput;
            try
            {
                quadTransOutput = quadTrans.Apply(srcImage).Size;
            }
            catch
            {
                quadTransOutput = Size.Empty;
            }
            numericUpDown1.Value = Clamp(quadTransOutput.Width, numericUpDown1.Minimum, numericUpDown1.Maximum);
            numericUpDown2.Value = Clamp(quadTransOutput.Height, numericUpDown2.Minimum, numericUpDown2.Maximum);
            numericUpDown1.Text = numericUpDown1.Value.ToString();
            numericUpDown2.Text = numericUpDown2.Value.ToString();
        }

        private void QuadrilateralCorrectionConfigDialog_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (quadControl11.SelectedNub == 0)
                return;

            e.Handled = true;

            int horAmount = 0;
            int verAmount = 0;

            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                if (e.Modifiers == System.Windows.Forms.Keys.Control)
                    verAmount = -5;
                else
                    verAmount = -1;
            }
            else if ((e.KeyCode == System.Windows.Forms.Keys.Right))
            {
                if (e.Modifiers == System.Windows.Forms.Keys.Control)
                    horAmount = 5;
                else
                    horAmount = 1;
            }
            else if ((e.KeyCode == System.Windows.Forms.Keys.Down))
            {
                if (e.Modifiers == System.Windows.Forms.Keys.Control)
                    verAmount = 5;
                else
                    verAmount = 1;
            }
            else if ((e.KeyCode == System.Windows.Forms.Keys.Left))
            {
                if (e.Modifiers == System.Windows.Forms.Keys.Control)
                    horAmount = -5;
                else
                    horAmount = -1;
            }
            else
            {
                return;
            }

            switch (quadControl11.SelectedNub)
            {
                case 1:
                    numericUpDownTopLeftX.Value = Clamp(numericUpDownTopLeftX.Value + horAmount, numericUpDownTopLeftX.Minimum, numericUpDownTopLeftX.Maximum);
                    numericUpDownTopLeftY.Value = Clamp(numericUpDownTopLeftY.Value + verAmount, numericUpDownTopLeftY.Minimum, numericUpDownTopLeftY.Maximum);
                    break;
                case 2:
                    numericUpDownTopRightX.Value = Clamp(numericUpDownTopRightX.Value + horAmount, numericUpDownTopRightX.Minimum, numericUpDownTopRightX.Maximum);
                    numericUpDownTopRightY.Value = Clamp(numericUpDownTopRightY.Value + verAmount, numericUpDownTopRightY.Minimum, numericUpDownTopRightY.Maximum);
                    break;
                case 3:
                    numericUpDownBottomRightX.Value = Clamp(numericUpDownBottomRightX.Value + horAmount, numericUpDownBottomRightX.Minimum, numericUpDownBottomRightX.Maximum);
                    numericUpDownBottomRightY.Value = Clamp(numericUpDownBottomRightY.Value + verAmount, numericUpDownBottomRightY.Minimum, numericUpDownBottomRightY.Maximum);
                    break;
                case 4:
                    numericUpDownBottomLeftX.Value = Clamp(numericUpDownBottomLeftX.Value + horAmount, numericUpDownBottomLeftX.Minimum, numericUpDownBottomLeftX.Maximum);
                    numericUpDownBottomLeftY.Value = Clamp(numericUpDownBottomLeftY.Value + verAmount, numericUpDownBottomLeftY.Minimum, numericUpDownBottomLeftY.Maximum);
                    break;
            }
        }

        private void QuadrilateralCorrectionConfigDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            string helpMessage = "The control nubs can be manipulated with the mouse in the following three ways:\n";
            helpMessage += "\n";
            helpMessage += "Left Mouse Button — Grab and Drag\n";
            helpMessage += "\n";
            helpMessage += "Middle Mouse Button — Grab and Drag with a Dead Zone\n";
            helpMessage += "\n";
            helpMessage += "Right Mouse Button — Select nub for Keyboard Arrow manipulation\n";
            helpMessage += "    Arrow — 1px\n";
            helpMessage += "    Ctrl + Arrow — 5px\n";
            System.Windows.Forms.MessageBox.Show(helpMessage, "Help");
        }
    }
}
