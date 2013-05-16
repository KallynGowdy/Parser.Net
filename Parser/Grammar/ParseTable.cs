using System;
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

        ///// <summary>
        ///// Adds a state to the table, relating Actions to terminals, and nonTerminals to gotos.
        ///// </summary>
        ///// <param name="actions"></param>
        ///// <param name="terminals"></param>
        ///// <param name="nonTerminals"></param>
        ///// <param name="gotos">The state to go to </param>
        //public void AddState(ParserAction[] actions, Terminal<T>[] terminals, NonTerminal<T>[] nonTerminals, int[] gotos)
        //{
        //    if (actions.Length != terminals.Length)
        //    {
        //        throw new ArgumentException("actions.Length and terminals.Length must be the same.");
        //    }
        //    if(nonTerminals.Length != gotos.Length)
        //    {
        //        throw new ArgumentException("nonTerminals.Length and gotos.Length must be the same.");
        //    }

        //    for(int i = 0; i < actions.Length; i++)
        //    {
        //        ActionTable.Add(ActionTable.Count, terminals[i], actions[i]);
        //    }

        //    for(int i = 0; i < nonTerminals.Length; i++)
        //    {
        //        GotoTable.Add(GotoTable.Count, nonTerminals[i], gotos[i]);
        //    }
        //}

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
            if(GotoTable.Keys.Any(a => a.Column.Equals(element) && a.Row == currentState))
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
        public ParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
        {

        }

        /// <summary>
        /// builds the parse table from the current node.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="currentStateIndex">The interger identifier of the given currentNode</param>
        private void buildParseTable(StateNode<GrammarElement<T>, LRItem<T>[]> currentNode, ref int currentStateIndex)
        {
            foreach (var transition in currentNode.FromTransitions)
            {
                //add a shift transition to the ActionTable
                if (transition.Key is Terminal<T>)
                {
                    //add a shift action from the current state to the next state
                    addAction((Terminal<T>)transition.Key, currentStateIndex, new[] { new ShiftAction(this, currentStateIndex++) });
                }
                //otherwise add to goto table
                else
                {
                    addGoto((NonTerminal<T>)transition.Key, currentStateIndex, currentStateIndex++);
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
