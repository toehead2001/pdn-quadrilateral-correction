using PaintDotNet.Effects;
using System.Drawing;

namespace QuadrilateralCorrectionEffect
{
    internal class QuadrilateralCorrectionConfigToken : EffectConfigToken
    {
        Point topLeft = new Point(0, 0);
        Point topRight = new Point(int.MaxValue, 0);
        Point bottomRight = new Point(int.MaxValue, int.MaxValue);
        Point bottomLeft = new Point(0, int.MaxValue);

        bool autoDims = true;
        int width = int.MaxValue;
        int height = int.MaxValue;
        //float scale;
        bool center = true;


        internal QuadrilateralCorrectionConfigToken() : base()
        {
            TopLeft = topLeft;
            TopRight = topRight;
            BottomRight = bottomRight;
            BottomLeft = bottomLeft;
            AutoDims = autoDims;
            Width = width;
            Height = height;
            //Scale = scale;
            Center = center;
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
            //Scale = copyMe.Scale;
            Center = copyMe.Center;
        }

        public override object Clone()
        {
            return new QuadrilateralCorrectionConfigToken(this);
        }

        internal Point TopLeft
        {
            get;
            set;
        }
        internal Point TopRight
        {
            get;
            set;
        }
        internal Point BottomRight
        {
            get;
            set;
        }
        internal Point BottomLeft
        {
            get;
            set;
        }
        internal bool AutoDims
        {
            get;
            set;
        }
        internal int Width
        {
            get;
            set;
        }
        internal int Height
        {
            get;
            set;
        }
        //public float Scale
        //{
        //    get;
        //    set;
        //}
        internal bool Center
        {
            get;
            set;
        }
    }
}