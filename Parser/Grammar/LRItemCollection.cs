using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.StateMachine;

namespace Parser.Grammar
{
    public class LRItemCollection<T> : IEnumerable<LRItem<T>>
    {
        private LRItem<T>[] items;

        /// <summary>
        /// Gets the items contained in this collection
        /// </summary>
        public LRItem<T>[] Items
        {
            get
            {
                return items;
            }
        }

        /// <summary>
        /// Gets or sets the value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public LRItem<T> this[int index]
        {
            get
            {
                return items[index];
            }
            set
            {
                items[index] = value;
            }
        }

        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LRItem<T>> GetEnumerator()
        {
            return items.AsEnumerable().GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the collection.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        public LRItemCollection(params LRItem<T>[] items)
        {
            this.items = items;
        }
    }
}
