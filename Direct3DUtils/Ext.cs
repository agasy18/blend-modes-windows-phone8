using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Direct3DUtils
{
    public static class Ext
    {
        public static T FirstAttribute<T>(this object element) where T : Attribute
        {
            System.Attribute[] attrs = System.Attribute.GetCustomAttributes(element.GetType(), typeof(T));

            if (attrs.Any())
            {
                return attrs.First() as T;
            }
            return null;
        }

        /// <summary>
        /// call action if not null
        /// </summary>
        /// <param name="action"></param>
        public static void SafeInvoke(this Action action)
        {
            if (action != null)
            {
                action();
            }
        }

        public static void PerformAction<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            if(action == null) return;
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in Descendents(child))
                    yield return descendent;
            }
        }



        public static void PerformActionToChilds(this UIElement elem, Action<UIElement> act)
        {
            act(elem);
            foreach (var v in Descendents(elem))
                if (v is UIElement)
                    act(v as UIElement);
        }

        public static TSource FirstItem<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate = null) where TSource : class
        {
            if (predicate == null)
            {
                try
                {
                    return source.First();
                }
                catch
                {
                    return null;
                }
            }
            foreach (var item in source)
            {
                if (predicate(item))
                    return item;

            }
            return null;
        }

        public static Dictionary<string, string> GetComponents(this Uri uri)
        {
            if (uri.Query.Length > 2)
            {
                string query = uri.Query.Substring(1, uri.Query.Length - 1);
                return query.GetComponents();
            }
            return null;
        }

        public static Dictionary<string, string> GetComponents(this string str)
        {
            string[] arr = str.Split('&');
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string item in arr)
            {
                string[] arr2 = item.Split('=');
                dict.Add(arr2[0], arr2[1]);
            }
            return dict;
        }

        public static bool IsRectEmpty(this Rect rect)
        {
            return rect.Width == 0 || rect.Height == 0 || double.IsInfinity(rect.Width) || double.IsInfinity(rect.Height) || double.IsNaN(rect.Width) || double.IsNaN(rect.Height);
        }

        public static bool IntersectsWith(this Rect r1, Rect r2)
        {
            return !(r1.Y + r1.Height < r2.Y || r1.Y > r2.Y + r2.Height || r1.X + r1.Width < r2.X || r1.X > r2.X + r2.Width);
        }


        public static Rect Scale(this Rect r1, double s)
        {
            return new Rect(r1.X - ((s - 1) * r1.Width / 2), r1.Y - ((s - 1) * r1.Height / 2), s * r1.Width, s * r1.Height);
        }


        /// <summary>
        /// Is r2 Contains in r1
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool Contains(this Rect r1, Rect r2)
        {
            return r2.X >= r1.X && r2.Y >= r1.Y && r2.X + r2.Width <= r1.X + r1.Width && r2.Y + r2.Height <= r1.Y + r1.Height;
        }

     
    }
}


