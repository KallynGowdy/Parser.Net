using LexicalAnalysis;

namespace KallynGowdy.ParserGenerator.Definitions
{
    /// <summary>
    /// Defines a relationship between a keyword and a ParserTokenDefinition.
    /// The Token type of the object is TokenTypes.KEYWORD.
    /// </summary>
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
        public override Token<string> GetToken(System.Text.RegularExpressions.Capture match)
        {
            return new KeywordToken(match.Index, match.Value);
        }

        public override Token<string> GetToken(int index, string tokenType, string value)
        {
            return new KeywordToken(index, value);
        }

        /// <summary>
        /// Creates a new Keyword Definition object that defines a relationship between the given keyword and a ParserTokenDefinition.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="keep"></param>
        public KeywordParserTokenDefinition(string keyword, bool keep)
            : base(BuildKeywordRegex(keyword), TokenTypes.KEYWORD, keep)
        {
            this.Keyword = keyword;
        }

        /// <summary>
        /// Creates a new Keyword Definition object that defines a relationship between the given keyword and a ParserTokenDefinition.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="keep"></param>
        public KeywordParserTokenDefinition(string keyword, bool keep, string tokenTypeToMatch)
            : base(BuildKeywordRegex(keyword), tokenTypeToMatch, keep)
        {
            this.Keyword = keyword;
        }

        /// <summary>
        /// Creates a new Regex object that matches the given keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static System.Text.RegularExpressions.Regex BuildKeywordRegex(string keyword)
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
