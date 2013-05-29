using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.Grammar;
using Parser.Collections;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Provides an implementation of a GLR Parser that can parse any table.
    /// </summary>
    public class GLRParser<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the parse table that is currently being used.
        /// </summary>
        public ParseTable<T> ParseTable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the end of input element that determines 
        /// </summary>
        public Terminal<T> EndOfInputElement
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the parse table from the given graph and end of input element.
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="endOfInputElement"></param>
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
            if (graph.Root != null)
            {
                if (graph.Root.Value.FirstOrDefault() != null)
                {
                    this.ParseTable = new ParseTable<T>(graph, graph.Root.Value.First().LeftHandSide);
                    this.EndOfInputElement = endOfInputElement;
                }
            }
            throw new ArgumentException("The given graph must have at least one state with at least one item.");
        }

        /// <summary>
        /// Sets the parse table from the given grammar.
        /// </summary>
        /// <param name="grammar"></param>
        public void SetParseTable(ContextFreeGrammar<T> grammar)
        {
            if (grammar == null)
            {
                throw new ArgumentNullException("grammar");
            }

            this.ParseTable = new ParseTable<T>(grammar);
            this.EndOfInputElement = grammar.EndOfInputElement;
        }
        

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="item">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        private ParseResult<T> getSyntaxErrorResult(List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, Terminal<T> item)
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
            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(string.Format("Syntax Error. Expected one of {{{0}}} but found \'{1}\'", rows.ConcatArray(", "), item.InnerValue), item, stateStack.Peek().Key));
            return result;
        }

        /// <summary>
        /// Gets a syntax error result from the current branches, stack and item.
        /// </summary>
        /// <param name="currentBranches">The current branches in the parse.</param>
        /// <param name="stateStack">The current state stack in the parse.</param>
        /// <param name="item">The next terminal item from the augmented input.</param>
        /// <returns></returns>
        private ParseResult<T> getSyntaxErrorResult(List<ParseTree<T>.ParseTreebranch> currentBranches, Stack<KeyValuePair<int, GrammarElement<T>>> stateStack, string item)
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
            ParseResult<T> result = new ParseResult<T>(false, new ParseTree<T>(root), stateStack.ToList(), new SyntaxParseError<T>(string.Format("Syntax Error. Expected one of {{{0}}} but found \'{1}\'", rows.ConcatArray(", "), item)));
            return result;
        }

        /// <summary>
        /// Gets the glr parse results given the input.
        /// </summary>
        public IEnumerable<ParseResult<T>> glrParse(IEnumerable<Terminal<T>> input, bool syntax)
        {
            
        }

        public ParseResult<T>[] ParseAST(IEnumerable<Terminal<T>> input)
        {
            
        }

        public ParseResult<T>[] ParseSyntaxTree(IEnumerable<Terminal<T>> input)
        {
            throw new NotImplementedException();
        }
    }
}
