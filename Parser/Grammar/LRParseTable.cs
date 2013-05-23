using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Parser.Grammar;

namespace Parser.StateMachine
{
    /// <summary>
    /// Provides an implementation of a LR(1) DFA parse table.
    /// </summary>
    [Serializable]
    public class LRParseTable<T> : IParseTable<T>
    {
        //A DFA for a parser contains two tables
        //1). The ACTION table
        //2). the GOTO table
        //
        //the ACTION table relates input to actions(shift/reduce)
        //the GOTO table tells us which state to go to when a certian reduction is performed

        /// <summary>
        /// Defines a table that maps states(int) and Tokens(T) to Actions(Action(state, token)). This property is read only.
        /// </summary>
        public Table<int, Terminal<T>, List<ParserAction<T>>> ActionTable
        {
            get;
            set;
        }

        /// <summary>
        /// Defines a table that maps states(int) and Tokens(T) to the next state(int). This property is read only.
        /// </summary>
        public Table<int, NonTerminal<T>, int?> GotoTable
        {
            get;
            set;
        }

        /// <summary>
        /// Writes this parse table to the given stream as an XML object.
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="System.Runtime.Serialization.SerializationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        public void WriteToStream(Stream stream)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(LRParseTable<T>));
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true
            });
            ser.WriteObject(writer, this);
        }

        /// <summary>
        /// Reads an LRParseTable object from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static LRParseTable<T> ReadFromStream(Stream stream)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(LRParseTable<T>));
            return (LRParseTable<T>)ser.ReadObject(stream);
        }

        /// <summary>
        /// Gets the action(s) given the current state and the next input.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="nextInput"></param>
        /// <exception cref="System.ArgumentNullException"/>
        /// <returns>A array of ShiftActions if the operation is to move, ReduceActions if the operation is to Reduce, or AcceptActions if the parse is valid.
        /// Returns null if the action does not exist.</returns>
        public ParserAction<T>[] this[int currentState, GrammarElement<T> nextInput]
        {
            get
            {
                if (nextInput == null)
                {
                    throw new ArgumentNullException("nextInput");
                }
                if (nextInput is Terminal<T>)
                {
                    //if the given state and next input are in the table
                    if (ActionTable.Any(a => a.Key.Row == currentState && a.Key.Column.Equals(nextInput)))
                    {
                        //return the action
                        return ActionTable[currentState, (Terminal<T>)nextInput].ToArray();
                    }
                }
                else
                {
                    //if the given state and next input are in the table
                    if (GotoTable.Any(a => a.Key.Row == currentState && a.Key.Column.Equals(nextInput)))
                    {
                        //return a new shift action representing the goto movement.
                        return new[] { new ShiftAction<T>(this, GotoTable[currentState, (NonTerminal<T>)nextInput].Value) };
                    }
                }
                //the item does not exist in the table, return null.
                return null;
            }
        }

        /// <summary>
        /// Adds a shift command to the action table at the position between the given element and given currentState, with the given actions as the values.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="currentState"></param>
        /// <param name="actions"></param>
        private void addAction(Terminal<T> element, int currentState, params ParserAction<T>[] actions)
        {
            //if the column already exists
            if (ActionTable.Keys.Any(a => a.Column == element && a.Row == currentState))
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
        public LRParseTable()
        {
            ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
            GotoTable = new Table<int, NonTerminal<T>, int?>();
        }

        /// <summary>
        /// Creates a new parse table from the given context free grammar.
        /// </summary>
        /// <param name="cfg"></param>
        public LRParseTable(ContextFreeGrammar<T> cfg)
        {
            ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
            GotoTable = new Table<int, NonTerminal<T>, int?>();

            buildParseTable(cfg.CreateStateGraph(), cfg.StartElement);
        }

        /// <summary>
        /// Creates a new parse table from the given graph.
        /// </summary>
        /// <param name="graph"></param>
        public LRParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, NonTerminal<T> startingElement)
        {
            ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
            GotoTable = new Table<int, NonTerminal<T>, int?>();

            //int state = 0;
            //build the parse table from the root of the graph
            buildParseTable(graph, startingElement);//, state, startingElement, ref state);
        }

        /// <summary>
        /// Builds the parse table from the given node and starting element.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="startingElement"></param>
        private void buildParseTable(StateNode<GrammarElement<T>, LRItem<T>[]> node, NonTerminal<T> startingElement)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (startingElement == null)
            {
                throw new ArgumentNullException("startingElement");
            }

            //clear the tables
            this.ActionTable.Clear();
            this.GotoTable.Clear();

            //get the breadth-first traversal of the graph
            var t = node.GetBreadthFirstTraversal().ToList();


            //add the transitions for each node in the traversal
            for (int i = 0; i < t.Count; i++)
            {
                //for each transition in the node, add either a shift action or goto action
                foreach (var transition in t[i].FromTransitions)
                {
                    if (transition.Key is Terminal<T>)
                    {
                        //add a shift from this state to the next state
                        addAction((Terminal<T>)transition.Key, i, new ShiftAction<T>(this, t.IndexOf(transition.Value)));
                    }
                    else
                    {
                        addGoto((NonTerminal<T>)transition.Key, i, t.IndexOf(transition.Value));
                    }
                }

                //for each of the items in the state that are at the end of a production,
                //add either a reduce action or accept action
                foreach (LRItem<T> item in t[i].Value.Where(a => a.IsAtEndOfProduction()))
                {
                    //if we would reduce to the starting element, then accept
                    if (item.LeftHandSide.Equals(startingElement))
                    {
                        addAction(item.LookaheadElement, i, new AcceptAction<T>(this, item));
                    }
                    //otherwise, add a reduce action
                    else
                    {
                        addAction(item.LookaheadElement, i, new ReduceAction<T>(this, item));
                    }
                }
            }
        }

        /// <summary>
        /// Builds the parse table from the given graph and starting element.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="startingElement"></param>
        private void buildParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, NonTerminal<T> startingElement)
        {
            if (graph == null)
            {
                throw new ArgumentNullException("graph");
            }
            if (startingElement == null)
            {
                throw new ArgumentNullException("startingElement");
            }

            //clear the tables
            this.ActionTable.Clear();
            this.GotoTable.Clear();

            //get the breadth-first traversal of the graph
            var t = graph.GetBreadthFirstTraversal().ToList();


            //add the transitions for each node in the traversal
            for (int i = 0; i < t.Count; i++)
            {
                //for each transition in the node, add either a shift action or goto action
                foreach (var transition in t[i].FromTransitions)
                {
                    if (transition.Key is Terminal<T>)
                    {
                        //add a shift from this state to the next state
                        addAction((Terminal<T>)transition.Key, i, new ShiftAction<T>(this, t.IndexOf(transition.Value)));
                    }
                    else
                    {
                        addGoto((NonTerminal<T>)transition.Key, i, t.IndexOf(transition.Value));
                    }
                }

                //for each of the items in the state that are at the end of a production,
                //add either a reduce action or accept action
                foreach (LRItem<T> item in t[i].Value.Where(a => a.IsAtEndOfProduction()))
                {
                    //if we would reduce to the starting element, then accept
                    if (item.LeftHandSide.Equals(startingElement))
                    {
                        addAction(item.LookaheadElement, i, new AcceptAction<T>(this, item));
                    }
                    //otherwise, add a reduce action
                    else
                    {
                        addAction(item.LookaheadElement, i, new ReduceAction<T>(this, item));
                    }
                }
            }
        }

        /// <summary>
        /// builds the parse table from the current node.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <param name="currentStateIndex">The interger identifier of the given currentNode</param>
        //private void buildParseTable(StateNode<GrammarElement<T>, LRItem<T>[]> currentNode, int currentStateIndex, NonTerminal<T> startingElement, ref int nextIndex)
        //{
        //    int currentState = currentStateIndex;

        //    if (nextIndex == 0)
        //    {
        //        nextIndex = 1;
        //    }

        //    foreach (var transition in currentNode.FromTransitions)
        //    {
        //        int index = (nextIndex);
        //        //add a shift transition to the ActionTable
        //        if (transition.Key is Terminal<T>)
        //        {
        //            //add a shift action from the current state to the next state
        //            addAction((Terminal<T>)transition.Key, currentStateIndex, new[] { new ShiftAction(this, index) });
        //        }
        //        //otherwise add to goto table
        //        else
        //        {
        //            addGoto((NonTerminal<T>)transition.Key, currentStateIndex, index);
        //        }
        //        nextIndex++;
        //    }

        //    //add the nodes of the transition nodes to the table
        //    foreach (var trans in currentNode.FromTransitions)
        //    {
        //        buildParseTable(trans.Value, nextIndex, startingElement, ref nextIndex);
        //    }

        //    //add a reduce for every item that is at the end of the production
        //    foreach (LRItem<T> i in currentNode.Value.Where(a => a.IsAtEndOfProduction()))
        //    {
        //        if (i.LeftHandSide.Equals(startingElement))
        //        {
        //            //accept
        //            addAction(i.LookaheadElement, currentStateIndex, new[] { new AcceptAction(this, i) });
        //        }
        //        else
        //        {
        //            //reduce
        //            addAction(i.LookaheadElement, currentStateIndex, new[] { new ReduceAction(this, i) });
        //        }
        //    }
        //}

        /// <summary>
        /// Creates a new parse table with the given states as rows, and possibleTerminals with possibleNonTerminals as columns for the Action and Goto tables respectively.
        /// </summary>
        /// <param name="possibleTerminals"></param>
        /// <param name="possibleNonTerminals"></param>
        /// <param name="states"></param>
        public LRParseTable(IEnumerable<Terminal<T>> possibleTerminals, IEnumerable<NonTerminal<T>> possibleNonTerminals, IEnumerable<int> states)
        {
            this.ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
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
