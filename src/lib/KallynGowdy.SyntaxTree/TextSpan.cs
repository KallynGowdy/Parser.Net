using System;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines a structure that represents an immutable span of text.
	/// </summary>
	public struct TextSpan : IEquatable<TextSpan>
	{
		/// <summary>
		/// Gets the start of the span.
		/// </summary>
		public int Start { get; }

		/// <summary>
		/// Gets the number of characters that the span crosses.
		/// </summary>
		public int Length { get; }

		/// <summary>
		/// Gets the end of the span.
		/// </summary>
		public int End => Start + Length;

		/// <summary>
		/// Gets whether the length of the span is 0.
		/// </summary>
		public bool IsEmpty => Length == 0;

		/// <summary>
		/// Creates a new span that starts at the given location and has the given length.
		/// </summary>
		/// <param name="start">The starting point of the span.</param>
		/// <param name="length">The length of the span.</param>
		public TextSpan(int start, int length)
		{
			if (start < 0) throw new ArgumentOutOfRangeException("start", "Must be greater than or equal to 0.");
			if (length < 0) throw new ArgumentOutOfRangeException("length", "Must be greater than or equal to 0.");
			Start = start;
			Length = length;
		}

		public bool Equals(TextSpan other)
		{
			return Start == other.Start && Length == other.Length;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			return obj is TextSpan && Equals((TextSpan) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Start*397) ^ Length;
			}
		}

		public static bool operator ==(TextSpan left, TextSpan right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(TextSpan left, TextSpan right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		/// Calculates the amount of overlap between this span and the given other span and returns a <see cref="TextSpan"/> that represents the result.
		/// Returns null if there is no overlap.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public TextSpan? Overlap(TextSpan other)
		{
			int start = Math.Max(Start, other.Start);
			int end = Math.Min(End, other.End);
			if (start < end)
			{
				return new TextSpan(start, end - start);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Determines if the current <see cref="TextSpan"/> overlaps with the given other <see cref="TextSpan"/>.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool OverlapsWith(TextSpan other)
		{
			return Overlap(other) != null;
		}

		public override string ToString()
		{
			return string.Format("({0}-{1})", Start, Length);
		}
	}
}