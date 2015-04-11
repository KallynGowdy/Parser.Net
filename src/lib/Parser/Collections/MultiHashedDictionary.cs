using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KallynGowdy.ParserGenerator.Collections
{
	/// <summary>
	///     Provides a dictionary for keys that can have multiple hashes that point to the same object such that when accessing
	///     an object the single hash code points to the desired object.
	/// </summary>
	public class MultiHashedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		where TKey : IMultiHashedObject
	{
		private readonly List<TKey> keys;
		private readonly Dictionary<int, TValue> lookup;

		public MultiHashedDictionary()
		{
			lookup = new Dictionary<int, TValue>();
			keys = new List<TKey>();
		}

		/// <summary>
		///     Adds the given key and value to the dictionary.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void Add(TKey key, TValue value)
		{
			keys.Add(key);
			int[] hashes = key.GetHashCodes();
			foreach (int hash in hashes)
				lookup.Add(hash, value);
		}

		/// <summary>
		///     Determines if the given key is contained in the dictionary.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsKey(TKey key)
		{
			return lookup.ContainsKey(key.GetHashCode());
		}

		public ICollection<TKey> Keys
		{
			get { return keys; }
		}

		public bool Remove(TKey key)
		{
			return lookup.Remove(keys.GetHashCode());
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return lookup.TryGetValue(key.GetHashCode(), out value);
		}

		public ICollection<TValue> Values
		{
			get { return lookup.Values; }
		}

		public TValue this[TKey key]
		{
			get { return lookup[keys.GetHashCode()]; }
			set { lookup[keys.GetHashCode()] = value; }
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			int[] hashes = item.Key.GetHashCodes();
			foreach (int hash in hashes)
				lookup.Add(hash, item.Value);
			keys.Add(item.Key);
		}

		public void Clear()
		{
			lookup.Clear();
			keys.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return lookup.ContainsKey(item.Key.GetHashCode());
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			KeyValuePair<int, TValue>[] temp = lookup.ToArray();
			temp.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return lookup.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			var result = false;
			int[] hashes = item.Key.GetHashCodes();
			foreach (int hash in hashes)
			{
				if (!result)
					result = lookup.Remove(hash);
				else
					lookup.Remove(hash);
			}
			return result;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return lookup.Select((a, i) => new KeyValuePair<TKey, TValue>(keys[i], a.Value)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return lookup.Select((a, i) => new KeyValuePair<TKey, TValue>(keys[i], a.Value)).GetEnumerator();
		}
	}
}