using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Provides an implementation of a GLR Parser that can parse any table.
	/// </summary>
	public class GLRParser<T> : LrParser<T>
		where T : IEquatable<T>
	{
		/// <summary>
		///     Creates a new GLRParser with a null parse table.
		/// </summary>
		public GLRParser()
		{
		}

		/// <summary>
		///     Creates a new GLRParse with the given parse table.
		/// </summary>
		/// <param name="table"></param>
		public GLRParser(ParseTable<T> table)
		{
			base.ParseTable = table;
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Gets the parse table used by this parser.
		/// </summary>
		public override ParseTable<T> ParseTable
		{
			get { return ParseTable; }
		}

		/// <summary>
		///     Sets the parse table of this GLRParser.
		/// </summary>
		/// <param name="table"></param>
		public override void SetParseTable(ParseTable<T> table)
		{
			base.ParseTable = table;
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Sets the parse table of this GLRParser. The graph is ignored.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="graph"></param>
		public override void SetParseTable(ParseTable<T> table, StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
		{
			base.ParseTable = table;
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Sets the parse table of this GLRParse based on the given grammar.
		/// </summary>
		/// <param name="grammar"></param>
		public override void SetParseTable(ContextFreeGrammar<T> grammar)
		{
			ParseTable = new ParseTable<T>(grammar);
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Sets the parse table of this GLRParser based on the given graph and end of input element.
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="endOfInputElement"></param>
		public override void SetParseTable(StateGraph<GrammarElement<T>, LRItem<T>[]> graph, Terminal<T> endOfInputElement)
		{
			ParseTable = new ParseTable<T>(graph);
			SetEndOfInputFromTable();
		}

		/// <summary>
		///     Parses a list of abstract syntax trees asyncronously.
		/// </summary>
		/// <param name="callback">Called when a single branch of parsing has completed.</param>
		/// <param name="newParseBeginning">Called when a new branche of parsing has started.</param>
		/// <param name="input">The input to parse</param>
		/// <param name="inputProgression">The index of the current item to parse from.</param>
		/// <param name="stateStack">The current stack used to parse.</param>
		/// <param name="currentBranches">The current progression of the created tree.</param>
		protected void ParseTrees(int id, ParseCallback callback, ParseBegin newParseBeginning, bool syntax, IEnumerable<Terminal<T>> input, int inputProgression = 0, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack = null, List<ParseTree<T>.ParseTreebranch> currentBranches = null)
		{
			var results = new List<ParseResult<T>>();

			newParseBeginning(id);

			ThreadPool.QueueUserWorkItem(a =>
			{
				//parse
				ParseResult<T> result = LRParse(syntax, input.ToArray(), inputProgression, stateStack, currentBranches);

				//if not successfull
				if (!result.Success)
				{
					if (result.Errors.Count > 0)
					{
						//if there are multiple parse actions for the current state.
						if (result.Errors[0] is MultipleActionsParseError<T>)
						{
							var e = (MultipleActionsParseError<T>) result.Errors[0];

							//parse each action
							foreach (ParserAction<T> action in e.PossibleActions)
							{
								//create a new stack for the new thread
								var stack = new Stack<KeyValuePair<int, GrammarElement<T>>>(result.Stack.Reverse());

								//get the ammount of progression through the input
								int progression = e.Progression;

								//get the item
								Terminal<T> item = input.ElementAt(progression >= input.Count() ? input.Count() - 1 : progression);

								//get a new list of current branches
								var branches = new List<ParseTree<T>.ParseTreebranch>(result.GetParseTree().Root.Children);

								//perform the action
								if (PerformAction(true, item, stack, branches, action, ref progression))
								{
									//if the action was to accept, obtain a lock on the results
									lock (results)
									{
										//add the successful result
										results.Add(new ParseResult<T>(true, new ParseTree<T>(branches.First()), stack.ToList()));
									}
									continue;
								}

								//parse from the action in a new thread
								ParseTrees(id + 1, callback, newParseBeginning, syntax, input, progression, stack, branches);
							}
						}
						else
							results.Add(result);
					}
					//otherwise, we have a unsucessful parse that we can do nothing about.
					else
						results.Add(result);
				}
				//otherwise, we have a successful parse
				else
					results.Add(result);

				//send a callback with the results
				callback.Invoke(id, results);
			});
		}

		/// <summary>
		///     Parses a list of syntax trees that represent the (possible) ambigous derivations of the given input.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<ParseResult<T>> ParseSyntaxTrees(IEnumerable<Terminal<T>> input)
		{
			return ParseTrees(input, true);
		}

		/// <summary>
		///     Parses a list of abstract syntax trees that represent the (possible) ambigous derivation of the given input.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public IEnumerable<ParseResult<T>> ParseAbstractSyntaxTrees(IEnumerable<Terminal<T>> input)
		{
			return ParseTrees(input, false);
		}

		/// <summary>
		///     Parses a list of trees(either AST or Syntax, based on the given parameter syntax) from the given input.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		protected IEnumerable<ParseResult<T>> ParseTrees(IEnumerable<Terminal<T>> input, bool syntax)
		{
			var results = new List<ParseResult<T>>();

			var hasResult = false;

			var totalThreads = new List<int>(5);

			//the current number of ongoing parses.
			var currentParses = 0;

			var handle = new object();

			//parse the input starting at index 0,
			//with callbacks for when a parse is done, and when a new parse is beginning.
			ParseTrees(0, (id, result) =>
			{
				//this method is called when a parse is done.

				//obtain a lock on the handle to syncronize
				lock (handle)
				{
					//remove the parse
					currentParses--;

					lock (totalThreads)
					{
						totalThreads.Remove(id);

						//add the parse results.
						results.AddRange(result);

						hasResult = totalThreads.Count == 0;
					}
				}
			},
			           id =>
			           {
				           //this method is called when a new parse is beginning.

				           //obtain a lock on the handle to syncronize
				           lock (handle)
				           {
					           //add the id of the work item to the current threads.
					           totalThreads.Add(id);

					           //add the new parse
					           currentParses++;
				           }
			           }, syntax, input, 0);


			//wait for each thread to finish
			while (!hasResult)
			{
			}

			//return the list
			return results;
		}

		/// <summary>
		///     Parses the given input and returns the first successful or unsuccessful result.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public override ParseResult<T> ParseAST(IEnumerable<Terminal<T>> input)
		{
			IEnumerable<ParseResult<T>> results = ParseAbstractSyntaxTrees(input);
			if (results.Any(a => a.Success))
				return results.First(a => a.Success);
			return results.FirstOrDefault();
		}

		/// <summary>
		///     Parses the given input and returns the first successful or unsuccessful result.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public override ParseResult<T> ParseSyntaxTree(IEnumerable<Terminal<T>> input)
		{
			IEnumerable<ParseResult<T>> results = ParseSyntaxTrees(input);
			if (results.Any(a => a.Success))
				return results.First(a => a.Success);
			return results.FirstOrDefault();
		}

		/// <summary>
		///     Defines a delegate that is called when a parse is done.
		/// </summary>
		/// <param name="results"></param>
		protected delegate void ParseCallback(int id, IEnumerable<ParseResult<T>> results);

		/// <summary>
		///     Defines a delegate that is called when a new parse is beginning
		/// </summary>
		protected delegate void ParseBegin(int id);
	}
}