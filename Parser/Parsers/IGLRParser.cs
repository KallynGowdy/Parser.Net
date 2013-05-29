using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser.Parsers
{
    public interface IGLRParser<T> : IParser<T>
    {
        /// <summary>
        /// Gets or sets the parse table of the GLR Parser.
        /// </summary>
        ParseTable<T> ParseTable
        {
            get;
            set;
        }

        ParseResult<T>[] ParseAST(IEnumerable<Terminal<T>> input);
        
        ParseResult<T>[] ParseSyntaxTree(IEnumerable<Terminal<T>> input); 
    }
}
