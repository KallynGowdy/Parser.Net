using System;
using System.Collections.Generic;
using LexicalAnalysis;
using Parser.Parsers;

namespace Parser.AllInOne
{
    /// <summary>
    /// Defines an interface for an All-in-One parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAllInOneParser<T>
    {
        ParseResult<Token<T>> ParseAST(IEnumerable<Token<T>> input);
        ParseResult<Token<T>> ParseSentaxTree(IEnumerable<Token<T>> input);
    }
}
