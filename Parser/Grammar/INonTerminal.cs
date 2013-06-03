using System;
namespace Parser.Grammar
{
    /// <summary>
    /// Provides an interface for non terminal elements, Equals(and therefore GetHashCode) is required to be implemented to provide a strong/consistant equality interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface INonTerminal<T> : IEquatable<INonTerminal<T>>, IGrammarElement<T>
    {
        bool Equals(object obj);

        int GetHashCode();

        string Name
        {
            get;
            set;
        }
    }
}
