using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.RegularExpressions.Tokens
{
    using Parser;

    /// <summary>
    /// Defines a terminal that equals any terminal but the given list of terminals.
    /// </summary>
    public class AnyButTerminal<T> : AnyTerminal<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the terminal elements that will not be matched.
        /// </summary>
        public ReadOnlyCollection<ITerminal<T>> ExcludedTerminals
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new Terminal(of T) object that equals any terminal but the given list of ITerminal(of T) objects.
        /// </summary>
        /// <param name="excludedTerminals">The ITerminal objects that this object should not equal.</param>
        /// <param name="keep">Whether to keep this Terminal object in the parse tree.</param>
        public AnyButTerminal(IEnumerable<ITerminal<T>> excludedTerminals, bool keep = true) : base(default(T), keep)
        {
            this.ExcludedTerminals = new ReadOnlyCollection<ITerminal<T>>(excludedTerminals.ToList());
        }

        /// <summary>
        /// Creates a new Terminal(of T) object that equals any terminal but the given list of ITerminal(of T) objects.
        /// </summary>
        /// <param name="excludedTerminals">The ITerminal objects that this object should not equal.</param>
        /// <param name="keep">Whether to keep this Terminal object in the parse tree.</param>
        public AnyButTerminal(IEnumerable<Terminal<T>> excludedTerminals, bool keep = true)
            : base(default(T), keep)
        {
            this.ExcludedTerminals = new ReadOnlyCollection<ITerminal<T>>(excludedTerminals.Cast<ITerminal<T>>().ToList());
        }

        /// <summary>
        /// Determines if the given terminal element equals this terminal based on the excluded terminals.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public override bool Equals(ITerminal<T> terminal)
        {
            return !ExcludedTerminals.Contains(terminal);
        }

        /// <summary>
        /// Determines if the given object equals this object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is ITerminal<T>)
            {
                return Equals((ITerminal<T>)obj);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if the given terminal equals this terminal based on the excluded terminals.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Terminal<T> other)
        {
            return !ExcludedTerminals.Contains(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("AnyBut({0})", ExcludedTerminals.ConcatStringArray(", "));
        }
    }
}
