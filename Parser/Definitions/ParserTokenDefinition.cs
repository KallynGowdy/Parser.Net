using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LexicalAnalysis.Definitions;
using LexicalAnalysis;
using Parser.Grammar;


namespace Parser.Definitions
{
    /// <summary>
    /// Defines a relation between a TokenDefinition and a Terminal element.
    /// Specifically, the generated terminal's equality comparer is mapped to the TokenDefintions's TokenType property. 
    /// </summary>
    [Serializable]
    public abstract class ParserTokenDefinition<T> : TokenDefinition<T>, ITerminalMatch<T> where T : IEquatable<T>
    {

        /// <summary>
        /// Determines if the produced Token should be kept in the Abstract Syntax Tree.
        /// </summary>
        public bool Keep
        {
            get;
            set;
        }



        public abstract override Token<T> GetToken(Capture match);
        public abstract override Token<T> GetToken(int index, string tokenType, T value);

        public ParserTokenDefinition(Regex regex, string tokenType, bool keep)
            : base(regex, tokenType)
        {
            this.Keep = keep;
        }

        /// <summary>
        /// Gets a terminal object based on the given token object.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual Terminal<Token<T>> GetTerminal(Token<T> token)
        {
            return token.ToTerminal(Keep);
        }

        public abstract bool TerminalMatch(Terminal<T> terminal);
    }
}
