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
       

        void tk_Completed(object sender, Microsoft.Phone.Tasks.PhotoResult e)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(e.ChosenPhoto);
            s.SetMainTexture(bitmap);
        }
        
        void SaveToMediaLibrary(BitmapSource bmp)
        {
            WriteableBitmap Wbmp = (bmp as WriteableBitmap)??new WriteableBitmap(bmp);
            MemoryStream stream = new MemoryStream();
            Wbmp.SaveJpeg(stream, Wbmp.PixelWidth, Wbmp.PixelHeight, 0, 100);
            MediaLibrary library = new MediaLibrary();
            library.SavePicture("BlendModes_" + DateTime.Now.ToString(), stream.GetBuffer());
            MessageBox.Show("Picture saved in gallery");
        }

        WriteableBitmap GetBitmap()
        {
            var h = Application.Current.Host.Content.ActualHeight;
            var w = Application.Current.Host.Content.ActualWidth;
            float s = Application.Current.Host.Content.ScaleFactor / 100.0f;
            return dxManager.SaveToBitmap((int)(s * w), (int)(s * h), 0, 0,(int) w,(int) h);
        }


        Sprite CurrentSprite;

        private void ApplicationBarIconButton_Add(object sender, EventArgs e)
        {

        }

        private void ApplicationBarIconButton_Delete(object sender, EventArgs e)
        {

        }

        private void ApplicationBarIconButton_Save(object sender, EventArgs e)
        {

        }

        private void ApplicationBarMenuItem_Texture(object sender, EventArgs e)
        {
            switch ((sender as ApplicationBarMenuItem).Text)
	        {
                case "main texture":
                    break;
                case "blend texture":
                    break;
                case "mask texture":
                    break;
	        }
        }

        private void ApplicationBarMenuItem_BlendMode(object sender, EventArgs e)
        {
            
        }

        private void ApplicationBarMenuItem_Color(object sender, EventArgs e)
        {

        }
    }
}