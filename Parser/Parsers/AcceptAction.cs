using System;
using System.Runtime.Serialization;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides a class that defines that the parse should be accepted(and therefore successful).
	/// </summary>
	[DataContract]
	public sealed class AcceptAction<T> : ParserAction<T>
		where T : IEquatable<T>
	{
		public AcceptAction(ParseTable<T> table, LRItem<T> acceptItem)
			: base(table)
		{
			if (acceptItem != null)
				AcceptItem = acceptItem;
			else
				throw new ArgumentNullException("acceptItem", "Must be non-null");
		}

		/// <summary>
		///     Gets the item that defines that the parse should be accepted.
		/// </summary>
		[DataMember]
		public LRItem<T> AcceptItem { get; private set; }

		public override string ToString()
		{
			return "Accept";
		}
	}
}