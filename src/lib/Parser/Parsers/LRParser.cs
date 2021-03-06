﻿using System;
using System.Collections.Generic;
using System.Linq;
using KallynGowdy.ParserGenerator.Collections;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Defines an LR(1) parser.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class LrParser<T> : IGrammarParser<T>, IGraphParser<T>
		where T : IEquatable<T>
	{
		/// <summary>
		///     The parse table.
		/// </summary>
		private ParseTable<T> parseTable;

		/// <summary>
		///     Gets the end of input element.
		/// </summary>
		public Terminal<T> EndOfInputElement { get; private set; }

		/// <summary>
		///     Gets or sets the parse table used to parse input.
		/// </summary>
		/// <exception cref="Parser.InvalidParseTableException">
		///     Thrown if an invalid parse table is provided when trying to set the
		///     parse table.
		/// </exception>
		public virtual ParseTable<T> ParseTable
		{
			get { return parseTable; }
			set { SetParseTable(value); }
		}

		/// <summary>
		///     Determines whether to take the first action (of the possible actions) through the parse table or to error.
		/// </summary>
		public bool TakeFirstRoute { get; set; }

		/// <summary>
		///     Parses an Abstract Syntax tree from the given input based on the rules defined in the Terminal elements.
		/// </summary>
		/// <param name="input"></param>
		/// <exception cref="System.ArgumentNullException" />
		/// <returns></returns>
		public virtual ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			CheckParseTable();

			return LRParse(false, input.ToArray());
		}

		/// <summary>
		///     Parses a concrete Syntax tree from the given input.
		/// </summary>
		/// <param name="input"></param>
		/// <exception cref="System.ArgumentNullException" />
		/// <returns></returns>
		public virtual ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input)
		{
			if (input == null)
				throw new ArgumentNullException("input");

			CheckParseTable();

			return LRParse(true, input.ToArray());
		}

		/// <summary>
		///     Sets Parse Table from the given grammar.
		/// </summary>
		/// <param name="grammar"></param>
		/// <exception cref="ArgumentNullException">The value of 'grammar' cannot be null. </exception>
		/// <exception cref="InvalidParseTableException">
		///     Thrown when the given parse table contains either a 'Shift
		///     Reduce' conflict or a 'Reduce Reduce' conflict.
		/// </exception>
		public virtual void SetParseTable(ContextFreeGrammar<T> grammar)
		{
			if (grammar == null)
				throw new ArgumentNullException("grammar", "The given grammar must be non-null");

			StateGraph<GrammarElement<T>, LRItem<T>[]> graph = grammar.CreateStateGraph();

			SetParseTable(new ParseTable<T>(graph, grammar.StartElement), graph);
			EndOfInputElement = grammar.EndOfInputElement;
		}

		/// <summary>
		///     Sets the Parse Table from the given graph.
		/// </summary>
		/// <param name="graph"></param>
		/// <exception cref="Parser.InvalidParseTableException" />
		public virtual void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, Terminal<T> endOfInputElement)
		{
			if (graph == null)
				throw new ArgumentNullException("graph", "The given graph must be non-null");

			if (endOfInputElement == null)
				throw new ArgumentNullException("endOfInputElement");
			if (graph.Root != null)
			{
				if (graph.Root.Value.FirstOrDefault() != null)
				{
					SetParseTable(new ParseTable<T>(graph, graph.Root.Value.First().LeftHandSide), graph);
					EndOfInputElement = endOfInputElement;
				}
			}

			throw new ArgumentException("The given graph must have at least one state with at least one item.");
		}

		/// <summary>
		///     Sets the end of input element from the table by finding the first terminal that identifies itself as the end of
		///     input element.
		/// </summary>
		protected void SetEndOfInputFromTable()
		{
			EndOfInputElement = parseTable.ActionTable.Select(a => a.Value.FirstOrDefault(b => b.Key.EndOfInput).Key).First(a => a != null);
		}

		/// <summary>
		///     Sets the parse table that the parser is to use. The given parse table is not checked for conflicts so a
		///     MultipleParseActions error might be returned during parse time.
		/// </summary>
		/// <param name="table"></param>
		public virtual void SetParseTable(ParseTable<T> table)
		{
			if (table == null)
				throw new ArgumentNullException("table");
			parseTable = table;
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Sets the parse table that the parser uses with the given state graph used to help resolve parse table conflicts.
		/// </summary>
		/// <exception cref="Parser.Parsers.InvalidParseTableException">
		///     Thrown when the given parse table contains either a 'Shift
		///     Reduce' conflict or a 'Reduce Reduce' conflict.
		/// </exception>
		/// <param name="value">The parse table to use for parsing</param>
		/// <param name="graph">The graph to use to return informative exceptions regarding conflicts.</param>
		public virtual void SetParseTable(ParseTable<T> value, StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			if (graph == null)
				throw new ArgumentNullException("graph");

			foreach (var colRow in value.ActionTable.Select(a =>
			{
				foreach (Terminal<T> b in a.Value.Keys)
				{
					return new
					{
						Row = a,
						Column = b,
						Value = a.Value[b]
					};
				}
				return null;
			}))
			{
				//if we have a conflict
				if (colRow.Value.Count > 1)
				{
					var conflicts = new List<ParseTableConflict<T>>();

					StateNode<GrammarElement<T>, LRItem<T>[]> node = graph.GetBreadthFirstTraversal().ElementAt(colRow.Row.Key);


					//ParseTableExceptionType exType;
					////if we have a shift-reduce conflict
					if (colRow.Value.Any(a => a is ShiftAction<T>) &&
						colRow.Value.Any(a => a is ReduceAction<T>))
					{
						LRItem<T>[] items = node.Value.Where(a =>
						{
							GrammarElement<T> e = a.GetNextElement();
							if (e != null)
								return e.Equals(colRow.Column);
							return false;
						}).Concat(colRow.Value.OfType<ReduceAction<T>>().Select(a => a.ReduceItem)).ToArray();

						conflicts.Add(new ParseTableConflict<T>(ParseTableExceptionType.SHIFT_REDUCE, new ColumnRowPair<int, Terminal<T>>(colRow.Row.Key, colRow.Column), node, items));

						//then check for a reduce-reduce conflict
						if (colRow.Value.Count(a => a is ReduceAction<T>) > 1)
						{
							items = colRow.Value.OfType<ReduceAction<T>>().Select(a => a.ReduceItem).ToArray();
							conflicts.Add(new ParseTableConflict<T>(ParseTableExceptionType.REDUCE_REDUCE, new ColumnRowPair<int, Terminal<T>>(colRow.Row.Key, colRow.Column), node, items));
						}
					}
					//otherwise we have a reduce-reduce conflict
					else
					{
						//get all of the reduce items
						LRItem<T>[] items = colRow.Value.OfType<ReduceAction<T>>().Select(a => a.ReduceItem).ToArray();

						conflicts.Add(new ParseTableConflict<T>(ParseTableExceptionType.REDUCE_REDUCE, new ColumnRowPair<int, Terminal<T>>(colRow.Row.Key, colRow.Column), node, items));
					}

					//throw invalid parse table exception
					throw new InvalidParseTableException<T>(value, node, conflicts.ToArray());
				}
			}
			parseTable = value;
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Performs an LR(1) parse on the table given the current augmented input, state stack, current branches, and whether
		///     to produce a syntax tree or AST.
		/// </summary>
		/// <param name="stateStack"></param>
		/// <param name="currentBranches"></param>
		/// <param name="syntax">Determines whether to produce a syntax tree or AST.</param>
		/// <returns></returns>
		protected ParseResult<T> LRParse(bool syntax, Terminal<T>[] input, int inputProgression = 0, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = null, List<SyntaxNode<T>> currentBranches = null)
		{
			if (input == null)
			{
				var root = new SyntaxNode<T>((GrammarElement<T>)null);
				root.AddChildren(currentBranches);
				return new ParseResult<T>(false, new SyntaxTree<T>(root), stateStack.ToList());
			}

			if (stateStack == null)
			{
				stateStack = new Stack<KeyValuePair<int, GrammarElement<T>>>();
				stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(0, null));
			}

			if (currentBranches == null)
				currentBranches = new List<SyntaxNode<T>>();

			Terminal<T>[] augmentedInput = input.Concat(new[] { EndOfInputElement }).ToArray();

			//cache the length
			int length = augmentedInput.Length;

			if (length > 0 &&
				inputProgression < length)
			{
				for (int i = inputProgression; i < length; i++)
				{
					//Get the item
					Terminal<T> item = augmentedInput[i];
					int s = stateStack.Peek().Key;

					//Get the possible actions for the current state and item from the table
					ParserAction<T>[] actions = ParseTable[s, item];

					if (actions != null)
					{
						ParserAction<T> action;
						//SHIFT_REDUCE or REDUCE_REDUCE
						if (actions.Length > 1)
						{
							if (!TakeFirstRoute)
							{
								KeyValuePair<int, GrammarElement<T>> currentState = stateStack.Peek();
								return new ParseResult<T>(false, new SyntaxTree<T>(new SyntaxNode<T>(currentBranches)), stateStack.ToList(), new MultipleActionsParseError<T>("", currentState.Key, item, i, actions.ToArray()));
							}
							action = actions.First();
						}
						else
							action = actions.SingleOrDefault();
						//if there is an action
						if (action != null)
						{
							//if we should shift
							if (action is ShiftAction<T>)
								Shift(stateStack, item, action);
							//otherwise if we should reduce
							else if (action is ReduceAction<T>)
								Reduce(syntax, stateStack, currentBranches, action, ref i);
							//otherwise if the parse if finished and we should accept
							else if (action is AcceptAction<T>)
							{
								//return a new SyntaxTree with the root as the current branch
								return new ParseResult<T>(true, new SyntaxTree<T>(currentBranches.First()), stateStack.ToList());
							}
							//should never be called, but the compiler will be satisfied...
							else
								return GetSyntaxErrorResult(input, i, currentBranches, stateStack, item);
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
						return GetSyntaxErrorResult(input, i, currentBranches, stateStack, item);
				}

				//Found "END_OF_FILE" but was expecting "stuff"
				return GetSyntaxErrorResult(input, inputProgression, currentBranches, stateStack, "END_OF_FILE");
			}
			//check for Start -> epsilon
			IEnumerable<ParserAction<T>> enumerableActions = ParseTable.ActionTable[0, new Terminal<T>(default(T))];
			if (enumerableActions != null &&
				enumerableActions.Any())
			{
				var action = (ReduceAction<T>)enumerableActions.First();

				return new ParseResult<T>(true, new SyntaxTree<T>(new SyntaxNode<T>(action.ReduceItem.LeftHandSide)), null);
			}
			return GetSyntaxErrorResult(null, null, EndOfInputElement);
			//return new ParseResult<T>(false, null, null, new SyntaxParseError<T>("Empty input is invalid. There is no reduction of Start -> epsilon"));
		}

		/// <summary>
		///     Gets a result with a syntax error describing the problem based on the given input error index, current branches,
		///     state stack, and unexpected element.
		/// </summary>
		/// <param name="input">The input that contains the syntax error.</param>
		/// <param name="index">The zero based index of the syntax error.</param>
		/// <param name="currentBranches">The current branches that were created from the input.</param>
		/// <param name="stateStack">The current stack representing the transitions that were taken through the state graph.</param>
		/// <param name="p">The string representation of the unexpected element.</param>
		/// <returns></returns>
		protected ParseResult<T> GetSyntaxErrorResult(Terminal<T>[] input, int index, List<SyntaxNode<T>> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, string p)
		{
			//create a new root branch for the tree
			var root = new SyntaxNode<T>((GrammarElement<T>)null);
			root.AddChildren(currentBranches);

			//get the possible expected elements
			IEnumerable<object> rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null).Select<Terminal<T>, object>(a =>
			{
				if (a == EndOfInputElement)
					return "END_OF_INPUT";
				return a;
			});

			//get the error position
			// TODO: Update with proper logic
			var pos = new Tuple<int, int>(0, 0); //new Tuple<int, int>(input.GetLineNumber(index, a => a == EndOfInputElement), input.GetColumnNumber(index, (a, i) => a == EndOfInputElement));

			//return a new result with the new root branch as the root of a new tree, the state stack, a new syntax error with the current state, unexpected element, position, and expected elements
			var result = new ParseResult<T>(false, new SyntaxTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, p, pos, rows.OfType<Terminal<T>>().ToArray()));
			return result;
		}

		/// <summary>
		///     Shifts the given action based on the given item, reflecting the given changes on the given stack.
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
		///     Performs the given action on the given stack, branches, and current index based on the current item.
		///     Returns whether the action was to accept.
		/// </summary>
		/// <param name="syntax"></param>
		/// <param name="stateStack"></param>
		/// <param name="currentBranches"></param>
		/// <param name="action"></param>
		/// <param name="currentIndex"></param>
		/// <returns></returns>
		protected bool PerformAction(bool syntax, Terminal<T> currentItem, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, List<SyntaxNode<T>> currentBranches, ParserAction<T> action, ref int currentIndex)
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
				return true;
			return false;
		}

		/// <summary>
		///     Reduces the given action, reflecting the changes on the given stack, branches, and index.
		/// </summary>
		/// <param name="syntax"></param>
		/// <param name="stateStack"></param>
		/// <param name="currentBranches"></param>
		/// <param name="action"></param>
		protected void Reduce(bool syntax, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, List<SyntaxNode<T>> currentBranches, ParserAction<T> action, ref int currentIndex)
		{
			if (action is ReduceAction<T>)
			{
				var r = (ReduceAction<T>)action;

				var e = new List<GrammarElement<T>>();

				//pop the number of elements in the RHS of the item
				for (var c = 0; c < r.ReduceItem.ProductionElements.Length; c++)
					e.Add(stateStack.Pop().Value);
				e.Reverse();

				//create a new branch with the value as the LHS of the reduction item.
				var newBranch = new SyntaxNode<T>(r.ReduceItem.LeftHandSide);


				//Determine whether to add each element to the new branch based on whether it should be kept.
				foreach (GrammarElement<T> element in e)
				{
					if (element is NonTerminal<T>)
					{
						if (element.Keep || syntax)
						{
							//find the first branch that matches the reduce element
							SyntaxNode<T> b = currentBranches.Last(a => a.Value.Equals(element));
							newBranch.AddChild(b);
							currentBranches.Remove(b);
						}
						else
						{
							//find the first branch that matches the reduce element
							SyntaxNode<T> b = currentBranches.Last(a => a.Value.Equals(element));
							//get the children of the branch since we don't want the current value
							IEnumerable<SyntaxNode<T>> branches = b.GetChildren();
							//add the children
							newBranch.AddChildren(branches);
							currentBranches.Remove(b);
						}
					}
					else
					{
						//if we should keep the terminal object, then add it to the new branch
						if (element == null ||
							element.Keep ||
							syntax)
							newBranch.AddChild(new SyntaxNode<T>(element));
					}
				}

				currentBranches.Add(newBranch);

				//push the LHS non-terminal and the next state
				stateStack.Push(new KeyValuePair<int, GrammarElement<T>>(ParseTable.GotoTable[stateStack.Peek().Key, r.ReduceItem.LeftHandSide].Value, r.ReduceItem.LeftHandSide));

				currentIndex--;
			}
		}

		/// <summary>
		///     Gets a syntax error result from the current branches, stack and item.
		/// </summary>
		/// <param name="currentBranches">The current branches in the parse.</param>
		/// <param name="stateStack">The current state stack in the parse.</param>
		/// <param name="item">The next terminal item from the augmented input.</param>
		/// <returns></returns>
		protected ParseResult<T> GetSyntaxErrorResult(IEnumerable<Terminal<T>> input, int index, List<SyntaxNode<T>> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item)
		{
			var root = new SyntaxNode<T>((GrammarElement<T>)null);
			root.AddChildren(currentBranches);
			IEnumerable<Terminal<T>> rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null);


			// TODO: Update with proper logic
			var pos = new Tuple<int, int>(0, 0); //new Tuple<int, int>(input.GetLineNumber(index, a => a == EndOfInputElement), input.GetColumnNumber(index, (a, i) => a == EndOfInputElement));

			var result = new ParseResult<T>(false, new SyntaxTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, item, pos, rows.ToArray()));
			return result;
		}

		/// <summary>
		///     Gets a syntax error result from the current branches, stack and item.
		/// </summary>
		/// <param name="currentBranches">The current branches in the parse.</param>
		/// <param name="stateStack">The current state stack in the parse.</param>
		/// <param name="item">The next terminal item from the augmented input.</param>
		/// <returns></returns>
		protected ParseResult<T> GetSyntaxErrorResult(List<SyntaxNode<T>> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item)
		{
			var root = new SyntaxNode<T>((GrammarElement<T>)null);
			root.AddChildren(currentBranches);
			IEnumerable<Terminal<T>> rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null);
			var result = new ParseResult<T>(false, new SyntaxTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, item, new Tuple<int, int>(-1, -1), rows.ToArray()));
			return result;
		}

		/// <summary>
		///     Gets a syntax error result from the current branches, stack and item.
		/// </summary>
		/// <param name="currentBranches">The current branches in the parse.</param>
		/// <param name="stateStack">The current state stack in the parse.</param>
		/// <param name="invalidItem">The next terminal item from the augmented input.</param>
		/// <returns></returns>
		protected ParseResult<T> GetSyntaxErrorResult(List<SyntaxNode<T>> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, string invalidItem)
		{
			var root = new SyntaxNode<T>((GrammarElement<T>)null);
			root.AddChildren(currentBranches);
			IEnumerable<Terminal<T>> rows = ParseTable.ActionTable.GetColumns(stateStack.Peek().Key).Where(a => a != null);
			var result = new ParseResult<T>(false, new SyntaxTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(stateStack.Peek().Key, invalidItem, new Tuple<int, int>(-1, -1), rows.ToArray()));
			return result;
		}

		/// <summary>
		///     Throws InvalidOperationException if the parse table is not set.
		/// </summary>
		protected void CheckParseTable()
		{
			if (ParseTable == null)
				throw new InvalidOperationException("ParseTable must be set before trying to parse.");
		}
	}
}