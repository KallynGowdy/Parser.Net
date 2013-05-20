﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser.Grammar;

namespace Parser.StateMachine
{


    /// <summary>
    /// Provides an implementation of a DFA table.
    /// </summary>
    public class ParseTable<T>
    {
        /// <summary>
        /// Provides an abstract class that defines an action that the parser should take when input is read based on the current state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class ParserAction
        {
            /// <summary>
            /// Gets the parse table that the Action uses to determine what to perform.
            /// </summary>
            public ParseTable<T> ParseTable
            {
                get;
                private set;
            }

            public ParserAction(ParseTable<T> table)
            {
                this.ParseTable = table;
            }
        }

        /// <summary>
        /// Provides a class that defines that a "shift" should occur at the location that the action is stored in the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class ShiftAction : ParserAction
        {
            /// <summary>
            /// Gets the next state that the parser should move to.
            /// </summary>
            public int NextState
            {
                get;
                private set;
            }

            public ShiftAction(ParseTable<T> table, int nextState)
                : base(table)
            {
                if (nextState < 0)
                {
                    throw new ArgumentOutOfRangeException("nextState", "Must be greater than or equal to 0");
                }
                this.NextState = nextState;
            }
        }

        /// <summary>
        /// Provides a class that defines that a "reduce" should occur at the location that the action is stored in the table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class ReduceAction : ParserAction
        {
            /// <summary>
            /// Gets the item that defines the reduction to perform.
            /// </summary>
            public LRItem<T> ReduceItem
            {
                get;
                private set;
            }

            public ReduceAction(ParseTable<T> table, LRItem<T> reduceItem)
                : base(table)
            {
                if (reduceItem != null)
                {
                    this.ReduceItem = reduceItem;
                }
                else
                {
                    throw new ArgumentNullException("reduceItem", "Must be non-null");
                }
            }
        }

        /// <summary>
        /// Provides a class that defines that the parse should be accepted(and therefore successful).
        /// </summary>
        public sealed class AcceptAction : ParserAction
        {
            /// <summary>
            /// Gets the item that defines that the parse should be accepted.
            /// </summary>
            public LRItem<T> AcceptItem
            {
                get;
                private set;
            }

            public AcceptAction(ParseTable<T> table, LRItem<T> acceptItem)
                : base(table)
            {
                if (acceptItem != null)
                {
                    this.AcceptItem = acceptItem;
                }
                else
                {
                    throw new ArgumentNullException("acceptItem", "Must be non-null");
                }
            }
        }

        //A DFA for a parser contains two tables
        //1). The ACTION table
        //2). the GOTO table
        //
        //the ACTION table relates input to actions(shift/reduce)
        //the GOTO table tells us which state to go to when a certian reduction is performed

        /// <summary>
        /// Defines a table that maps states(int) and Tokens(T) to Actions(Action(state, token)). This property is read only.
        /// </summary>
        public Table<int, Terminal<T>, List<ParserAction>> ActionTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Defines a table that maps states(int) and Tokens(T) to the next state(int). This property is read only.
        /// </summary>
        public Table<int, NonTerminal<T>, int?> GotoTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds a shift command to the action table at the position between the given element and given currentState, with the given actions as the values.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentState"></param>
        /// <param name="actions"></param>
        private void addAction(Terminal<T> element, int currentState, params ParserAction[] actions)
        {
            //if the column already exists
            if (ActionTable.Keys.Any(a => a.Column.Equals(element) && a.Row == currentState))
            {
                //add the actions
                ActionTable[currentState, element].AddRange(actions);
            }
            //otherwise, create new
            else
            {
                ActionTable.Add(currentState, element, actions.ToList());
            }
        }

        /// <summary>
        /// Adds a goto command to the goto table at the position defined by the currentState and element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentState"></param>
        /// <param name="goto"></param>
        private void addGoto(NonTerminal<T> element, int currentState, int @goto)
        {
            //if the column already exists
            if (GotoTable.Keys.Any(a => a.Column.Equals(element) && a.Row == currentState))
            {
                //add the goto state
                GotoTable[currentState, element] = @goto;
            }
            //otherwise, create new
            else
            {
                GotoTable.Add(currentState, element, @goto);
            }
        }

        /// <summary>
        /// Creates a new, empty parse table.
        /// </summary>
        public ParseTable()
        {
            ActionTable = new Table<int, Terminal<T>, List<ParserAction>>();
            GotoTable = new Table<int, NonTerminal<T>, int?>();
        }

        /// <summary>
        /// Creates a new parse table from the given graph.
        /// </summary>
        /// <param name="graph"></param>
        public ParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, NonTerminal<T> startingElement)
        {
            ActionTable = new Table<int, Terminal<T>, List<ParserAction>>();
            GotoTable = new Table<int, NonTerminal<T>, int?>();

            //int state = 0;
            //build the parse table from the root of the graph
            buildParseTable(graph, startingElement);//, state, startingElement, ref state);
        }

        private void buildParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, NonTerminal<T> startingElement)
        {
            if(graph == null)
            {
                throw new ArgumentNullException("graph");
            }
            if(startingElement == null)
            {
                throw new ArgumentNullException("startingElement");
            }

            //get the breadth-first traversal of the graph
            var t = graph.GetBreadthFirstTraversal().ToList();


            //add the transitions for each node in the traversal
            for(int i = 0; i < t.Count; i++)
            {
                //for each transition in the node, add either a shift action or goto action
                foreach(var transition in t[i].FromTransitions)
                {
                    if(transition.Key is Terminal<T>)
                    {
                        //add a shift from this state to the next state
                        addAction((Terminal<T>)transition.Key, i, new ShiftAction(this, t.IndexOf(transition.Value)));
                    }
                    else
                    {
                        addGoto((NonTerminal<T>)transition.Key, i, t.IndexOf(transition.Value));
                    }
                }

                //for each of the items in the state that are at the end of a production,
                //add either a reduce action or accept action
                foreach(LRItem<T> item in t[i].Value.Where(a => a.IsAtEndOfProduction()))
                {
                    //if we would reduce to the starting element, then accept
                    if(item.LeftHandSide.Equals(startingElement))
                    {
                        addAction(item.LookaheadElement, i, new AcceptAction(this, item));
                    }
                    //otherwise, add a reduce action
                    else
                    {
                        addAction(item.LookaheadElement, i, new ReduceAction(this, item));
                    }
                }
            }
        }

        /// <summary>
        /// builds the parse table from the current node.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="currentStateIndex">The interger identifier of the given currentNode</param>
        private void buildParseTable(StateNode<GrammarElement<T>, LRItem<T>[]> currentNode, int currentStateIndex, NonTerminal<T> startingElement, ref int nextIndex)
        {
            int currentState = currentStateIndex;

            if(nextIndex == 0)
            {
                nextIndex = 1;
            }

            foreach (var transition in currentNode.FromTransitions)
            {
                int index = (nextIndex);
                //add a shift transition to the ActionTable
                if (transition.Key is Terminal<T>)
                {
                    //add a shift action from the current state to the next state
                    addAction((Terminal<T>)transition.Key, currentStateIndex, new[] { new ShiftAction(this, index) });
                }
                //otherwise add to goto table
                else
                {
                    addGoto((NonTerminal<T>)transition.Key, currentStateIndex, index);
                }

                //if (index == -1)
                //{
                //    index = currentIndex + currentNode.FromTransitions.Length;
                //}
                //build from the next transition

                nextIndex++;
            }

            //add the nodes of the transition nodes to the table
            foreach (var trans in currentNode.FromTransitions)
            {
                buildParseTable(trans.Value, nextIndex, startingElement, ref nextIndex);
            }

            //add a reduce for every item that is at the end of the production
            foreach (LRItem<T> i in currentNode.Value.Where(a => a.IsAtEndOfProduction()))
            {
                if (i.LeftHandSide.Equals(startingElement))
                {
                    //accept
                    addAction(i.LookaheadElement, currentStateIndex, new[] { new AcceptAction(this, i) });
                }
                else
                {
                    //reduce
                    addAction(i.LookaheadElement, currentStateIndex, new[] { new ReduceAction(this, i) });
                }
            }
        }

        /// <summary>
        /// Creates a new parse table with the given states as rows, and possibleTerminals with possibleNonTerminals as columns for the Action and Goto tables respectively.
        /// </summary>
        /// <param name="possibleTerminals"></param>
        /// <param name="possibleNonTerminals"></param>
        /// <param name="states"></param>
        public ParseTable(IEnumerable<Terminal<T>> possibleTerminals, IEnumerable<NonTerminal<T>> possibleNonTerminals, IEnumerable<int> states)
        {
            this.ActionTable = new Table<int, Terminal<T>, List<ParserAction>>();
            foreach (Terminal<T> t in possibleTerminals)
            {
                foreach (int s in states)
                {
                    this.ActionTable.Add(s, t, null);
                }
            }

            this.GotoTable = new Table<int, NonTerminal<T>, int?>();
            foreach (NonTerminal<T> nt in possibleNonTerminals)
            {
                foreach (int s in states)
                {
                    GotoTable.Add(s, nt, null);
                }
            }
        }
    }
}
