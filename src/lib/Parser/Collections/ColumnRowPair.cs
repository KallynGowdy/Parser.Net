﻿using System;

namespace KallynGowdy.ParserGenerator.Collections
{
	/// <summary>
	///     Defines a column that relates to a row.
	/// </summary>
	/// <typeparam name="TRow">The type of the column.</typeparam>
	/// <typeparam name="TColumn">The type of the row.</typeparam>
	public struct ColumnRowPair<TRow, TColumn> : IEquatable<ColumnRowPair<TRow, TColumn>>
		where TRow : IEquatable<TRow>
		where TColumn : IEquatable<TColumn>
	{
		/// <summary>
		///     Creates a new column row pair object from the given row and column.
		/// </summary>
		/// <param name="row"></param>
		/// <param name="column"></param>
		public ColumnRowPair(TRow row, TColumn column)
			: this()
		{
			Column = column;
			Row = row;
		}

		/// <summary>
		///     Gets or sets the value of the row.
		/// </summary>
		public TColumn Column { get; }

		/// <summary>
		///     Gets or sets the value of the column.
		/// </summary>
		public TRow Row { get; }

		/// <summary>
		///     Determines if this column row pair object is equal to the given other object.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(ColumnRowPair<TRow, TColumn> other)
		{
			return other.Row.Equals(Row) && other.Column.Equals(Column);
		}

		public static bool operator ==(ColumnRowPair<TRow, TColumn> left, ColumnRowPair<TRow, TColumn> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ColumnRowPair<TRow, TColumn> left, ColumnRowPair<TRow, TColumn> right)
		{
			return !left.Equals(right);
		}

		/// <summary>
		///     Determines if this column row pair object is equal to the given other object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj is ColumnRowPair<TRow, TColumn>)
				return Equals((ColumnRowPair<TRow, TColumn>) obj);
			return base.Equals(obj);
		}

		/// <summary>
		///     Gets the hash code for this ColumnRowPair.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			if (Row != null &&
			    Column != null)
				return Row.GetHashCode() ^ Column.GetHashCode();
			if (Column != null)
				return unchecked(Column.GetHashCode()*23);
			if (Row != null)
				return unchecked(Row.GetHashCode()*23);
			return base.GetHashCode();
		}
	}
}