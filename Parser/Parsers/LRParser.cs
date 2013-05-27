using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Defines an LR(1) parser.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LRParser<T> : IGrammarParser<T>, IGraphParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the end of input element.
        /// </summary>
        public Terminal<T> EndOfInputElement
        {
            get;
            private set;
        }

        /// <summary>
        /// The parse table.
        /// </summary>
        private LRParseTable<T> parseTable;




        /// <summary>
        /// Gets or sets the parse table used to parse input.
        /// </summary>
        /// <exception cref="Parser.InvalidParseTableException">Thrown if an invalid parse table is provided when trying to set the parse table.</exception>
        public LRParseTable<T> ParseTable
        {
            get
            {
                return parseTable;
            }
            set
            {
                foreach (var colRow in value.ActionTable)
                {
                    //if we have a conflict
                    if (colRow.Value.Count > 1)
                    {
                        List<Tuple<ParseTableExceptionType, int, GrammarElement<T>>> conflicts = new List<Tuple<ParseTableExceptionType, int, GrammarElement<T>>>();



                        //ParseTableExceptionType exType;
                        ////if we have a shift-reduce conflict
                        if (colRow.Value.Any(a => a is ShiftAction<T>) && colRow.Value.Any(a => a is ReduceAction<T>))
                        {
                            conflicts.Add(new Tuple<ParseTableExceptionType, int, GrammarElement<T>>(ParseTableExceptionType.SHIFT_REDUCE, colRow.Key.Row, colRow.Key.Column));

                            //then check for a reduce-reduce conflict
                            if (colRow.Value.Where(a => a is ReduceAction<T>).Count() > 1)
                            {
                                conflicts.Add(new Tuple<ParseTableExceptionType, int, GrammarElement<T>>(ParseTableExceptionType.REDUCE_REDUCE, colRow.Key.Row, colRow.Key.Column));
                            }
                        }
                        //otherwise we have a reduce-reduce conflict
                        else
                        {
                            conflicts.Add(new Tuple<ParseTableExceptionType, int, GrammarElement<T>>(ParseTableExceptionType.REDUCE_REDUCE, colRow.Key.Row, colRow.Key.Column));
                        }

                        //throw invalid parse table exception
                        throw new InvalidParseTableException<T>(value, conflicts.ToArray());
                    }
                }
                this.parseTable = value;
            }
        }

        /// <summary>
        /// Parses an Abstract Syntax tree from the given input based on the rules defined in the Terminal elements.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input)
        {
            checkParseTable();

            List<ParseTree<T>.ParseTreebranch> currentBranches = new List<ParseTree<T>.ParseTreebranch>();

            //create a stack to record which states we have been to.
            Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = new Stack<KeyValuePair<int, GrammarElement<T>>>();

            //push the first state
            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(0, null));

            //List<Terminal<T>> augmentedInput = new List<Terminal<T>>(input);
            //augmentedInput.Add(EndOfInputElement);

            Terminal<T>[] augmentedInput = input.Concat(new[] { EndOfInputElement }).ToArray();

            int length = augmentedInput.Length;

            //always true
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    Terminal<T> item = augmentedInput[i];
                    int s = stateStack.Peek().Key;
                    var actions = ParseTable.ActionTable[s, item];

                    if (actions != null)
                    {
                        ParserAction<T> action = actions.First();
                        //if there is an action
                        if (action != null)
                        {
                            //if we should shift
                            if (action is ShiftAction<T>)
                            {
                                //push the state and item
                                stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(((ShiftAction<T>)action).NextState, item));
                            }
                            //otherwise if we should reduce
                            else if (action is ReduceAction<T>)
                            {
                                ReduceAction<T> r = (ReduceAction<T>)action;

                                List<GrammarElement<T>> e = new List<GrammarElement<T>>();

                                //pop the number of elements in the RHS of the item
                                for (int c = 0; c < r.ReduceItem.ProductionElements.Length; c++)
                                {
                                    e.Add(stateStack.Pop().Value);
                                }
                                e.Reverse();

                                //create a new branch with the value as the LHS of the reduction item.
                                ParseTree<T>.ParseTreebranch newBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);

                                //Determine whether to add each element to the new branch based on whether it should be kept.
                                foreach (GrammarElement<T> element in e)
                                {
                                    if (element is NonTerminal<T>)
                                    {
                                        if (element.Keep)
                                        {
                                            //find the first branch that matches the reduce element
                                            var b = currentBranches.First(a => a.Value.Equals(element));
                                            newBranch.AddChild(b);
                                            currentBranches.Remove(b);
                                        }
                                        else
                                        {
                                            //find the first branch that matches the reduce element
                                            var b = currentBranches.First(a => a.Value.Equals(element));
                                            //get the children of the branch since we dont want the current value
                                            IEnumerable<ParseTree<T>.ParseTreebranch> branches = b.GetChildren();
                                            //add the children
                                            newBranch.AddChildren(branches);
                                            currentBranches.Remove(b);
                                        }
                                    }
                                    else
                                    {
                                        //if we should keep the terminal object, then add it to the new branch
                                        if (element.Keep)
                                        {
                                            newBranch.AddChild(new ParseTree<T>.ParseTreebranch(element));
                                        }
                                    }
                                }


                                currentBranches.Add(newBranch);

                                //push the LHS non-terminal and the next state
                                stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(parseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

                                //subtract 1 from i so we don't consume the current item
                                i--;
                            }
                            //otherwise if the parse if finished and we should accept
                            else if (action is AcceptAction<T>)
                            {
                                //return a new ParseTree with the root as the current branch
                                return new ParseResult<T>(true, new ParseTree<T>(currentBranches.First()), stateStack.ToList());
                            }
                        }
                        else
                        {
                            return getSyntaxError(currentBranches, stateStack, item);
                        }
                    }
                    //otherwise, there is no action and a Syntax error has occured.
                    else
                    {
                        return getSyntaxError(currentBranches, stateStack, item);
                    }
                }

                if(currentBranches.Count > 0)
                {
                    return getSyntaxError(currentBranches, stateStack, "END_OF_FILE");
                }
                else
                {
                    return new ParseResult<T>(false, null, stateStack.ToList(), new ParseResult<T>.ParseError("No reductions found for the input"));
                }
            }
            else
            {
                //check for Start -> epsilon
                IEnumerable<ParserAction<T>> actions = parseTable.ActionTable[0, null];
                if (actions != null && actions.Count() > 0)
                {
                    var action = (ReduceAction<T>)actions.First();

                    return new ParseResult<T>(true, new ParseTree<T>(new ParseTree<T>.ParseTreebranch(action.ReduceItem.LeftHandSide)), null);
                }
                else
                {
                    return new ParseResult<T>(false, null, null, new ParseResult<T>.ParseError("Empty input is invalid. There is no reduction of Start -> epsilon"));
                }
            }
        }

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="item">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        private ParseResult<T> getSyntaxError(List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item)
        {
            ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
            root.AddChildren(currentBranches);
            var rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null).Select<Terminal<T>, object>(a =>
            {
                if(a == EndOfInputElement)
                {
                    return "END_OF_INPUT";
                }
                return a;
            });
            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new ParseResult<T>.ParseError(string.Format("Syntax Error. Expected one of {{{0}}} but found \'{1}\'", rows.ConcatArray(", "), item.InnerValue)));
            return result;
        }

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="invalidItem">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        private ParseResult<T> getSyntaxError(List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, string invalidItem)
        {
            ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
            root.AddChildren(currentBranches);
            var rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null).Select<Terminal<T>, object>(a =>
            {
                if (a == EndOfInputElement)
                {
                    return "END_OF_INPUT";
                }
                return a;
            });
            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new ParseResult<T>.ParseError(string.Format("Syntax Error. Expected one of: {{{0}}} but found \'{1}\'", rows.ConcatArray(", "), invalidItem)));
            return result;
        }

        /// <summary>
        /// Parses a concrete Syntax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input)
        {
            checkParseTable();

            List<ParseTree<T>.ParseTreebranch> currentBranches = new List<ParseTree<T>.ParseTreebranch>();

            //create a stack to record which states we have been to.
            Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = new Stack<KeyValuePair<int, GrammarElement<T>>>();

            //push the first state
            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(0, null));

            List<Terminal<T>> augmentedInput = new List<Terminal<T>>(input);
            augmentedInput.Add(EndOfInputElement);
            
            int length = augmentedInput.Count;

            for (int i = 0; i < length; i++)
            {
                Terminal<T> item = augmentedInput[i];
                int s = stateStack.Peek().Key;
                var actions = ParseTable.ActionTable[s, item];

                if (actions != null)
                {
                    ParserAction<T> action = actions.First();
                    //if there is an action
                    if (action != null)
                    {
                        //if we should shift
                        if (action is ShiftAction<T>)
                        {
                            //push the state and item
                            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(((ShiftAction<T>)action).NextState, item));
                        }
                        //otherwise if we should reduce
                        else if (action is ReduceAction<T>)
                        {
                            ReduceAction<T> r = (ReduceAction<T>)action;

                            List<GrammarElement<T>> e = new List<GrammarElement<T>>();

                            //pop the number of elements in the RHS of the item
                            for (int c = 0; c < r.ReduceItem.ProductionElements.Length; c++)
                            {
                                e.Add(stateStack.Pop().Value);
                            }
                            //e.Reverse();

                            ParseTree<T>.ParseTreebranch newBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);

                            foreach (GrammarElement<T> element in e)
                            {
                                if (element is NonTerminal<T>)
                                {
                                    var b = currentBranches.First(a => a.Value.Equals(element));
                                    newBranch.AddChild(b);
                                    currentBranches.Remove(b);
                                }
                                else
                                {
                                    newBranch.AddChild(new ParseTree<T>.ParseTreebranch(element));
                                }
                            }

                            currentBranches.Add(newBranch);

                            //push the LHS non-terminal and the next state
                            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(parseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

                            //subtract 1 from i so we don't consume the current item
                            i--;
                        }
                        //otherwise if the parse if finished and we should accept
                        else if (action is AcceptAction<T>)
                        {
                            //if we have more than one current branch
                            if (currentBranches.Count > 1)
                            {
                                //then something went wrong
                            }
                            //return a new ParseTree with the root as the current branch
                            return new ParseResult<T>(true, new ParseTree<T>(currentBranches.First()), stateStack.ToList());
                        }
                    }
                }
                //otherwise, there is no action and a syntax error has occured.
                else
                {
                    ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
                    root.AddChildren(currentBranches);
                    ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new ParseResult<T>.ParseError(string.Format("Syntax Error. Expected {{{0}}} but found {1}", ParseTable.ActionTable[stateStack.Peek().Key].ConcatArray(", "), item.ToString())));
                }
            }

            if (currentBranches.Count > 0)
            {
                ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
                root.AddChildren(currentBranches);
                return new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new ParseResult<T>.ParseError("No valid accept action found by end of input"));
            }
            else
            {
                return new ParseResult<T>(false, null, stateStack.ToList(), new ParseResult<T>.ParseError("No reductions found for the input"));
            }
        }

        /// <summary>
        /// Throws InvalidOperationException if the parse table is not set.
        /// </summary>
        private void checkParseTable()
        {
            if (ParseTable == null)
            {
                throw new InvalidOperationException("ParseTable must be set before trying to parse.");
            }
        }

        /// <summary>
        /// Sets the Parse Table from the given graph.
        /// </summary>
        /// <param name="graph"></param>
        /// <exception cref="Parser.InvalidParseTableException"/>
        public void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, Terminal<T> endOfInputElement)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph", "The given graph must be non-null");
            }

            if (endOfInputElement == null)
            {
                throw new ArgumentNullException("endOfInputElement");
            }

            if (graph.Root.Value.FirstOrDefault() != null)
            {
                ParseTable = new LRParseTable<T>(graph, graph.Root.Value.First().LeftHandSide);
                this.EndOfInputElement = endOfInputElement;
            }
            else
            {
                throw new ArgumentException("The given graph must have at least one state with at least one item.");
            }
        }

        /// <summary>
        /// Sets Parse Table from the given grammar.
        /// </summary>
        /// <param name="grammar"></param>
        /// <exception cref="Parser.InvalidParseTableException"/>
        public void SetParseTable(ContextFreeGrammar<T> grammar)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar", "The given grammar must be non-null");
            }

            ParseTable = new LRParseTable<T>(grammar.CreateStateGraph(), grammar.StartElement);
            EndOfInputElement = grammar.EndOfInputElement;
        }
    }
}
