using System;
using System.Runtime.Serialization;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	/// Provides a class that defines that a "shift" should occur at the location that the action is stored in the table.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DataContract]
	public sealed class ShiftAction<T> : ParserAction<T> where T : IEquatable<T>
	{
		/// <summary>
		/// Gets the next state that the parser should move to.
		/// </summary>
		[DataMember]
		public int NextState
		{
			get;
			private set;
		}

		public ShiftAction(ParseTable<T> table, int nextState)
			: base(table)
		{
			if (nextState < 0)
			{
				throw new ArgumentOutOfRangeException("nextState", "Must be greater than or equal to 0");
			}
			this.NextState = nextState;
		}

		public override string ToString()
		{
			return string.Format("Shift({0})", NextState);
		}
	}
}