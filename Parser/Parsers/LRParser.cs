using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;
using LexicalAnalysis;

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
        protected ParseTable<T> InternalParseTable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the parse table used to parse input.
        /// </summary>
        /// <exception cref="Parser.InvalidParseTableException">Thrown if an invalid parse table is provided when trying to set the parse table.</exception>
        public virtual ParseTable<T> ParseTable
        {
            get
            {
                return InternalParseTable;
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
                this.InternalParseTable = value;
            }
        }


        /// <summary>
        /// Performs an LR(1) parse on the table given the current augmented input, state stack, current branches, and whether to produce a syntax tree or AST.
        /// </summary>
        /// <param name="stateStack"></param>
        /// <param name="currentBranches"></param>
        /// <param name="syntax">Determines whether to produce a syntax tree or AST.</param>
        /// <returns></returns>
        protected ParseResult<T> LRParse(bool syntax, Terminal<T>[] input, int inputProgression = 0, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = null, List<ParseTree<T>.ParseTreebranch> currentBranches = null)
        {
            if (input == null)
            {
                ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
                root.AddChildren(currentBranches);
                return new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList());
            }

            if (stateStack == null)
            {
                stateStack = new Stack<KeyValuePair<int, GrammarElement<T>>>();
                stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(0, null));
            }

            if (currentBranches == null)
            {
                currentBranches = new List<ParseTree<T>.ParseTreebranch>();
            }

            Terminal<T>[] augmentedInput = input.Concat(new[] { EndOfInputElement }).ToArray();

            //cache the length
            int length = augmentedInput.Length;

            if (length > 0 && inputProgression < length)
            {
                for (int i = inputProgression; i < length; i++)
                {
                    //Get the item
                    Terminal<T> item = augmentedInput[i];
                    int s = stateStack.Peek().Key;

                    //Get the possible actions for the current state and item from the table
                    var actions = ParseTable.ActionTable[s, item];

                    if (actions != null)
                    {
                        //SHIFT_REDUCE or REDUCE_REDUCE
                        if (actions.Count > 1)
                        {
                            KeyValuePair<int, GrammarElement<T>> currentState = stateStack.Peek();
                            return new ParseResult<T>(false, new ParseTree<T>(new ParseTree<T>.ParseTreebranch(currentBranches)), stateStack.ToList(), new MultipleActionsParseError<T>("", currentState.Key, item, i, actions.ToArray()));
                        }

                        ParserAction<T> action = actions.SingleOrDefault();
                        //if there is an action
                        if (action != null)
                        {
                            //if we should shift
                            if (action is ShiftAction<T>)
                            {
                                Shift(stateStack, item, action);
                            }
                            //otherwise if we should reduce
                            else if (action is ReduceAction<T>)
                            {
                                if (action is ReduceAction<T>)
                                {
                                    ReduceAction<T> r = (ReduceAction<T>)action;

                                    List<GrammarElement<T>> e = new List<GrammarElement<T>>();

                                    //pop the number of elements in the RHS of the item
                                    for (int c = 0; c < r.ReduceItem.ProductionElements.Length; c++)
                                    {
                                        e.Add(stateStack.Pop().Value);
                                    }
                                    //e.Reverse();

                                    //create a new branch with the value as the LHS of the reduction item.
                                    ParseTree<T>.ParseTreebranch newBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);

                                    //Determine whether to add each element to the new branch based on whether it should be kept.
                                    foreach (GrammarElement<T> element in e)
                                    {
                                        if (element is NonTerminal<T>)
                                        {
                                            if (element.Keep || syntax)
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
                                            if (element == null || element.Keep || syntax)
                                            {
                                                newBranch.AddChild(new ParseTree<T>.ParseTreebranch(element));
                                            }
                                        }

                                    }

                                    currentBranches.Add(newBranch);

                                    //push the LHS non-terminal and the next state
                                    stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(ParseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

                                    i--;
                                }
                            }
                            //otherwise if the parse if finished and we should accept
                            else if (action is AcceptAction<T>)
                            {
                                //return a new ParseTree with the root as the current branch
                                return new ParseResult<T>(true, new ParseTree<T>(currentBranches.First()), stateStack.ToList());
                            }
                            //will never be called, but the compiler will be satisfied...
                            else
                            {
                                return GetSyntaxErrorResult(input, i, currentBranches, stateStack, item);
                            }
                        }
                        else
                        {
                            //no action == syntax error
                            //If you're happy and you know it, SENTAX ERROR!
                            return GetSyntaxErrorResult(input, i, currentBranches, stateStack, item);
                        }
                    }

                    //otherwise, there is no action and a Syntax error has occured.
                    else
                    {
                        return GetSyntaxErrorResult(input, i, currentBranches, stateStack, item);
                    }
                }

                //Found "END_OF_FILE" but was expecting "stuff"
                return GetSyntaxErrorResult(input, inputProgression, currentBranches, stateStack, "END_OF_FILE");
            }
            else
            {
                //check for Start -> epsilon
                IEnumerable<ParserAction<T>> actions = ParseTable.ActionTable[0, null];
                if (actions != null && actions.Count() > 0)
                {
                    var action = (ReduceAction<T>)actions.First();

                    return new ParseResult<T>(true, new ParseTree<T>(new ParseTree<T>.ParseTreebranch(action.ReduceItem.LeftHandSide)), null);
                }
                else
                {
                    return GetSyntaxErrorResult(null, null, EndOfInputElement);
                    //return new ParseResult<T>(false, null, null, new SyntaxParseError<T>("Empty input is invalid. There is no reduction of Start -> epsilon"));
                }
            }
        }


        /// <summary>
        /// Gets a result with a syntax error describing the problem.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="index"></param>
        /// <param name="currentBranches"></param>
        /// <param name="stateStack"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        protected ParseResult<T> GetSyntaxErrorResult(Terminal<T>[] input, int index, List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, string p)
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
            Tuple<int, int> pos = new Tuple<int, int>(input.GetLineNumber(index, a => a == EndOfInputElement), input.GetColumnNumber(index, (a, i) => a == EndOfInputElement));

            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, p, pos, rows.OfType<Terminal<T>>().ToArray()));
            return result;
        }

        /// <summary>
        /// Shifts the given action based on the given item, reflecting the given changes on the given stack.
        /// </summary>
        /// <param name="stateStack"></param>
        /// <param name="item"></param>
        /// <param name="action"></param>
        protected static void Shift(Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item, ParserAction<T> action)
        {
            //push the state and item
            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(((ShiftAction<T>)action).NextState, item));
        }

        /// <summary>
        /// Performs the given action on the given stack, branches, and current index based on the current item.
        /// Returns whether the action was to accept.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="stateStack"></param>
        /// <param name="currentBranches"></param>
        /// <param name="action"></param>
        /// <param name="currentIndex"></param>
        /// <returns></returns>
        protected bool PerformAction(bool syntax, Terminal<T> currentItem, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, List<ParseTree<T>.ParseTreebranch> currentBranches, ParserAction<T> action, ref int currentIndex)
        {
            if (action is ShiftAction<T>)
            {
                Shift(stateStack, currentItem, action);
                currentIndex++;
            }
            else if (action is ReduceAction<T>)
            {
                Reduce(syntax, stateStack, currentBranches, action, ref currentIndex);
                currentIndex++;
            }
            else if (action is AcceptAction<T>)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Reduces the given action, reflecting the changes on the given stack, branches, and index.
        /// </summary>
        /// <param name="syntax"></param>
        /// <param name="stateStack"></param>
        /// <param name="currentBranches"></param>
        /// <param name="action"></param>
        protected void Reduce(bool syntax, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, List<ParseTree<T>.ParseTreebranch> currentBranches, ParserAction<T> action, ref int currentIndex)
        {
            if (action is ReduceAction<T>)
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
                        if (element.Keep || syntax)
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
                        if (element == null || element.Keep || syntax)
                        {
                            newBranch.AddChild(new ParseTree<T>.ParseTreebranch(element));
                        }
                    }

                }

                currentBranches.Add(newBranch);

                //push the LHS non-terminal and the next state
                stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(ParseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

                currentIndex--;
            }
        }

        /// <summary>
        /// Parses an Abstract Syntax tree from the given input based on the rules defined in the Terminal elements.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <returns></returns>
        public virtual ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            CheckParseTable();

            return LRParse(false, input.ToArray());
        }

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="item">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        protected ParseResult<T> GetSyntaxErrorResult(IEnumerable<Terminal<T>> input, int index, List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item)
        {
            ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
            root.AddChildren(currentBranches);
            var rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null);
            Tuple<int, int> pos = new Tuple<int, int>(input.GetLineNumber(index, a => a == EndOfInputElement), input.GetColumnNumber(index, (a, i) => a == EndOfInputElement));

            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, item, pos, rows.ToArray()));
            return result;
        }

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="item">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        protected ParseResult<T> GetSyntaxErrorResult(List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item)
        {
            ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
            root.AddChildren(currentBranches);
            var rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null);
            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, item, new Tuple<int,int>(-1,-1), rows.ToArray()));
            return result;
        }

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="invalidItem">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        protected ParseResult<T> GetSyntaxErrorResult(List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, string invalidItem)
        {
            ParseTree<T>.ParseTreebranch root = new ParseTree<T>.ParseTreebranch((GrammarElement<T>)null);
            root.AddChildren(currentBranches);
            var rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null);
            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, invalidItem, new Tuple<int,int>(-1, -1), rows.ToArray()));
            return result;
        }

        /// <summary>
        /// Parses a concrete Syntax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <exception cref="System.InvalidOperationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        /// <returns></returns>
        public virtual ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            CheckParseTable();

            return LRParse(true, input.ToArray());
        }

        /// <summary>
        /// Throws InvalidOperationException if the parse table is not set.
        /// </summary>
        protected void CheckParseTable()
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
        public void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph", "The given graph must be non-null");
            }

            if (graph.Root != null)
            {
                LRItem<T> firstItem;
                if ((firstItem = graph.Root.Value.FirstOrDefault()) != null)
                {
                    ParseTable = new ParseTable<T>(graph, firstItem.LeftHandSide);
                    this.EndOfInputElement = firstItem.LookaheadElement;
                }
            }

            throw new ArgumentException("The given graph must have at least one state with at least one item.");
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

            ParseTable = new ParseTable<T>((new ParserGenerator<T>{Grammar=grammar}).CreateStateGraph(), grammar.StartElement);
            EndOfInputElement = grammar.EndOfInputElement;
        }
    }
}
