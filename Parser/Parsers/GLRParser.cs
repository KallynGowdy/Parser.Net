using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Parser.Grammar;
using Parser.Collections;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Provides an implementation of a GLR Parser that can parse any table.
    /// </summary>
    public class GLRParser<T> : LRParser<T> where T : IEquatable<T>
    {

        /// <summary>
        /// Gets or sets the parse table to use.
        /// </summary>
        public override ParseTable<T> ParseTable
        {
            get
            {
                return parseTable;
            }
            set
            {
                parseTable = value;
            }
        }

        protected ParseResult<T>[] ParseAbstractSyntaxTrees(IEnumerable<Terminal<T>> input, int inputProgression = 0, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = null, List<ParseTree<T>.ParseTreebranch> currentBranches = null, List<ParseResult<T>> results = null)
        {
            if (results == null)
            {
                results = new List<ParseResult<T>>();
            }

            //run in parallel
            ThreadPool.QueueUserWorkItem(a =>
            {
                //parse
                ParseResult<T> result = LRParse(false, input.ToArray(), inputProgression, stateStack, currentBranches);

                //if not successfull
                if (!result.Success)
                {
                    
                    if (result.Errors.Count > 0)
                    {
                        //if there are multiple parse actions for the current state.
                        if (result.Errors[0] is MultipleActionsParseError<T>)
                        {
                            MultipleActionsParseError<T> e = (MultipleActionsParseError<T>)result.Errors[0];

                            //parse each action in its own thread.
                            foreach (ParserAction<T> action in e.PossibleActions)
                            {
                                //TODO: Perform the action and then LRParse to prevent loops
                                ParseAbstractSyntaxTrees(input, e.Progression, new Stack<KeyValuePair<int, GrammarElement<T>>>(result.Stack), new List<ParseTree<T>.ParseTreebranch>(result.GetParseTree().Root.Children), results);
                            }
                        }
                    }
                    lock (results)
                    {
                        results.Add(result);
                    }
                }
                else
                {
                    lock (results)
                    {
                        results.Add(result);
                    }
                }
            });

            return results.ToArray();
        }

        public ParseResult<T>[] ParseAbstractSyntaxTrees(IEnumerable<Terminal<T>> input)
        {
            return ParseAbstractSyntaxTrees(input, 0);
        }

        public override ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input)
        {
            return base.ParseAST(input);
        }

        public override ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input)
        {
            return base.ParseSyntaxTree(input);
        }
    }
}
