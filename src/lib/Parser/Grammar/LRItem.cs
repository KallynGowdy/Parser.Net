using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace KallynGowdy.ParserGenerator.Grammar
{
	/// <summary>
	///     Defines an LR(1) item that describes the current state of reducing a production.
	/// </summary>
	[DataContract]
	public class LRItem<T> : IEquatable<LRItem<T>>
		where T : IEquatable<T>
	{
		/// <summary>
		///     Creates a new LR(n) Item based on the given dot index and the given production.
		/// </summary>
		/// <param name="dotIndex"></param>
		/// <param name="production"></param>
		public LRItem(int dotIndex, Production<T> production)
		{
			LeftHandSide = production.NonTerminal;
			ProductionElements = production.DerivedElements.ToArray();

			//if it is a valid index
			if (dotIndex < ProductionElements.Length &&
			    dotIndex >= 0)
				DotIndex = dotIndex;
			else if (dotIndex < 0)
				DotIndex = 0;
			//otherwise put it in front of the last element.
			else
			{
				if (ProductionElements.Length > 0)
					DotIndex = ProductionElements.Length - 1;
				else
					DotIndex = 0;
			}
		}

		/// <summary>
		///     Creates a new LRItem with the given dot index from the given other item.
		/// </summary>
		/// <param name="dotIndex"></param>
		/// <param name="otherItem"></param>
		public LRItem(int dotIndex, LRItem<T> otherItem)
		{
			LeftHandSide = otherItem.LeftHandSide;
			ProductionElements = otherItem.ProductionElements;
			DotIndex = dotIndex;
			LookaheadElement = otherItem.LookaheadElement;
		}

		/// <summary>
		///     Creates a new LRItem with the given dot index from the given production with the given lookahead element.
		/// </summary>
		/// <param name="dotIndex"></param>
		/// <param name="production"></param>
		/// <param name="lookahead"></param>
		public LRItem(int dotIndex, Production<T> production, Terminal<T> lookahead)
		{
			LeftHandSide = production.NonTerminal;
			ProductionElements = production.DerivedElements.ToArray();

			//if it is a valid index
			if (dotIndex < ProductionElements.Length &&
			    dotIndex >= 0)
				DotIndex = dotIndex;
			else if (dotIndex < 0)
				DotIndex = 0;
			//otherwise put it in front of the last element.
			else
			{
				if (ProductionElements.Length > 0)
					DotIndex = ProductionElements.Length - 1;
				else
					DotIndex = 0;
			}
			LookaheadElement = lookahead;
		}

		/// <summary>
		///     Gets the index of the dot, this defines how much of the production we have already seen.
		/// </summary>
		[DataMember(Name = "DotIndex")]
		public int DotIndex { get; set; }

		/// <summary>
		///     Gets the non-terminal of the left hand side of the production.
		/// </summary>
		[DataMember(Name = "LeftHandSide")]
		public NonTerminal<T> LeftHandSide { get; private set; }

		/// <summary>
		///     Gets or sets the lookahead element.
		/// </summary>
		[DataMember(Name = "LookaheadElement")]
		public Terminal<T> LookaheadElement { get; set; }

		/// <summary>
		///     Gets the IEQuality(LRItem(T)) comparer for LRItems(T)
		/// </summary>
		/// <summary>
		///     Gets the elements on the Right hand side of the production.
		/// </summary>
		[DataMember(Name = "ProductionElements")]
		public GrammarElement<T>[] ProductionElements { get; private set; }

		/// <summary>
		///     Returns whether the value(s) contained by the given other LRItem equals this object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(LRItem<T> other)
		{
			if (LookaheadElement != null &&
			    other.LookaheadElement != null)
				return (LeftHandSide.Equals(other.LeftHandSide) && DotIndex == other.DotIndex && ProductionElements.SequenceEqual(other.ProductionElements) && LookaheadElement.Equals(other.LookaheadElement));
			return (LeftHandSide.Equals(other.LeftHandSide) && DotIndex == other.DotIndex && ProductionElements.SequenceEqual(other.ProductionElements));
		}

		/// <summary>
		///     Gets the element that is right in front of the dot.
		///     Returns null if the reduction is at the end.
		/// </summary>
		/// <returns></returns>
		public GrammarElement<T> GetNextElement()
		{
			if (DotIndex < ProductionElements.Length)
				return ProductionElements[DotIndex];
			return null;
		}

		/// <summary>
		///     Gets the Element at the index, Returns null if index is out of range.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public GrammarElement<T> GetElement(int index)
		{
			if (index >= 0 &&
			    index < ProductionElements.Length)
				return ProductionElements[index];
			return null;
		}

		/// <summary>
		///     Gets the element that is lookahead items in front of the next element.
		///     Returns Null if it is the end of the production.
		/// </summary>
		/// <returns></returns>
		public GrammarElement<T> GetNextElement(int lookahead)
		{
			if (DotIndex + lookahead < ProductionElements.Length)
				return ProductionElements[DotIndex + lookahead];
			return null;
		}

		/// <summary>
		///     Copies this LRItem into a new LRItem with eqivalent properties.
		/// </summary>
		/// <returns></returns>
		public LRItem<T> Copy()
		{
			return new LRItem<T>(DotIndex, this);
		}

		/// <summary>
		///     Returns whether the given other object equals this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is LRItem<T>)
				return Equals((LRItem<T>) obj);
			return base.Equals(obj);
		}

		/// <summary>
		///     Gets the hash code for the production elements.
		/// </summary>
		/// <returns></returns>
		private int getProductionHash()
		{
			int hash = unchecked(521*ProductionElements.Length + 48976213);
			for (var i = 0; i < ProductionElements.Length; i++)
				hash = unchecked((ProductionElements[i].GetHashCode() ^ hash)*(i + 1));
			return hash;
		}

		/// <summary>
		///     Gets the hash code for this object.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (LookaheadElement != null)
				return getProductionHash() ^ DotIndex ^ LookaheadElement.GetHashCode();
			return getProductionHash() ^ DotIndex;
		}

		/// <summary>
		///     Returns whether the dot(•) is at the end of the production.
		/// </summary>
		/// <returns></returns>
		public bool IsAtEndOfProduction()
		{
			return (DotIndex >= ProductionElements.Length);
		}

		/// <summary>
		///     Gets the element that is before the dot(•). Returns null if the dot is a the end of the production.
		/// </summary>
		/// <returns></returns>
		public GrammarElement<T> LastElement()
		{
			if (DotIndex > 0)
				return ProductionElements[DotIndex - 1];
			return null;
		}

		/// <summary>
		///     Formats the item into a 'nice' string of the form:
		///     "N -> a•w" where 'N' is the non terminal, 'a' is an arbitrary string of terminals/non-terminals,
		///     '•' denotes the index of DotIndex, and 'w' is an arbitrary string of terminals/non-terminals.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var b = new StringBuilder();
			// b.AppendFormat("{0} -> ", LeftHandSide);
			b.Append(LeftHandSide);
			b.Append(" -> ");

			for (var i = 0; i < ProductionElements.Length; i++)
			{
				if (i == DotIndex)
				{
					//append a dot(•)
					b.Append('\u25CF');
				}
				b.Append(ProductionElements[i]);
				b.Append(" ");
			}

			//if the dot is at the end
			if (DotIndex >= ProductionElements.Length)
			{
				//append •
				b.Append('\u25CF');
			}

			if (LookaheadElement != null)
				b.AppendFormat(", {0}", LookaheadElement);

			return b.ToString();
		}
	}
}