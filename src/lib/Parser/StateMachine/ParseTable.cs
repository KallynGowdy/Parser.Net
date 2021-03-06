﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using KallynGowdy.ParserGenerator.Collections;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.Parsers;

namespace KallynGowdy.ParserGenerator.StateMachine
{
	/// <summary>
	///     Provides an implementation of a parse table.
	///     Stores the Deterministic/Nondeterministic Finite Automa for a parser.
	/// </summary>
	[DataContract(IsReference = true, Name = "LRParseTable")]
	public class ParseTable<T> : IParseTable<T>
		where T : IEquatable<T>
	{
		/// <summary>
		///     Creates a new, empty parse table.
		/// </summary>
		public ParseTable()
		{
			ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
			GotoTable = new Table<int, NonTerminal<T>, int?>();
		}

		/// <summary>
		///     Creates a new parse table from the given context free grammar.
		/// </summary>
		/// <param name="cfg"></param>
		public ParseTable(ContextFreeGrammar<T> cfg)
		{
			ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
			GotoTable = new Table<int, NonTerminal<T>, int?>();

			buildParseTable(cfg.CreateStateGraph(), cfg.StartElement);
		}

		/// <summary>
		///     Creates a new parse table from the given graph.
		/// </summary>
		/// <param name="graph"></param>
		public ParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, NonTerminal<T> startingElement)
		{
			ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
			GotoTable = new Table<int, NonTerminal<T>, int?>();

			//int state = 0;
			//build the parse table from the root of the graph
			buildParseTable(graph, startingElement); //, state, startingElement, ref state);
		}

		/// <summary>
		///     Creates a new parse table with the given states as rows, and possibleTerminals with possibleNonTerminals as columns
		///     for the Action and Goto tables respectively.
		/// </summary>
		/// <param name="possibleTerminals"></param>
		/// <param name="possibleNonTerminals"></param>
		/// <param name="states"></param>
		public ParseTable(IEnumerable<Terminal<T>> possibleTerminals, IEnumerable<NonTerminal<T>> possibleNonTerminals, IEnumerable<int> states)
		{
			ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
			foreach (Terminal<T> t in possibleTerminals)
			{
				foreach (int s in states)
					ActionTable.Add(s, t, null);
			}

			GotoTable = new Table<int, NonTerminal<T>, int?>();
			foreach (NonTerminal<T> nt in possibleNonTerminals)
			{
				foreach (int s in states)
					GotoTable.Add(s, nt, null);
			}
		}

		/// <summary>
		///     Creates a new Parse Table from the given State Graph.
		/// </summary>
		/// <param name="graph"></param>
		public ParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
		{
			if (graph.Root != null &&
			    graph.Root.Value.Length > 0)
			{
				ActionTable = new Table<int, Terminal<T>, List<ParserAction<T>>>();
				GotoTable = new Table<int, NonTerminal<T>, int?>();
				buildParseTable(graph, graph.Root.Value.First().LeftHandSide);
			}
			else
				throw new ArgumentException("The given graph must have at least one item in the root node.", "graph");
		}

		//A DFA for a parser contains two tables
		//1). The ACTION table
		//2). the GOTO table
		//
		//the ACTION table relates input to actions(shift/reduce)
		//the GOTO table tells us which state to go to when a certian reduction is performed

		/// <summary>
		///     Defines a table that maps states(int) and Tokens(T) to Actions(Action(state, token)). This property is read only.
		/// </summary>
		[DataMember(Name = "ActionTable")]
		public Table<int, Terminal<T>, List<ParserAction<T>>> ActionTable { get; set; }

		/// <summary>
		///     Defines a table that maps states(int) and Tokens(T) to the next state(int). This property is read only.
		/// </summary>
		[DataMember(Name = "GotoTable")]
		public Table<int, NonTerminal<T>, int?> GotoTable { get; set; }

		/// <summary>
		///     Gets the action(s) given the current state and the next input.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="nextInput"></param>
		/// <exception cref="System.ArgumentNullException" />
		/// <returns>
		///     A array of ShiftActions if the operation is to move, ReduceActions if the operation is to Reduce, or AcceptActions
		///     if the parse is valid.
		///     Returns null if the action does not exist.
		/// </returns>
		public ParserAction<T>[] this[int currentState, GrammarElement<T> nextInput]
		{
			get
			{
				if (nextInput == null)
					throw new ArgumentNullException("nextInput");

				if (nextInput is Terminal<T>)
				{
					//if the given state is in the table
					if (ActionTable.ContainsKey(currentState))
					{
						var actions = new List<ParserAction<T>>();
						//if the next input is in the table
						if (ActionTable[currentState].ContainsKey((Terminal<T>) nextInput))
						{
							Terminal<T> key = ActionTable[currentState].GetKey((Terminal<T>) nextInput);

							//if the stored column is not negated
							if (!key.Negated)
							{
								//return the action
								actions.AddRange(ActionTable[currentState][key].ToArray());
							}
						}
						//Negated values will never match the end of input element
						if (!((Terminal<T>) nextInput).EndOfInput)
						{
							//Negated values act as an 'and' clause instead of an 'or' clause
							//input is not 'a' and input is not 'b', instead of input is not 'a' or input is not 'b'

							//If all of the negated keys do not equal the next input.
							if (ActionTable[currentState].All(a => (a.Key.Negated && !a.Key.Equals(nextInput)) || !a.Key.Negated))
							{
								//if the state is contained in the table, and if there is a negated input element that does not match the given input
								//A Terminal with a null value that is negated will match anything except END_OF_INPUT.
								KeyValuePair<Terminal<T>, List<ParserAction<T>>> result = ActionTable[currentState].FirstOrDefault(a => a.Key.Negated && !a.Key.Equals(nextInput));
								if (!result.Equals(default(KeyValuePair<Terminal<T>, List<ParserAction<T>>>)))
									actions.AddRange(result.Value.ToArray());
							}
						}
						return actions.ToArray();
					}
				}
				else
				{
					//if the given state and next input are in the table
					if (GotoTable.ContainsKey(currentState) &&
					    GotoTable[currentState].ContainsKey((NonTerminal<T>) nextInput))
					{
						//return a new shift action representing the goto movement.
						return new[] {new ShiftAction<T>(this, GotoTable[currentState, (NonTerminal<T>) nextInput].Value)};
					}
				}

				//the item does not exist in the table, return null.
				return null;
			}
		}

		/// <summary>
		///     Writes this parse table to the given stream as an XML object.
		/// </summary>
		/// <param name="stream"></param>
		/// <exception cref="System.Runtime.Serialization.SerializationException" />
		/// <exception cref="System.ArgumentNullException" />
		public void WriteToStream(Stream stream)
		{
			var ser = new DataContractSerializer(typeof (ParseTable<T>),
			                                     new[]
			                                     {
				                                     typeof (ShiftAction<T>),
				                                     typeof (ReduceAction<T>),
				                                     typeof (AcceptAction<T>),
				                                     typeof (LRItem<T>),
				                                     typeof (Terminal<T>),
				                                     typeof (NonTerminal<T>)
			                                     });
			//new[]
			//{
			//    typeof(DelegateSerializationHolder
			//});

			using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
			{
				Indent = true
			}))
				ser.WriteObject(writer, this);
		}

		/// <summary>
		///     Reads an LRParseTable object from the given stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static ParseTable<T> ReadFromStream(Stream stream)
		{
			var ser = new DataContractSerializer(typeof (ParseTable<T>),
			                                     new[]
			                                     {
				                                     typeof (ShiftAction<T>),
				                                     typeof (ReduceAction<T>),
				                                     typeof (AcceptAction<T>),
				                                     typeof (LRItem<T>),
				                                     typeof (Terminal<T>),
				                                     typeof (NonTerminal<T>)
			                                     });

			return (ParseTable<T>) ser.ReadObject(stream);
		}

		/// <summary>
		///     Adds a shift command to the action table at the position between the given element and given currentState, with the
		///     given actions as the values.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="currentState"></param>
		/// <param name="actions"></param>
		private void addAction(Terminal<T> element, int currentState, params ParserAction<T>[] actions)
		{
			//if the column already exists
			if (ActionTable.ContainsKey(currentState, element))
			{
				//add the actions
				ActionTable[currentState, element].AddRange(actions);
			}
			//otherwise, create new
			else
				ActionTable.Add(currentState, element, actions.ToList());
		}

		/// <summary>
		///     Adds a goto command to the goto table at the position defined by the currentState and element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="currentState"></param>
		/// <param name="goto"></param>
		private void addGoto(NonTerminal<T> element, int currentState, int @goto)
		{
			//if the column already exists
			if (GotoTable.ContainsKey(currentState, element))
			{
				//add the goto state
				GotoTable[currentState, element] = @goto;
			}
			//otherwise, create new
			else
				GotoTable.Add(currentState, element, @goto);
		}

		/// <summary>
		///     Builds the parse table from the given node and starting element.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="startingElement"></param>
		private void buildParseTable(StateNode<GrammarElement<T>, LRItem<T>[]> node, NonTerminal<T> startingElement)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			if (startingElement == null)
				throw new ArgumentNullException("startingElement");

			//clear the tables
			ActionTable.Clear();
			GotoTable.Clear();

			//get the breadth-first traversal of the graph
			List<StateNode<GrammarElement<T>, LRItem<T>[]>> t = node.GetBreadthFirstTraversal().ToList();


			//add the transitions for each node in the traversal
			for (var i = 0; i < t.Count; i++)
			{
				//for each transition in the node, add either a shift action or goto action
				foreach (KeyValuePair<GrammarElement<T>, StateNode<GrammarElement<T>, LRItem<T>[]>> transition in t[i].FromTransitions)
				{
					if (transition.Key is Terminal<T>)
					{
						//add a shift from this state to the next state
						addAction((Terminal<T>) transition.Key, i, new ShiftAction<T>(this, t.IndexOf(transition.Value)));
					}
					else
						addGoto((NonTerminal<T>) transition.Key, i, t.IndexOf(transition.Value));
				}

				//for each of the items in the state that are at the end of a production,
				//add either a reduce action or accept action
				foreach (LRItem<T> item in t[i].Value.Where(a => a.IsAtEndOfProduction()))
				{
					//if we would reduce to the starting element, then accept
					if (item.LeftHandSide.Equals(startingElement))
						addAction(item.LookaheadElement, i, new AcceptAction<T>(this, item));
					//otherwise, add a reduce action
					else
						addAction(item.LookaheadElement, i, new ReduceAction<T>(this, item));
				}
			}
		}

		/// <summary>
		///     Builds the parse table from the given graph and starting element.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="startingElement"></param>
		private void buildParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, NonTerminal<T> startingElement)
		{
			if (graph == null)
				throw new ArgumentNullException("graph");
			if (startingElement == null)
				throw new ArgumentNullException("startingElement");

			//clear the tables
			ActionTable.Clear();
			GotoTable.Clear();

			//get the breadth-first traversal of the graph
			List<StateNode<GrammarElement<T>, LRItem<T>[]>> t = graph.GetBreadthFirstTraversal().ToList();


			//add the transitions for each node in the traversal
			for (var i = 0; i < t.Count; i++)
			{
				//for each transition in the node, add either a shift action or goto action
				foreach (KeyValuePair<GrammarElement<T>, StateNode<GrammarElement<T>, LRItem<T>[]>> transition in t[i].FromTransitions)
				{
					if (transition.Key is Terminal<T>)
					{
						//add a shift from this state to the next state
						addAction((Terminal<T>) transition.Key, i, new ShiftAction<T>(this, t.IndexOf(transition.Value)));
					}
					else
						addGoto((NonTerminal<T>) transition.Key, i, t.IndexOf(transition.Value));
				}

				//for each of the items in the state that are at the end of a production,
				//add either a reduce action or accept action
				foreach (LRItem<T> item in t[i].Value.Where(a => a.IsAtEndOfProduction()))
				{
					//if we would reduce to the starting element, then accept
					if (item.LeftHandSide.Equals(startingElement))
						addAction(item.LookaheadElement, i, new AcceptAction<T>(this, item));
					//otherwise, add a reduce action
					else
						addAction(item.LookaheadElement, i, new ReduceAction<T>(this, item));
				}
			}
		}
	}
}