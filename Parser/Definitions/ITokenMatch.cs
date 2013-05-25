using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.Definitions
{
    /// <summary>
    /// Defines an interface for objects that support matching themselves to terminals
    /// </summary>
    public interface ITerminalMatch<T> where T : IEquatable<T>
    {
        bool TerminalMatch(Terminal<T> terminal);
    }
}
