using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direct3DUtils
{
    public class ObjectMonitor
    {
        public class DefaultCategories
        {
            public static string Page = "Page";
            public static string Bitmap = "Bitmap";
            public static string Stream = "Stream";
            public static string DirectX = "DirectX";
            public static string Other = "Other";
        }
        static ObjectMonitor _instance=new ObjectMonitor();
        public static ObjectMonitor Instance { get { return _instance; } }
        private Dictionary<string, ReferenceList<object>> objectCategories = new Dictionary<string, ReferenceList<object>>();
        private ObjectMonitor()
        {
        }

        private void AddCategory(string name)
        {
            if (objectCategories.ContainsKey(name)==false)
            {
                objectCategories.Add(name, new ReferenceList<object>(false));
            }
        }
        public void AddObject(object obj,object category = null)
        {
#if DEBUG
            if (obj==null)
            {
                return;
            }
            string categoryS= (category ?? DefaultCategories.Other).ToString();
            AddCategory(categoryS);
            objectCategories[categoryS].AddIfNotExist(obj);
#endif
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine("\t\tObjectMonitor info");
            b.AppendLine("//////////////////////////////////////////////////////");
            foreach (var item in objectCategories)
            {
                b.AppendLine("\t" + item.Key + "(" + item.Value.Count + ")");
            }
            b.AppendLine("//////////////////////////////////////////////////////");
            return b.ToString();
        }

        private void LogCategory(string name)
        {
#if DEBUG
            ReferenceList<object> list;
            if (objectCategories.TryGetValue(name,out list))
            {
                list.DebugDesc().Log(name);
            }
            else
            {
                "Not found".Log(name);
            }
#endif
        }

        public void Log()
        {
#if DEBUG
            ToString().Log();
#endif
        }

        public void DeepLog()
        {
#if DEBUG
            foreach (var item in objectCategories)
            {
                LogCategory(item.Key);
            }
#endif
        }

    }

    public static class ObjectMonitorExt
    {
        public static void AddToObjectMonitor(this object obj,object category = null)
        {
            ObjectMonitor.Instance.AddObject(obj,category);
        }
    }

}
