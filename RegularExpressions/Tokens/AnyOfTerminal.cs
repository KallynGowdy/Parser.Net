using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.RegularExpressions.Tokens
{
    /// <summary>
    /// Defines a terminal element that equals any of the given terminal elements.
    /// </summary>
    public class AnyOfTerminal<T> : AnyTerminal<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the terminal elements that this object matches. 
        /// </summary>
        public IEnumerable<ITerminal<T>> MatchedTerminals
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Creates a new Terminal object that equals any of the given matched terminals.
        /// </summary>
        /// <param name="matchedTerminals"></param>
        /// <param name="keep"></param>
        public AnyOfTerminal(IEnumerable<ITerminal<T>> matchedTerminals, bool keep = true) : base(default(T), keep)
        {

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
            return false;        
        }

        /// <summary>
        /// Determines if the given object equals this object.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public override bool Equals(ITerminal<T> terminal)
        {
            return MatchedTerminals.Contains(terminal);
        }

        /// <summary>
        /// Determines if the given object equals this object.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(Terminal<T> other)
        {
            return MatchedTerminals.Contains(other);
        }
    }
}
