﻿using System;
using System.Runtime.Serialization;

namespace KallynGowdy.ParserGenerator.Grammar
{
	/// <summary>
	///     Defines an element that has one or more terminal elements that represent it.
	///     Equality is determined by the name of the Non-Terminal.
	/// </summary>
	[DataContract(Name = "NonTerminal")]
	[Serializable]
	public class NonTerminal<T> : GrammarElement<T>, IEquatable<NonTerminal<T>>
	{
		/// <summary>
		///     Creates a new NonTerminal(T) object with the given inner value
		///     and name as value.ToString().
		/// </summary>
		/// <param name="value"></param>
		public NonTerminal(T value)
			: base(value)
		{
			Name = value.ToString();
		}

		public NonTerminal(string name)
		{
			Name = name;
		}

		public NonTerminal(string name, bool keep)
			: base(keep)
		{
			Name = name;
		}

		/// <summary>
		///     Creates an new NonTerminal(T) object with the given name and value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public NonTerminal(string name, T value)
			: base(value)
		{
			Name = name;
		}

		private NonTerminal()
		{
			InnerValue = default(T);
			Name = string.Empty;
		}

		/// <summary>
		///     Gets the name of this Non-Terminal.
		/// </summary>
		[DataMember(Name = "Name")]
		public string Name { get; }

		/// <summary>
		///     Determines if the given NonTerminal(T) is the same as this NonTerminal(T).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public bool Equals(NonTerminal<T> obj)
		{
			return obj.Name.Equals(Name);
		}

		/// <summary>
		///     Gets a unique hash code describing this object that is guarenteed to not change over the lifetime
		///     of this object.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Negated.GetHashCode();
		}

		/// <summary>
		///     Determines if the given object contains the same value as this object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is NonTerminal<T>)
				return Equals((NonTerminal<T>) obj);
			return base.Equals(obj);
		}

		/// <summary>
		///     Returns the Non Terminal element formatted as: "Name(InnerValue)"
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			//if (!InnerValue.Equals(default(T)))
			//{
			//    return string.Format("{0}({1})", Name, InnerValue);
			//}
			//else
			//{
			return Name;
			//}
		}

		public static Production<T> operator ^(NonTerminal<T> left, GrammarElement<T>[] right)
		{
			return new Production<T>(left, right);
		}

		public static Production<T> operator ^(NonTerminal<T> left, GrammarElement<T> right)
		{
			return left ^ new[] { right };
		}
	}
}