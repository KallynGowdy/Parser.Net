using System;
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
			return new[] {GetHashCode()};
		}

		public override bool Equals(object obj)
		{
			return (obj.GetHashCode() == GetHashCode());
		}

		public override int GetHashCode()
		{
			if (InnerValue != null)
				return InnerValue.GetHashCode();
			return unchecked(521*typeof (T).GetHashCode());
		}
	}
}