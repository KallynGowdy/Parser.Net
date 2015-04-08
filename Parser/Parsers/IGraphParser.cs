using System;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides an interface for a parser that can parse input with a StateGraph into either an AST or Parse Tree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IGraphParser<T> : IParser<T>
		where T : IEquatable<T>
	{
		/// <summary>
		///     Sets the internal parse table of the parser from the given graph.
		/// </summary>
		/// <param name="grammar"></param>
		void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, Terminal<T> endOfInputElement);
	}
}