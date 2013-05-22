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

        public Terminal(T value, bool keep = true, Predicate<T> equalityOperator = null)
            : base(value)
        {
            this.Keep = keep;
            if (equalityOperator == null)
            {
                this.EqualityOperator = a => this.InnerValue.Equals(a);
            }
            else
            {
                this.EqualityOperator = equalityOperator;
            }
        }

        /// <summary>
        /// Gets or sets the equality operator that determines if this terminal equals another terminal.
        /// </summary>
        public Predicate<T> EqualityOperator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this Terminal element should be kept or discarded when building an abstract sentax tree.
        /// </summary>
        public bool Keep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a unique interger value that describes this object that is garenteed not to change.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public static bool operator ==(Terminal<T> left, Terminal<T> right)
        {
            if (((object)left) == null || ((object)right) == null)
            {
                return ((object)left) == ((object)right);
            }
            else
            {
                return left.Equals(right);
            }
        }

        public static bool operator !=(Terminal<T> left, Terminal<T> right)
        {
            if (((object)left) == null || ((object)right) == null)
            {
                return ((object)left) != ((object)right);
            }
            else
            {
                return !left.Equals(right);
            }
        }


        /// <summary>
        /// Determines if this terminal is equal to the given other terminal.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public bool Equals(Terminal<T> terminal)
        {
            if (terminal != null)
            {
                return this.EqualityOperator(terminal.InnerValue);
            }
            else
            {
                return (object)this.InnerValue == (object)terminal;
            }
        }

        /// <summary>
        /// Determines if this terminal object is equal to the given other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Terminal<T> || obj == null)
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
            if (this.InnerValue != null)
            {
                return InnerValue.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
