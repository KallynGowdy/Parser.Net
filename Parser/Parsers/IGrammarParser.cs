using System;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides an interface for a parser that can parse input with a grammar into either an AST or Parse Tree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IGrammarParser<T> : IParser<T>
		where T : IEquatable<T>
	{
		/// <summary>
		///     Sets the internal parse table of the parser from the given grammar.
		/// </summary>
		/// <param name="grammar"></param>
		void SetParseTable(ContextFreeGrammar<T> grammar);
	}
}