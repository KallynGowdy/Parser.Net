using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis
{
    /// <summary>
    /// Defines a keyword token.
    /// Equality of keywords are defined by their values, not by their token types(unlike normal tokens).
    /// </summary>
    [Serializable]
    public class KeywordToken : Token<string>, IEquatable<KeywordToken>
    {
        public KeywordToken(int index, string keyword)
            : base(index, TokenTypes.KEYWORD, keyword)
        {
            
        }


        /// <summary>
        /// Determines if this object is equal to the given other object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if(obj is KeywordToken)
            {
                return Equals((KeywordToken)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Determines if this object is equal to the given other keyword token.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(KeywordToken other)
        {
            if(other != null)
            {
                return this.Value.Equals(other.Value);
            }
            else
            {
                return this == other;
            }
        }

        /// <summary>
        /// Gets the hash code of this keyword token.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
