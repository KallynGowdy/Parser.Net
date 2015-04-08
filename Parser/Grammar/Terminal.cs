using System;

namespace KallynGowdy.ParserGenerator.Grammar
{
	/// <summary>
	///     Defines a value that cannot be derived into a further 'child' element.
	/// </summary>
	[Serializable]
	public class Terminal<T> : GrammarElement<T>, ITerminal<T>, IEquatable<Terminal<T>>
		where T : IEquatable<T>
	{
		public Terminal(T value, bool keep = true, bool negated = false)
			: base(value)
		{
			Keep = keep;
			Negated = negated;
		}

		/// <summary>
		///     Gets whether this terminal element is an end of input element.
		/// </summary>
		public bool EndOfInput
		{
			get { return InnerValue == null && !Negated; }
		}

		/// <summary>
		///     Determines if this terminal element equals the given other element.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual bool Equals(Terminal<T> other)
		{
			return Equals((ITerminal<T>) other);
		}

		/// <summary>
		///     Gets a unique interger value that describes this object that is garenteed not to change.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (InnerValue != null)
				return base.GetHashCode();
			return Negated.GetHashCode();
		}

		/// <summary>
		///     Determines if this terminal is equal to the given other terminal.
		/// </summary>
		/// <param name="terminal"></param>
		/// <returns></returns>
		public virtual bool Equals(ITerminal<T> terminal)
		{
			if (terminal != null)
			{
				if (InnerValue != null &&
				    terminal.InnerValue != null)
					return InnerValue.Equals(terminal.InnerValue);
				return (object) InnerValue == (object) terminal.InnerValue;
			}
			return (object) InnerValue == terminal;
		}

		/// <summary>
		///     Determines if this terminal object is equal to the given other object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is ITerminal<T> ||
			    obj == null)
				return Equals((ITerminal<T>) obj);
			return base.Equals(obj);
		}

		public static bool operator ==(Terminal<T> left, Terminal<T> right)
		{
			if (((object) left) == null ||
			    ((object) right) == null)
				return left == ((object) right);
			return left.Equals(right);
		}

		public static bool operator !=(Terminal<T> left, Terminal<T> right)
		{
			//if the left or right objects are null
			if (((object) left) == null ||
			    ((object) right) == null)
				return left != ((object) right);
			//otherwise return if the left object equals the right
			return !left.Equals(right);
		}

		/// <summary>
		///     Returns this object as a string represented by the inner value.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (InnerValue != null)
			{
				if (Negated)
					return string.Format("Not {0}", InnerValue);
				return InnerValue.ToString();
			}
			if (!Negated)
				return "END_OF_INPUT";
			return "Anything";
		}
	}
}