using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
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
    }
}
