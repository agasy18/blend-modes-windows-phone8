using Direct3DUtilsComp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Foundation;
using Windows.Storage;
using System.Windows.Controls;
using System.Windows.Media;

namespace Direct3DUtils
{
    
    public class DirectXMenager : IDisposable
    {
        public DirectXMenager()
        {
            Interop = new Direct3DInterop();
            Interop.AddToObjectMonitor("Interop");
            Interop.ConnectEvent += m_d3dInterop_ConnectEvent;
            Interop.DisconnectEvent += m_d3dInterop_DisconnectEvent;
        }

        TaskCompletionSource<object> interopIsLoadedTaskSource;
        TaskCompletionSource<object> InteropIsLoadedTaskSource
        {
            get { return interopIsLoadedTaskSource ?? (interopIsLoadedTaskSource = new TaskCompletionSource<object>()); }
            set { interopIsLoadedTaskSource = value; }
        }
        public Task InteropIsLoaded { get { return InteropIsLoadedTaskSource.Task; } }

        public bool EnableRestoring { get; set; }
        void m_d3dInterop_DisconnectEvent()
        {
            InteropIsLoadedTaskSource = null;
            this.Log("Disconnected");
        }

        private void m_d3dInterop_ConnectEvent()
        {
            InteropIsLoadedTaskSource.TrySetResult(null);
            if (EnableRestoring)
            {
                RestoreTextures();
            }
            this.Log("Connected");
        }


        internal List<Sprite> sprites = new List<Sprite>();

        bool isDisposed = false;

        public void Dispose()
        {
            if (isDisposed == false)
            {
                Interop.ConnectEvent -= m_d3dInterop_ConnectEvent;
                Interop.DisconnectEvent -= m_d3dInterop_DisconnectEvent;
                isDisposed = true;
                restoreActions.PerformAction(item => item());
                
                try
                {
                    DrawingSurfaceBackground.SetBackgroundContentProvider(null);
                    DrawingSurfaceBackground.SetBackgroundManipulationHandler(null);
                }
                catch (Exception e)
                {
                    e.Log("SetBackgroundContentProvider");
                }


                while (sprites.Any())
                {
                    sprites.First().Dispose();
                }
                Interop = null; 
            }
            
        }

        public Sprite CreateSprite(bool enableStore= true)
        {
            var t = new Sprite(this)
            {
                EnableImageStoring = enableStore
            };
            sprites.Add(t);
           return t;
        }
      

        public void DeletSprite(Sprite spr)
        {
            spr.Dispose();
        }

        ~DirectXMenager()
        {
            try
            {
                Dispose();
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Console.WriteLine("~DirectXMenager" + e.Message);
                }
            }
        }


        
        public Direct3DInterop Interop { get; private set; }

        public async void RestoreTextures()
        {
            TaskCompletionSource<object> ts = new TaskCompletionSource<object>();
            AddTask(ts.Task);
            try
            {
                if (EnableRestoring)
                {
                    foreach (var item in sprites)
                    {
                        await item.RestoreTextures();
                    }
                }
            }
            finally
            {
                ts.TrySetResult(null);
            }
        }

#region TaskManeger

        
        HashSet<Task> allTaskes = new HashSet<Task>();
        public Task AllTaskes { get { return Task.WhenAll(InteropIsLoaded, Task.WhenAll(allTaskes)); } }
        private async void AddTask(Task<object> task)
        {
          
            allTaskes.Add(task);
            if (TaskAdded != null)
            {
                TaskAdded(this, null);
            }
            try
            {
                await task;
            }
            finally
            {
                allTaskes.Remove(task);
                if (TaskRemoved!=null)
                {
                    TaskRemoved(this,null);
                }
            }
        }
        public event EventHandler TaskAdded;
        public event EventHandler TaskRemoved;
#endregion TaskManeger


        List<Action> restoreActions= new List<Action>();
        
        public void Load(System.Windows.Controls.DrawingSurfaceBackgroundGrid dSurface, FrameworkElement drawingLayer = null)
        {
            DrawingSurfaceBackground = dSurface;
            var content = Application.Current.Host.Content;
            Interop.WindowBounds = new Windows.Foundation.Size((float)content.ActualWidth, (float)content.ActualHeight);
            Interop.NativeResolution = new Windows.Foundation.Size((float)Math.Floor(content.ActualWidth * content.ScaleFactor / 100.0f + 0.5f),
                                                                        (float)Math.Floor(content.ActualHeight * content.ScaleFactor / 100.0f + 0.5f));
            Interop.RenderResolution = Interop.NativeResolution;
            var pr = Interop.CreateContentProvider();
            pr.AddToObjectMonitor("PROVIDER");
            DrawingSurfaceBackground.SetBackgroundContentProvider(pr);
            DrawingSurfaceBackground.SetBackgroundManipulationHandler(Interop);


            if (drawingLayer != null)
            {
                List<UIElement> parList = new List<UIElement>();
                foreach (var i in DrawingSurfaceBackground.SelectElements<UIElement>(drawingLayer, parList))
                {
                    if (i.Visibility == Visibility.Visible)
                    {
                        restoreActions.Add(() => i.Visibility = Visibility.Visible);
                        i.Visibility = Visibility.Collapsed;
                    }
                }
                foreach (var item in parList)
                {
                    if (item is Panel)
                    {
                        var bitem = item as Panel;
                        var brush = bitem.Background;
                        bitem.Background = null;
                        restoreActions.Add(() => bitem.Background = brush);
                    }
                }
            }
        }


        #region StorageFolder
        StorageFolder localFolder;
        public async Task<StorageFolder> GetRootStorage()
        {
            if (!EnableRestoring)
                return null;
            if (localFolder != null)
            {
                return localFolder;
            }
            else
            {
                try
                {
                    StorageFolder local = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(RootFolder);
                    this.Log("Deleteing last folder");
                    await local.DeleteAsync();
                }
                catch
                {
                    
                }
            }
            try
            {
                this.Log("CreateFolderAsync");
                localFolder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(RootFolder);
                return localFolder;
            }
            catch
            {
                localFolder = null;
            }
            if (localFolder == null)
            {
                this.Log("GetFolderAsync");
                return localFolder= await Windows.Storage.ApplicationData.Current.LocalFolder.GetFolderAsync(RootFolder);
            }
            throw new Exception("cannt open foder");
        }
        const string RootFolder = "textureTemp";
        public WriteableBitmap SaveToBitmap(int imageWidth, int imageHeight, int x, int y, int width, int height)
        {
            var bmp = new WriteableBitmap(imageWidth, imageHeight);
            Interop.SaveToBitmap(out bmp.Pixels[0], imageWidth, imageHeight, x, y, width, height);
            return bmp;
        }
        #endregion



        public DrawingSurfaceBackgroundGrid DrawingSurfaceBackground { get; set; }
    }
}
