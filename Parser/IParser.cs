using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser
{
    /// <summary>
    /// Provides an interface for a parser that can parse input into either an AST or Parse Tree.
    /// </summary>
    public interface IParser<T>
    {
        /// <summary>
        /// Parses an Abstract Sentax Tree.
        /// </summary>
        /// <returns></returns>
        ParseTree<T> ParseAST(IEnumerable<T> input);

        /// <summary>
        /// Parses a sentax tree that completely describes the parsed input.
        /// </summary>
        /// <returns></returns>
        ParseTree<T> ParseSentaxTree(IEnumerable<T> input);
    }

    /// <summary>
    /// Provides an interface for a parser that can parse input with a grammar into either an AST or Parse Tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGrammarParser<T> : IParser<T>
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
    public interface IGraphParser<T> : IParser<T>
    {
        /// <summary>
        /// Sets the internal parse table of the parser from the given graph.
        /// </summary>
        /// <param name="grammar"></param>
        void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph);
    }

    /// <summary>
    /// Provides an interface for a parser that can parse input with a StateGraph into either an AST or Parse Tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITableParser<T> : IParser<T>
    {
        IParseTable<T> Table
        {
            get;
        }
    }
}
