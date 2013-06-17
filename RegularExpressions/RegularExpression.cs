using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Definitions;
using Parser.Grammar;
using LexicalAnalysis;
using Parser.Parsers;
using LexicalAnalysis.Definitions;
using Parser.RegularExpressions.Tokens;

namespace Parser.RegularExpressions
{
    /// <summary>
    /// Defines a Regular Expression.
    /// </summary>
    public sealed class RegularExpression
    {

        #region Pattern Grammar
        /// <summary>
        /// The grammar used for a LR(1) parser that parses regular expression grammars.
        /// </summary>
        public static ParserProductionTokenDefinition<string> PatternGrammar
        {
            get
            {
                return new ParserProductionTokenDefinition<string>
                (
                    new ParserTokenDefinitionCollection<string>
                    (
                        new[]
                        {
                            //defines a period. In regex it is matched as any character
                            new StringedParserTokenDefinition(@"\.", ".", true),

                            //defines '(' and ')'
                            new StringedParserTokenDefinition(@"\(", "(", true),
                            new StringedParserTokenDefinition(@"\)", ")", true),

                            //defines '[' and ']'
                            new StringedParserTokenDefinition(@"\[", "[", true),
                            new StringedParserTokenDefinition(@"\]", "]", true),

                            //defines '+'
                            new StringedParserTokenDefinition(@"\+", "+", true),

                            //defines '*'
                            new StringedParserTokenDefinition(@"\*", "*", true),

                            //defines ','
                            new StringedParserTokenDefinition(@"\,", ",", true),

                            //defines '\'
                            new StringedParserTokenDefinition(@"\\", @"\", true),

                            //defines '{' and '}'
                            new StringedParserTokenDefinition(@"\{", "{", true),
                            new StringedParserTokenDefinition(@"\}", "}", true),

                            //defines '|'
                            new StringedParserTokenDefinition(@"\|", "|", true),

                            //defines '?'
                            new StringedParserTokenDefinition(@"\?", "?", true),

                            //defines '^'
                            new StringedParserTokenDefinition(@"\^", "^", true),

                            //defines a literal as anything single character that is not a space
                            new StringedParserTokenDefinition(@"[^\s]", "Literal", true),
                        }
                    ),
                    new[]
                    {
                        //Regex -> TermLst '|' Regex
                        new Production<string>("Regex".ToNonTerminal(), "TermLst".ToNonTerminal(), "|".ToTerminal(), "Regex".ToNonTerminal()),

                        //Regex -> TermLst
                        new Production<string>("Regex".ToNonTerminal(), "TermLst".ToNonTerminal()),

                        //TermLst -> nothing
                        //new Production<string>("TermLst".ToNonTerminal()),

                        //TermLst -> Term
                        new Production<string>("TermLst".ToNonTerminal(), "Term".ToNonTerminal()),

                        //TermLst -> TermLst Term
                        new Production<string>("TermLst".ToNonTerminal(), "TermLst".ToNonTerminal(), "Term".ToNonTerminal()),

                        //Term -> Factor
                        new Production<string>("Term".ToNonTerminal(), "Factor".ToNonTerminal()),

                        #region Factor

                        //Factor -> Base Modifier
                        new Production<string>("Factor".ToNonTerminal(), "Base".ToNonTerminal(), "Modifier".ToNonTerminal()),

                        //Factor -> Base
                        new Production<string>("Factor".ToNonTerminal(), "Base".ToNonTerminal()),

                        #endregion

                        #region Modifier

                        //Modifier -> '*'
                        new Production<string>("Modifier".ToNonTerminal(), "*".ToTerminal()),

                        //Modifier -> '+'
                        new Production<string>("Modifier".ToNonTerminal(), "+".ToTerminal()),

                        //Modifier -> '?'
                        new Production<string>("Modifier".ToNonTerminal(), "?".ToTerminal()),

                        //Modifier -> { Literal }
                        new Production<string>("Modifier".ToNonTerminal(), "{".ToTerminal(), "Literal".ToTerminal(), "}".ToTerminal()),

                        //Modifier -> { Literal , }
                        new Production<string>("Modifier".ToNonTerminal(), "{".ToTerminal(), "Literal".ToTerminal(), ",".ToTerminal(), "}".ToTerminal()),

                        //Modifier -> { Literal , Literal }
                        new Production<string>("Modifier".ToNonTerminal(), "{".ToTerminal(), "Literal".ToTerminal(), ",".ToTerminal(), "Literal".ToTerminal(), "}".ToTerminal()),

                        #endregion

                        #region Base
		                //Base -> Literal
                        //new Production<string>("Base".ToNonTerminal(), "Literal".ToTerminal()),

                        //Base -> .
                        new Production<string>("Base".ToNonTerminal(), ".".ToTerminal()),

                        //Base -> AnyLiteral
                        new Production<string>("Base".ToNonTerminal(), "AnyLiteral".ToNonTerminal()),

                        ////Base -> '\' Literal
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "Literal".ToTerminal()),

                        ////Base -> '\' +
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "+".ToTerminal()),

                        ////Base -> '\' *
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "*".ToTerminal()),

                        ////Base -> '\' ?
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "?".ToTerminal()),

                        ////Base -> '\' {
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "{".ToTerminal()),

                        ////Base -> '\' }
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "}".ToTerminal()),

                        ////Base -> '\' |
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "|".ToTerminal()),

                        ////Base -> '\' [
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "[".ToTerminal()),

                        ////Base -> '\' ]
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "]".ToTerminal()),

                        ////Base -> '\' (
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "(".ToTerminal()),

                        ////Base -> '\' )
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), ")".ToTerminal()),

                        ////Base -> '\' '\'
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), @"\".ToTerminal()),

                        ////Base -> '\' '.'
                        //new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), ".".ToTerminal()),

                        //Base -> [ LiteralLst ]
                        new Production<string>("Base".ToNonTerminal(), "[".ToTerminal(), "LiteralLst".ToNonTerminal(), "]".ToTerminal()),

                        //Base -> [ ^ LiteralLst ]
                        new Production<string>("Base".ToNonTerminal(), "[".ToTerminal(), "^".ToTerminal(), "LiteralLst".ToNonTerminal(), "]".ToTerminal()),

                        //Base -> '(' Regex ')'
                        new Production<string>("Base".ToNonTerminal(), "(".ToTerminal(), "Regex".ToNonTerminal(), ")".ToTerminal()),
	                    #endregion

                        #region LiteralLst
		                //LiteralLst -> AnyLiteral
                        new Production<string>("LiteralLst".ToNonTerminal(), "AnyLiteral".ToNonTerminal()),

                        //LiteralLst -> LiteralLst AnyLiteral
                        new Production<string>("LiteralLst".ToNonTerminal(), "LiteralLst".ToNonTerminal(), "AnyLiteral".ToNonTerminal()), 
	                    #endregion

                        #region AnyLiteral
                        //AnyLiteral -> Literal
                        new Production<string>("AnyLiteral".ToNonTerminal(), "Literal".ToTerminal()),

                        //AnyLiteral -> '\' .
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), ".".ToTerminal()),

                        //AnyLiteral -> '\' Literal
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "Literal".ToTerminal()),

                        //AnyLiteral -> '\' +
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "+".ToTerminal()),

                        //AnyLiteral -> '\' *
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "*".ToTerminal()),

                        //AnyLiteral -> '\' ?
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "?".ToTerminal()),

                        //AnyLiteral -> '\' {
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "{".ToTerminal()),

                        //AnyLiteral -> '\' }
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "}".ToTerminal()),

                        //AnyLiteral -> '\' |
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "|".ToTerminal()),

                        //AnyLiteral -> '\' [
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "[".ToTerminal()),

                        //AnyLiteral -> '\' ]
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "]".ToTerminal()),

                        //AnyLiteral -> '\' (
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), "(".ToTerminal()),

                        //AnyLiteral -> '\' )
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), ")".ToTerminal()),

                        //AnyLiteral -> '\' '\'
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), @"\".ToTerminal()),

                        //AnyLiteral -> '\' '^'
                        new Production<string>("AnyLiteral".ToNonTerminal(), @"\".ToTerminal(), @"^".ToTerminal()),
                        #endregion

                        //Anything -> .
                        new Production<string>("Anything".ToNonTerminal(), ".".ToTerminal()),

                        //new Production<string>("Anything".ToNonTerminal(), 

                    }
                );

            }
        }
        #endregion

        /// <summary>
        /// Gets the LR(1) grammar used to parse regular expression patterns.
        /// </summary>
        /// <returns></returns>
        public static ContextFreeGrammar<Token<string>> GetPatternGrammar()
        {
            return PatternGrammar.GetGrammar();
        }

        /// <summary>
        /// Gets the regular expression grammar used by this Regex object.
        /// </summary>
        public ContextFreeGrammar<string> Grammar
        {
            get
            {
                return grammar.DeepCopy();
            }
        }

        /// <summary>
        /// The grammar that was generated from the given regex pattern.
        /// </summary>
        private ContextFreeGrammar<string> grammar;

        /// <summary>
        /// The parser used to parse input that should match regular expressions.
        /// </summary>
        LRParser<string> parser;

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

            Lexer lexer = new Lexer
            {
                ThrowSyntaxErrors = true
            };
            lexer.SetDefintions(PatternGrammar.GetLexerDefinitions());
            LRParser<Token<string>> patternParser = new LRParser<Token<string>>();
            patternParser.SetParseTable(GetPatternGrammar());

            var result = patternParser.ParseAST(PatternGrammar.ConvertToTerminals(lexer.ReadTokens(pattern)));

            if (result.Success)
            {
                grammar = buildGrammar(result.GetParseTree());
                parser = new LRParser<string>();
                parser.SetParseTable(grammar);
            }
            else
            {
                throw new ArgumentException("Please provide a valid regular expression pattern.", "pattern");
            }
        }

        /// <summary>
        /// Determines if the given input string matches the regular expression.
        /// </summary>
        /// <param name="input">The input string to match against the regular expression.</param>
        /// <returns></returns>
        public bool IsMatch(string input)
        {
            //List<Terminal<string>> terminals = new List<Terminal<string>>();

            var result = parser.ParseAST(input.Select(a => new Terminal<string>(a.ToString())));
            return result.Success;
        }

        /// <summary>
        /// Builds a context free grammar from the given abstract syntax tree that was parsed from a regular expression pattern.
        /// </summary>
        /// <param name="ast"></param>
        /// <returns></returns>
        private ContextFreeGrammar<string> buildGrammar(ParseTree<Token<string>> ast)
        {
            Stack<Production<string>> productions = new Stack<Production<string>>();
            buildProduction(ast.Root, productions);
            return new ContextFreeGrammar<string>(productions.First().NonTerminal, productions);
        }

        /// <summary>
        /// Builds productions from the given branch such that the input regex pattern is converted to a context free grammar.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        private void buildProduction(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions)
        {
            if (currentBranch.Value is NonTerminal<Token<string>>)
            {
                string name = ((NonTerminal<Token<string>>)currentBranch.Value).Name;
                if (name.Equals("Regex"))
                {
                    buildRegexBranch(currentBranch, productions);
                }
                else if (name.Equals("Term"))
                {
                    buildTermBranch(currentBranch, productions);
                }
            }
        }

        private int currentRegexNameIndex = 0;

        private static System.Security.Cryptography.RNGCryptoServiceProvider rngProvider = new System.Security.Cryptography.RNGCryptoServiceProvider();

        private static int currentRegexIndex = 0;

        private static string getNewRegexName()
        {
            return string.Format("Regex:{0}", currentRegexIndex++);
        }

        private static string getCurrentRegexName()
        {
            return string.Format("Regex:{0}", currentRegexIndex);
        }

        /// <summary>
        /// Gets a random number.
        /// </summary>
        /// <returns></returns>
        private static int getRN()
        {
            byte[] b = new byte[4];

            rngProvider.GetBytes(b);

            return BitConverter.ToInt32(b, 0);
        }

        /// <summary>
        /// Builds a set of productions from the current 'Regex' branch built from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <param name="currentNameIndex"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildRegexBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, string name = null)
        {
            if (name == null)
            {
                name = getNewRegexName();
            }
            if (currentBranch.Children.Count == 3)
            {
                NonTerminal<string> nt = new NonTerminal<string>(name);

                productions.Push(new Production<string>(nt, buildTermLstBranch(currentBranch.Children[0], productions)));
                buildRegexBranch(currentBranch.Children[2], productions, name);
                return new[] { nt };
            }
            else
            {
                NonTerminal<string> nt = new NonTerminal<string>(name);
                productions.Push(new Production<string>(nt, buildTermLstBranch(currentBranch.Children[0], productions)));
                return new[] { nt };
            }
        }

        /// <summary>
        /// Builds a set of productions from the current 'Term' branch built from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildTermBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions)
        {
            int num = getRN();
            string name = "Repetition:" + num;
            NonTerminal<string> nt = new NonTerminal<string>(name);

            //If the production is Term -> Factor and then Factor -> Base Modifier
            if (currentBranch.Children[0].Children.Count == 2)
            {
                //If the modifier is the '*' sign
                if (currentBranch.Children[0].Children[1].Children[0].Value.InnerValue.Value == "*")
                {
                    //Create a set of productions that match the behaviour of the '*' sign in a regular expression

                    //R -> elements R
                    productions.Push(new Production<string>(nt, buildBaseBranch(currentBranch.Children[0].Children[0], productions).Concat(new[] { nt }).ToArray()));

                    //R -> nothing
                    productions.Push(new Production<string>(nt));
                }
                //if the modifier is the '+' sign
                else if (currentBranch.Children[0].Children[1].Children[0].Value.InnerValue.Value == "+")
                {
                    //Create a set of productions that match the behaviour of the '+' sign in a regular expression

                    //R1
                    NonTerminal<string> x1 = new NonTerminal<string>("Repetition:" + (num + 1));

                    GrammarElement<string>[] elements = buildBaseBranch(currentBranch.Children[0].Children[0], productions).Concat(new[] { x1 }).ToArray();

                    //R -> elements R1
                    //R1 -> elements
                    //R1 -> nothing
                    productions.Push(new Production<string>(nt, elements));
                    productions.Push(new Production<string>(x1, elements));
                    productions.Push(new Production<string>(x1));
                }
                //if the modifier is the '?' sign
                else if (currentBranch.Children[0].Children[1].Children[0].Value.InnerValue.Value == "?")
                {
                    //create a set of productions that match either one or none

                    //R -> elements
                    productions.Push(new Production<string>(nt, buildBaseBranch(currentBranch.Children[0].Children[0], productions)));

                    //R -> nothing
                    productions.Push(new Production<string>(nt));
                }

            }
            //Otherwise the production is of the form Factor -> Base
            else
            {
                return buildBaseBranch(currentBranch.Children[0].Children[0], productions);
            }

            //Return the created non terminal that represents the entry point into the productions we created.
            return new[] { nt };
        }

        /// <summary>
        /// Builds a set of productions from the current 'Base' branch built from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildBaseBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions)
        {
            //If the production is not of the form: Base -> ( Regex )
            if (currentBranch.Children.Count != 3 && currentBranch.Children.Count != 4)
            {
                //Base -> AnyLiteral
                if (currentBranch.Children[0].Value is NonTerminal<Token<string>>)
                {
                    return buildAnyLiteralBranch(currentBranch.Children[0], productions);
                }
                else
                {
                    //The index at witch to start taking elements, either 0 or 1(to prevent including '\')
                    int startIndex = 0;
                    if (currentBranch.Children[0].Value is Terminal<Token<string>>)
                    {
                        startIndex = ((Terminal<Token<string>>)currentBranch.Children[0].Value).InnerValue.Value == @"\" ? 1 : 0;
                    }

                    return currentBranch.Children.Skip(startIndex).Select<ParseTree<Token<string>>.ParseTreebranch, GrammarElement<string>>(a =>
                    {
                        if (a.Value is NonTerminal<Token<string>>)
                        {
                            return new NonTerminal<string>(((NonTerminal<Token<string>>)a.Value).Name);
                        }
                        else
                        {
                            return new Terminal<string>(a.Value.InnerValue.Value);
                        }
                    }).ToArray();
                }
            }
            //If the production is of the above form(Base -> '(' Regex ')' or Base -> [ LiteralLst ] or Base -> [ ^ LiteralLst ])
            else
            {
                if (currentBranch.Children[1].Value is NonTerminal<Token<string>>)
                {
                    //Get the 'Regex' non-terminal
                    NonTerminal<Token<string>> nt = currentBranch.Children[1].Value as NonTerminal<Token<string>>;

                    if (nt.Name.Equals("Regex"))
                    {
                        //build the productions for the branch
                        return buildRegexBranch(currentBranch.Children[1], productions);
                    }
                    //Base -> [ LiteralLst ]
                    else if (nt.Name.Equals("LiteralLst"))
                    {
                        string name = string.Format("Condition:{0}", getRN());

                        List<Terminal<string>> elements = new List<Terminal<string>>(buildLiteralLstBranch(currentBranch.Children[1], productions));

                        NonTerminal<string> condition = new NonTerminal<string>(name);

                        //Condition -> One of Elements
                        foreach (var e in elements)
                        {
                            productions.Push(new Production<string>(condition, e));
                        }

                        return new[] { condition };
                    }
                }
                //Base -> [ ^ LiteralLst ]
                else
                {

                    NonTerminal<Token<string>> nt = currentBranch.Children[2].Value as NonTerminal<Token<string>>;

                    if (nt.Name.Equals("LiteralLst"))
                    {
                        string name = string.Format("Condition:{0}", getRN());
                        List<Terminal<string>> elements = new List<Terminal<string>>(buildLiteralLstBranch(currentBranch.Children[2], productions));

                        NonTerminal<string> condition = new NonTerminal<string>(name);

                        AnyButTerminal<string> t = new AnyButTerminal<string>(elements, true);

                        //Condition -> AnyBut(elements)
                        productions.Push(new Production<string>(condition, t));

                        return new[] { condition };
                    }
                }
            }
            return new GrammarElement<string>[0];
        }

        /// <summary>
        /// builds a list of productions based on the given "LiteralLst" branch.
        /// </summary>
        /// <param name="parseTreebranch"></param>
        /// <param name="productions"></param>
        /// <returns>A list of grammar elements that represent the grammar elemetns contained in this LiteralLst</returns>
        private Terminal<string>[] buildLiteralLstBranch(ParseTree<Token<string>>.ParseTreebranch parseTreebranch, Stack<Production<string>> productions)
        {
            //LiteralLst -> AnyLiteral
            if (parseTreebranch.Children.Count == 1)
            {
                return buildAnyLiteralBranch(parseTreebranch.Children[0], productions);
            }
            //LiteralLst -> LiteralLst AnyLiteral
            else
            {
                List<Terminal<string>> elements = new List<Terminal<string>>();
                elements.AddRange(buildLiteralLstBranch(parseTreebranch.Children[0], productions));
                elements.AddRange(buildAnyLiteralBranch(parseTreebranch.Children[1], productions));

                return elements.ToArray();
            }
        }

        private Terminal<string>[] buildAnyLiteralBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions)
        {
            //The index at witch to start taking elements, either 0 or 1(to prevent including '\')
            int startIndex = 0;
            if (currentBranch.Children[0].Value is Terminal<Token<string>>)
            {
                startIndex = currentBranch.Children[0].Value.InnerValue.Value == @"\" ? 1 : 0;
            }

            return currentBranch.Children.Skip(startIndex).Select<ParseTree<Token<string>>.ParseTreebranch, Terminal<string>>(a => new Terminal<string>(a.Value.InnerValue.Value)).ToArray();
        }

        /// <summary>
        /// Builds a set of productions from the current 'TermLst' branch build from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildTermLstBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions)
        {
            //if the current branch is of the form: TermLst -> TermLst Term
            if (currentBranch.Children.Count == 2)
            {
                List<GrammarElement<string>> elements = new List<GrammarElement<string>>();

                //Build the TermLst branch
                elements.AddRange(buildTermLstBranch(currentBranch.Children[0], productions));

                //and then the Term branch
                elements.AddRange(buildTermBranch(currentBranch.Children[1], productions));

                //return the total elements generated.
                return elements.ToArray();
            }
            //Otherwise the branch is of the form: TermLst -> Term
            else
            {
                return buildTermBranch(currentBranch.Children[0], productions);
            }
        }
    }
}
