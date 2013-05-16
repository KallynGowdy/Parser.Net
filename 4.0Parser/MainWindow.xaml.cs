using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Parser.Defininitions;
using Parser.Grammar;
using Parser.LexicalAnalysis;
using Parser.StateMachine;

namespace _4._0Parser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NonTerminal<string>[] terms = new NonTerminal<string>[]
            {
                new NonTerminal<string>("A"),
                new NonTerminal<string>("B")
                //new NonTerminal<string>("E"),
                //new NonTerminal<string>("T")
            };

            ContextFreeGrammar<string> grammar = new ContextFreeGrammar<string>(
                "S".ToNonTerminal(),
                "$".ToTerminal(),
                new Production<string>[]
                {
                    ////A -> A + B
                    //new Production<string>(terms[0], terms[0], new Terminal<string>("+"), terms[1]),
                    ////A -> a
                    //new Production<string>(terms[0], new Terminal<string>("a")),
                    ////B -> b
                    //new Production<string>(terms[1], new Terminal<string>("b"))

                    ////E -> T
                    //new Production<string>(terms[0], terms[1]),
                    ////E -> (E)
                    //new Production<string>(terms[0], ("(").ToTerminal(), terms[0], (")").ToTerminal()),
                    ////T -> n
                    //new Production<string>(terms[1], ("n").ToTerminal()),
                    ////T -> + T
                    //new Production<string>(terms[1], ("+").ToTerminal(), terms[1]),
                    ////T -> T + n
                    //new Production<string>(terms[1], terms[1], ("+").ToTerminal(), ("n").ToTerminal())
                    
                    //S -> AB
                    new Production<string>("S".ToNonTerminal(), "A".ToNonTerminal(), "B".ToNonTerminal()),
                    //A -> aAb
                    new Production<string>("A".ToNonTerminal(), "a".ToTerminal(), "A".ToNonTerminal(), "b".ToTerminal()),
                    //A -> a
                    new Production<string>("A".ToNonTerminal(), "a".ToTerminal()),
                    //B -> d
                    new Production<string>("B".ToNonTerminal(), "d".ToTerminal())
                });

            //    var closure = grammar.Closure(grammar.Productions[0]);

            string text = File.ReadAllText("../../Program.cs");

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

            var tokens = lexer.ReadTokens(text);

            var c = grammar.Closure(grammar.Productions[0]);

            var graph = grammar.CreateStateGraph();

            var table = new ParseTable<string>(graph, grammar.StartElement);
            w.Stop();

        }


    }
}
