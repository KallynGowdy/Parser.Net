using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using Parser.Grammar;

namespace Parser.Definitions
{
    /// <summary>
    /// Defines a relation between a production and a ParserTokenDefintionCollection.
    /// </summary>
    [Serializable]
    public class ParserProductionTokenDefinition<T>
    {
        /// <summary>
        /// Gets or sets the productions matching TokenTypes as terminals.
        /// </summary>
        public List<Production<string>> Productions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Defintions matching Tokens to Terminals.
        /// </summary>
        public ParserTokenDefintionCollection<T> Definitions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets an array of productions derived from the Productions and Defintions stored in the class.
        /// </summary>
        /// <returns></returns>
        public Production<Token<T>>[] GetProductions()
        {
            List<Production<Token<T>>> productions = new List<Production<Token<T>>>();
            //add a new production for each production in the class
            foreach (var p in Productions)
            {
                Production<Token<T>> newP = new Production<Token<T>>(p.NonTerminal.Name.ToNonTerminal<Token<T>>());
                //create a new terminal or non-terminal for each production
                foreach (var e in p.DerivedElements)
                {
                    //if e is a terminal create a new Terminal<Token<T>> for it
                    if (e is Terminal<T>)
                    {
                        //get the first defintion whose token type matches the value of the current terminal
                        ParserTokenDefinition<T> def = Definitions.First(a => a.TokenTypeToMatch.Equals(e.InnerValue));
                        newP.DerivedElements.Add(new Terminal<Token<T>>(new Token<T>(0, e.InnerValue, default(T)), def.Keep, a => a != null && def.TokenTypeToMatch.Equals(a.TokenType)));
                    }
                    else
                    {
                        newP.DerivedElements.Add(((NonTerminal<string>)e).Name.ToNonTerminal<Token<T>>());
                    }
                }
                productions.Add(newP);
            }

            return productions.ToArray();
        }
    }
}
