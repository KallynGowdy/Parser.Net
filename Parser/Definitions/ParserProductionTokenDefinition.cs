using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using LexicalAnalysis;
using LexicalAnalysis.Definitions;
using Parser.Grammar;

namespace Parser.Definitions
{
    /// <summary>
    /// Defines a relation between a production and a ParserTokenDefintionCollection. This is done in two parts, The Productions and the Definitions.
    /// The Productions define the productions of the language, were each terminal's value is the TokenType of the desired token.
    /// The Definitions define the different possible Tokens, and how find them(using regex). The tokenTypeToMatch of each definition matches to any terminal on the
    /// right hand side of a production with the same value.
    /// </summary>
    [DataContract]
    public class ParserProductionTokenDefinition<T> : IList<Production<string>> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the productions matching TokenTypes as terminals.
        /// </summary>
        [DataMember(Name="Productions")]
        public List<Production<string>> Productions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Defintions matching Tokens to Terminals.
        /// </summary>
        [DataMember(Name="Definitions")]
        public ParserTokenDefinitionCollection<T> Definitions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the TokenDefininitionCollection from this definition used for a lexer.
        /// </summary>
        /// <returns></returns>
        public TokenDefinitionCollection<T> GetLexerDefinitions()
        {
            return Definitions.GetNormalDefinitions();
        }

        /// <summary>
        /// Gets the context free grammar that defines this definition for a parser.
        /// </summary>
        /// <returns></returns>
        public ContextFreeGrammar<Token<T>> GetGrammar()
        {
            var productions = GetProductions();

            return new ContextFreeGrammar<Token<T>>(productions.First().NonTerminal, new Terminal<Token<T>>(null, false), productions);
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
                Production<Token<T>> newP = new Production<Token<T>>(p.NonTerminal.Name.ToNonTerminal<Token<T>>(p.NonTerminal.Keep));
                //create a new terminal or non-terminal for each production
                foreach (var e in p.DerivedElements)
                {
                    //if e is a terminal create a new Terminal<Token<T>> for it
                    if (e is Terminal<T>)
                    {
                        //get the first defintion whose token type matches the value of the current terminal
                        ParserTokenDefinition<T> def = Definitions.First(a => a.TerminalMatch((Terminal<T>)(object)e));
                        newP.DerivedElements.Add(new Terminal<Token<T>>(new Token<T>(0, e.InnerValue, default(T)), def.Keep));
                    }
                    else
                    {
                        newP.DerivedElements.Add(((NonTerminal<string>)e).Name.ToNonTerminal<Token<T>>(e.Keep));
                    }
                }
                productions.Add(newP);
            }

            return productions.ToArray();
        }

        /// <summary>
        /// Converts the given collection of input tokens into terminals that can be parsed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public IEnumerable<Terminal<Token<T>>> ConvertToTerminals(IEnumerable<Token<T>> input)
        {
            List<Terminal<Token<T>>> tokens = new List<Terminal<Token<T>>>(input.Count());
            foreach (Token<T> token in input)
            {
                ParserTokenDefinition<T> def = Definitions.GetDefinition(token);
                if (def != null)
                {
                    tokens.Add(def.GetTerminal(token));
                }
            }
            return tokens;
        }

        public int IndexOf(Production<string> item)
        {
            return Productions.IndexOf(item);
        }

        public void Insert(int index, Production<string> item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Production<string> this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(Production<string> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Production<string> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Production<string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool Remove(Production<string> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Production<string>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
