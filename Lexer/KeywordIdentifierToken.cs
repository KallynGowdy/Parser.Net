using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis
{
    /// <summary>
    /// Defines a keyword whose token type is the configured token type.
    /// Equality is first determined by keyword, and then by token type.
    /// </summary>
    public class KeywordIdentifierToken : KeywordToken, IEquatable<KeywordIdentifierToken>
    {

        public KeywordIdentifierToken(int index, string keyword, string tokenType = TokenTypes.IDENTIFIER)
            : base(index, keyword, tokenType)
        {

        }

        /// <summary>
        /// Determines if this object equals the given other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is KeywordIdentifierToken)
            {
                return Equals((KeywordIdentifierToken)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if this keyword identifier token equals the given other token.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(KeywordIdentifierToken other)
        {
            if (other != null)
            {
                if (other.Value == this.Value)
                {
                    return true;
                }
                else if (other.TokenType == this.TokenType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the hash code of this token.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (Value != null)
            {
                return (Value.GetHashCode());
            }
            else
            {
                return (TokenType.GetHashCode());
            }
        }
    }
}
