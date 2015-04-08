using System;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Defines a set of values that determine the possible errors(shift-reduce/reduce-reduce) that can occur with a parse
	///     table.
	/// </summary>
	[Flags]
	public enum ParseTableExceptionType
	{
		SHIFT_REDUCE = 1,
		REDUCE_REDUCE = 2,
		FIRST_SET = 3
	};
}