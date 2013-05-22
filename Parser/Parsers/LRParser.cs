using System;
using System.Collections.Generic;
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
    public class LRParser<T> : IGrammarParser<T>, IGraphParser<T>
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
                        ParseTableExceptionType exType;
                        //if we have a shift-reduce conflict
                        if (colRow.Value.Any(a => a is ShiftAction<T>) && colRow.Value.Any(a => a is ReduceAction<T>))
                        {
                            exType = ParseTableExceptionType.SHIFT_REDUCE;

                            //then check for a reduce-reduce conflict
                            if (colRow.Value.Where(a => a is ReduceAction<T>).Count() > 1)
                            {
                                exType = ParseTableExceptionType.SHIFT_REDUCE | ParseTableExceptionType.REDUCE_REDUCE;
                            }
                        }
                        //otherwise we have a reduce-reduce conflict
                        else
                        {
                            exType = ParseTableExceptionType.REDUCE_REDUCE;
                        }

                        //throw invalid parse table exception
                        throw new InvalidParseTableException<T>(exType, value);
                    }
                }
                this.parseTable = value;
            }
        }

        /// <summary>
        /// Parses an Abstract sentax tree from the given input based on the rules defined in the Terminal elements.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseTree<T> ParseAST(IEnumerable<Terminal<T>> input)
        {
            checkParseTable();

            ParseTree<T>.ParseTreebranch currentBranch = null;

            //create a stack to record which states we have been to.
            Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = new Stack<KeyValuePair<int, GrammarElement<T>>>();

            //push the first state
            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(0, null));

            List<Terminal<T>> augmentedInput = new List<Terminal<T>>(input);
            augmentedInput.Add(EndOfInputElement);

            for (int i = 0; i < augmentedInput.Count; i++)
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

                            //set the current branch if it is null
                            if (currentBranch == null)
                            {
                                currentBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);
                                currentBranch.AddChildren(e.Where(a =>
                                {
                                    if (a is Terminal<T>)
                                    {
                                        return ((Terminal<T>)a).Keep;
                                    }
                                    return true;
                                }).Select(a => new ParseTree<T>.ParseTreebranch(a)));
                            }
                            //otherwise create a new branch and set that as the parent of the current branch
                            else
                            {
                                ParseTree<T>.ParseTreebranch newBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);
                                newBranch.AddChild(currentBranch);

                                //add all of the terminal and non terminal elements that should be kept
                                newBranch.AddChildren(e.Where(a =>
                                {
                                    if (!a.Equals(currentBranch.Value))
                                    {
                                        if (a is Terminal<T>)
                                        {
                                            return ((Terminal<T>)a).Keep;
                                        }

                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }).Select(a => new ParseTree<T>.ParseTreebranch(a)));

                                currentBranch = newBranch;
                            }


                            //push the LHS non-terminal and the next state
                            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(parseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

                            //subtract 1 from i so we don't consume the current item
                            i--;
                        }
                        //otherwise if the parse if finished and we should accept
                        else if (action is AcceptAction<T>)
                        {
                            //return a new ParseTree with the root as the current branch
                            return new ParseTree<T>(currentBranch);
                        }
                    }
                }
                //otherwise, there is no action and a sentax error has occured.
                else
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Parses a concrete sentax tree from the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ParseTree<T> ParseSentaxTree(IEnumerable<Terminal<T>> input)
        {
            checkParseTable();
            ParseTree<T>.ParseTreebranch currentBranch = null;

            //create a stack to record which states we have been to.
            Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = new Stack<KeyValuePair<int, GrammarElement<T>>>();

            //push the first state
            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(0, null));

            List<Terminal<T>> augmentedInput = new List<Terminal<T>>(input);
            augmentedInput.Add(EndOfInputElement);

            for (int i = 0; i < augmentedInput.Count; i++)
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

                            //set the current branch if it is null
                            if (currentBranch == null)
                            {
                                currentBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);
                                currentBranch.AddChildren(e.Where(a =>
                                {
                                    if (a is Terminal<T>)
                                    {
                                        return ((Terminal<T>)a).Keep;
                                    }
                                    return true;
                                }).Select(a => new ParseTree<T>.ParseTreebranch(a)));
                            }
                            //otherwise create a new branch and set that as the parent of the current branch
                            else
                            {
                                ParseTree<T>.ParseTreebranch newBranch = new ParseTree<T>.ParseTreebranch(r.ReduceItem.LeftHandSide);
                                newBranch.AddChild(currentBranch);

                                //add all of the terminal and non terminal elements
                                newBranch.AddChildren(e.Select(a => new ParseTree<T>.ParseTreebranch(a)));

                                currentBranch = newBranch;
                            }


                            //push the LHS non-terminal and the next state
                            stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(parseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

                            //subtract 1 from i so we don't consume the current item
                            i--;
                        }
                        //otherwise if the parse if finished and we should accept
                        else if (action is AcceptAction<T>)
                        {
                            //return a new ParseTree with the root as the current branch
                            return new ParseTree<T>(currentBranch);
                        }
                    }
                }
                //otherwise, there is no action and a sentax error has occured.
                else
                {
                    return null;
                }
            }
            return null;
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
