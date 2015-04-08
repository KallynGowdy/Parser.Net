using System.Collections.Generic;
using LexicalAnalysis;

namespace KallynGowdy.ParserGenerator.Parsers.AllInOne
{
    /// <summary>
    /// Defines an interface for an All-in-One parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAllInOneParser<T>
    {
        ParseResult<Token<T>> ParseAST(IEnumerable<Token<T>> input);
        ParseResult<Token<T>> ParseSyntaxTree(IEnumerable<Token<T>> input);
    }
}
