using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Defines a LR(1) parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LRParser<T> : IGrammarParser<T>, IGraphParser<T>
    {

        /// <summary>
        /// Gets or sets the parse table used to parse input.
        /// </summary>
        public LRParseTable<T> ParseTable
        {
            get;
            set;
        }

        /// <summary>
        /// Parses an Abstract sentax tree from the given input based on the rules defined in the Terminal elements.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseTree<T> ParseAST(IEnumerable<T> input)
        {
            checkParseTable();
            return null;
        }

        /// <summary>
        /// Parses a concrete sentax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseTree<T> ParseSentaxTree(IEnumerable<T> input)
        {
            checkParseTable();
            return null;
        }

        /// <summary>
        /// Throws InvalidOperationException if the parse table is not set.
        /// </summary>
        private void checkParseTable()
        {
            if(ParseTable == null)
            {
                throw new InvalidOperationException("ParseTable must be set before trying to parse.");
            }
        }

        /// <summary>
        /// Sets the graph used to parse the input.
        /// </summary>
        /// <exception cref="System.ArgumentNullException"/>
        /// <exception cref="System.ArgumentException"/>
        public StateGraph<GrammarElement<T>, LRItem<T>[]> Graph
        {
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("value", "The given graph must be non-null");
                }

                if (value.Root.Value.FirstOrDefault() != null)
                {
                    ParseTable = new LRParseTable<T>(value, value.Root.Value.First().LeftHandSide);
                }
                else
                {
                    throw new ArgumentException("The given graph must have at least one state with at least one item.");
                }
            }
        }

        /// <summary>
        /// Sets the Parse Table from the given graph.
        /// </summary>
        /// <param name="graph"></param>
        public void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("value", "The given graph must be non-null");
            }

            if (graph.Root.Value.FirstOrDefault() != null)
            {
                ParseTable = new LRParseTable<T>(graph, graph.Root.Value.First().LeftHandSide);
            }
            else
            {
                throw new ArgumentException("The given graph must have at least one state with at least one item.");
            }
        }

        /// <summary>
        /// Sets Parse Table from the given grammar.
        /// </summary>
        /// <param name="grammar"></param>
        public void SetParseTable(ContextFreeGrammar<T> grammar)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar", "The given grammar must be non-null");
            }

            ParseTable = new LRParseTable<T>(grammar.CreateStateGraph(), grammar.StartElement);
        }
    }
}
