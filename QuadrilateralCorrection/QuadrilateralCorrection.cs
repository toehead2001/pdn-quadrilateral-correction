using System;
using System.Drawing;
using System.Reflection;
using System.Collections.Generic;
using PaintDotNet;
using PaintDotNet.Effects;
using AForge;
using AForge.Imaging.Filters;

namespace QuadrilateralCorrectionEffect
{
    public class PluginSupportInfo : IPluginSupportInfo
    {
        public string Author
        {
            get
            {
                return ((AssemblyCopyrightAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0]).Copyright;
            }
        }
        public string Copyright
        {
            get
            {
                return ((AssemblyDescriptionAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0]).Description;
            }
        }

        public string DisplayName
        {
            get
            {
                return ((AssemblyProductAttribute)base.GetType().Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0]).Product;
            }
        }

        public Version Version
        {
            get
            {
                return base.GetType().Assembly.GetName().Version;
            }
        }

        public Uri WebsiteUri
        {
            get
            {
                return new Uri("http://forums.getpaint.net/index.php?showtopic=110247");
            }
        }
    }

    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Quadrilateral Correction")]
    internal class QuadrilateralCorrectionEffectPlugin : Effect<QuadrilateralCorrectionConfigToken>
    {
        public static string StaticName
        {
            get
            {
                return "Quadrilateral Correction";
            }
        }

        public static Image StaticIcon
        {
            get
            {
                return new Bitmap(typeof(QuadrilateralCorrectionEffectPlugin), "Icon.png");
            }
        }

        public static string SubmenuName
        {
            get
            {
                return SubmenuNames.Distort;
            }
        }

        public QuadrilateralCorrectionEffectPlugin()
            : base(StaticName, StaticIcon, SubmenuName, EffectFlags.Configurable)
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
            //scale = newToken.Scale;
            center = newToken.Center;

            // define quadrilateral's corners
            List<IntPoint> corners = new List<IntPoint>();
            corners.Add(new IntPoint(topLeft.X, topLeft.Y));
            corners.Add(new IntPoint(topRight.X, topRight.Y));
            corners.Add(new IntPoint(bottomRight.X, bottomRight.Y));
            corners.Add(new IntPoint(bottomLeft.X, bottomLeft.Y));

            // create filter
            QuadrilateralTransformation quadTrans = new QuadrilateralTransformation();
            quadTrans.SourceQuadrilateral = corners;
            quadTrans.AutomaticSizeCalculaton = autoDims;
            quadTrans.NewWidth = width;
            quadTrans.NewHeight = height;
            //filter.UseInterpolation = Amount8;

            Rectangle selection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds).GetBoundsInt();
            PdnRegion exactSelection = EnvironmentParameters.GetSelection(srcArgs.Surface.Bounds);

            Bitmap srcImage = srcArgs.Surface.CreateAliasedBitmap(selection);

            Bitmap quadTransOutput;
            try
            {
                quadTransOutput = quadTrans.Apply(srcImage);
            }
            catch
            {
                quadTransOutput = new Bitmap(1, 1);
            }
            srcImage.Dispose();


            System.Drawing.Point offSet = new System.Drawing.Point();
            offSet.X = selection.X + (center ? (selection.Width - quadTransOutput.Width) / 2 : 0);
            offSet.Y = selection.Y + (center ? (selection.Height - quadTransOutput.Height) / 2 : 0);

            Bitmap alignedImage = new Bitmap(srcArgs.Surface.Width, srcArgs.Surface.Height);
            using (Graphics graphics = Graphics.FromImage(alignedImage))
            {
                graphics.DrawImage(quadTransOutput, offSet);
            }
            quadTransOutput = alignedImage;


            if (quadrilateralSurface == null)
                quadrilateralSurface = new Surface(srcArgs.Surface.Size);

            quadrilateralSurface = Surface.CopyFromBitmap(quadTransOutput);
            quadTransOutput.Dispose();

            dstArgs.Surface.Clear(exactSelection, Color.Transparent);
            dstArgs.Surface.CopySurface(quadrilateralSurface, exactSelection);


            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }


        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            return;
        }

        System.Drawing.Point topLeft;
        System.Drawing.Point topRight;
        System.Drawing.Point bottomRight;
        System.Drawing.Point bottomLeft;
        bool autoDims;
        int width;
        int height;
        //float scale;
        bool center;

        Surface quadrilateralSurface;
    }
}
