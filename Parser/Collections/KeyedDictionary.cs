using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Collections
{
    /// <summary>
    /// Provides a dictionary that supports keys that contain extra information that does not affect equality.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class KeyedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, Tuple<TKey, TValue>> lookup;

        /// <summary>
        /// Creates a new dictionary that supports retrival of keys that may contain different information than the key used to access it.
        /// </summary>
        public KeyedDictionary()
        {
            lookup = new Dictionary<TKey,Tuple<TKey,TValue>>();   
        }

        /// <summary>
        /// Creates a new dictionary that supports retrival of keys that may contain different information than the key used to access it.
        /// </summary>
        public KeyedDictionary(IEqualityComparer<TKey> comparer)
        {
            lookup = new Dictionary<TKey,Tuple<TKey,TValue>>(comparer);
        }

        /// <summary>
        /// Creates a new dictionary that supports retrival of keys that may contain different information than the key used to access it.
        /// </summary>
        public KeyedDictionary(IDictionary<TKey, TValue> dictionary)
        {
            lookup = new Dictionary<TKey,Tuple<TKey,TValue>>(dictionary.ToDictionary(a => a.Key, a => new Tuple<TKey, TValue>(a.Key, a.Value)));
        }

        /// <summary>
        /// Adds the given value to the dictionary with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            lookup.Add(key, new Tuple<TKey,TValue>(key, value));
        }

        /// <summary>
        /// Determines if the given key exists in the dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return lookup.ContainsKey(key);
        }

        /// <summary>
        /// Gets the key stored in the dictionary based on the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TKey GetKey(TKey key)
        {
            return lookup[key].Item1;
        }

        /// <summary>
        /// Gets the collection of keys stored in the dictionary.
        /// </summary>
        public ICollection<TKey> Keys
        {
            get
            {
                return lookup.Keys;
            }
        }

        /// <summary>
        /// Removes the value from the dictionary based on the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            return lookup.Remove(key);
        }

        /// <summary>
        /// Tries to get the value based on the key. Returns whether the operation was successful.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Tuple<TKey, TValue> keyVal;
            bool result = lookup.TryGetValue(key, out keyVal);
            value = keyVal != null ? keyVal.Item2 : default(TValue);
            return result;
        }

        /// <summary>
        /// Gets the list of values stored in the dictionary.
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                return lookup.Values.Select(a => a.Item2).ToArray();
            }
        }

        /// <summary>
        /// Gets the value related to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="System.IndexOutOfRangeException"/>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                return lookup[key].Item2;
            }
            set
            {
                lookup[key] = new Tuple<TKey,TValue>(key, value);
            }
        }

        /// <summary>
        /// Adds the given item to the dictionary.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lookup.Add(item.Key, new Tuple<TKey,TValue>(item.Key, item.Value));
        }

        /// <summary>
        /// Removes all of the items from this dictionary.
        /// </summary>
        public void Clear()
        {
            lookup.Clear();
        }

        /// <summary>
        /// Determines if the given item is contained in this dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return lookup.ContainsKey(item.Key);
        }

        /// <summary>
        /// Copies this dictionary to the given array starting at the given index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            var temp = this.lookup.ToArray();
            temp.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the number of items in this dictionary.
        /// </summary>
        public int Count
        {
            get
            {
                return lookup.Count;
            }
        }

        /// <summary>
        /// Determines if this dictionary is imutable. Always false.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Removes the given item from the dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return lookup.Remove(item.Key);
        }

        /// <summary>
        /// Gets the generic enumerator for this dictionary.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return lookup.Select(a => new KeyValuePair<TKey, TValue>(a.Key, a.Value.Item2)).GetEnumerator();
        }

        /// <summary>
        /// Gets the non generic enumerator for this dicionary.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return lookup.Select(a => new KeyValuePair<TKey, TValue>(a.Key, a.Value.Item2)).GetEnumerator();
        }
    }
}
