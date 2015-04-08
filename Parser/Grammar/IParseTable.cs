using System;
using System.Collections.Generic;
using KallynGowdy.ParserGenerator.Collections;
using KallynGowdy.ParserGenerator.Parsers;

namespace KallynGowdy.ParserGenerator.Grammar
{
    /// <summary>
    /// Provides an interface for a parse table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParseTable<T> where T : IEquatable<T>
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
