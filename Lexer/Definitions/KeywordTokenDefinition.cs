using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LexicalAnalysis.Defininitions;

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

        public override Token<string> GetToken(Capture match)
        {
            return new KeywordToken(match.Index, match.Value);
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
