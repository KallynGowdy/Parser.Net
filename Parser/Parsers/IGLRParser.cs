using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser.Parsers
{
    public interface IGLRParser<T> : IParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the parse table of the GLR Parser.
        /// </summary>
        ParseTable<T> ParseTable
        {
            get;
            set;
        }

        new ParseResult<T>[] ParseAST(IEnumerable<Terminal<T>> input);
        
        new ParseResult<T>[] ParseSyntaxTree(IEnumerable<Terminal<T>> input); 
    }
}
