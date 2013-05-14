using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser.Grammar
{
    /// <summary>
    /// Defines a value that cannot be derived into a further 'child' element.
    /// </summary>
    [Serializable]
    public class Terminal<T> : GrammarElement<T>
    {

        public Terminal(T value)
            : base(value)
        {
        }


        /// <summary>
        /// Gets a unique interger value that describes this object that is garenteed not to change.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        /// <summary>
        /// Determines if this terminal is equal to the given other terminal.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public bool Equals(Terminal<T> terminal)
        {
            return this.InnerValue.Equals(terminal.InnerValue);
        }

        /// <summary>
        /// Determines if this terminal object is equal to the given other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Terminal<T>)
            {
                return Equals((Terminal<T>)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Returns this object as a string represented by the inner value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return InnerValue.ToString();
        }
    }
}
