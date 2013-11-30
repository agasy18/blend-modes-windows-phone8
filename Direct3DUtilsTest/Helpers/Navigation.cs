using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Direct3DUtils
{

    public static class Navigation
    {
        public static bool Navigate(this DependencyObject from, object to)
        {
            var frame = from;
            while (!(frame is PhoneApplicationFrame))
                try { frame = VisualTreeHelper.GetParent(frame); }
                catch { return false; }
            var p = frame as PhoneApplicationFrame;
            var back = p.Content;
            p.Content = to;
            EventHandler<System.ComponentModel.CancelEventArgs> f = null;
            f = new EventHandler<System.ComponentModel.CancelEventArgs>((s, e) =>
            {
                if (!e.Cancel)
                {
                    e.Cancel = true;
                    p.Content = back;
                    p.BackKeyPress -= f;
                }
            });
            p.BackKeyPress += f;
            return true;
        }

        public static bool Navigate<T>(this DependencyObject from, Action<Frame, T> setParamsBlock = null) where T : class
        {
            string url = typeof(T).ToString().Replace('.', '/').Replace("PicsArt", "") + ".xaml";
            return Navigate<T>(from, url, setParamsBlock);
        }
        public static bool Navigate<T>(this DependencyObject from, string to, Action<Frame, T> setParamsBlock = null) where T : class
        {
            return Navigate<T>(from, new Uri(to, UriKind.Relative), setParamsBlock);
        }
        public static bool Navigate<T>(this DependencyObject from, Uri to, Action<Frame, T> setParamsBlock = null) where T : class
        {
            var frame = from;
            if (!(frame is Frame))
            {
                frame = Application.Current.RootVisual;
            }
            var p = frame as Frame;
            if (setParamsBlock != null)
            {
                NavigatedEventHandler handle = null;
                handle = new NavigatedEventHandler((s, e) =>
                {
                    if (handle == null) return;
                    if (e.Uri == to && e.NavigationMode == NavigationMode.New)
                    {
                        (!(handle == null || !(e.Content is T))).Assert("Navigate<"+typeof(T)+"> e.Content is "+e.Content);
                        setParamsBlock(p, e.Content as T);
                        p.Navigated -= handle;
                        handle = null;
                    }
                });
                p.Navigated += handle;
            }
            return p.Navigate(to);
        }
        public static bool Navigate<T>(this NavigationService from, Action<NavigationService, T> setParamsBlock = null) where T : class
        {
            string url = typeof(T).ToString().Replace('.', '/').Replace("PicsArt", "") + ".xaml";
            return Navigate<T>(from, url, setParamsBlock);
        }
        public static bool Navigate<T>(this NavigationService from, string to, Action<NavigationService, T> setParamsBlock = null) where T : class
        {
            return Navigate<T>(from, new Uri(to, UriKind.Relative), setParamsBlock);
        }
        public static bool Navigate<T>(this NavigationService from, Uri to, Action<NavigationService, T> setParamsBlock = null) where T : class
        {
            var p = from;
            if (setParamsBlock != null)
            {
                NavigatedEventHandler handle = null;
                handle = new NavigatedEventHandler((s, e) =>
                {
                    if (e.Uri == to && e.NavigationMode == NavigationMode.New)
                    {                        
                        setParamsBlock(p, e.Content as T);
                        p.Navigated -= handle;
                    }
                });
                p.Navigated += handle;
            }
            return p.Navigate(to);
        }


        public class ContentX<T> where T : class
        {
            private System.Reflection.PropertyInfo prop;
            private object obj;
            public ContentX(object obj)
            {
                this.obj = obj;
                foreach (var item in obj.GetType().GetCustomAttributes(true))
                {
                    var v = item as System.Windows.Markup.ContentPropertyAttribute;
                    if (v != null)
                    {
                        prop = obj.GetType().GetProperty(v.Name);
                        break;
                    }
                }
            }
            public T Content
            {
                get { return (T)prop.GetValue(obj); }
                set { prop.SetValue(obj, value); }
            }
        }

    }
}
