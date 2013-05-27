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
using Parser.AllInOne;
using System.Threading;
using System.IO;
using System.IO.Compression;

namespace ParserGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new Thread(aio)).Start(txtCFG.Text);



            #region Parse Table Builder
            //List<dynamic> gridCollection = new List<dynamic>();

            //foreach (var item in table.ActionTable.Select(a => new
            //{
            //    State = a.Key.Row,
            //    Input = a.Key.Column,
            //    Value = a.Value
            //}))
            //{
            //    gridCollection.Add(item);
            //}

            //foreach (var item in table.GotoTable.Select(a => new
            //{
            //    State = a.Key.Row,
            //    Input = a.Key.Column,
            //    Value = a.Value
            //}))
            //{
            //    gridCollection.Add(item);
            //}
            ////grdTable.AutoGenerateColumns = true;

            //grdTable.Columns.Clear();

            //grdTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //foreach (dynamic column in gridCollection.OrderBy(a => a is Terminal<string>).Select(a => a.Input).DistinctBy(a => a.ToString()))
            //{
            //    DataGridViewColumn col = new DataGridViewTextBoxColumn();
            //    col.HeaderText = column.ToString();
            //    col.SortMode = DataGridViewColumnSortMode.NotSortable;
            //    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            //    grdTable.Columns.Add(col);
            //}


            //grdTable.Columns[grdTable.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            //grdTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //var rows = gridCollection.GroupBy(a => a.State).Select(a => new List<dynamic>(a)).ToArray();
            //for (int i = 0; i < rows.Length; i++)
            //{
            //    List<dynamic> row = new List<dynamic>();
            //    for (int r = 0; r < grdTable.Columns.Count; r++)
            //    {

            //        if (r < rows[i].Count())
            //        {
            //            if (rows[i].Any(a =>
            //            {
            //                if (a != null)
            //                {
            //                    return a.Input.ToString().Equals(grdTable.Columns[r].HeaderText);
            //                }
            //                return false;
            //            }))
            //            {
            //                row.Add(concatArray(rows[i].First(a =>
            //                {
            //                    if (a != null)
            //                    {
            //                        return a.Input.ToString().Equals(grdTable.Columns[r].HeaderText);
            //                    }
            //                    return false;
            //                }).Value, "\n"));
            //            }
            //            else
            //            {
            //                row.Add(null);
            //                rows[i].Insert(r, null);
            //            }
            //        }
            //    }
            //    grdTable.Rows.Add(row.ToArray());
            //    grdTable.Rows[i].HeaderCell.Value = i.ToString();

            //}

            //foreach(DataGridViewColumn column in grdTable.Columns)
            //{
            //    int colw = column.Width;
            //    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            //    column.Width = colw;
            //} 
            #endregion
        }

        private void NewMethod()
        {
            string input = txtCFG.Text;

            string[] inputProductions = input.Replace("\r\n", string.Empty).Split(new[] { ';', '.' }, StringSplitOptions.RemoveEmptyEntries);


            List<Production<string>> productions = new List<Production<string>>();

            List<List<string>> splits = new List<List<string>>();

            foreach (string s in inputProductions)
            {
                splits.Add(new List<string>(s.Split(new[] { "->" }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim())));
            }

            foreach (string[] split in splits.Select(a => a.ToArray()))
            {
                if (split.Length == 2)
                {
                    List<GrammarElement<string>> element = new List<GrammarElement<string>>();
                    string[] elements = split[1].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim()).ToArray();
                    foreach (string el in elements)
                    {
                        if (splits.Any(a => a[0].Equals(el)))
                        {
                            element.Add(el.ToNonTerminal<string>());
                        }
                        else
                        {
                            element.Add(el.ToTerminal());
                        }
                    }

                    productions.Add(new Production<string>(split[0].ToNonTerminal<string>(), element.ToArray()));
                }
            }

            ContextFreeGrammar<Token<string>> grammar = new ContextFreeGrammar<Token<string>>(
                //productions.First().NonTerminal,
                "E".ToNonTerminal<Token<string>>(),
                (new Token<string>(0, "$", null)).ToTerminal(true),
                new Production<Token<string>>[]
                {
                    ////A -> A + B
                    //new Production<string>(terms[0], terms[0], new Terminal<string>("+"), terms[1]),
                    ////A -> a
                    //new Production<string>(terms[0], new Terminal<string>("a")),
                    ////B -> b
                    //new Production<string>(terms[1], new Terminal<string>("b"))

                    //E -> T
                    new Production<Token<string>>("E".ToNonTerminal<Token<string>>(), "T".ToNonTerminal<Token<string>>()),
                    //E -> (E)
                    new Production<Token<string>>("E".ToNonTerminal<Token<string>>(), ((new Token<string>(0, "(", null))).ToTerminal(true), "E".ToNonTerminal<Token<string>>(), ((new Token<string>(0, ")", null))).ToTerminal(true)),
                    //T -> n
                    new Production<Token<string>>("T".ToNonTerminal<Token<string>>(), ((new Token<string>(0, "n", null))).ToTerminal(true)),
                    ////T -> + T
                    //new Production<string>(terms[1], ("+").ToTerminal(), terms[1]),
                    //T -> T + n
                    new Production<Token<string>>("T".ToNonTerminal<Token<string>>(), "T".ToNonTerminal<Token<string>>(), (new Token<string>(0, "+", null)).ToTerminal(true), (new Token<string>(0, "n", null)).ToTerminal(true))

                    ////S -> AB
                    //new Production<string>("S".ToNonTerminal(), "A".ToNonTerminal(), "B".ToNonTerminal()),
                    ////A -> aAb
                    //new Production<string>("A".ToNonTerminal(), "a".ToTerminal(), "A".ToNonTerminal(), "b".ToTerminal()),
                    ////A -> a
                    //new Production<string>("A".ToNonTerminal(), "a".ToTerminal()),
                    ////B -> d
                    //new Production<string>("B".ToNonTerminal(), "d".ToTerminal())

                    ////E -> EE
                    //new Production<string>("E".ToNonTerminal(), "E".ToNonTerminal(), "E".ToNonTerminal()),
                    ////E -> E
                    //new Production<string>("E".ToNonTerminal(), "E".ToNonTerminal()),
                    ////E -> a
                    //new Production<string>("E".ToNonTerminal(), "a".ToTerminal()),
                });
            //productions.ToArray());

            //    var closure = grammar.Closure(grammar.Productions[0]);

            //string text = File.ReadAllText("../../Program.cs");

            #region Keywords
            string[] CSharpKeywords = new string[] 
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
            #endregion

            #region Definitions
            TokenDefinitionCollection<string> collection = new TokenDefinitionCollection<string>
            {
                Definitions = new List<TokenDefinition<string>>
                {
                    //precedence is based on the order that they are defined
                    new StringedTokenDefinition(String.Format(@"//.*(?={0})|/\*.*\*/", Environment.NewLine), "COMMENT"),
                    new StringedTokenDefinition(@"#\w+", "PREPROCESSOR_Directive"),
                    new StringedTokenDefinition(@"(\b|^)-?[\d]+(\b|$)", "NUMBER"),
                    new StringedTokenDefinition(@"[\s\n]+", "WHITESPACE"),
                    new StringedTokenDefinition(KeywordDefinitionBuilder.GetPattern(CSharpKeywords), "KEYWORD"),
                    new StringedTokenDefinition(@"(@""([^""]|"""")*""|""(\\.|[^\\""])*"")", "STRING"),
                    new StringedTokenDefinition(@"'.'|'\\['""\\0abfnrtuUxv]'", "CHARACTER_STRING"),
                    new StringedTokenDefinition(@"\.", "DOT"),
                    new StringedTokenDefinition(@"{|\(", "OPEN_BRACE"),
                    new StringedTokenDefinition(@"}|\)", "CLOSE_BRACE"),
                    new StringedTokenDefinition(@";", "SEMICOLIN"),
                    new StringedTokenDefinition(@",", "COMMA"),
                    new StringedTokenDefinition(@"=", "SET_OPERATOR"),
                    new StringedTokenDefinition(@"==", "EQUALS_OPERATOR"),
                    new StringedTokenDefinition(@"!=", "NOT_EQUALS_OPERATOR"),
                    new StringedTokenDefinition(@"<=", "LESS_THAN_OR_EQUAL"),
                    new StringedTokenDefinition(@">=", "GREATER_THAN_OR_EQUAL"),
                    new StringedTokenDefinition(@"<", "LEFT_CARROT"),
                    new StringedTokenDefinition(@">", "RIGHT_CARROT"),
                    new StringedTokenDefinition(@"\+", "PLUS_OPERATOR"),
                    new StringedTokenDefinition(@"-", "MINUS_OPERATOR"),
                    new StringedTokenDefinition(@"\*", "TIMES_OPERATOR"),
                    new StringedTokenDefinition(@"/", "DIVIDE_OPERATOR"),
                    new StringedTokenDefinition(@"\[", "LEFT_BRACKET"),
                    new StringedTokenDefinition(@"\]", "RIGHT_BRACKET"), 
                    new StringedTokenDefinition(@"\b[a-zA-Z@_][\w_]*\b", "IDENTIFIER")
                }
            };
            #endregion

            //Lexer lexer = new Lexer
            //{
            //    Definitions = collection
            //};

            TokenDefinitionCollection<string> defs = new TokenDefinitionCollection<string>(new[]
            {
                new StringedTokenDefinition(@"(\b|^)-?[\d]+(\b|$)", "n"),
                new StringedTokenDefinition(@"\+", "+"),
                new StringedTokenDefinition(@"\(", "("),
                new StringedTokenDefinition(@"\)", ")")
            });

            Lexer lexer = new Lexer();
            lexer.SetDefintions(defs);

            Stopwatch w = Stopwatch.StartNew();

            var tokens = lexer.ReadTokens(txtCFG.Text);

            var c = grammar.Closure(grammar.Productions[0]);

            var graph = grammar.CreateStateGraph();

            var table = new LRParseTable<Token<string>>(graph, grammar.StartElement);

            var parser = new LRParser<Token<string>>();

            parser.SetParseTable(grammar);

            var tree = parser.ParseAST(tokens.Select(a => a.ToTerminal(true)));
            w.Stop();
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

        private void aio(object p)
        {
            string text = (string)p;

            #region Def
            //// build the definitions
            ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            {
                Definitions = new ParserTokenDefinitionCollection<string>
                {

                    Definitions = new List<ParserTokenDefinition<string>>
                    {
                        //map 'n' to a number, define that we should keep this terminal
                        new StringedParserTokenDefinition(@"(?:\b|^)[\d]+(?:\b|$)", "n", true),
                        //map '(', define that we should discard this terminal
                        new StringedParserTokenDefinition(@"\(", "(", false),
                        //map ')', define that we should discard this terminal
                        new StringedParserTokenDefinition(@"\)", ")", false),
                        //map '+', define that we should keep this terminal
                        new StringedParserTokenDefinition(@"\+", "+", false),
                        //map '*'
                        new StringedParserTokenDefinition(@"\*", "*", false),
                        //map '/'
                        new StringedParserTokenDefinition(@"/|\\", "/", false),
                        //map '-'
                        new StringedParserTokenDefinition(@"\-", "-", false),
                        //map '^'
                        new StringedParserTokenDefinition(@"\^", "^", false)
                    }
                },
                Productions = new List<Production<string>>
                {
                    //E -> T
                    new Production<string>("E".ToNonTerminal<string>(false), "T".ToNonTerminal<string>()),

                    //T -> n
                    //the 'n' terminal generated into another terminal which contains the properties to map to a Token with the same TokenType as this terminal's value
                    //therefore 'n' will map to the number token definition defined above.
                    //since 'n'(Terminal) == 'n'(TokenDefinition).TokenType then 'n' maps to Terminal<Token<string>>((new Token(0, 'n', null).ToTerminal(keep, where evaluated token.TokenType == 'n')
                    new Production<string>("T".ToNonTerminal<string>(), "n".ToTerminal()),

                    //E -> ( E )
                    new Production<string>("E".ToNonTerminal<string>(), "(".ToTerminal(), "E".ToNonTerminal<string>(), ")".ToTerminal()),

                    //T -> E + n
                    new Production<string>("T".ToNonTerminal<string>(), "T".ToNonTerminal<string>(false), "+".ToTerminal(), "n".ToTerminal()),

                    //M -> E * n
                    new Production<string>("M".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "*".ToTerminal(), "n".ToTerminal()),

                    //D -> E / n
                    new Production<string>("D".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "/".ToTerminal(), "n".ToTerminal()),

                    //S -> E - n
                    new Production<string>("S".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "-".ToTerminal(), "n".ToTerminal()),

                    //P -> E ^ n
                    new Production<string>("P".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "^".ToTerminal(), "n".ToTerminal()),

                    ////E -> A
                    //new Production<string>("E".ToNonTerminal<string>(false), "A".ToNonTerminal<string>()),

                    //E -> M
                    new Production<string>("E".ToNonTerminal<string>(false), "M".ToNonTerminal<string>()),

                    //E -> D
                    new Production<string>("E".ToNonTerminal<string>(false), "D".ToNonTerminal<string>()),

                    //E -> S
                    new Production<string>("E".ToNonTerminal<string>(false), "S".ToNonTerminal<string>()),
                }
            };
            #endregion

            #region Def
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //{
            //    Definitions = new ParserTokenDefinitionCollection<string>
            //    {
            //        Definitions = new List<ParserTokenDefinition<string>>
            //        {
            //            new KeywordParserTokenDefinition("public", true),
            //            new KeywordParserTokenDefinition("private", true),
            //            new KeywordParserTokenDefinition("protected", true),
            //            new StringedParserTokenDefinition(@"\b\w+\b", "Id", true),
            //            new StringedParserTokenDefinition(@"\(","(", false),
            //            new StringedParserTokenDefinition(@"\)", ")", false),
            //            new StringedParserTokenDefinition(@";", ";", false),
            //            new StringedParserTokenDefinition(@"\{", "{", false),
            //            new StringedParserTokenDefinition(@"\}", "}", false),
            //        }
            //    },
            //    Productions = new List<Production<string>>
            //    {
            //        //Method -> AccessMod Id ( ) { StmtLst }
            //        new Production<string>("Method".ToNonTerminal<string>(), "AccessMod".ToNonTerminal<string>(), "Id".ToTerminal<string>(), "(".ToTerminal(), ")".ToTerminal(), "{".ToTerminal<string>(), "StmtLst".ToNonTerminal<string>(), "}".ToTerminal<string>()),
                    
            //        //AccessMod -> public | private | protected
            //        new Production<string>("AccessMod".ToNonTerminal<string>(), "public".ToTerminal()),
            //        new Production<string>("AccessMod".ToNonTerminal<string>(), "private".ToTerminal()),
            //        new Production<string>("AccessMod".ToNonTerminal<string>(), "protected".ToTerminal()),

            //        new Production<string>("AccessMod".ToNonTerminal<string>()),

            //        //StmtLst -> Stmt
            //        new Production<string>("StmtLst".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //StmtLst -> Stmt StmtLst
            //        new Production<string>("StmtLst".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>(), "StmtLst".ToNonTerminal<string>()),

            //        //Stmt -> Expr ;
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "Expr".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //Expr -> Term
            //        new Production<string>("Expr".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Term -> Id ( Expr )
            //        new Production<string>("Term".ToNonTerminal<string>(), "Id".ToTerminal<string>(), "(".ToTerminal<string>(), "Expr".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Term -> Id ( )
            //        new Production<string>("Term".ToNonTerminal<string>(), "Id".ToTerminal<string>(), "(".ToTerminal<string>(), ")".ToTerminal<string>())                             
            //    }
            //}; 
            #endregion

            #region Def
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //{
            //    Definitions = new ParserTokenDefinitionCollection<string>
            //    {
            //        Definitions = new List<ParserTokenDefinition<string>>
            //        {
            //            new StringedParserTokenDefinition(@"\w+", "id", true),
            //            new StringedParserTokenDefinition(@"->", "->", false),
            //            new StringedParserTokenDefinition(@";|\.", ";", false),
            //            new StringedParserTokenDefinition(@"\|", "|", false),
            //            new StringedParserTokenDefinition(@"\(", "(", false),
            //            new StringedParserTokenDefinition(@"\)", ")", false)
            //        }
            //    },
            //    Productions = new List<Production<string>>
            //    {
            //        //L -> P P
            //        new Production<string>("L".ToNonTerminal<string>(), "P".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //L -> P
            //        new Production<string>("L".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //P -> Term "->" Group ;
            //        new Production<string>("P".ToNonTerminal<string>(), "Term".ToNonTerminal<string>(), "->".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //P -> Term "->" Stmt ;
            //       // new Production<string>("P".ToNonTerminal<string>(), "Term".ToNonTerminal<string>(), "->".ToTerminal<string>(), "Stmt".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //Group ->  Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Group -> Group | Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Group".ToNonTerminal<string>(), "|".ToTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Stmt -> ( Group )
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "(".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Stmt -> Terms
            //        new Production<string>("Stmt".ToNonTerminal<string>(false), "Terms".ToNonTerminal<string>()),

            //        //Terms ->  Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Terms -> Terms Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),



            //        //Stmt -> ( Stmt )
            //        //new Production<string>("Stmt".ToNonTerminal<string>(), "(".ToTerminal<string>(), "Stmt".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Stmt -> Stmt "|" Term
            //        //new Production<string>("Stmt".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>(), "|".ToTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Term -> id
            //        new Production<string>("Term".ToNonTerminal<string>(), "id".ToTerminal<string>())

            //    }
            //}; 
            #endregion

            long totalParseTime = 0;
            long totalLexTime = 0;

            //for (int i = 0; i < 10; i++)
            //{

                if (aioParser == null)
                {
                    aioParser = new AIOLRParser<string>
                    {
                        Definitions = def
                    };

                }
                //LRParseTable<Token<string>> table;

                //if (File.Exists("./ParseTable.tbl.gz"))
                //{
                //    using (var stream = new GZipStream(File.OpenRead("./ParseTable.tbl.gz"), CompressionMode.Decompress))
                //    {
                //        table = LRParseTable<Token<string>>.ReadFromStream(stream);
                //    }
                //}
                //else
                //{
                    
                //    table = aioParser.Parser.ParseTable;
                //    using (var stream = new GZipStream(File.OpenWrite("./ParseTable.tbl.gz"), CompressionLevel.Fastest))
                //    {
                //        table.WriteToStream(stream);
                //    }
                //}

                try
                {
                if (lexer == null)
                {
                    lexer = new Lexer();
                    lexer.SetDefintions(def.Definitions.GetNormalDefinitions());
                }
                Stopwatch w = Stopwatch.StartNew();

                //if (tokens == null)
                //{

                    tokens = lexer.ReadTokens(text);
                //}
                w.Stop();

                Stopwatch sw = Stopwatch.StartNew();
                //get the parse tree from it
                var tree = aioParser.ParseAST(tokens);
                sw.Stop();

                totalLexTime += w.ElapsedMilliseconds;
                totalParseTime += sw.ElapsedMilliseconds;

                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                    return;
                }
                //(new Thread(OnParseDone)).Start(new
                //{
                //    w = w,
                //    sw = sw,
                //    p = tokens.Count(),
                //    l = text.Length
                //});
            //}

            MessageBox.Show(string.Format("Totally done. Average time to lex {0} chars: {1}. Average time to parse {2} tokens: {3}", text.Length, totalLexTime, tokens.Count(), totalParseTime), "Done", MessageBoxButtons.OK);
        }

        private void OnParseDone(dynamic param)
        {
            MessageBox.Show(string.Format("Parsing is done. {0} string chars were lexed in {1} milliseconds. {2} tokens were parsed in {3} milliseconds", param.l, param.w.ElapsedMilliseconds, param.p, param.sw.ElapsedMilliseconds), "Done", MessageBoxButtons.OK);
        }
    }
}
