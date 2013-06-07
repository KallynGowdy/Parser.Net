using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LexicalAnalysis.Definitions
{
    /// <summary>
    /// Defines a KeywordIdentifierTokenDefinition that maps a regular expression match to a 
    /// </summary>
    public class KeywordIdentifierTokenDefinition : KeywordTokenDefinition
    {
        /// <summary>
        /// Creates a new KeywordIdentifierTokenDefinition
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="tokenType"></param>
        public KeywordIdentifierTokenDefinition(string keyword, string tokenType = TokenTypes.IDENTIFIER) : base(keyword, tokenType)
        {

        }

        /// <summary>
        /// Gets the token that is matched to this definition with the values contained in the given Capture object.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public override Token<string> GetToken(System.Text.RegularExpressions.Capture match)
        {
            return new KeywordIdentifierToken(match.Index, match.Value, this.TokenTypeToMatch);
        }

        public override Token<string> GetToken(int index, string tokenType, string value)
        {
            return new KeywordIdentifierToken(index, value, this.TokenTypeToMatch);
        }
    }
}
