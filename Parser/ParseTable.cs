using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.Grammar;
using Parser;

namespace Parser.StateMachine
{
    /// <summary>
    /// Defines a parse table.
    /// </summary>
    public class ParseTable<T>
    {
        public Table<int, Terminal<T>, ParseAction<T>> ActionTable;

        public Table<int, NonTerminal<T>, int> GotoTable;

    }
}
