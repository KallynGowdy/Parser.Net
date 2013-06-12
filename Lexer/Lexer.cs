using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LexicalAnalysis
{
    using System;
using System.Security.Permissions;
using System.Text;
using Definitions;

    /// <summary>
    /// Defines an exception that is thrown when an unexpected character is encountered.
    /// </summary>
    [Serializable]
    [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
    public class SyntaxErrorException : Exception
    {
        /// <summary>
        /// Gets the line number that the Syntax error occured at. (Default = -1)
        /// </summary>
        public int LineNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the column number that the Syntax error occured at. (Default = -1)
        /// </summary>
        public int ColumnNumber
        {
            get;
            private set;
        }

        private static string buildExceptionMessage(int lineNumber, int columnNumber, string message)
        {
            return string.Format("Syntax error occured:\nLineNumber: {0}\nColumnNumber: {1}\nMessage: {2}", lineNumber, columnNumber, message);
        }

        public SyntaxErrorException(int lineNumber, int columnNumber, string message, Exception innerException = null)
            : base(buildExceptionMessage(lineNumber, columnNumber, message), innerException)
        {
            this.LineNumber = lineNumber;
            this.ColumnNumber = columnNumber;
        }

        public SyntaxErrorException(string message)
            : this(-1, -1, message)
        {
        }

        public SyntaxErrorException(string message, Exception innerException)
            : this(-1, -1, message, innerException)
        {
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("LineNumber", LineNumber);
            info.AddValue("ColumnNumber", ColumnNumber);
            base.GetObjectData(info, context);
        }
    }

    /// <summary>
    /// Defines a class that, given a set of definitions, can convert a given string into a set of tokens.
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// The definitions to use for lexing.
        /// </summary>
        public TokenDefinitionCollection<string> Definitions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets wether to throw a syntax error when an unidentified character is encountered.
        /// Default = true.
        /// </summary>
        public bool ThrowSyntaxErrors
        {
            get;
            set;
        }

        public Lexer()
        {
            ThrowSyntaxErrors = true;
        }

        public void SetDefintions(TokenDefinitionCollection<string> definitions)
        {
            this.Definitions = definitions;
            buildRegex();
        }

        /// <summary>
        /// Generates a regular expression object from the definitions set in the lexer.
        /// </summary>
        private void buildRegex()
        {
            StringBuilder b = new StringBuilder();

            int refID = int.MaxValue;

            foreach (var def in Definitions)
            {
                //add the regex definition as a named group with the id counting down from int.MaxValue
                b.AppendFormat("(?<{1}>{0})|", def.Regex.ToString(), refID--);
            }

            //append the error group to the regex, if we get to this group, then there is a Syntax error.
            b.AppendFormat(@"(?<{0}>[^\s\s])", refID);


            completeRegex = new Regex(b.ToString());
        }

        /// <summary>
        /// The complete regular expression that defines all of the definitions in one.
        /// </summary>
        private Regex completeRegex;

        /// <summary>
        /// Reads an array of Token(string) objects from the input string.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <returns></returns>
        public Token<string>[] ReadTokens(string input)
        {
            //get the matches from our generated regex object.
            MatchCollection matches = completeRegex.Matches(input);


            List<Token<string>> tokens = new List<Token<string>>();

            //itterate through the matches
            foreach (Match m in matches)
            {
                //itterate through the groups
                for (int i = 0; i < Definitions.Count + 1; i++)
                {
                    int currentIndex = (int.MaxValue - i);
                    Group g = m.Groups[currentIndex];
                    if (g.Success)
                    {
                        //if we have not reached the last group. if we have then a Syntax error has occured.
                        if (currentIndex > int.MaxValue - Definitions.Count)
                        {
                            tokens.Add(Definitions[i].GetToken(g));
                        }
                        else
                        {
                            //throw a syntax error if we should.
                            if (ThrowSyntaxErrors)
                            {
                                int lineNumber = input.GetLineNumber(g.Index);

                                int columnNumber = input.GetColumnNumber(g.Index);

                                //Syntax error
                                throw new SyntaxErrorException(lineNumber, columnNumber, string.Format("Found {0}.", g.Value));
                            }
                            //otherwise, add as a token with TokenType 'UNKNOWN'.
                            else
                            {
                                tokens.Add(new Token<string>(g.Index, TokenTypes.UNKNOWN, g.Value));
                            }
                        }
                    }
                }
            }

            tokens = tokens.OrderBy(a => a.Index).ToList();


            //tokens = allMatches.SelectMany<MatchCollection, Token<string>>((a, i) => a.Cast<Match>().Select(t => Definitions[i].GetToken(t))).OrderBy(t => t.Index).ToList();

            //itterate though the matches backwards and remove all matches that are contained by the next/last
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i + 1 < tokens.Count)
                {
                    //remove the next token from the list if it is contained by this token.
                    //Because we already start with all of the tokens in the list, we only need to remove
                    //the invalid tokens.
                    if (tokens[i].Index + tokens[i].Value.Length > tokens[i + 1].Index)
                    {
                        tokens.RemoveAt(i + 1);
                        //move i backward to remain at this index
                        //this way we can remove all of the future tokens that are contained by this token
                        i--;
                    }
                }
            }
            return tokens.ToArray();
        }
    }
}
