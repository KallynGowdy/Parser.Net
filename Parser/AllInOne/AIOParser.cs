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
    public class AIOLRParser<T> : IParser<Token<T>>, IAllInOneParser<T> where T : IEquatable<T>
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
                if (Definitions == null && parser == null)
                {
                    throw new InvalidOperationException("Defintions must be set before trying to get a Parser from it.");
                }
                if (parser == null)
                {
                    
                    ContextFreeGrammar<Token<T>> cfg = Definitions.GetGrammar();
                    parser = new LRParser<Token<T>>();
                    parser.SetParseTable(cfg);
                }
                return parser;
            }
        }

        /// <summary>
        /// Sets the parse table used to parse the input.
        /// </summary>
        /// <param name="table"></param>
        public void SetParseTable(LRParseTable<Token<T>> table)
        {
            parser = new LRParser<Token<T>>();
            parser.ParseTable = table;
        }

        /// <summary>
        /// Parses an abstract sentax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseResult<Token<T>> ParseAST(IEnumerable<Token<T>> input)
        {
            if (Definitions == null)
            {
                throw new InvalidOperationException("Definitions must be set before calling ParseAST");
            }

            return Parser.ParseAST(Definitions.ConvertToTerminals(input));
        }

        /// <summary>
        /// Parses a concrete sentax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseResult<Token<T>> ParseSentaxTree(IEnumerable<Token<T>> input)
        {
            if (Definitions == null)
            {
                throw new InvalidOperationException("Definitions must be set before calling ParseAST");
            }

            return Parser.ParseSentaxTree(Definitions.ConvertToTerminals(input));
        }

        /// <summary>
        /// Parses an Abstract Sentax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <returns></returns>
        public ParseResult<Token<T>> ParseAST(IEnumerable<Terminal<Token<T>>> input)
        {
            if (Definitions == null)
            {
                throw new InvalidOperationException("Definitions must be set before calling ParseAST");
            }
            return Parser.ParseAST(input);
        }

        /// <summary>
        /// Parses a Concrete Sentax Tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <returns></returns>
        public ParseResult<Token<T>> ParseSentaxTree(IEnumerable<Terminal<Token<T>>> input)
        {
            if (Definitions == null)
            {
                throw new InvalidOperationException("Definitions must be set before calling ParseSentaxTree");
            }
            return Parser.ParseSentaxTree(input);
        }
    }
}
