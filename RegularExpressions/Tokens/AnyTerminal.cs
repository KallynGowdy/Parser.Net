using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.RegularExpressions.Tokens
{
    /// <summary>
    /// Defines a terminal that equals any Terminal(of string) object.
    /// </summary>
    public class AnyTerminal<T> : Terminal<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Creates a new Terminal(of T) object that equals any ITerminal(of T) object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keep"></param>
        public AnyTerminal(T value, bool keep = true) : base(value, keep)
        {
            
        }

        /// <summary>
        /// Determines if this terminal object equals the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is Terminal<T>)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines if this terminal object equals the given terminal object.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public override bool Equals(ITerminal<T> terminal)
        {
            return true;
        }

        /// <summary>
        /// Determines if this terminal object equals the given terminal object.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public override bool Equals(Terminal<T> other)
        {
            return true;
        }

        public virtual bool Equals(GrammarElement<T> other)
        {
            return true;
        }

        public override string ToString()
        {
            return "AnyTerminal";
        }

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return unchecked(370248451 * typeof(T).GetHashCode());
        }
    }
}
