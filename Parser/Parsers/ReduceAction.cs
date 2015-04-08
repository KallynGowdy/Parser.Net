using System;
using System.Runtime.Serialization;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides a class that defines that a "reduce" should occur at the location that the action is stored in the table.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DataContract]
	public sealed class ReduceAction<T> : ParserAction<T>
		where T : IEquatable<T>
	{
		public ReduceAction(ParseTable<T> table, LRItem<T> reduceItem)
			: base(table)
		{
			if (reduceItem != null)
				ReduceItem = reduceItem;
			else
				throw new ArgumentNullException("reduceItem", "Must be non-null");
		}

		/// <summary>
		///     Gets the item that defines the reduction to perform.
		/// </summary>
		[DataMember]
		public LRItem<T> ReduceItem { get; private set; }

		public override string ToString()
		{
			return string.Format("Reduce({0})", ReduceItem);
		}
	}
}