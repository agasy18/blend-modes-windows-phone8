using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Direct3DUtils
{
    public class ReferenceList<T>:IList<T>, ICollection<T>,  IReadOnlyCollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable where  T:class
    {
        public bool DefaultStrong { get; set; }

        List<Reference<T>> __listRefrerence ;
        public List<Reference<T>> ListRefrerence { get { DeleteDeadObjects(); return __listRefrerence; } }

        public ReferenceList(bool isStrong = false)
        {
            DefaultStrong = isStrong;
            __listRefrerence = new List<Reference<T>>();
        }
        public ReferenceList(IEnumerable<T> collection, bool isStrong = false)
        {
            DefaultStrong=isStrong;
            var coll = (from x in collection select new Reference<T>(x));
            __listRefrerence = new List<Reference<T>>(coll);
            
        }
        public ReferenceList(int capacity,bool isStrong = false)
        {
            DefaultStrong=isStrong;
            __listRefrerence = new List<Reference<T>>(capacity);
        }
        


        bool IsReadOnly { get { return false; } }



        public int Capacity { get { return ListRefrerence.Capacity; } set { ListRefrerence.Capacity = value; } }


        public int Count
        {
            get
            {
                return ListRefrerence.Count;
            }
        }

        public void DeleteDeadObjects()
        { 
            __listRefrerence.RemoveAll(reference => reference.IsAlive == false); 
        }



        public T this[int index] 
        {
            get
            {
                return ListRefrerence[index].Object;
            }
            set
            {
                ListRefrerence[index].Object = value;
            }
        }



        public void Add(T item)
        {
            __listRefrerence.Add(new Reference<T>(item,DefaultStrong));
        }


        public void AddRange(IEnumerable<T> collection)
        {
            ListRefrerence.AddRange((from x in collection select new Reference<T>(x)));
        }


        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }


        public void Clear()
        {
            __listRefrerence.Clear();
        }


        public bool Contains(T item)
        {
            return ListRefrerence.Any(i => i.Object == item);
        }


        public void CopyTo(T[] array)
        {
            int i=0;
            foreach (var item in this)
            {
                array[i] = item;
                i++;
            }
        }


        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < arrayIndex; i++)
            {
                if (i == this.Count - 1) return;
                array[i] = this[i];
            }
        }
 

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
 
        }


        public bool Exists(Predicate<T> match)
        {
            return ListRefrerence.Exists((o)=>match(o.Object));
        }


        public T Find(Predicate<T> match)        
        {
            return ListRefrerence.Find((o) => match(o.Object)).Object;
        }


        public List<T> FindAll(Predicate<T> match)
        {
            return (from x in ListRefrerence where match(x.Object) select x.Object).ToList(); ;
        }


        public int FindIndex(Predicate<T> match)
        {
            return ListRefrerence.FindIndex(o => match(o.Object));
        }


        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return ListRefrerence.FindIndex(startIndex, o => match(o.Object));
        }


        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            return ListRefrerence.FindIndex(startIndex,count, o => match(o.Object));
        }


        public T FindLast(Predicate<T> match)
        {
            return ListRefrerence.FindLast(o => match(o.Object)).Object;
        }


        public int FindLastIndex(Predicate<T> match)
        {
            return ListRefrerence.FindLastIndex(o => match(o.Object));
        }


        public int FindLastIndex(int startIndex, Predicate<T> match) 
        {
            return ListRefrerence.FindLastIndex(startIndex,o => match(o.Object));
        }


        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            return ListRefrerence.FindLastIndex(startIndex,count, o => match(o.Object));
        }


        public void ForEach(Action<T> action)
        {
            foreach (var item in ListRefrerence)
            {
                action(item.Object);
            }
        }


        public List<T> GetRange(int index, int count)
        {
            return (from x in ListRefrerence.GetRange(index, count) select x.Object).ToList();
        }


        public int IndexOf(T item)
        {
            int index = 0;
            foreach (var itemx in ListRefrerence)
            {
                if (item==itemx.Object)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }


        public void Insert(int index, T item)
    {
            ListRefrerence.Insert(index,new Reference<T>(item,DefaultStrong));
    }


        public void InsertRange(int index, IEnumerable<T> collection)
        {
            ListRefrerence.InsertRange(index,(from x in collection select new Reference<T>(x,DefaultStrong)));
        }



        public bool Remove(T item)
        {

            return ListRefrerence.Remove(ListRefrerence.Find(i => item == i.Object));
        }


        public int RemoveAll(Predicate<T> match)
        {
            return ListRefrerence.RemoveAll(o => match(o.Object));
        }
     
        public void RemoveAt(int index)
        {
            ListRefrerence.RemoveAt(index);
        }


        public void RemoveRange(int index, int count)
        {
            ListRefrerence.RemoveRange(index,count);
        }


        public void Reverse()
        {
            ListRefrerence.Reverse();
        }


        public void Reverse(int index, int count)
        {
            ListRefrerence.Reverse(index, count);
        }


        public void Sort()
        {
            ListRefrerence.Sort();
        }


        public void Sort(Comparison<T> comparison)
        {
            ListRefrerence.Sort((x, y) => comparison(x.Object, y.Object));
        }


        public T[] ToArray()
        {
             return  ((IEnumerable<T>)this).ToArray();
        }

        public void TrimExcess()
        {
            ListRefrerence.TrimExcess();
        }

        public bool TrueForAll(Predicate<T> match)
        {
           return ListRefrerence.TrueForAll(o=>match(o.Object));
        }        

        public IEnumerator<T> GetEnumerator()
        {
            return (from x in ListRefrerence select x.Object).GetEnumerator();
        }



        bool ICollection<T>.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }



        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        bool IList.IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index]=value as T;
            }
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public bool IsSynchronized
        {
            get {return false; }
        }

        public object SyncRoot
        {
            get {return null; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool AddIfNotExist(T item)
        {
            if (!Contains(item))
            {
                Add(item);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
