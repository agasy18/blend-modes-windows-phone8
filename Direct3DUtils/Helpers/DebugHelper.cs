using Microsoft.Phone.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Navigation;

namespace Direct3DUtils
{
    public static class DebugHelper
    {
        private static DateTime last = DateTime.Now;

        public static string LogSystemInfo(bool full = false)
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine();
            str.AppendLine("                           SystemInfo                             ");
            str.AppendLine("//////////////////////////////////////////////////////////////////");
            str.AppendLine("Time : " + DateTime.Now.ToString("hh:mm:ss.fff"));
            str.AppendLine("MemoryUsage : " + ((float)(Microsoft.Phone.Info.DeviceStatus.ApplicationCurrentMemoryUsage / 1204.0 / 1024.0)) + " MB");
            str.AppendLine("MemoryUsageLimit : " + ((float)(Microsoft.Phone.Info.DeviceStatus.ApplicationMemoryUsageLimit / 1204.0 / 1024.0)) + " MB");
            str.AppendLine("PeakMemoryUsage : " + ((float)(Microsoft.Phone.Info.DeviceStatus.ApplicationPeakMemoryUsage / 1204.0 / 1024.0)) + " MB");

            str.AppendLine(ObjectMonitor.Instance.ToString());

            if (full)
            {
                str.AppendLine("FirmwareVersion : " + Microsoft.Phone.Info.DeviceStatus.DeviceFirmwareVersion);
                str.AppendLine("HardwareVersion : " + Microsoft.Phone.Info.DeviceStatus.DeviceHardwareVersion);
                str.AppendLine("Manufacturer : " + Microsoft.Phone.Info.DeviceStatus.DeviceManufacturer);
                str.AppendLine("TotalMemory : " + ((float)(Microsoft.Phone.Info.DeviceStatus.DeviceTotalMemory/ 1204.0 / 1024.0)) + " MB");
                str.AppendLine("DeviceName : " + Microsoft.Phone.Info.DeviceStatus.DeviceName);
            }
            str.AppendLine("//////////////////////////////////////////////////////////////////");
            if(System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debug.WriteLine(str);
            return str.ToString();
        }
        public static string GetTimeDesc()
        {
            var timeNow=DateTime.Now;
            string te = timeNow.ToString("hh:mm:ss.fff") + "(" + (timeNow - last).ToString() + "): ";
            last = timeNow;
            return te;
        }
        public static void Log(params object[] parameters)
        {
#if DEBUG
            StringBuilder str = new StringBuilder();
            str.Append(GetTimeDesc());
            foreach (var param in parameters)
            {
                str.Append((param ?? "null").ToString() + ",");
            }
            System.Diagnostics.Debug.WriteLine(str.Remove(str.Length - 1, 1).ToString());
#endif
        }
        public static void Log(this Object obj, params object[] parameters)
        {
#if DEBUG
            StringBuilder str = new StringBuilder();
            str.Append(GetTimeDesc());
            foreach (var param in parameters)
            {
                str.Append((param ?? "null").ToString() + ",");
            }
            str.Remove(str.Length - 1, 1);
            str.Append(" = " + (obj ?? "null").ToString());
            System.Diagnostics.Debug.WriteLine(str.ToString());
#endif
        }

        public static T Log<T>(this T obj, params object[] parameters)
        {
#if DEBUG
            StringBuilder str = new StringBuilder();
            str.Append(GetTimeDesc());
            foreach (var param in parameters)
            {
                str.Append((param ?? "null").ToString() + ",");
            }
            str.Remove(str.Length - 1, 1);
            str.Append(" = " + (obj == null ? "null" : obj.ToString()));
            System.Diagnostics.Debug.WriteLine(str.ToString());
#endif
            return obj;
        }
        public static T Log<T>(this T obj)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(GetTimeDesc() +(obj == null ? "null" : obj.ToString()));
#endif
            return obj;
        }

        public static bool Assert(this bool t, string mes = "Condition is false")
        {
#if DEBUG
            if (!t)
            {
                ShowDebugInfo("Assert", new Exception(mes));
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
#endif
            return t;
        }

        public static T Assert<T>(this T t, string mes = "Object is null") where T : class
        {
#if DEBUG
            if (t == null)
            {
                ShowDebugInfo("Assert", new Exception(mes));
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
#endif
            return t;
        }
        static string bugKey = "bugKeywp8Hello";
        static int bugIndex=0;
        public static void ShowDebugInfo(string type, params object[] detales)
        {
#if DEBUG
            StringBuilder sB = new StringBuilder();
            foreach (var item in detales)
            {
                sB.AppendLine(DebugDesc(item));
            }
            sB.AppendLine(LogSystemInfo(true));
            string text = sB.ToString();
            if (System.Diagnostics.Debugger.IsAttached)
            {
                text.Log(type);
            }
            bool t = true;
            try
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        t = MessageBox.Show(text, type, MessageBoxButton.OKCancel) == MessageBoxResult.OK;
                    });
               
            }
            finally
            {
                if(t)
                {
                IsolatedStorageSettings.ApplicationSettings.Add(bugKey + bugIndex++, new KeyValuePair<string, string>(type, text));
                IsolatedStorageSettings.ApplicationSettings.Save();
                }
            }
#endif
        }
        public static void ChackForExeption()
        {
#if DEBUG
            KeyValuePair<string, string> kp;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(bugKey + 0, out kp))
            {
                StringBuilder strb = new StringBuilder();
                int i = 0;
                while (IsolatedStorageSettings.ApplicationSettings.TryGetValue(bugKey + i, out kp))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove(bugKey + i);
                    strb.AppendLine("N " + i);
                    strb.AppendLine("Type=" + kp.Key);
                    strb.AppendLine(kp.Value);
                    i++;
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        if (MessageBox.Show("", "Report last exception", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                        {
                            EmailComposeTask emailComposeTask = new EmailComposeTask();
                            emailComposeTask.Subject = DateTime.Now.ToString("hh:mm:ss.fff");
                            emailComposeTask.Body = strb.ToString();
                            emailComposeTask.To = "picswp8@outlook.com"; //Qw123456789
                            emailComposeTask.Show();
                        }

                    });
            }
#endif
        }
         
        public static string DebugDesc(this object obj)
        {
            if(obj==null)
                return "null";
            StringBuilder str = new StringBuilder();
            str.AppendLine("Type : " + obj.GetType().ToString());
            if (obj is NavigationFailedEventArgs)
            {
                var o = obj as NavigationFailedEventArgs;
                str.AppendLine("Handled = " + o.Handled);
                str.AppendLine(DebugDesc(o.Exception));
            }
            else
            if (obj is ApplicationUnhandledExceptionEventArgs)
            {
                var o = obj as ApplicationUnhandledExceptionEventArgs;
                str.AppendLine("Handled = " + o.Handled);
                str.AppendLine(DebugDesc(o.ExceptionObject));
            }
            else  if (obj is Exception)
            {
                var o = obj as Exception;
                str.AppendLine("Message = " + o.Message);
                str.AppendLine("InnerException =\n" + o.InnerException.DebugDesc());
                str.AppendLine("HelpLink = " + o.HelpLink);
                str.AppendLine("Data =\n" + o.Data.DebugDesc());
                str.AppendLine("Source = " + o.Source);
                str.AppendLine("Source = " + o.Source);
                str.AppendLine("StackTrace = " + o.StackTrace);                
            }
            else if (obj is IDictionary)
            {
                var o = obj as IDictionary;
                if (o.Count == 0)
                {
                    str.Append("{empty}");
                }
                else
                {
                    str.AppendLine("{\n");

                    var ok = o.Keys.GetEnumerator();
                    var ov = o.Values.GetEnumerator();
                    for (int i = 0; i < o.Count; i++)
                    {
                        str.AppendLine(ok.Current + " : " + ov.Current);
                        ok.MoveNext();
                        ov.MoveNext();
                    }
                    str.AppendLine("\n}");
                }
            }
            else if (obj is ICollection)
            {

                var o = obj as ICollection;
                if (o.Count == 0)
                {
                    str.Append("{empty}");
                }
                else
                {
                    str.Append("[\n");
                    foreach (var item in o)
                    {
                        str.Append(item);
                        str.AppendLine(",");
                    }
                    str.AppendLine("\n]");
                }
            }
            else
            {
                obj.ToString();
            }
            return str.ToString();
        }
    }
}
