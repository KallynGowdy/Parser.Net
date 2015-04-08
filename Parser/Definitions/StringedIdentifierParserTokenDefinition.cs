using System.Text.RegularExpressions;
using LexicalAnalysis;

namespace KallynGowdy.ParserGenerator.Definitions
{
    /// <summary>
    /// Defines a definition that allows use of KeywordIdentifierTokens with regex patterns.
    /// </summary>
    public class StringedIdentifierParserTokenDefinition : StringedParserTokenDefinition
    {
        public StringedIdentifierParserTokenDefinition(string regex, string tokenType, bool keep)
            : base(regex, tokenType, keep)
        {

        }

        public StringedIdentifierParserTokenDefinition(Regex regex, string tokenType, bool keep)
            : base(regex, tokenType, keep)
        {

        }

        /// <summary>
        /// Gets a token that matches the definition contained in this object.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public override LexicalAnalysis.Token<string> GetToken(Capture match)
        {
            return new KeywordIdentifierToken(match.Index, match.Value, this.TokenTypeToMatch);
        }

        /// <summary>
        /// Determines if the given terminal matches this definition.
        /// </summary>
        /// <param name="terminal"></param>
        /// <returns></returns>
        public override bool TerminalMatch(Grammar.Terminal<string> terminal)
        {
            if(terminal != null)
            {
                return this.Regex.IsMatch(terminal.InnerValue);
            }
            return false;
        }
    }
}
