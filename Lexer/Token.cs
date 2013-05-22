
namespace LexicalAnalysis
{
    /// <summary>
    /// Defines a token at a certian location with a given value and type.
    /// </summary>
    /// <typeparam name="T">The type of the value of the Token</typeparam>
    public class Token<T> : ITokenElement
    {
        /// <summary>
        /// A generic token identifier(i.e. KEYWORD, OPERATOR, ect.)
        /// </summary>
        public string TokenType
        {
            get;
            private set;
        }

        /// <summary>
        /// The value contained by this token.
        /// </summary>
        public T Value
        {
            get;
            private set;
        }

        /// <summary>
        /// The index that this element starts at.
        /// </summary>
        public int Index
        {
            get;
            private set;
        }

        public Token(int index, string tokenType, T value)
        {
            this.Index = index;
            this.TokenType = tokenType;
            this.Value = value;
        }

        public override string ToString()
        {
            return TokenType;
        }
    }
}
