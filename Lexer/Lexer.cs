﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LexicalAnalysis
{
    using System.Text;
    using Defininitions;

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

        public void SetDefintions(TokenDefinitionCollection<string> definitions)
        {
            this.Definitions = definitions;
            buildRegex();
        }

        private void buildRegex()
        {
            StringBuilder b = new StringBuilder();

            int refID = int.MaxValue;

            foreach (var def in Definitions)
            {
                b.AppendFormat("(?<{1}>{0})|", def.Regex.ToString(), refID--);
            }

            //remove the last '|' char
            b = b.Remove(b.Length - 1, 1);

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
            

            //Find all of the matches for all of the definitions,
            //then find which matches encapsulate other matches
            //and remove the contained matches
            //List<Token<string>> matches = new List<Token<string>>();

            MatchCollection[] allMatches = new MatchCollection[Definitions.Count];

            //Get all of the matches based on the regex defined in the definitions
            for (int i = 0; i < Definitions.Count; i++)
            {
                allMatches[i] = Definitions[i].Regex.Matches(input);
            }

            MatchCollection matches = completeRegex.Matches(input);


            //flatten the array of MatchCollection objects into a single Match collection and create Tokens from
            //that collection.
            List<Token<string>> tokens = new List<Token<string>>();
            //itterate through the matches
            foreach (Match m in matches)
            {
                //itterate through the groups
                for (int i = 0; i < Definitions.Count; i++)
                {
                    Group g = m.Groups[(int.MaxValue - i).ToString()];
                    if (g.Success)
                    {
                        tokens.Add(Definitions[i].GetToken(g));
                    }
                }
            }

            tokens = tokens.OrderBy(a => a.Index).ToList();

            //for (int i = 0; i < allMatches.Length; i++)
            //{
            //    int count = allMatches[i].Count;
            //    for (int m = 0; m < count; m++)
            //    {
            //        tokens.Add(Definitions[i].GetToken(allMatches[i][m]));
            //    }
            //}

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
