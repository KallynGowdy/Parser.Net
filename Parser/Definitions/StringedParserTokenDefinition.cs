using System.Text.RegularExpressions;
using LexicalAnalysis;

namespace KallynGowdy.ParserGenerator.Definitions
{
    /// <summary>
    /// Defines a parser token defintion that contains a string.
    /// </summary>
    public class StringedParserTokenDefinition : ParserTokenDefinition<string>
    {
        public override Token<string> GetToken(Capture match)
        {
            return new Token<string>(match.Index, this.TokenTypeToMatch, match.Value);
        }

        public StringedParserTokenDefinition(Regex regex, string tokenType, bool keep)
            : base(regex, tokenType, keep)
        { }

        public StringedParserTokenDefinition(string regex, string tokenType, bool keep)
            : base(new Regex(regex), tokenType, keep)
        { }

        public override bool TerminalMatch(Grammar.Terminal<string> terminal)
        {
            if(terminal != null && terminal.InnerValue != null)
            {
                return this.TokenTypeToMatch.Equals(terminal.InnerValue);
            }
            return false;
        }

        public override Token<string> GetToken(int index, string tokenType, string value)
        {
            return new Token<string>(index, this.TokenTypeToMatch, value);
        }
    }
}
