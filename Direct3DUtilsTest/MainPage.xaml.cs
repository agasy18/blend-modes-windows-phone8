using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;
using System.Threading;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media;
using Direct3DUtils;

namespace Direct3DUtilsTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        DirectXMenager dxManager = new DirectXMenager();
        
        public MainPage()
        {
            InitializeComponent();
            ManipulationDelta += MainPage_ManipulationDelta;
            StreamResourceInfo resourceInfo = Application.GetResourceStream(new Uri("Assets/tux.png", UriKind.Relative));
            bitmap = new BitmapImage();

            bitmap.SetSource(resourceInfo.Stream);
            dxManager.Interop.ConnectEvent += Interop_ConnectEvent;
        }

        void Interop_ConnectEvent()
        {
            if (s != null)
            {
                setMainTex(null, null);
            }
        }

        void MainPage_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
        }
        private void DrawingSurfaceBackground_Loaded(object sender, RoutedEventArgs e)
        {
            dxManager.Load(DrawingSurfaceBackground);
            CreateSprite_act(null, null);

            setMainTex(null, null);
            s.Width = 1000;
            s.Height = 1000;
            CreateSprite_act(null, null);
            setMainTex(null, null);
            s.Width = 500;
            s.Height = 500;
            s.Alpha = 0.5f;
        }

        Sprite s;
        private void CreateSprite_act(object sender, RoutedEventArgs e)
        {
            if (s != null)
                s.Activate = false;
            s = dxManager.CreateSprite(false);
            Canvas.SetZIndex(s, zindex);
            DrawingSurfaceBackground.Children.Add(s);
            s.Tap += s_Tap;
        }
        int zindex = 0;
        void s_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            s.Activate = false;
            s = sender as Sprite;
            s.Activate = true;
            Canvas.SetZIndex(s, ++zindex);
            s.SetTransform();
        }
        int i = 0;
        private void Move_act(object sender, RoutedEventArgs e)
        {
            s.Blendmode = BlendMode.Screen;
            s.Fillmode = FillMode.Fill;
            s.ColorFill = Color.FromArgb(255, 10, 5, 255);



        }
        private void blend_res_act(object sender, RoutedEventArgs e)
        {

        }
        private void setMainTex(object sender, RoutedEventArgs e)
        {

            s.SetMainTexture(bitmap);

            //var v = new WriteableBitmap(sender as UIElement, null);
            //s.setMainTex(v);


            //Microsoft.Phone.Tasks.PhotoChooserTask tk = new Microsoft.Phone.Tasks.PhotoChooserTask();
            //tk.ShowCamera = true;
            //tk.Completed += tk_Completed;
            //tk.Show();
        }
        void tk_Completed(object sender, Microsoft.Phone.Tasks.PhotoResult e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(e.ChosenPhoto);
            s.SetMainTexture(bitmap);
        }
        public void Save(BitmapSource bmp)
        {
            // stream = new MemoryStream(Wbmp.PixelWidth * Wbmp.PixelHeight * 4);
            WriteableBitmap Wbmp = bmp as WriteableBitmap;
            if (Wbmp == null)
            {
                Wbmp = new WriteableBitmap(bmp);
            }
            MemoryStream stream = new MemoryStream(Wbmp.PixelWidth * Wbmp.PixelHeight);
            Wbmp.SaveJpeg(stream, Wbmp.PixelWidth, Wbmp.PixelHeight, 0, 100);
            MediaLibrary library = new MediaLibrary();
            library.SavePicture("PicsArt_" + DateTime.Now.ToString(), stream.GetBuffer());
            MessageBox.Show("picture saved in gallery");
        }

        public BitmapImage bitmap { get; set; }



        private void saveAction(object sender, RoutedEventArgs e)
        {
            float s = Application.Current.Host.Content.ScaleFactor / 100.0f;
            sImage.Source = dxManager.SaveToBitmap(500, 500, 0, 100, (int)(500 / s), (int)(500 / s));
        }
    }
}