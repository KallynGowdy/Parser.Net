﻿using System;
using System.Text.RegularExpressions;

namespace LexicalAnalysis.Definitions
{
    /// <summary>
    /// A TokenDefinition(string) that provides support for GetToken(Match)
    /// </summary>
    [Serializable]
    public class StringedTokenDefinition : TokenDefinition<string>
    {
        public override Token<string> GetToken(System.Text.RegularExpressions.Capture match)
        {
            return new Token<string>(match.Index, this.TokenTypeToMatch, match.Value);
        }

        public StringedTokenDefinition(Regex pattern, string typeToMatch)
            : base(pattern, typeToMatch)
        {
        }

        public StringedTokenDefinition(string pattern, string typeToMatch)
            : base(pattern, typeToMatch)
        {
        }

        public StringedTokenDefinition()
            : base()
        {

        }

        public override Token<string> GetToken(int index, string tokenType, string value)
        {
            return new Token<string>(index, this.TokenTypeToMatch, value);
        }
    }
}
