using System;
namespace Parser.Grammar
{
    /// <summary>
    /// Interface for Terminal elements, Equals(and therefore GetHashCode) is required to be overriden to provide a strong equality interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITerminal<T> : IGrammarElement<T>
     where T : IEquatable<T>
    {
        bool Equals(ITerminal<T> terminal);
        bool Equals(object obj);
        int GetHashCode();
    }
}
