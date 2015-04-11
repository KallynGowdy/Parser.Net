using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KallynGowdy.ParserGenerator.Grammar
{
	/// <summary>
	///     Defines a relation between a non terminal element and several terminal/non-terminal elements.
	/// </summary>
	[Serializable]
	public class Production<T> : IEquatable<Production<T>>
	{
		public Production(NonTerminal<T> nonTerminal, params GrammarElement<T>[] derivedElements)
		{
			NonTerminal = nonTerminal;
			DerivedElements = new List<GrammarElement<T>>(derivedElements);
		}

		/// <summary>
		///     Gets or sets the elements that relate to the non-terminal.
		/// </summary>
		public List<GrammarElement<T>> DerivedElements { get; }

		/// <summary>
		///     Gets or sets the non terminal element that can be reduced to or derived from. (NonTerminal -> DerivedElements)
		/// </summary>
		public NonTerminal<T> NonTerminal { get; }

		/// <summary>
		///     Gets the element at index, returns null if index is out of range.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public GrammarElement<T> GetElement(int index)
		{
			if (index >= 0 &&
				index < DerivedElements.Count)
				return DerivedElements[index];
			return null;
		}

		/// <summary>
		///     Returns a string representation of this Production in the form of: NonTerminal -> DerivedElements
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var b = new StringBuilder();
			//add the NonTerminal
			b.Append(NonTerminal);

			//then ' -> '
			b.Append(" -> ");

			//add each of the derived elements
			foreach (GrammarElement<T> element in DerivedElements)
				b.Append(element);

			return b.ToString();
		}

		public bool Equals(Production<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return 
				NonTerminal.Equals(other.NonTerminal) &&
				DerivedElements.SequenceEqual(other.DerivedElements);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Production<T>)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (NonTerminal.GetHashCode() * 397) ^ DerivedElements.GetHashCode();
			}
		}

		public static bool operator ==(Production<T> left, Production<T> right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Production<T> left, Production<T> right)
		{
			return !Equals(left, right);
		}
	}
}