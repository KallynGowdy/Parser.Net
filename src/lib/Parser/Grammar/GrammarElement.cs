using System;
using System.Linq;
using System.Runtime.Serialization;
using KallynGowdy.ParserGenerator.Collections;

namespace KallynGowdy.ParserGenerator.Grammar
{
	/// <summary>
	///     Defines a element of grammar that is used in productions.
	///     This class is abstract.
	/// </summary>
	[DataContract(Name = "GrammarElement")]
	[Serializable]
	public abstract class GrammarElement<T> : IGrammarElement<T>, IMultiHashedObject
	{
		protected GrammarElement(bool keep = true)
		{
			InnerValue = default(T);
			Keep = keep;
		}

		protected GrammarElement(T value)
		{
			InnerValue = value;
		}

		protected GrammarElement(GrammarElement<T> other)
		{
			InnerValue = other.InnerValue;
		}

		/// <summary>
		///     Gets or sets whether to match this element or anything but this element. This value does not affect equality,
		///     therefore it should be evaluated separately.
		/// </summary>
		[DataMember(Name = "Negated")]
		public bool Negated { get; set; }

		/// <summary>
		///     Gets the Value stored inside this GrammarElement.
		/// </summary>
		[DataMember(Name = "InnerValue")]
		public T InnerValue { get; set; }

		/// <summary>
		///     Gets or sets whether this element should be kept or discarded.
		/// </summary>
		[DataMember(Name = "Keep")]
		public bool Keep { get; set; }

		public virtual int[] GetHashCodes()
		{
			return new[] { GetHashCode() };
		}

		public override bool Equals(object obj)
		{
			return (obj.GetHashCode() == GetHashCode());
		}

		public override int GetHashCode()
		{
			if (InnerValue != null)
				return InnerValue.GetHashCode();
			return unchecked(521 * typeof(T).GetHashCode());
		}

		public static GrammarElement<T>[] operator +(GrammarElement<T> left, GrammarElement<T> right)
		{
			return new[] { left, right };
		}

		public static GrammarElement<T>[] operator +(GrammarElement<T>[] left, GrammarElement<T> right)
		{
			GrammarElement<T>[] elements = new GrammarElement<T>[left.Length + 1];
			left.CopyTo(elements, 0);
			elements[elements.Length - 1] = right;
			return elements;
		}

		public static GrammarElement<T>[] operator *(GrammarElement<T> left, int right)
		{
			if (right < 0) throw new ArgumentOutOfRangeException("right", "Must be greater than or equal to 0.");
			GrammarElement<T>[] elements = new GrammarElement<T>[right];

			for (int i = 0; i < right; i++)
			{
				elements[i] = left;
			}

			return elements;
		}
	}
}