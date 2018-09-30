using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using PaintDotNet;
using PaintDotNet.Effects;
using AForge;
using AForge.Imaging.Filters;
using Point = System.Drawing.Point;

namespace QuadrilateralCorrectionEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author => base.GetType().Assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
        public string Copyright => base.GetType().Assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
        public string DisplayName => base.GetType().Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
        public Version Version => base.GetType().Assembly.GetName().Version;
        public Uri WebsiteUri => new Uri("https://forums.getpaint.net/index.php?showtopic=110247");
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Quadrilateral Correction")]
    internal class QuadrilateralCorrectionEffectPlugin : Effect<QuadrilateralCorrectionConfigToken>
    {
        private static readonly Image StaticIcon = new Bitmap(typeof(QuadrilateralCorrectionEffectPlugin), "Icon.png");

        public QuadrilateralCorrectionEffectPlugin()
            : base("Quadrilateral Correction", StaticIcon, "Tools", EffectFlags.Configurable)
        {
        }

        public override EffectConfigDialog CreateConfigDialog()
        {
            return new QuadrilateralCorrectionConfigDialog();
        }

        protected override void OnSetRenderInfo(QuadrilateralCorrectionConfigToken newToken, RenderArgs dstArgs, RenderArgs srcArgs)
        {
            topLeft = newToken.TopLeft;
            topRight = newToken.TopRight;
            bottomRight = newToken.BottomRight;
            bottomLeft = newToken.BottomLeft;
            autoDims = newToken.AutoDims;
            width = newToken.Width;
            height = newToken.Height;
            center = newToken.Center;

            // create filter
            QuadrilateralTransformation quadTrans = new QuadrilateralTransformation
            {
                SourceQuadrilateral = new List<IntPoint>
                {
                    new IntPoint(topLeft.X, topLeft.Y),
                    new IntPoint(topRight.X, topRight.Y),
                    new IntPoint(bottomRight.X, bottomRight.Y),
                    new IntPoint(bottomLeft.X, bottomLeft.Y)
                },
                AutomaticSizeCalculaton = autoDims,
                NewWidth = width,
                NewHeight = height
            };

            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();
            PdnRegion exactSelection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds);

            Bitmap quadTransOutput;
            try
            {
                using (Bitmap srcImage = srcArgs.Surface.CreateAliasedBitmap(selection))
                    quadTransOutput = quadTrans.Apply(srcImage);
            }
            catch
            {
                quadTransOutput = new Bitmap(1, 1);
            }

            Point offSet = new Point
            {
                X = selection.X + (center ? (selection.Width - quadTransOutput.Width) / 2 : 0),
                Y = selection.Y + (center ? (selection.Height - quadTransOutput.Height) / 2 : 0)
            };

            Bitmap alignedImage = new Bitmap(srcArgs.Surface.Width, srcArgs.Surface.Height);
            using (Graphics graphics = Graphics.FromImage(alignedImage))
            {
                graphics.DrawImage(quadTransOutput, offSet);
            }
            quadTransOutput.Dispose();

            if (quadrilateralSurface == null)
            {
                quadrilateralSurface = new Surface(srcArgs.Surface.Size);
            }

            quadrilateralSurface = Surface.CopyFromBitmap(alignedImage);
            alignedImage.Dispose();

            dstArgs.Surface.Clear(exactSelection, Color.Transparent);
            dstArgs.Surface.CopySurface(quadrilateralSurface, exactSelection);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            return;
        }

        private Point topLeft;
        private Point topRight;
        private Point bottomRight;
        private Point bottomLeft;
        private bool autoDims;
        private int width;
        private int height;
        private bool center;

        private Surface quadrilateralSurface;
    }
}
