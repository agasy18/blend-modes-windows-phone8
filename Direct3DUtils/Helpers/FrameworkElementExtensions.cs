using Microsoft.Phone.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Direct3DUtils
{
    public static class FrameworkElementExtensions
    {

        public static string RedundantTextByWidth(this TextBlock tBlock, double TextBlockWidth)
        {
            int firstReplaceableCharCount = 1;
            int oterTimesReplaceableCharsCount = 4;
            string replacingString = "...";

            bool firstTitme = true;


            while (tBlock.ActualWidth > TextBlockWidth)
            {
                if (firstTitme)
                {
                    tBlock.Text = string.Format(tBlock.Text.Remove(tBlock.Text.Length - firstReplaceableCharCount) + replacingString);
                    firstTitme = false;
                }
                else
                    tBlock.Text = string.Format(tBlock.Text.Remove(tBlock.Text.Length - oterTimesReplaceableCharsCount) + replacingString);
            }

            return tBlock.Text;
        }

        public static string RedundantTextByHeight(this TextBlock tBlock, double TextBlockHeight)
        {
            int firstReplaceableCharCount = 1;
            int oterTimesReplaceableCharsCount = 4;
            string replacingString = "...";

            bool firstTitme = true;


            while (tBlock.ActualHeight > TextBlockHeight)
            {
                if (firstTitme)
                {
                    tBlock.Text = string.Format(tBlock.Text.Remove(tBlock.Text.Length - firstReplaceableCharCount) + replacingString);
                    firstTitme = false;
                }
                else
                    tBlock.Text = string.Format(tBlock.Text.Remove(tBlock.Text.Length - oterTimesReplaceableCharsCount) + replacingString);
            }

            return tBlock.Text;
        }

        public static void RemoveFromParent(this FrameworkElement elem)
        {
            if (elem.Parent == null)
            {
                return;
            }
            try
            {
                ContentX contentProv = new ContentX(elem.Parent);
                var content = contentProv.Content;
                if (content is UIElement)
                {
                    contentProv.Content = null;
                }
                if (content is UIElementCollection)
                {
                    ((UIElementCollection)content).Remove(elem);
                }
                else
                {
                    throw new Exception("unknown parent type");
                }
            }
            catch (Exception exep)
            {
                throw new Exception("can't solve RemoveFromParent", exep);
            }
        }


        public static Task InvokeAsync(this Dispatcher disp,Action act=null)
        {
            var res =new TaskCompletionSource<object>();
            try
            {
                disp.BeginInvoke(() =>
                {
                    act.SafeInvoke();
                    res.TrySetResult(null);
                });
            }
            catch (Exception e)
            {
                res.TrySetException(e);
            }
            return res.Task;
        }

        public static Task InvokeAsync(this DependencyObject obj,Action act = null)
        {
            return obj.Dispatcher.InvokeAsync(act);
        }

        public static Task AddChild(this Panel panel,FrameworkElement elem,int index=-1)
        { 
           var  t =new  TaskCompletionSource<object>();
           RoutedEventHandler lAct = null;
           try
           {
               lAct = (s, e) =>
                  {
                      panel.Loaded -= lAct;
                      t.TrySetResult(null);
                  };
               elem.Loaded += lAct;
               if (index == -1 )
               {
                   panel.Children.Add(elem);
               }
               else
               {
                   panel.Children.Insert(index, elem);
               }
           }
           catch (Exception e)
           {
               t.TrySetException(e);
           }
           return t.Task;
        }

        public static Task SetVisibility(this FrameworkElement element, bool visibility, bool animated)
        {
            var taskSource = new TaskCompletionSource<object>();
            try
            {
                if (animated)
                {
                    Storyboard sb = new Storyboard();
                    DoubleAnimation da = new DoubleAnimation();
                    sb.Children.Add(da);
                    Storyboard.SetTarget(da, element);
                    Storyboard.SetTargetProperty(da, new PropertyPath(UIElement.OpacityProperty));
                    if (visibility)
                    {
                        da.From = 0;
                        da.To = 1;
                    }
                    else
                    {
                        da.From = 1;
                        da.To = 0;
                    }
                    da.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                    sb.Completed += async (s, e) =>
                    {
                        try
                        {
                            await element.InvokeAsync();
                            taskSource.SetResult(null);
                        }
                        catch (Exception exc)
                        {
                            taskSource.SetException(exc);
                        }
                    };
                    sb.Begin();
                }
                else
                {
                    element.Opacity = visibility ? 1 : 0;
                    Func<Task> act = async () =>
                    {
                        try
                        {
                            await element.InvokeAsync();
                            taskSource.SetResult(null);
                        }
                        catch (Exception exc)
                        {
                            taskSource.SetException(exc);
                        }
                    };
                    return act();
                }
            }
            catch(Exception e)
            {
                taskSource.SetException(e);
            }
            return taskSource.Task;
        }


        public static IEnumerable<T> GetChildren<T>(this FrameworkElement elem) where T : DependencyObject
        {
            ContentX content = new ContentX(elem);
            if (content.Content != null)
            {
                var enumer = content.Content as IEnumerable;
                if (enumer != null)
                {
                    foreach (var item in enumer)
                    {
                        if (item is T)
                        {
                            yield return item as T;
                        }
                    }
                }
                else
                {
                    if (content.Content is T)
                    {
                        yield return content.Content as T;
                    }
                }
            }
        }

        public static IEnumerable<T> SelectElements<T>(this FrameworkElement from, FrameworkElement to,List<T> outList=null) where T : DependencyObject
        {            
            var par = to.Parent as FrameworkElement;
            var fpar = from.Parent as FrameworkElement;
            if (par != null)
            {
                if (fpar != par)
                {
                    if (outList!=null)
                    {
                        var p= to.Parent as T;
                        if (p != null)
                        {
                            outList.Add(p);
                        }
                    }
                    foreach (var item in par.GetChildren<T>())
                    {
                        if (item == to)
                        {
                            break;
                        }
                        yield return item;
                    }
                    foreach (var item in SelectElements<T>(from, par,outList))
                    {
                        yield return item;
                    }
                }
                else
                {
                    bool started = false;
                    foreach (var item in par.GetChildren<T>())
                    {

                        if (started)
                        {
                            if (item == to)
                            {
                                break;
                            }
                            yield return item;
                        }
                        else
                        {
                            if (item == from)
                            {
                                started = true;
                            }
                            continue;
                        }
                    }
                }
            }

        }
    }
    public class ContentX
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
        public object Content
        {
            get { return (object)prop.GetValue(obj); }
            set { prop.SetValue(obj, value); }
        }

    }
}
