using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LexicalAnalysis;
using Parser.Definitions;
using Parser.Grammar;
using Parser.Parsers;

namespace Parser.RegularExpressions
{
    /// <summary>
    /// Defines a class that builds the Context Free Grammar for a given regular expression.
    /// </summary>
    public class RegexBuilder
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

                            //defines '\-'
                            new StringedParserTokenDefinition(@"\\\-", "Literal", true),

                            //defines '-'
                            new StringedParserTokenDefinition(@"\-", "-", false),

                            //defines '<' or '>'
                            new StringedIdentifierParserTokenDefinition(@"\<|\>", "Literal", true),

                            //defines '='
                            new StringedIdentifierParserTokenDefinition(@"\=", "Literal", true),

                            //defines '!'
                            new StringedIdentifierParserTokenDefinition(@"\!", "Literal", true),

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

                        //Negative zero-width assertion
                        //Base -> '(' '? '!' Regex ')'
                        new Production<string>("Base".ToNonTerminal(), "(".ToTerminal(), "?".ToTerminal(), "!".ToTerminal(), "Regex".ToNonTerminal(), ")".ToTerminal()),

                        //Positive zero-width assertion
                        //Base -> '(' '? '=' Regex ')'
                        new Production<string>("Base".ToNonTerminal(), "(".ToTerminal(), "?".ToTerminal(), "=".ToTerminal(), "Regex".ToNonTerminal(), ")".ToTerminal()),
                        #endregion

                        #region LiteralLst

		                //LiteralRange -> AnyLiteral
                        new Production<string>("LiteralRange".ToNonTerminal(), "AnyLiteral".ToNonTerminal()),

                        //LiteralRange -> AnyLiteral - AnyLiteral
                        new Production<string>("LiteralRange".ToNonTerminal(), "AnyLiteral".ToNonTerminal(), "-".ToTerminal(), "AnyLiteral".ToNonTerminal()),

                        //LiteralLst -> LiteralRange
                        new Production<string>("LiteralLst".ToNonTerminal(), "LiteralRange".ToNonTerminal()),

                        //LiteralLst -> LiteralLst LiteralRange
                        new Production<string>("LiteralLst".ToNonTerminal(), "LiteralLst".ToNonTerminal(), "LiteralRange".ToNonTerminal()), 
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

        private int currentRegexNameIndex = 0;

        private int currentRepetitionNameIndex = 0;

        private int currentAlternationNameIndex = 0;

        private int currentZeroWidthNameIndex = 0;

        private ContextFreeGrammar<Token<string>> patternGrammar;

        public RegexBuilder()
        {
            patternGrammar = PatternGrammar.GetGrammar();
        }

        private string getNewRegexName()
        {
            return string.Format("Regex:{0}", ++currentRegexNameIndex);
        }

        private string getCurrentRegexName()
        {
            return string.Format("Regex:{0}", currentRegexNameIndex);
        }

        private string getNewRepetitionName()
        {
            return string.Format("Repetition:{0}", ++currentRepetitionNameIndex);
        }

        private string getCurrentRepetitionName()
        {
            return string.Format("Repetition:{0}", currentRepetitionNameIndex);
        }

        private string getNewAlternationName()
        {
            return string.Format("Alternation:{0}", ++currentAlternationNameIndex);
        }

        private string getCurrentAlternationName()
        {
            return string.Format("Alternation:{0}", currentAlternationNameIndex);
        }

        private string getNewZeroWidthName()
        {
            return string.Format("Zero-Width:{0}", ++currentZeroWidthNameIndex);
        }

        private string getCurrentZeroWidthName()
        {
            return string.Format("Zero-Width:{0}", currentZeroWidthNameIndex);
        }

        /// <summary>
        /// Builds a Context Free Grammar representing the given regular expression.
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        public ContextFreeGrammar<string> BuildGrammar(string regex)
        {
            Lexer lexer = new Lexer
            {
                ThrowSyntaxErrors = true
            };
            lexer.SetDefintions(PatternGrammar.GetLexerDefinitions());
            LRParser<Token<string>> patternParser = new LRParser<Token<string>>();

            //Stopwatch w = Stopwatch.StartNew();

            patternParser.SetParseTable(patternGrammar);

            //w.Stop();

            var result = patternParser.ParseAST(PatternGrammar.ConvertToTerminals(lexer.ReadTokens(regex)));

            if (result.Success)
            {
                return buildGrammar(result.GetParseTree());
            }
            else
            {
                throw new ArgumentException("The given regular expression must be a regular expression", "regex");
            }
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
                    buildRegexBranch(currentBranch, productions, false, true);
                }
                else if (name.Equals("Term"))
                {
                    buildTermBranch(currentBranch, productions, false, true);
                }
            }
        }

        /// <summary>
        /// Builds a set of productions from the current 'Regex' branch built from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <param name="currentNameIndex"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildRegexBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, bool negated, bool keep, string name = null)
        {
            if (name == null)
            {
                name = getNewRegexName();
            }
            //Regex -> TermLst '|' Regex
            if (currentBranch.Children.Count == 3)
            {
                //Create new regex non-terminal
                NonTerminal<string> nt = new NonTerminal<string>(name);

                //Build two productions of the form:
                //
                //Regex -> TermLst
                //Regex -> RegexChild

                productions.Push(new Production<string>(nt, buildTermLstBranch(currentBranch.Children[0], productions, negated, keep)));

                //Regex -> RegexChild
                buildRegexBranch(currentBranch.Children[2], productions, negated, keep, name);
                return new[] { nt };
            }
            //Regex -> TermLst
            else
            {
                NonTerminal<string> nt = new NonTerminal<string>(name);
                productions.Push(new Production<string>(nt, buildTermLstBranch(currentBranch.Children[0], productions, negated, keep)));
                return new[] { nt };
            }
        }

        /// <summary>
        /// Builds a set of productions from the current 'Term' branch built from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildTermBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, bool negated, bool keep)
        {
            string name = getNewRepetitionName();
            NonTerminal<string> nt = new NonTerminal<string>(name);

            //If the production is Term -> Factor and then Factor -> Base Modifier
            if (currentBranch.Children[0].Children.Count == 2)
            {
                //If the modifier is the '*' sign
                if (currentBranch.Children[0].Children[1].Children[0].Value.InnerValue.Value == "*")
                {
                    //Create a set of productions that match the behaviour of the '*' sign in a regular expression

                    //R -> elements R
                    productions.Push(new Production<string>(nt, buildBaseBranch(currentBranch.Children[0].Children[0], productions, negated, keep).Concat(new[] { nt }).ToArray()));

                    //R -> nothing
                    productions.Push(new Production<string>(nt));
                }
                //if the modifier is the '+' sign
                else if (currentBranch.Children[0].Children[1].Children[0].Value.InnerValue.Value == "+")
                {
                    //Create a set of productions that match the behaviour of the '+' sign in a regular expression

                    //R1
                    NonTerminal<string> x1 = new NonTerminal<string>(getNewRepetitionName());

                    GrammarElement<string>[] elements = buildBaseBranch(currentBranch.Children[0].Children[0], productions, negated, keep).Concat(new[] { x1 }).ToArray();

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
                    productions.Push(new Production<string>(nt, buildBaseBranch(currentBranch.Children[0].Children[0], productions, negated, keep)));

                    //R -> nothing
                    productions.Push(new Production<string>(nt));
                }

            }
            //Otherwise the production is of the form Factor -> Base
            else
            {
                return buildBaseBranch(currentBranch.Children[0].Children[0], productions, negated, keep);
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
        private GrammarElement<string>[] buildBaseBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, bool negated, bool keep)
        {
            //If the production is not of the form: Base -> ( Regex )
            if (currentBranch.Children.Count == 1)
            {
                //Then there are only two possible forms:
                //
                //1). Base -> AnyLiteral
                //2). Base -> '.'

                //Base -> AnyLiteral
                if (currentBranch.Children[0].Value is NonTerminal<Token<string>>)
                {
                    return buildAnyLiteralBranch(currentBranch.Children[0], productions, negated, keep);
                }
                //Base -> '.'
                else
                {
                    return new[] { new Terminal<string>(null, true, negated) };
                }
            }
            //If the production is of the above form(Base -> '(' Regex ')' or Base -> [ LiteralLst ] or Base -> [ ^ LiteralLst ])
            else
            {
                //Base -> ( Regex ); or Base -> [ LiteralLst ]
                if (currentBranch.Children[1].Value is NonTerminal<Token<string>>)
                {
                    //Get the 'Regex' non-terminal
                    NonTerminal<Token<string>> nt = currentBranch.Children[1].Value as NonTerminal<Token<string>>;

                    //Base -> ( Regex )
                    if (nt.Name.Equals("Regex"))
                    {
                        //build the productions for the branch
                        return buildRegexBranch(currentBranch.Children[1], productions, negated, keep);
                    }
                    //Base -> [ LiteralLst ]
                    else if (nt.Name.Equals("LiteralLst"))
                    {
                        //Get all of the possible elements for the alternation
                        List<Terminal<string>> elements = new List<Terminal<string>>(buildLiteralLstBranch(currentBranch.Children[1], productions, negated, keep));

                        //build the non-terminal to represent it
                        NonTerminal<string> alternation = new NonTerminal<string>(getNewAlternationName());

                        //Alternation -> One of Elements
                        foreach (var e in elements)
                        {
                            productions.Push(new Production<string>(alternation, e));
                        }

                        return new[] { alternation };
                    }
                }
                else
                {
                    //Base -> [ ^ LiteralLst ]
                    if (currentBranch.Children[2].Value is NonTerminal<Token<string>>)
                    {
                        NonTerminal<Token<string>> nt = currentBranch.Children[2].Value as NonTerminal<Token<string>>;

                        if (nt.Name.Equals("LiteralLst"))
                        {
                            string name = string.Format(getNewAlternationName());
                            List<Terminal<string>> elements = new List<Terminal<string>>(buildLiteralLstBranch(currentBranch.Children[2], productions, negated, keep));

                            NonTerminal<string> condition = new NonTerminal<string>(name);
                            condition.Negated = true;


                            foreach (var e in elements)
                            {
                                e.Negated = true;
                                productions.Push(new Production<string>(condition, e));
                            }

                            return new[] { condition };
                        }
                    }
                    //Base -> ( ? either then either '!' or '=' then Regex )
                    else
                    {
                        Terminal<Token<string>> negationTerm = currentBranch.Children[2].Value as Terminal<Token<string>>;
                        NonTerminal<Token<string>> regex = currentBranch.Children[3].Value as NonTerminal<Token<string>>;
                        NonTerminal<string> nt = new NonTerminal<string>(getNewZeroWidthName());
                        if (negationTerm.InnerValue.Value.Equals("!"))
                        {
                            GrammarElement<string>[] elements = buildRegexBranch(currentBranch.Children[3], productions, !negated, false);
                            productions.Push(new Production<string>(nt, elements));
                        }
                        //Not negated
                        else
                        {
                            GrammarElement<string>[] elements = buildRegexBranch(currentBranch.Children[3], productions, negated, false);

                            productions.Push(new Production<string>(nt, elements));

                            
                        }
                        return new[] { nt };
                    }
                }

            }
            return new GrammarElement<string>[0];
        }



        private Terminal<string>[] buildLiteralRangeBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, bool negated, bool keep)
        {
            //LiteralRange -> AnyLiteral
            if (currentBranch.Children.Count == 1)
            {
                return buildAnyLiteralBranch(currentBranch.Children[0], productions, negated, keep);
            }
            //LiteralRange -> AnyLiteral - AnyLiteral
            else
            {
                List<Terminal<string>> elements = new List<Terminal<string>>();
                var first = buildAnyLiteralBranch(currentBranch.Children[0], productions, negated, keep).First();
                var last = buildAnyLiteralBranch(currentBranch.Children[1], productions, negated, keep).First();

                int dir = first.InnerValue[0] - last.InnerValue[0];


                //if the last's character is greater than the first's
                if (dir > 0)
                {
                    //count from the first to the last
                    for (int i = last.InnerValue[0]; i <= first.InnerValue[0]; i++)
                    {
                        //add each character between the first and last character in the group.
                        elements.Add(new Terminal<string>(((char)i).ToString(), keep, negated));
                    }
                }
                else if (dir < 0)
                {
                    //count from the last to the first.
                    for (int i = first.InnerValue[0]; i <= last.InnerValue[0]; i++)
                    {
                        //add each character between the last and first character in the group.
                        elements.Add(new Terminal<string>(((char)i).ToString(), keep, negated));
                    }
                }
                else
                {
                    //add the first/last element because they are the same
                    elements.Add(new Terminal<string>(first.InnerValue, keep, negated));
                }

                return elements.Distinct().ToArray();
            }
        }

        /// <summary>
        /// builds a list of productions based on the given "LiteralLst" branch.
        /// </summary>
        /// <param name="parseTreebranch"></param>
        /// <param name="productions"></param>
        /// <returns>A list of grammar elements that represent the grammar elemetns contained in this LiteralLst</returns>
        private Terminal<string>[] buildLiteralLstBranch(ParseTree<Token<string>>.ParseTreebranch parseTreebranch, Stack<Production<string>> productions, bool negated, bool keep)
        {
            //LiteralLst -> LiteralRange
            if (parseTreebranch.Children.Count == 1)
            {
                return buildLiteralRangeBranch(parseTreebranch.Children[0], productions, negated, keep);
            }
            //LiteralLst -> LiteralLst LiteralRange
            else
            {
                //if (((NonTerminal<Token<string>>)parseTreebranch.Children[0].Value).Name.Equals("LiteralLst"))
                //{
                List<Terminal<string>> elements = new List<Terminal<string>>();
                elements.AddRange(buildLiteralLstBranch(parseTreebranch.Children[0], productions, negated, keep));
                elements.AddRange(buildLiteralRangeBranch(parseTreebranch.Children[1], productions, negated, keep));

                return elements.Distinct().ToArray();
                //}
                ////LiteralLst -> AnyLiteral - AnyLiteral
                //else
                //{
                //    List<Terminal<string>> elements = new List<Terminal<string>>();
                //    var first = buildAnyLiteralBranch(parseTreebranch.Children[0], productions).First();
                //    var last = buildAnyLiteralBranch(parseTreebranch.Children[1], productions).First();

                //    int dir = first.InnerValue[0] - last.InnerValue[0];


                //    //if the last's character is greater than the first's
                //    if (dir > 0)
                //    {
                //        //count from the first to the last
                //        for (int i = last.InnerValue[0]; i <= first.InnerValue[0]; i++)
                //        {
                //            //add each character between the first and last character in the group.
                //            elements.Add(new Terminal<string>(((char)i).ToString()));
                //        }
                //    }
                //    else if (dir < 0)
                //    {
                //        //count from the last to the first.
                //        for (int i = first.InnerValue[0]; i <= last.InnerValue[0]; i++)
                //        {
                //            //add each character between the last and first character in the group.
                //            elements.Add(new Terminal<string>(((char)i).ToString()));
                //        }
                //    }
                //    else
                //    {
                //        //add the first/last element because they are the same
                //        elements.Add(new Terminal<string>(first.InnerValue));
                //    }

                //    return elements.ToArray();
                //}
            }
        }

        private Terminal<string>[] buildAnyLiteralBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, bool negated, bool keep)
        {
            //The index at witch to start taking elements, either 0 or 1(to prevent including '\')
            int startIndex = 0;
            if (currentBranch.Children[0].Value is Terminal<Token<string>>)
            {
                startIndex = currentBranch.Children[0].Value.InnerValue.Value == @"\" ? 1 : 0;
            }

            return currentBranch.Children.Skip(startIndex).Select<ParseTree<Token<string>>.ParseTreebranch, Terminal<string>>(a => new Terminal<string>(a.Value.InnerValue.Value, keep, negated)).ToArray();
        }

        /// <summary>
        /// Builds a set of productions from the current 'TermLst' branch build from the patternParser.
        /// </summary>
        /// <param name="currentBranch"></param>
        /// <param name="productions"></param>
        /// <returns></returns>
        private GrammarElement<string>[] buildTermLstBranch(ParseTree<Token<string>>.ParseTreebranch currentBranch, Stack<Production<string>> productions, bool negated, bool keep)
        {
            //if the current branch is of the form: TermLst -> TermLst Term
            if (currentBranch.Children.Count == 2)
            {
                List<GrammarElement<string>> elements = new List<GrammarElement<string>>();

                //Build the TermLst branch
                elements.AddRange(buildTermLstBranch(currentBranch.Children[0], productions, negated, keep));

                //and then the Term branch
                elements.AddRange(buildTermBranch(currentBranch.Children[1], productions, negated, keep));

                //return the total elements generated.
                return elements.ToArray();
            }
            //Otherwise the branch is of the form: TermLst -> Term
            else
            {
                return buildTermBranch(currentBranch.Children[0], productions, negated, keep);
            }
        }

    }
}
