using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KallynGowdy.ParserGenerator.Grammar
{
	public class LRItemCollection<T> : IEnumerable<LRItem<T>>
		where T : IEquatable<T>
	{
		public LRItemCollection(params LRItem<T>[] items)
		{
			Items = items;
		}

		/// <summary>
		///     Gets or sets the value at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public LRItem<T> this[int index]
		{
			get { return Items[index]; }
			set { Items[index] = value; }
		}

		/// <summary>
		///     Gets the items contained in this collection
		/// </summary>
		public LRItem<T>[] Items { get; }

		/// <summary>
		///     Gets the enumerator for the collection.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<LRItem<T>> GetEnumerator()
		{
			return Items.AsEnumerable().GetEnumerator();
		}

		/// <summary>
		///     Gets the enumerator for the collection.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return Items.GetEnumerator();
		}
	}
}