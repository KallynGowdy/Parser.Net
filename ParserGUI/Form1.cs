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
            ////// build the definitions
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //(
            //    new ParserTokenDefinitionCollection<string>
            //    (

            //        new List<ParserTokenDefinition<string>>
            //        {
            //            //map 'n' to a number, define that we should keep this terminal
            //            new StringedParserTokenDefinition(@"(?:\b|^)[\d]+(?:\b|$)", "n", true),
            //            //map '(', define that we should discard this terminal
            //            new StringedParserTokenDefinition(@"\(", "(", false),
            //            //map ')', define that we should discard this terminal
            //            new StringedParserTokenDefinition(@"\)", ")", false),
            //            //map '+', define that we should keep this terminal
            //            new StringedParserTokenDefinition(@"\+", "+", true),
            //            //map '*'
            //            new StringedParserTokenDefinition(@"\*", "*", false),
            //            //map '/'
            //            new StringedParserTokenDefinition(@"/|\\", "/", false),
            //            //map '-'
            //            new StringedParserTokenDefinition(@"\-", "-", false),
            //            //map '^'
            //            new StringedParserTokenDefinition(@"\^", "^", false)
            //        }
            //    ),
            //    new List<Production<string>>
            //    {
            //        //E -> T
            //        new Production<string>("E".ToNonTerminal<string>(), "T".ToNonTerminal<string>()),

            //        //T -> n
            //        //the 'n' terminal generated into another terminal which contains the properties to map to a Token with the same TokenType as this terminal's value
            //        //therefore 'n' will map to the number token definition defined above.
            //        //since 'n'(Terminal) == 'n'(TokenDefinition).TokenType then 'n' maps to Terminal<Token<string>>((new Token(0, 'n', null).ToTerminal(keep, where evaluated token.TokenType == 'n')
            //        new Production<string>("T".ToNonTerminal<string>(), "n".ToTerminal()),

            //        //E -> ( E )
            //        new Production<string>("E".ToNonTerminal<string>(), "(".ToTerminal(), "E".ToNonTerminal<string>(), ")".ToTerminal()),

            //        //E -> E + T
            //        new Production<string>("E".ToNonTerminal<string>(), "E".ToNonTerminal<string>(), "+".ToTerminal(), "T".ToNonTerminal<string>()),

            //        ////M -> E * n
            //        //new Production<string>("M".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "*".ToTerminal(), "n".ToTerminal()),

            //        ////D -> E / n
            //        //new Production<string>("D".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "/".ToTerminal(), "n".ToTerminal()),

            //        ////S -> E - n
            //        //new Production<string>("S".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "-".ToTerminal(), "n".ToTerminal()),

            //        ////P -> E ^ n
            //        //new Production<string>("P".ToNonTerminal<string>(), "E".ToNonTerminal<string>(false), "^".ToTerminal(), "n".ToTerminal()),

            //        //////E -> A
            //        ////new Production<string>("E".ToNonTerminal<string>(false), "A".ToNonTerminal<string>()),

            //        ////E -> M
            //        //new Production<string>("E".ToNonTerminal<string>(false), "M".ToNonTerminal<string>()),

            //        ////E -> D
            //        //new Production<string>("E".ToNonTerminal<string>(false), "D".ToNonTerminal<string>()),

            //        ////E -> S
            //        //new Production<string>("E".ToNonTerminal<string>(false), "S".ToNonTerminal<string>()),
            //    }
            //);
            #endregion

            #region Def
            //C# Definition
            ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            (
                new ParserTokenDefinitionCollection<string>
                (
                    new List<ParserTokenDefinition<string>>
                    {
                        //Matches to the 'public' keyword, we should keep this keyword.
                        new KeywordParserTokenDefinition("public", true),
                        //Matches to the 'private' keyword, we should keep this keyword.
                        new KeywordParserTokenDefinition("private", true),
                        //Matches to the 'protected' keyword, we should keep this keyword.
                        new KeywordParserTokenDefinition("protected", true),
                        //Matches to a word of arbitrary length as an identifier, we should keep this.
                        new StringedParserTokenDefinition(@"\b\w+\b", "Id", true),
                        //matches to an opening parenthese, we should discard this token.
                        new StringedParserTokenDefinition(@"\(","(", false),
                        //matches to an closing parenthese, we should discard this token.
                        new StringedParserTokenDefinition(@"\)", ")", false),
                        //matches to a semicolin, we should discard this token.
                        new StringedParserTokenDefinition(@";", ";", false),
                        //matches to an opening curly-brace, we should discard this token.
                        new StringedParserTokenDefinition(@"\{", "{", false),
                        //matches to a closing curly-brace, we should discard this token.
                        new StringedParserTokenDefinition(@"\}", "}", false),
                        //matches to a comma, we should discard this token
                        new StringedParserTokenDefinition(@",", ",", false),
                        //matches to the plus sign, we should keep this token
                        new StringedParserTokenDefinition(@"\+", "+", true),
                        //matches to the minus sign, we should keep this token
                        new StringedParserTokenDefinition(@"\-", "-", true),
                        //matches to the times sign, we should keep this token
                        new StringedParserTokenDefinition(@"\*", "*", true),
                        //matches to the division sign, we should keep this token
                        new StringedParserTokenDefinition(@"/", "/", true),
                        //matches to the assignment operator, we should keep this token
                        new StringedParserTokenDefinition(@"=", "=", true),
                    }
                ),
                new List<Production<string>>
                {
                    //Method -> AccessMod Id ( ) { StmtLst }
                    new Production<string>("Method".ToNonTerminal(), "AccessMod".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), ")".ToTerminal(), "{".ToTerminal(), "StmtLst".ToNonTerminal(), "}".ToTerminal()),

                    //Method -> AccessMod Id ( ArgLst ) { StmtLst }
                    new Production<string>("Method".ToNonTerminal(), "AccessMod".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), "ArgLst".ToNonTerminal(), ")".ToTerminal(), "{".ToTerminal(), "StmtLst".ToNonTerminal(), "}".ToTerminal()),

                    //AccessMod -> public | private | protected
                    new Production<string>("AccessMod".ToNonTerminal(), "public".ToTerminal()),
                    new Production<string>("AccessMod".ToNonTerminal(), "private".ToTerminal()),
                    new Production<string>("AccessMod".ToNonTerminal(), "protected".ToTerminal()),

                    //AccessMod -> nothing
                    new Production<string>("AccessMod".ToNonTerminal()),

                    //StmtLst -> Stmt
                    new Production<string>("StmtLst".ToNonTerminal(), "Stmt".ToNonTerminal()),

                    //StmtLst -> Stmt StmtLst
                    new Production<string>("StmtLst".ToNonTerminal(), "Stmt".ToNonTerminal(), "StmtLst".ToNonTerminal()),

                    //Stmt -> Expr ;
                    new Production<string>("Stmt".ToNonTerminal(), "Expr".ToNonTerminal(), ";".ToTerminal()),

                    //Expr -> Term
                    new Production<string>("Expr".ToNonTerminal(), "Term".ToNonTerminal()),

                    //Expr -> Id = Term
                    new Production<string>("Expr".ToNonTerminal(), "Id".ToTerminal(), "=".ToTerminal(), "Term".ToNonTerminal()),

                    //Term -> Expr BiOp Term
                    new Production<string>("Expr".ToNonTerminal(), "Expr".ToNonTerminal(), "BiOp".ToNonTerminal(), "Term".ToNonTerminal()),

                    //Term -> UnaryOp Term
                    new Production<string>("Term".ToNonTerminal(), "UnaryOp".ToNonTerminal(), "Term".ToNonTerminal()),

                    //Term -> ( Term )
                    new Production<string>("Term".ToNonTerminal(), "(".ToTerminal(), "Term".ToNonTerminal(), ")".ToTerminal()),

                    //BiOp -> +
                    new Production<string>("BiOp".ToNonTerminal(), "+".ToTerminal()),

                    //BiOp -> -
                    new Production<string>("BiOp".ToNonTerminal(), "-".ToTerminal()),

                    //BiOp -> *
                    new Production<string>("BiOp".ToNonTerminal(), "*".ToTerminal()),

                    //BiOp -> /
                    new Production<string>("BiOp".ToNonTerminal(), "/".ToTerminal()),

                    //UnaryOp -> -
                    new Production<string>("UnaryOp".ToNonTerminal(), "-".ToTerminal()),

                    //Term -> Id ( ParamLst )
                    new Production<string>("Term".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), "ParamLst".ToNonTerminal(), ")".ToTerminal()),

                    //Term -> Id ( )
                    new Production<string>("Term".ToNonTerminal(), "Id".ToTerminal(), "(".ToTerminal(), ")".ToTerminal()),
                    
                    //Term -> Id
                    new Production<string>("Term".ToNonTerminal(), "Id".ToTerminal()),

                    //Defines an argument list(e.g. the signature of the method)

                    //ArgLst -> ArgLst, Arg
                    new Production<string>("ArgLst".ToNonTerminal(), "ArgLst".ToNonTerminal(), ",".ToTerminal(), "Arg".ToNonTerminal()),

                    //ArgLst -> nothing
                    //new Production<string>("ArgLst".ToNonTerminal()),

                    //ArgLst -> Arg
                    new Production<string>("ArgLst".ToNonTerminal(), "Arg".ToNonTerminal()),

                    //Arg -> Id Id
                    new Production<string>("Arg".ToNonTerminal(), "Id".ToTerminal(), "Id".ToTerminal()),

                    //ParamLst -> Param
                    new Production<string>("ParamLst".ToNonTerminal(), "Param".ToNonTerminal()),

                    //ParamLst -> ParamLst , Param
                    new Production<string>("ParamLst".ToNonTerminal(), "ParamLst".ToNonTerminal(), ",".ToTerminal(), "Param".ToNonTerminal()),

                    //Param -> Term
                    new Production<string>("Param".ToNonTerminal(), "Expr".ToNonTerminal())
                    
                }
            ); 
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

            //        //Groups -> Group
            //        new Production<string>("Groups".ToNonTerminal<string>(), "Group".ToNonTerminal<string>()),

            //        //Groups -> Groups Group
            //        new Production<string>("Groups".ToNonTerminal<string>(), "Groups".ToNonTerminal<string>(), "Group".ToNonTerminal<string>()),

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

            #region Def
            //ParserProductionTokenDefinition<string> def = new ParserProductionTokenDefinition<string>
            //(
            //    new ParserTokenDefinitionCollection<string>
            //    (
            //        new List<ParserTokenDefinition<string>>
            //        {
            //            new StringedParserTokenDefinition(@"\w+", "id", true),
            //            new StringedParserTokenDefinition(@"->", "->", false),
            //            new StringedParserTokenDefinition(@";|\.", ";", false),
            //            new StringedParserTokenDefinition(@"\|", "|", false),
            //            new StringedParserTokenDefinition(@"\(", "(", false),
            //            new StringedParserTokenDefinition(@"\)", ")", false)
            //        }
            //    ),
            //    new List<Production<string>>
            //    {
            //        //L -> P P
            //        new Production<string>("L".ToNonTerminal<string>(), "L".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //L -> P
            //        new Production<string>("L".ToNonTerminal<string>(), "P".ToNonTerminal<string>()),

            //        //P -> Term "->" Group ;
            //        new Production<string>("P".ToNonTerminal<string>(), "Term".ToNonTerminal<string>(), "->".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ";".ToTerminal<string>()),

            //        //Group ->  Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Group -> Stmt | Stmt
            //        new Production<string>("Group".ToNonTerminal<string>(), "Stmt".ToNonTerminal<string>(), "|".ToTerminal<string>(), "Stmt".ToNonTerminal<string>()),

            //        //Stmt -> ( Group )
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "(".ToTerminal<string>(), "Group".ToNonTerminal<string>(), ")".ToTerminal<string>()),

            //        //Stmt ->  Terms
            //        new Production<string>("Stmt".ToNonTerminal<string>(), "Terms".ToNonTerminal<string>()),

            //        //Terms -> Terms Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Terms -> Term
            //        new Production<string>("Terms".ToNonTerminal<string>(), "Term".ToNonTerminal<string>()),

            //        //Term -> id
            //        new Production<string>("Term".ToNonTerminal<string>(), "id".ToTerminal<string>())
            //    }
            //); 
            #endregion

            long totalParseTime = 0;
            long totalLexTime = 0;


            LRParser<Token<string>> parser = new LRParser<Token<string>>();
            parser.SetParseTable(def.GetGrammar());


            if (lexer == null)
            {
                lexer = new Lexer();
                lexer.SetDefintions(def.Definitions.GetNormalDefinitions());
            }
            Stopwatch w = Stopwatch.StartNew();


            tokens = lexer.ReadTokens(text);

            w.Stop();

            Stopwatch sw = Stopwatch.StartNew();
            
            ParseResult<Token<string>> tree = parser.ParseAST(def.ConvertToTerminals(tokens));
            
            sw.Stop();

            totalLexTime += w.ElapsedMilliseconds;
            totalParseTime += sw.ElapsedMilliseconds;

            MessageBox.Show(string.Format("Totally done. Average time to lex {0} chars: {1}. Average time to parse {2} tokens: {3}", text.Length, totalLexTime, tokens.Count(), totalParseTime), "Done", MessageBoxButtons.OK);
        }

        private void OnParseDone(dynamic param)
        {
            MessageBox.Show(string.Format("Parsing is done. {0} string chars were lexed in {1} milliseconds. {2} tokens were parsed in {3} milliseconds", param.l, param.w.ElapsedMilliseconds, param.p, param.sw.ElapsedMilliseconds), "Done", MessageBoxButtons.OK);
        }
    }
}
