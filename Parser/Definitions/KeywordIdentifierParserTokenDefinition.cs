using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;

namespace Parser.Definitions
{
    /// <summary>
    /// Provides a definition to match Terminal objects KeywordIdentifierTokens(Identifiers that is certian contexts are keywords).
    /// </summary>
    public class KeywordIdentifierParserTokenDefinition : KeywordParserTokenDefinition
    {
        public KeywordIdentifierParserTokenDefinition(string keyword, bool keep, string tokenTypeToMatch = TokenTypes.IDENTIFIER)
            : base(keyword, keep, tokenTypeToMatch)
        {

        }

        /// <summary>
        /// Gets the token from this definition filled with values from the given Capture object.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public override Token<string> GetToken(System.Text.RegularExpressions.Capture match)
        {
            return new KeywordIdentifierToken(match.Index, match.Value, TokenTypeToMatch);
        }

        /// <summary>
        /// Gets a KeywordIdentifierToken from this definition filled with the given values.
        /// </summary>
        /// <param name="index">The index of the token.</param>
        /// <param name="tokenType">Ignored. The token type to match from this definition is used instead.</param>
        /// <param name="value">The value of the token.</param>
        /// <returns></returns>
        public override Token<string> GetToken(int index, string tokenType, string value)
        {
            return new KeywordIdentifierToken(index, tokenType, this.TokenTypeToMatch);
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
                bool val = this.Keyword.Equals(terminal.InnerValue);// || this.TokenTypeToMatch.Equals(terminal.InnerValue);
                return val;
            }
            return false;
        }
    }
}
