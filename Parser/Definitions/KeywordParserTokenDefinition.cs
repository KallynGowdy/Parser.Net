using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis.Definitions;
using LexicalAnalysis;

namespace Parser.Definitions
{
    public class KeywordParserTokenDefinition : StringedParserTokenDefinition
    {
        /// <summary>
        /// Gets the keyword used in this definition.
        /// </summary>
        public string Keyword
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Keyword token from this keyword definition.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public override LexicalAnalysis.Token<string> GetToken(System.Text.RegularExpressions.Capture match)
        {
            return new KeywordToken(match.Index, match.Value);
        }

        public KeywordParserTokenDefinition(string keyword, bool keep)
            : base(BuildKeyword(keyword), TokenTypes.KEYWORD, keep)
        {
            this.Keyword = keyword;
        }

        /// <summary>
        /// Creates a new Regex object that matches the given keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static System.Text.RegularExpressions.Regex BuildKeyword(string keyword)
        {
            return new System.Text.RegularExpressions.Regex(string.Format(@"\b{0}\b", keyword));
        }

        /// <summary>
        /// Determines if the given terminal matches the token defined in this definition.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public override bool TerminalMatch(Grammar.Terminal<string> terminal)
        {
            if(terminal != null && terminal.InnerValue != null)
            {
                return Keyword.Equals(terminal.InnerValue);
            }
            return false;
        }
    }


}
