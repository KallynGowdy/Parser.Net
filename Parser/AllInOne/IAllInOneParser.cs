using System;
using System.Collections.Generic;
using LexicalAnalysis;

namespace Parser.AllInOne
{
    /// <summary>
    /// Defines an interface for an All-in-One parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAllInOneParser<T>
    {
        ParseTree<Token<T>> ParseAST(IEnumerable<Token<T>> input);
        ParseTree<Token<T>> ParseSentaxTree(IEnumerable<Token<T>> input);
    }
}
