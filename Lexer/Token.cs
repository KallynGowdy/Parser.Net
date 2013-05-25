
using System;
using System.Runtime.Serialization;
namespace LexicalAnalysis
{
    /// <summary>
    /// Defines a token at a certian location with a given value and type.
    /// </summary>
    /// <typeparam name="T">The type of the value of the Token</typeparam>
    [DataContract(IsReference=true)]
    public class Token<T> : ITokenElement, IEquatable<Token<T>>
    {
        /// <summary>
        /// A generic token identifier(i.e. KEYWORD, OPERATOR, ect.)
        /// </summary>
        [DataMember]
        public string TokenType
        {
            get;
            set;
        }

        /// <summary>
        /// The value contained by this token.
        /// </summary>
        [DataMember]
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// The index that this element starts at.
        /// </summary>
        [DataMember]
        public int Index
        {
            get;
            set;
        }

        public Token(int index, string tokenType, T value)
        {
            this.Index = index;
            this.TokenType = tokenType;
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return TokenType.GetHashCode();
        }

        public Token()
        {
        }

        public override string ToString()
        {
            return TokenType;
        }

        public bool Equals(Token<T> other)
        {
            if (other != null)
            {
                return this.GetHashCode() == other.GetHashCode();
            }
            return false;
        }
    }
}
