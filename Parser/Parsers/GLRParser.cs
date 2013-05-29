using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.Grammar;
using Parser.Collections;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Provides an implementation of a GLR Parser that can parse any table.
    /// </summary>
    public class GLRParser<T> : IGrammarParser<T>, IGraphParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the parse table that is currently being used.
        /// </summary>
        public ParseTable<T> ParseTable
        {
            get;
            private set;
        }

        public void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, Terminal<T> endOfInputElement)
        {
            throw new NotImplementedException();
        }

        public ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input)
        {
            throw new NotImplementedException();
        }

        public ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input)
        {
            throw new NotImplementedException();
        }

        public void SetParseTable(ContextFreeGrammar<T> grammar)
        {
            throw new NotImplementedException();
        }
    }
}
