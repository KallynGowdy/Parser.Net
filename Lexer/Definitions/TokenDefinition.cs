using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LexicalAnalysis.Definitions
{
    /// <summary>
    /// Defines a serializable relation between a regex pattern and a token.
    /// Defines a grammar production of the form TokenTypeToMatch -> RegexPattern.
    /// </summary>
    [Serializable]
    public abstract class TokenDefinition<T>
    {
        /// <summary>
        /// The regular expression describing the input pattern.
        /// </summary>
        public Regex Regex
        {
            get;
            set;
        }

        /// <summary>
        /// The type describing the token to match to.
        /// </summary>
        public string TokenTypeToMatch
        {
            get;
            set;
        }

        /// <summary>
        /// The order that this definition should be at.
        /// </summary>
        [Obsolete("Not used at all")]
        public int Precedence
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the coresponding Token for the given match.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public abstract Token<T> GetToken(Capture match); 

        protected TokenDefinition(Regex regex, string typeToMatch)
        {
            this.Regex = regex;
            this.TokenTypeToMatch = typeToMatch;
        }

        /// <summary>
        /// Creates a new TokenDefinition(T) object based on the given pattern and typeToMatch.
        /// Creates a new Regex object from the given pattern with RegexOptions.Compiled, RegexOptions.ExplicitCapture, and RegexOptions.Multiline as options.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="typeToMatch"></param>
        protected TokenDefinition(string pattern, string typeToMatch)
        {
            //build a new regex object from the given pattern, define 
            this.Regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Multiline);
            this.TokenTypeToMatch = typeToMatch;
        }

        protected TokenDefinition()
        {
            this.Regex = null;
            this.TokenTypeToMatch = string.Empty;
        }
    }
}
