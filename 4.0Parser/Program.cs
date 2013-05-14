using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Parser
{
    using System.Diagnostics;
    using System.IO;
    using Defininitions;
    using LexicalAnalysis;
    class Program
    {
        static void Main1(string[] args)
        {
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

            TokenDefinitionCollection<string>.WriteToFile("CSharpTokenDefinitions.xml", collection);

            Lexer lexer = new Lexer
            {
                Definitions = collection
            };

            Stopwatch w = Stopwatch.StartNew();

            var ts = lexer.ReadTokens(File.ReadAllText("Program.cs"));

            w.Stop();

            Console.WriteLine(w.ElapsedMilliseconds);

            Token<string>[] tokens = ts.OrderBy(a => a.Index).ToArray();

            //write the token with a certian color based on what it is.
            foreach (Token<string> t in tokens)
            {
                if (t.TokenType == "STRING")
                {
                    WriteColor(t.Value, ConsoleColor.Yellow);
                }
                else if (t.TokenType == "KEYWORD")
                {
                    WriteColor(t.Value, ConsoleColor.Blue);
                }
                else if (t.TokenType == "IDENTIFIER")
                {
                    WriteColor(t.Value, ConsoleColor.White);
                }
                else if (t.TokenType == "NUMBER")
                {
                    WriteColor(t.Value, ConsoleColor.Red);
                }
                else if (t.TokenType == "COMMENT")
                {
                    WriteColor(t.Value, ConsoleColor.DarkGreen);
                }
                else
                {
                    Console.Write(t.Value);
                }
            }

            Console.Read();
        }

        /// <summary>
        /// Writes the given string with the given color to the console
        /// </summary>
        /// <param name="s"></param>
        /// <param name="color"></param>
        public static void WriteColor(string s, ConsoleColor color)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(s);
            Console.ForegroundColor = oldColor;
        }
    }
}
