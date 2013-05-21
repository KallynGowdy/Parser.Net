using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Grammar
{
    /// <summary>
    /// Provides an interface for a parse table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParseTable<T>
    {
        Table<int, Terminal<T>, List<ParserAction<T>>> ActionTable
        {
            get;
        }

        Table<int, NonTerminal<T>, int?> GotoTable
        {
            get;
        }

        ParserAction<T>[] this[int currentState, GrammarElement<T> nextInput]
        {
            get;
        }
    }
}
