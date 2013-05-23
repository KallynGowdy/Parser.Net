
using System;
namespace LexicalAnalysis
{
    /// <summary>
    /// Defines a token at a certian location with a given value and type.
    /// </summary>
    /// <typeparam name="T">The type of the value of the Token</typeparam>
    [Serializable]
    public class Token<T> : ITokenElement
    {
        /// <summary>
        /// A generic token identifier(i.e. KEYWORD, OPERATOR, ect.)
        /// </summary>
        public string TokenType
        {
            get;
            set;
        }

        /// <summary>
        /// The value contained by this token.
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// The index that this element starts at.
        /// </summary>
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
    }
}
