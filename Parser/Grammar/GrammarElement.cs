using System;
using System.Runtime.Serialization;
using KallynGowdy.ParserGenerator.Collections;

namespace KallynGowdy.ParserGenerator.Grammar
{
    /// <summary>
    /// Defines a element of grammar that is used in productions.
    /// This class is abstract.
    /// </summary>
    [DataContract(Name = "GrammarElement")]
    [Serializable]
    public abstract class GrammarElement<T> : IGrammarElement<T>, IMultiHashedObject
    {
        /// <summary>
        /// Gets the Value stored inside this GrammarElement.
        /// </summary>
        [DataMember(Name = "InnerValue")]
        public T InnerValue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether this element should be kept or discarded.
        /// </summary>
        [DataMember(Name = "Keep")]
        public bool Keep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to match this element or anything but this element. This value does not affect equality, therefore it should be evaluated separately.
        /// </summary>
        [DataMember(Name = "Negated")]
        public bool Negated
        {
            get;
            set;
        }

        protected GrammarElement(bool keep = true)
        {
            this.InnerValue = default(T);
            this.Keep = keep;
        }

        protected GrammarElement(T value)
        {
            this.InnerValue = value;
        }

        protected GrammarElement(GrammarElement<T> other)
        {
            this.InnerValue = other.InnerValue;
        }

        public override bool Equals(object obj)
        {
            return (obj.GetHashCode() == GetHashCode());
        }

        public override int GetHashCode()
        {
            if (InnerValue != null)
            {
                return InnerValue.GetHashCode();
            }
            return unchecked(521 * typeof(T).GetHashCode());
        }

        public virtual int[] GetHashCodes()
        {
            return new int[] { GetHashCode() };
        }
    }
}
