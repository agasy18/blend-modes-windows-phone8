using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using Direct3DUtilsComp;
using System.Windows.Resources;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace Direct3DUtils
{

    public enum SpriteTextureType
    {
        Main,
        Blend,
        Mask
    }

    public enum BlendMode
    {
        Normal = 1,
        Multiply = 2,
        Screen = 3,
        Overlay = 4,
        Darken = 5,
        Lighten = 6,
        ColorDodge = 7,
        ColorBurn = 8,
        SoftLight = 9,
        HardLight = 10,
        Difference = 11,
        Exclusion = 12,
        Clear = 17,
        Copy = 18,
        SourceIn = 19,
        SourceOut = 20,
        SourceAtop = 21,
        DestinationOver = 22,
        DestinationIn = 23,
        DestinationOut = 24,
        DestinationAtop = 25,
        XOR = 26,
        PlusDarker = 27,
        PlusLighter = 28,

    };

    public enum FillMode
    {
        None = -1,
        Fill = 0,
        Background = 1

    }

    public partial class Sprite : UserControl, IDisposable
    {
        public CompositeTransform CT { get { return RenderTransform as CompositeTransform; } }

        Rectangle re = new Rectangle() { Fill = new SolidColorBrush(Colors.Transparent) };
        float alpha;
        public float Alpha
        {
            set { m_d3dInterop.SetAlpha(id, alpha = value); }
            get { return alpha; }
        }
        bool hvm = false;
        public bool HorizAndVerticalMove
        {
            get
            {
                return hvm;
            }
            set
            {
                hvm = value;
            }
        }
        int maxZC = 5;
        public int MaxZoomCoefficient { get { return maxZC; } set { maxZC = value; } }

        public bool Activate { set { Content = value ? (UIElement)gr : (UIElement)re; } }
        public bool ShowRectangles
        {
            set
            {
                if (value == false)
                {
                    rectLeftTop.Visibility = Visibility.Collapsed;
                    rectRightTop.Visibility = Visibility.Collapsed;
                    rectLeftBottom.Visibility = Visibility.Collapsed;
                    rectRightBottom.Visibility = Visibility.Collapsed;
                    border.BorderThickness = new Thickness(0);
                }
                else
                {
                    rectLeftTop.Visibility = Visibility.Visible;
                    rectRightTop.Visibility = Visibility.Visible;
                    rectLeftBottom.Visibility = Visibility.Visible;
                    rectRightBottom.Visibility = Visibility.Visible;
                    border.BorderThickness = new Thickness(1);
                }
            }
        }
        internal Sprite(DirectXMenager manager)
        {
            DXMenager = manager;
            id = m_d3dInterop.SpriteCreate();
            Blendmode = BlendMode.Normal;
            //Tigran
            Fillmode = FillMode.None;
            
            //
            InitializeComponent();
            gr.ManipulationDelta += Sprite_ManipulationDelta;
            gr.ManipulationStarted += (s, e) => { isnewmanipul = true; };
            SizeChanged += (s, e) => { Dispatcher.BeginInvoke(SetTransform); };
            //LayoutUpdated += (s, e) => {SetTransform(); };
        }
        bool isnewmanipul = true;
        double r;
        double tx;
        double ty;
        void Sprite_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            GeneralTransform gt = TransformToVisual(e.ManipulationContainer).Inverse;
            if (e.PinchManipulation != null)
            {
                if (isnewmanipul)
                {
                    tx = CT.TranslateX;
                    ty = CT.TranslateY;
                    r = CT.Rotation;
                    isnewmanipul = false;
                }
                Point originalCenter = RenderTransform.Transform(gt.Transform(e.PinchManipulation.Original.Center));
                Point newCenter = RenderTransform.Transform(gt.Transform(e.PinchManipulation.Current.Center));

                CT.TranslateX = tx + newCenter.X - originalCenter.X;
                CT.TranslateY = ty + newCenter.Y - originalCenter.Y;
                CT.Rotation = r + angleBetween2Lines(
                        gt.Transform(e.PinchManipulation.Current.PrimaryContact),
                        gt.Transform(e.PinchManipulation.Current.SecondaryContact),
                        gt.Transform(e.PinchManipulation.Original.PrimaryContact),
                        gt.Transform(e.PinchManipulation.Original.SecondaryContact)) * (CT.ScaleX * CT.ScaleY > 0 ? 1 : -1);

                //CT.ScaleX=CT.ScaleY*= e.PinchManipulation.DeltaScale;
                double k = e.PinchManipulation.DeltaScale;
                if (firstManipuletion)
                {
                    firstWidth = Width;
                    firstHeight = Height;
                    firstManipuletion = false;
                }

                Width = Math.Min(Width * k, firstWidth * maxZC);
                Height = Math.Min(Height * k, firstHeight * maxZC);
            }
            else
            {
                Point start = RenderTransform.Transform(gt.Transform(new Point(0, 0)));
                Point end = RenderTransform.Transform(gt.Transform(e.DeltaManipulation.Translation));
                CT.TranslateX += end.X - start.X;
                CT.TranslateY += end.Y - start.Y;
                isnewmanipul = true;
            }
            e.Handled = true;

            //SetTransform();
            Dispatcher.BeginInvoke(SetTransform);
        }
        double angleBetween2Lines(Point PrimaryContact1, Point SecondaryContact1, Point PrimaryContact2, Point SecondaryContact2)
        {
            double angle1 = Math.Atan2(PrimaryContact1.Y - SecondaryContact1.Y, PrimaryContact1.X - SecondaryContact1.X);
            double angle2 = Math.Atan2(PrimaryContact2.Y - SecondaryContact2.Y, PrimaryContact2.X - SecondaryContact2.X);
            return (angle1 - angle2) * 180 / Math.PI;
        }
        public void SetTransform()
        {
            var gt = TransformToVisual(Application.Current.RootVisual);
            var p = gt.Transform(new Point(ActualWidth / 2, ActualHeight / 2));
            //var px = gt.Transform(new Point(ActualWidth, ActualHeight / 2));getlenqht(px, p)
            //var py = gt.Transform(new Point(ActualWidth / 2, ActualHeight));getlenqht(py, p)
            m_d3dInterop.SpriteTranslate(id, (float)p.X, (float)p.Y, Canvas.GetZIndex(this), (float)CT.Rotation, (float)(CT.ScaleX * Width), (float)(CT.ScaleY * Height));
        }
        double getlenqht(Point p1, Point p2)
        {
            double x = p1.X - p2.X;
            double y = p1.Y - p2.Y;
            return Math.Sqrt(x * x + y * y) * 2;
        }

        double firstWidth;
        double firstHeight;
        bool firstManipuletion = true;

        private void ElMAnipDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var p = e.DeltaManipulation.Translation;
            if (fe.HorizontalAlignment == System.Windows.HorizontalAlignment.Left)
                p.X *= -1;
            if (fe.VerticalAlignment == System.Windows.VerticalAlignment.Top)
                p.Y *= -1;

            double w;
            double h;
            if (hvm)
            {
                w = Width + p.X * 2;
                h = Height + p.Y * 2;
            }
            else
            {
                w = Width + p.X + p.Y;
                h = Height * w / Width;
            }

            if (w < 0)
            {
                CT.ScaleX *= -1;
                w *= -1;
            }
            if (h < 0)
            {
                CT.ScaleY *= -1;
                h *= -1;
            }
            if (firstManipuletion)
            {
                firstWidth = Width;
                firstHeight = Height;
                firstManipuletion = false;
            }

            Width = System.Math.Min(w, firstWidth * maxZC);
            Height = System.Math.Min(h, firstHeight * maxZC);


            e.Handled = true;
        }


        private BlendMode blendmode;
        public BlendMode Blendmode
        {
            get { return blendmode; }
            set
            {
                blendmode = value;
                m_d3dInterop.SprieSetBlendMode(id, (int)blendmode);
            }
        }
        private FillMode fillmode;
        public FillMode Fillmode
        {
            get { return fillmode; }
            set
            {
                fillmode = value;
                m_d3dInterop.SpriteSetFillMode(id, (int)fillmode);
            }
        }
        private Color colorFill;
        public Color ColorFill
        {
            get { return colorFill; }
            set
            {
                colorFill = value;

                m_d3dInterop.SetFillColor(id,(float)colorFill.R / 255, (float)colorFill.G / 255, (float)colorFill.B / 255, (float)colorFill.A / 255);
            }
        }

        public void BringToFront()
        {
            m_d3dInterop.BringToFront(id);
            var p = Parent as Panel;
            if (p != null)
            {
                p.Children.Remove(this);
                p.Children.Add(this);
            }
        }

        public int id { get; private set; }
        Direct3DInterop m_d3dInterop
        {
            get
            {
                try
                {
                    var t = DXMenager.Interop;
                    if (t == null)
                    {
                        throw new Exception("m_d3dInterop is Invalide");
                    }
                    return t;
                }
                catch (Exception ex)
                {
                    throw new Exception("m_d3dInterop is Invalide", ex);
                }
            }
        }

        #region Texture
        public void SetMainTexture(BitmapSource bitmapsource)
        {
            WriteableBitmap bmp = bitmapsource as WriteableBitmap ?? new WriteableBitmap(bitmapsource);
            SetMainTexture(bmp);
        }

        public void SetMainTexture(WriteableBitmap bmp)
        {
            SetImage(bmp, SpriteTextureType.Main);
        }

        public void SetBlendTexture(WriteableBitmap bmp)
        {
            SetImage(bmp, SpriteTextureType.Blend);
        }
        public void SetMaskTexture(WriteableBitmap bmp)
        {
            SetImage(bmp, SpriteTextureType.Mask);
        }

        public void SetImage(WriteableBitmap bmp, SpriteTextureType type)
        {
            ObjectMonitorExt.AddToObjectMonitor(bmp, ObjectMonitor.DefaultCategories.Bitmap);
            if (DXMenager.EnableRestoring)
            {
                StoreImage(bmp, type);
            }
            SetTexture(bmp, type);
        }

        private void SetTexture(WriteableBitmap bmp, SpriteTextureType type)
        {
            switch (type)
            {

                case SpriteTextureType.Main:
                    m_d3dInterop.SpriteCreateMainTexture(id, out bmp.Pixels[0], bmp.PixelWidth, bmp.PixelHeight);
                    break;
                case SpriteTextureType.Blend:
                    m_d3dInterop.SpriteCreateBlendTexture(id, out bmp.Pixels[0], bmp.PixelWidth, bmp.PixelHeight);
                    break;
                case SpriteTextureType.Mask:
                    m_d3dInterop.SpriteCreateMaskTexture(id, out bmp.Pixels[0], bmp.PixelWidth, bmp.PixelHeight);
                    break;
                default:
                    break;
            }
        }

        public void ClearTexture(SpriteTextureType type)
        {
            ClearStorage(type);
            int[] bmpPix = null;
            switch (type)
            {

                case SpriteTextureType.Main:
                    m_d3dInterop.SpriteCreateMainTexture(id, out bmpPix[0], 0, 0);
                    break;
                case SpriteTextureType.Blend:
                    m_d3dInterop.SpriteCreateBlendTexture(id, out bmpPix[0], 0, 0);
                    break;
                case SpriteTextureType.Mask:
                    m_d3dInterop.SpriteCreateMaskTexture(id, out bmpPix[0], 0, 0);
                    break;
                default:
                    break;
            }

        }


        #endregion

        public DirectXMenager DXMenager { private set; get; }

        public void Dispose()
        {
            if (EnableImageStoring && DXMenager.EnableRestoring)
            {
                ClearCurrentStorage();
            }
            m_d3dInterop.SpriteDelete(id);
            DXMenager.sprites.Remove(this);
            DXMenager = null;
        }




    }


}
