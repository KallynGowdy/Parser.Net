using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Definitions;
using Parser.Grammar;
using LexicalAnalysis;
using Parser.Parsers;
using LexicalAnalysis.Definitions;
using System.Diagnostics;

namespace Parser.RegularExpressions
{
    /// <summary>
    /// Defines a Regular Expression.
    /// </summary>
    public sealed class RegularExpression
    {
        /// <summary>
        /// Gets the LR(1) grammar used to parse regular expression patterns.
        /// </summary>
        /// <returns></returns>
        public static ContextFreeGrammar<Token<string>> GetPatternGrammar()
        {
            return RegexBuilder.PatternGrammar.GetGrammar();
        }

        /// <summary>
        /// Gets the regular expression grammar used by this Regex object.
        /// </summary>
        public ContextFreeGrammar<string> GetGrammar()
        {
            return grammar.DeepCopy();
        }

        /// <summary>
        /// Gets the parser used to parse input based on the pattern.
        /// </summary>
        /// <returns></returns>
        public GLRParser<string> GetParser()
        {
            return new GLRParser<string>(parser.ParseTable.DeepCopy());
        }

        /// <summary>
        /// The grammar that was generated from the given regex pattern.
        /// </summary>
        private ContextFreeGrammar<string> grammar;

        /// <summary>
        /// The parser used to parse input that should match regular expressions.
        /// </summary>
        GLRParser<string> parser;

        /// <summary>
        /// Gets the regular expression pattern used by this Regex matcher.
        /// </summary>
        public string Pattern
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new regular expression based on the given pattern.
        /// </summary>
        /// <param name="pattern"></param>
        public RegularExpression(string pattern)
        {
            this.Pattern = pattern;

            this.grammar = (new RegexBuilder()).BuildGrammar(pattern);

            this.parser = new GLRParser<string>();
            parser.SetParseTable(grammar);
        }

        /// <summary>
        /// Determines if the given input string matches the regular expression.
        /// </summary>
        /// <param name="input">The input string to match against the regular expression.</param>
        /// <returns></returns>
        public bool IsMatch(string input)
        {
            //List<Terminal<string>> terminals = new List<Terminal<string>>();

            var result = parser.ParseAbstractSyntaxTrees(input.Select(a => new Terminal<string>(a.ToString())));

            return result.Any(a => a.Success);
        }
    }
}
