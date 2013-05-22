using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using Parser.Definitions;
using Parser.Grammar;
using Parser.Parsers;
using Parser.StateMachine;

namespace Parser.AllInOne
{
    /// <summary>
    /// Defines an All-in-One LR parser, that provided a collection of ParserTokenDefinitions can successfully parse a given string of input characters.
    /// </summary>
    public class AIOLRParser<T> : IParser<Token<T>>
    {

        /// <summary>
        /// Gets or sets the defintions used to parse a given input string.
        /// </summary>
        public ParserProductionTokenDefinition<T> Definitions
        {
            get;
            set;
        }

        private LRParser<Token<T>> parser;

        /// <summary>
        /// Gets the parser used to parse input.
        /// </summary>
        /// <exception cref="System.InvalidOperationException"/>
        public LRParser<Token<T>> Parser
        {
            get
            {
                if (Definitions == null)
                {
                    throw new InvalidOperationException("Defintions must be set before trying to get a Parser from it.");
                }
                if (parser == null)
                {
                    Production<Token<T>>[] productions = Definitions.GetProductions();
                    ContextFreeGrammar<Token<T>> cfg = new ContextFreeGrammar<Token<T>>(productions.First().NonTerminal, new Terminal<Token<T>>(null, false, a => a == null), productions);
                    parser = new LRParser<Token<T>>
                    {
                        ParseTable = new LRParseTable<Token<T>>(cfg)
                    };
                }
                return parser;
            }
        }

        /// <summary>
        /// Parses an Abstract Sentax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <returns></returns>
        public ParseTree<Token<T>> ParseAST(IEnumerable<Terminal<Token<T>>> input)
        {
            if (Definitions == null)
            {
                throw new InvalidOperationException("Definitions must be set before calling ParseAST"); 
            }
            return Parser.ParseAST(input);
        }

        /// <summary>
        /// Parses a Sentaxt Tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <returns></returns>
        public ParseTree<Token<T>> ParseSentaxTree(IEnumerable<Terminal<Token<T>>> input)
        {
            if (Definitions == null)
            {
                throw new InvalidOperationException("Definitions must be set before calling ParseSentaxTree");
            }
            return Parser.ParseSentaxTree(input);
        }
    }
}
