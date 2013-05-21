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
using LexicalAnalysis.Defininitions;
using LexicalAnalysis;

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
                            element.Add(el.ToNonTerminal());
                        }
                        else
                        {
                            element.Add(el.ToTerminal());
                        }
                    }

                    productions.Add(new Production<string>(split[0].ToNonTerminal(), element.ToArray()));
                }
            }

            ContextFreeGrammar<string> grammar = new ContextFreeGrammar<string>(
                productions.First().NonTerminal,
                "$".ToTerminal(),
                //new Production<string>[]
                //{
                //    ////A -> A + B
                //    //new Production<string>(terms[0], terms[0], new Terminal<string>("+"), terms[1]),
                //    ////A -> a
                //    //new Production<string>(terms[0], new Terminal<string>("a")),
                //    ////B -> b
                //    //new Production<string>(terms[1], new Terminal<string>("b"))

                //    ////E -> T
                //    //new Production<string>(terms[0], terms[1]),
                //    ////E -> (E)
                //    //new Production<string>(terms[0], ("(").ToTerminal(), terms[0], (")").ToTerminal()),
                //    ////T -> n
                //    //new Production<string>(terms[1], ("n").ToTerminal()),
                //    ////T -> + T
                //    //new Production<string>(terms[1], ("+").ToTerminal(), terms[1]),
                //    ////T -> T + n
                //    //new Production<string>(terms[1], terms[1], ("+").ToTerminal(), ("n").ToTerminal())

                //    ////S -> AB
                //    //new Production<string>("S".ToNonTerminal(), "A".ToNonTerminal(), "B".ToNonTerminal()),
                //    ////A -> aAb
                //    //new Production<string>("A".ToNonTerminal(), "a".ToTerminal(), "A".ToNonTerminal(), "b".ToTerminal()),
                //    ////A -> a
                //    //new Production<string>("A".ToNonTerminal(), "a".ToTerminal()),
                //    ////B -> d
                //    //new Production<string>("B".ToNonTerminal(), "d".ToTerminal())

                //    //E -> EE
                //    new Production<string>("E".ToNonTerminal(), "E".ToNonTerminal(), "E".ToNonTerminal()),
                //    //E -> E
                //    new Production<string>("E".ToNonTerminal(), "E".ToNonTerminal()),
                //    //E -> a
                //    new Production<string>("E".ToNonTerminal(), "a".ToTerminal()),
                //});
                productions.ToArray());

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

            Lexer lexer = new Lexer
            {
                Definitions = collection
            };

            Stopwatch w = Stopwatch.StartNew();

            //var tokens = lexer.ReadTokens(text);

            var c = grammar.Closure(grammar.Productions[0]);

            var graph = grammar.CreateStateGraph();

            var table = new LRParseTable<string>(graph, grammar.StartElement);
            w.Stop();

            List<dynamic> gridCollection = new List<dynamic>();

            foreach (var item in table.ActionTable.Select(a => new
            {
                State = a.Key.Row,
                Input = a.Key.Column,
                Value = a.Value
            }))
            {
                gridCollection.Add(item);
            }

            foreach (var item in table.GotoTable.Select(a => new
            {
                State = a.Key.Row,
                Input = a.Key.Column,
                Value = a.Value
            }))
            {
                gridCollection.Add(item);
            }
            //grdTable.AutoGenerateColumns = true;

            grdTable.Columns.Clear();

            grdTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            foreach (dynamic column in gridCollection.OrderBy(a => a is Terminal<string>).Select(a => a.Input).DistinctBy(a => a.ToString()))
            {
                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.HeaderText = column.ToString();
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                grdTable.Columns.Add(col);
            }
            

            grdTable.Columns[grdTable.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            grdTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            var rows = gridCollection.GroupBy(a => a.State).Select(a => new List<dynamic>(a)).ToArray();
            for (int i = 0; i < rows.Length; i++)
            {
                List<dynamic> row = new List<dynamic>();
                for (int r = 0; r < grdTable.Columns.Count; r++)
                {

                    if (r < rows[i].Count())
                    {
                        if (rows[i].Any(a =>
                        {
                            if (a != null)
                            {
                                return a.Input.ToString().Equals(grdTable.Columns[r].HeaderText);
                            }
                            return false;
                        }))
                        {
                            row.Add(concatArray(rows[i].First(a =>
                            {
                                if (a != null)
                                {
                                    return a.Input.ToString().Equals(grdTable.Columns[r].HeaderText);
                                }
                                return false;
                            }).Value, "\n"));
                        }
                        else
                        {
                            row.Add(null);
                            rows[i].Insert(r, null);
                        }
                    }
                }
                grdTable.Rows.Add(row.ToArray());
                grdTable.Rows[i].HeaderCell.Value = i.ToString();
                
            }

            foreach(DataGridViewColumn column in grdTable.Columns)
            {
                int colw = column.Width;
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                column.Width = colw;
            }
        }

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
    }
}
