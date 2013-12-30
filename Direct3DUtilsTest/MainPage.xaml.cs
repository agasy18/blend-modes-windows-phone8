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
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;

namespace Direct3DUtilsTest
{
    public partial class MainPage : PhoneApplicationPage
    {
        DirectXMenager dxManager = new DirectXMenager() {EnableRestoring = true};
        
        public MainPage()
        {
            InitializeComponent();
            deleteButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            CurrentSprite = null;
            DialogGrid.SetVisibility(false, false);
        }

        int canvasZIndex = 0;
       
        
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

        Sprite currentSprite;
        Sprite CurrentSprite
        {
            set
            {
                currentSprite = value;
                var isE = currentSprite != null;
                ApplicationBar.IsMenuEnabled = isE;
                deleteButton.IsEnabled = isE;
            }
            get { return currentSprite; }
        }

        Task<WriteableBitmap> ChoosePhoto() 
        {
            TaskCompletionSource<WriteableBitmap> tSource = new TaskCompletionSource<WriteableBitmap>();
             var photoChooserTask = new PhotoChooserTask();
             photoChooserTask.Completed += (s, e) =>
             {
                 if (e.TaskResult == TaskResult.OK)
                 {
                     BitmapImage img = new BitmapImage();
                     img.SetSource(e.ChosenPhoto);
                     tSource.TrySetResult(new WriteableBitmap(img));
                 }
                 else
                 {
                     tSource.TrySetResult(null);
                 }
             };
             photoChooserTask.ShowCamera = true;
             photoChooserTask.Show();
            return tSource.Task;
        }

        async Task<WriteableBitmap> ChoosePhotoAndInteropIsLoaded()
        {
            var t = ChoosePhoto();
            await this.InvokeAsync();
            await dxManager.AllTaskes;
            return await t;
        }

        private async void ApplicationBarIconButton_Add(object sender, EventArgs e)
        {
           var img = await ChoosePhotoAndInteropIsLoaded();
           if (img!=null)
           {
               if (CurrentSprite!=null)
               {
                   CurrentSprite.Activate = false;
               }
               var sprite = dxManager.CreateSprite();
               sprite.Loaded+=(s,es)=>sprite.SetMainTexture(img);
               ApplyAspectRatio(sprite, img);
               Canvas.SetZIndex(sprite, ++canvasZIndex);
               LayoutRoot.Children.Add(sprite);
               sprite.Tap += CurrentSprite_Tap;
               CurrentSprite = sprite;
               CurrentSprite.Tag = BlendModeChooser.ResetParams.ResetParamsZiro();
           }
        }

        private void ApplyAspectRatio(Sprite CurrentSprite, WriteableBitmap img)
        {
            const float spriteSize = 400;
            float s = Math.Min(spriteSize / img.PixelWidth,spriteSize / img.PixelHeight);
            CurrentSprite.Width = s * img.PixelWidth;
            CurrentSprite.Height = s * img.PixelHeight;
        }

        void CurrentSprite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            DeselectSprite();
            CurrentSprite = sender as Sprite;
            CurrentSprite.Activate = true;
            Canvas.SetZIndex(CurrentSprite, ++canvasZIndex);
            CurrentSprite.BringToFront();
        }

        private void ApplicationBarIconButton_Delete(object sender, EventArgs e)
        {
            HideBlendMenu();
            dxManager.DeletSprite(CurrentSprite);
            LayoutRoot.Children.Remove(CurrentSprite);
            DeselectSprite();
        }

        private void HideBlendMenu()
        {
            if (DialogGrid.Visibility == System.Windows.Visibility.Visible)
                DialogGrid.SetVisibility(false, true);
        }
        private void ShowBlendMenu(BlendModeChooser.ResetParams param)
        {
            if (DialogGrid.Visibility == System.Windows.Visibility.Collapsed)
            {
                DialogGrid.SetVisibility(true, true);
                blendChooser.CurrentParrams = param;
            }
        }

        bool IsSpriteSelected { get { return CurrentSprite != null; } }

        private void ApplicationBarIconButton_Save(object sender, EventArgs e)
        {
            SaveToMediaLibrary(GetBitmap());
        }

        private void ApplicationBarMenuItem_Texture(object sender, EventArgs e)
        {
            if (!IsSpriteSelected)
            {
                return;
            }

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
            if (IsSpriteSelected)
            {
                ShowBlendMenu((BlendModeChooser.ResetParams)CurrentSprite.Tag);
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            if (DialogGrid.Visibility == System.Windows.Visibility.Visible)
            {
                e.Cancel = true;
                HideBlendMenu();
            }
        }

        private void LayoutRoot_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (e.Handled)
            {
                return;
            }
            e.Handled = true;
            DeselectSprite();
        }

        private void DeselectSprite()
        {
            if (IsSpriteSelected)
            {
                CurrentSprite.Activate = false;
                CurrentSprite = null;
            }
        }

        private void DrawingSurfaceBackground_Loaded_1(object sender, RoutedEventArgs e)
        {
            dxManager.Load(DrawingSurfaceBackground);
        }

        private void blendChooser_Selected(object sender, BlendMode e)
        {
            if (IsSpriteSelected)
            {
                CurrentSprite.Tag = blendChooser.CurrentParrams;
                CurrentSprite.Blendmode = e;
            }
        }

        private void blendChooser_SelectedColor(object sender, Color e)
        {
            if (IsSpriteSelected)
            {
                CurrentSprite.Tag = blendChooser.CurrentParrams;
                CurrentSprite.FillColor = e;
            }
        }

        private void blendChooser_SelectedFillMode(object sender, FillMode e)
        {
            if (IsSpriteSelected)
            {
                CurrentSprite.Tag = blendChooser.CurrentParrams;
                CurrentSprite.FillMode = e;
            }
        }

        private void blendChooser_SelectedSpriteAlpha(object sender, double e)
        {
            if (IsSpriteSelected)
            {
                CurrentSprite.Tag = blendChooser.CurrentParrams;
                CurrentSprite.Alpha = (float)e / 255;
            }
        }

    }
}