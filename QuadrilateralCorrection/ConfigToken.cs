using PaintDotNet.Effects;
using System.Drawing;

namespace QuadrilateralCorrectionEffect
{
    internal class QuadrilateralCorrectionConfigToken : EffectConfigToken
    {
        internal QuadrilateralCorrectionConfigToken()
        {
            TopLeft = new Point(0, 0);
            TopRight = new Point(int.MaxValue, 0);
            BottomRight = new Point(int.MaxValue, int.MaxValue);
            BottomLeft = new Point(0, int.MaxValue);
            AutoDims = true;
            Width = int.MaxValue;
            Height = int.MaxValue;
            Center = true;
        }

        private QuadrilateralCorrectionConfigToken(QuadrilateralCorrectionConfigToken copyMe)
        {
            TopLeft = copyMe.TopLeft;
            TopRight = copyMe.TopRight;
            BottomRight = copyMe.BottomRight;
            BottomLeft = copyMe.BottomLeft;
            AutoDims = copyMe.AutoDims;
            Width = copyMe.Width;
            Height = copyMe.Height;
            Center = copyMe.Center;
        }

        public override object Clone()
        {
            return new QuadrilateralCorrectionConfigToken(this);
        }

        internal Point TopLeft { get; set; }
        internal Point TopRight { get; set; }
        internal Point BottomRight { get; set; }
        internal Point BottomLeft { get; set; }
        internal bool AutoDims { get; set; }
        internal int Width { get; set; }
        internal int Height { get; set; }
        internal bool Center { get; set; }
    }
}