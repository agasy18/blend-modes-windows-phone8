using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Direct3DUtils
{
    public class Reference<T>where T:class
    {
        public Reference()
        {
            Strong = true;
        }

        public Reference(T obj):this()
        {
            Object = obj;            
        }

        public Reference(T obj,bool strong):this(strong)
        {
            Object = obj;
        }
        public Reference(bool strong)
        {
            Strong = strong;
        }
        private WeakReference wObject = new WeakReference(null);
        private T sObject ;
        public bool IsAlive { get { return wObject.IsAlive; } }
        public T Object
        {
            get
            {
                return wObject.IsAlive ? wObject.Target as T : null;
            }
            set
            {
                wObject.Target = value;
                if (Strong)
                {
                    sObject = value;
                }
            }
        }
        private bool strong;
        public bool Strong
        {
            get { return strong; }
            set
            {
                if (strong != value)
                {


                    if (strong)
                    {                        
                        sObject = Object;
                    }
                    else
                    {
                        sObject = null;
                    }
                    strong = value;
                }
            }
        }
        public static explicit operator T(Reference<T> refo)
        {
            return refo == null ? null : refo.Object ;
        }
        public static explicit operator Reference<T>(T obj)
        {
            return new Reference<T>(obj,false);
        }
    }
}