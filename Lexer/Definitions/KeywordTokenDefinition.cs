using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LexicalAnalysis.Definitions;

namespace LexicalAnalysis.Definitions
{
    public class KeywordTokenDefinition : StringedTokenDefinition
    {
        /// <summary>
        /// Gets the keyword stored in this definition.
        /// </summary>
        public string Keyword
        {
            get;
            private set;
        }

        public KeywordTokenDefinition(string keyword)
            : base(BuildKeyword(keyword), TokenTypes.KEYWORD)
        {
            this.Keyword = keyword;
        }

        public KeywordTokenDefinition(string keyword, string tokenType) : base(BuildKeyword(keyword), tokenType)
        {
            this.Keyword = keyword;
        }

        /// <summary>
        /// Gets a new Keyword token based on the given match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public override Token<string> GetToken(Capture match)
        {
            return new KeywordToken(match.Index, match.Value, TokenTypeToMatch);
        }

        /// <summary>
        /// Gets a new keyword token based on the given index and value.
        /// tokenType is ignored.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tokenType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Token<string> GetToken(int index, string tokenType, string value)
        {
            return new KeywordToken(index, value, TokenTypeToMatch);
        }

        /// <summary>
        /// Creates a new Regex object to match the given keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static Regex BuildKeyword(string keyword)
        {
            return new Regex(string.Format(@"\b{0}\b", keyword));
        }
    }
}
