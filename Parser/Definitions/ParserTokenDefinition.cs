using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LexicalAnalysis.Defininitions;
using LexicalAnalysis;
using Parser.Grammar;


namespace Parser.Definitions
{
    /// <summary>
    /// Defines a relation between a TokenDefinition and a Terminal element.
    /// Specifically, the generated terminal's equality comparer is mapped to the TokenDefintions's TokenType property. 
    /// </summary>
    [Serializable]
    public abstract class ParserTokenDefinition<T> : TokenDefinition<T>
    {

        /// <summary>
        /// Determines if the produced Token should be kept in the Abstract Sentax Tree.
        /// </summary>
        public bool Keep
        {
            get;
            set;
        }

        public abstract override Token<T> GetToken(Match match);

        public ParserTokenDefinition(Regex regex, string tokenType, bool keep)
            : base(regex, tokenType)
        {
            this.Keep = keep;
        }

        /// <summary>
        /// Gets the terminal that represents this definition that can be used in a CFG.
        /// </summary>
        /// <returns></returns>
        public virtual Terminal<Token<T>> GetTerminal()
        {
            return (new Token<T>(0, TokenTypeToMatch, default(T))).ToTerminal(Keep, a => a != null && this.TokenTypeToMatch.Equals(a.TokenType));
        }

        public virtual Terminal<Token<T>> GetTerminal(T value)
        {
            return (new Token<T>(0, TokenTypeToMatch, value)).ToTerminal(Keep, a => a != null && this.TokenTypeToMatch.Equals(a.TokenType));
        }
    }
}
