using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parser.Grammar;
using Parser.StateMachine;
using LexicalAnalysis.Definitions;
using LexicalAnalysis;
using Parser.Parsers;
using Parser.Definitions;
using System.Threading;
using System.IO;
using System.IO.Compression;
using Parser.Parsers.AllInOne;
using Parser;
using Parser.RegularExpressions;

namespace ParserGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        RegularExpression r;

        private void button1_Click(object sender, EventArgs e)
        {
            r = new RegularExpression(txtCFG.Text);

            //(new Thread(aio)).Start(txtCFG.Text);

        }

        IEnumerable<Token<string>> tokens;

        Lexer lexer;

        /// <summary>
        /// Concats each individual element in the given dynamic object to produce a single string representing all elements.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        private string concatArray(dynamic a, string seperator = " ")
        {
            if (a is IEnumerable<dynamic>)
            {
                StringBuilder b = new StringBuilder();
                foreach (dynamic e in a)
                {
                    b.Append(e.ToString());
                    b.Append(seperator);
                }
                return b.ToString();
            }
            else
            {
                return a.ToString();
            }
        }

        AIOLRParser<string> aioParser;

        static readonly string[] CSharpKeywords = new string[] 
        {
            "new",
            "class",
            "struct",
            "using",
            "namespace",
            "static",
            "void",
            "int",
            "float",
            "double",
            "decimal",
            "abstract",
            "as",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "default",
            "delegate",
            "do",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "volatile",
            "while",
            "string"
        };

        private static IEnumerable<ParserTokenDefinition<string>> GetKeywords(IEnumerable<string> keywords)
        {
            List<ParserTokenDefinition<string>> words = new List<ParserTokenDefinition<string>>(keywords.Count());
            foreach (string keyword in keywords)
            {
                words.Add(new KeywordParserTokenDefinition(keyword, true));
            }
            return words;
        }

        private void aio(object p)
        {
            string text = (string)p;


            ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            (
                new ParserTokenDefinitionCollection<string>
                (
                    new[]
                    {
                        ////defines'\w'. In regex it is matched as a word.
                        //new StringedParserTokenDefinition(@"\\w", "Word", false),

                        ////defines '\n'. In regex it is matched as a number.
                        //new StringedParserTokenDefinition("@\\n", "Number", false),

                        ////defines '\s'. In regex it is matched as a space/control char.
                        //new StringedParserTokenDefinition(@"\\s", "Space", false),

                        ////defines '\W'. In regex it is matched as not a word.
                        //new StringedParserTokenDefinition(@"\\W", "NotWord", false),

                        ////defines '\N'. In regex it is matched as not a number.
                        //new StringedParserTokenDefinition(@"\\N", "NotNumber", false),

                        ////defines '\S'. In regex it is matched as not a space.
                        //new StringedParserTokenDefinition(@"\\S", "NotSpace", false),

                        ////defines a period. In regex it is matched as any character
                        //new StringedParserTokenDefinition(@"\.", "Any", false),

                        ////defines '\b'. In regex it is matched as a border.
                        //new StringedParserTokenDefinition(@"\\b", "Border", false),

                        ////defines '$'
                        //new StringedParserTokenDefinition(@"\$", "$", false),

                        ////defines '^'
                        //new StringedParserTokenDefinition(@"\^", "^", false),

                        //defines '(' and ')'
                        new StringedParserTokenDefinition(@"\(", "(", false),
                        new StringedParserTokenDefinition(@"\)", ")", false),

                        //defines '[' and ']'
                        new StringedParserTokenDefinition(@"\[", "[", false),
                        new StringedParserTokenDefinition(@"\]", "]", false),

                        ////defines '-'
                        //new StringedParserTokenDefinition(@"\-", "-", false),

                        //defines '+'
                        new StringedParserTokenDefinition(@"\+", "+", false),

                        //defines '*'
                        new StringedParserTokenDefinition(@"\*", "*", false),

                        //defines ','
                        new StringedParserTokenDefinition(@"\,", ",", false),

                        //defines '\'
                        new StringedParserTokenDefinition(@"\\", @"\", false),

                        //defines '{' and '}'
                        new StringedParserTokenDefinition(@"\{", "{", false),
                        new StringedParserTokenDefinition(@"\}", "}", false),

                        //defines '|'
                        new StringedParserTokenDefinition(@"\|", "|", false),

                        //defines '?'
                        new StringedParserTokenDefinition(@"\?", "?", false),

                        ////defines a number
                        //new StringedParserTokenDefinition(@"\n+", "Number", true),

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

                    //Factor -> Base Modifier
                    new Production<string>("Factor".ToNonTerminal(), "Base".ToNonTerminal(), "Modifier".ToNonTerminal()),

                    //Factor -> Base
                    new Production<string>("Factor".ToNonTerminal(), "Base".ToNonTerminal()),

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

                    #region Base
		            //Base -> Literal
                    new Production<string>("Base".ToNonTerminal(), "Literal".ToTerminal()),

                    //Base -> '\' Literal
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "Literal".ToTerminal()),

                    //Base -> '\' +
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "+".ToTerminal()),

                    //Base -> '\' *
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "*".ToTerminal()),

                    //Base -> '\' ?
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "?".ToTerminal()),

                    //Base -> '\' {
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "{".ToTerminal()),

                    //Base -> '\' }
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "}".ToTerminal()),

                    //Base -> '\' |
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "|".ToTerminal()),

                    //Base -> '\' [
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "[".ToTerminal()),

                    //Base -> '\' ]
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "]".ToTerminal()),

                    //Base -> '\' (
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), "(".ToTerminal()),

                    //Base -> '\' )
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), ")".ToTerminal()),

                    //Base -> '\' '\'
                    new Production<string>("Base".ToNonTerminal(), @"\".ToTerminal(), @"\".ToTerminal()),

                    //Base -> [ LiteralLst ]
                    new Production<string>("Base".ToNonTerminal(), "[".ToTerminal(), "LiteralLst".ToNonTerminal(), "]".ToTerminal()),

                    //Base -> '(' Regex ')'
                    new Production<string>("Base".ToNonTerminal(), "(".ToTerminal(), "Regex".ToNonTerminal(), ")".ToTerminal()),
	                #endregion

                    //LiteralLst -> Literal
                    new Production<string>("LiteralLst".ToNonTerminal(), "Literal".ToTerminal()),

                    //LiteralLst -> LiteralLst Literal
                    new Production<string>("LiteralLst".ToNonTerminal(), "LiteralLst".ToNonTerminal(), "Literal".ToTerminal()),

                    //Anything -> 
                    //new Production<string>("Anything".ToNonTerminal(), 

                }
            ); 


            long totalParseTime = 0;
            long totalLexTime = 0;

            LRParser<Token<string>> parser = new LRParser<Token<string>>();
            parser.SetParseTable(def.GetGrammar());

            //ParseTable<Token<string>> table = null;// = getParseTable();
            //if(table == null)
            //{
            //    Stopwatch e = Stopwatch.StartNew();

            //    table = createParseTable(def.GetGrammar(), parser);

            //    e.Stop();
            //}


            if (lexer == null)
            {
                lexer = new Lexer();
                lexer.SetDefintions(def.Definitions.GetLexerDefinitions());
            }
            Stopwatch w = Stopwatch.StartNew();


            tokens = lexer.ReadTokens(text);

            w.Stop();

            Stopwatch sw = Stopwatch.StartNew();

            /*ParseResult<Token<string>>*/
            var tree = parser.ParseAST(def.ConvertToTerminals(tokens));

            sw.Stop();

            totalLexTime += w.ElapsedMilliseconds;
            totalParseTime += sw.ElapsedMilliseconds;

            MessageBox.Show(string.Format("Totally done. Average time to lex {0} chars: {1}. Average time to parse {2} tokens: {3}", text.Length, totalLexTime, tokens.Count(), totalParseTime), "Done", MessageBoxButtons.OK);
        }

        private ParseTable<Token<string>> getParseTable()
        {
            if (File.Exists(string.Format("{0}/ParseTable.ptbl", AppDomain.CurrentDomain.BaseDirectory)))
            {
                using (Stream s = File.OpenRead(string.Format("{0}/ParseTable.ptbl", AppDomain.CurrentDomain.BaseDirectory)))
                {
                    return ParseTable<Token<string>>.ReadFromStream(s);
                }
            }
            return null;
        }

        private ParseTable<Token<string>> createParseTable(ContextFreeGrammar<Token<string>> cfg, LRParser<Token<string>> parser)
        {
            var w = Stopwatch.StartNew();
            parser.SetParseTable(cfg);
            var table = parser.ParseTable;
            w.Stop();

            var sw = Stopwatch.StartNew();
            using(Stream s = File.Create(string.Format("{0}/ParseTable.ptbl", AppDomain.CurrentDomain.BaseDirectory)))
            {
                table.WriteToStream(s);
            }
            sw.Stop();
            return table;
        }

        private void OnParseDone(dynamic param)
        {
            MessageBox.Show(string.Format("Parsing is done. {0} string chars were lexed in {1} milliseconds. {2} tokens were parsed in {3} milliseconds", param.l, param.w.ElapsedMilliseconds, param.p, param.sw.ElapsedMilliseconds), "Done", MessageBoxButtons.OK);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show(r.IsMatch(txtCFG.Text).ToString());
        }
    }
}
