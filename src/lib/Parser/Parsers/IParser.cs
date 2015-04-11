using System;
using System.Collections.Generic;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides an interface for a parser that can parse input into either an AST or Parse Tree.
	/// </summary>
	public interface IParser<T>
		where T : IEquatable<T>
	{
		/// <summary>
		///     Parses an Abstract Syntax Tree.
		/// </summary>
		/// <returns></returns>
		ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input);

		/// <summary>
		///     Parses a Syntax tree that completely describes the parsed input.
		/// </summary>
		/// <returns></returns>
		ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input);
	}
}