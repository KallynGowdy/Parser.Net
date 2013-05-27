using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.Parsers;
using Parser.StateMachine;

namespace Parser
{
    /// <summary>
    /// Provides an interface for a parser that can parse input into either an AST or Parse Tree.
    /// </summary>
    public interface IParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Parses an Abstract Sentax Tree.
        /// </summary>
        /// <returns></returns>
        ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input);

        /// <summary>
        /// Parses a sentax tree that completely describes the parsed input.
        /// </summary>
        /// <returns></returns>
        ParseResult<T> ParseSentaxTree(IEnumerable<Terminal<T>> input);
    }

    /// <summary>
    /// Provides an interface for a parser that can parse input with a grammar into either an AST or Parse Tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGrammarParser<T> : IParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Sets the internal parse table of the parser from the given grammar.
        /// </summary>
        /// <param name="grammar"></param>
        void SetParseTable(ContextFreeGrammar<T> grammar);
    }

    /// <summary>
    /// Provides an interface for a parser that can parse input with a StateGraph into either an AST or Parse Tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGraphParser<T> : IParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Sets the internal parse table of the parser from the given graph.
        /// </summary>
        /// <param name="grammar"></param>
        void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, Terminal<T> endOfInputElement);
    }

    /// <summary>
    /// Provides an interface for a parser that can parse input with a StateGraph into either an AST or Parse Tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITableParser<T> : IParser<T> where T : IEquatable<T>
    {
        IParseTable<T> Table
        {
            get;
        }
    }
}
