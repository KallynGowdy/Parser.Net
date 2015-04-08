using System;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides an interface for a parser that can parse input with a StateGraph into either an AST or Parse Tree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ITableParser<T> : IParser<T>
		where T : IEquatable<T>
	{
		IParseTable<T> Table { get; }
	}
}