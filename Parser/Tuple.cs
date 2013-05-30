using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Represents a triple of items.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    public class Tuple<T1, T2, T3>
    {
        /// <summary>
        /// Gets or sets the first item of the Tuple.
        /// </summary>
        public T1 Item1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second item of the tuple.
        /// </summary>
        public T2 Item2
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the third item of the tuple.
        /// </summary>
        public T3 Item3
        {
            get;
            set;
        }

        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }

        /// <summary>
        /// Determines if the given tuple object is equal to this tuple.
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public bool Equals(Tuple<T1, T2, T3> tuple)
        {
            if(tuple == null)
            {
                return false;
            }
            else
            {
                return this.Item1.Equals(tuple.Item1) && this.Item2.Equals(tuple.Item2) && this.Item3.Equals(tuple.Item3);
            }
        }

        /// <summary>
        /// Determines if this tuple is equal to the given other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is Tuple<T1, T2, T3> && obj != null)
            {
                return Equals((Tuple<T1, T2, T3>)obj);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Gets the hash code for this tuple.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, 2})", Item1.ToString(), Item2.ToString(), Item3.ToString());
        }

        public string ToString(string format)
        {
            return string.Format(format, Item1.ToString(), Item2.ToString(), Item3.ToString());
        }
    }

    /// <summary>
    /// Defines a 2-tuple.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Tuple<T1, T2>
    {
        public T1 Item1
        {
            get;
            set;
        }

        public T2 Item2
        {
            get;
            set;
        }

        public Tuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        /// <summary>
        /// Determines if this object equals the given other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(Tuple<T1, T2> obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (Item1 != null && Item2 != null)
            {
                return Item1.Equals(obj.Item1) && Item2.Equals(obj.Item2);
            }
            else if (Item1 != null)
            {
                return Item1.Equals(obj.Item1) && (object)Item2 == (object)obj.Item2;
            }
            else if (Item2 != null)
            {
                return Item2.Equals(obj.Item2) && (object)Item1 == (object)obj.Item1;
            }
            else
            {
                return (object)Item1 == (object)obj.Item1 && (object)Item2 == (object)obj.Item2;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Tuple<T1, T2>)
            {
                return Equals((Tuple<T1, T2>)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            if (Item1 != null && Item2 != null)
            {
                return Item1.GetHashCode() ^ Item2.GetHashCode();
            }
            else if (Item1 != null)
            {
                return Item1.GetHashCode() * 463;
            }
            else if (Item2 != null)
            {
                return Item2.GetHashCode() * 463;
            }
            else
            {
                return 463;
            }
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", Item1, Item2);
        }

        public string ToString(string format)
        {
            return string.Format(format, Item1, Item2);
        }
    }
}
