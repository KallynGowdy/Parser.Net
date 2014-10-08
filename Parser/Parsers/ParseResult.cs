using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;

namespace Parser.Parsers
{
    /// <summary>
    /// Defines a parse error.
    /// </summary>
    public interface IParseError
    {
        /// <summary>
        /// Gets the message of the error.
        /// </summary>
        string Message
        {
            get;
        }

        /// <summary>
        /// Gets the state that the error occured at.
        /// </summary>
        int State
        {
            get;
        }
    }

    public struct MultipleActionsParseError<T> : IParseError where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the message of the error.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state of the parse table that the error occured at.
        /// </summary>
        public int State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets how far through the input the parse got.
        /// </summary>
        public int Progression
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the input element.
        /// </summary>
        public Terminal<T> Input
        {
            get;
            private set;
        }

        private IEnumerable<ParserAction<T>> possibleActions;

        /// <summary>
        /// Gets the possible actions.
        /// </summary>
        public ReadOnlyCollection<ParserAction<T>> PossibleActions
        {
            get
            {
                return new ReadOnlyCollection<ParserAction<T>>(possibleActions.ToList());
            }
        }

        public MultipleActionsParseError(string message, int state, Terminal<T> input, int progression, params ParserAction<T>[] possibleActions)
            : this()
        {
            this.Input = input;
            this.Message = message;
            this.State = state;
            this.possibleActions = possibleActions;
            this.Progression = progression;
        }
    }

    public struct SyntaxParseError<T> : IParseError where T : IEquatable<T>
    {
        Tuple<int, int> errorPos;

        /// <summary>
        /// Gets the index of the error position.
        /// The tuple is of the form:
        /// LineNumber, ColumnNumber.
        /// </summary>
        public Tuple<int, int> ErrorPosition
        {
            get
            {
                return new Tuple<int, int>(errorPos.Item1, errorPos.Item2);
            }
        }

        /// <summary>
        /// Gets the message of the error.
        /// </summary>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the invalid input element.
        /// </summary>
        public Terminal<T> UnexpectedInputElement
        {
            get;
            private set;
        }

        private IEnumerable<Terminal<T>> expectedElements;

        /// <summary>
        /// Gets the expected elements.
        /// </summary>
        public ReadOnlyCollection<Terminal<T>> ExpectedElements
        {
            get
            {
                return new ReadOnlyCollection<Terminal<T>>(expectedElements.ToList());
            }
        }

        [Obsolete("Use the other constructor")]
        public SyntaxParseError(string message, Terminal<T> unexpectedInput = null, int state = -1, Tuple<int, int> errorPos = null)
            : this()
        {
            this.Message = message;
            this.UnexpectedInputElement = unexpectedInput;
            this.State = state;
            this.errorPos = new Tuple<int, int>(-1, -1);
        }

        public SyntaxParseError(int state, Terminal<T> unexpectedInput, Tuple<int, int> errorPos, params Terminal<T>[] expectedInput)
            : this()
        {
            this.Message = buildMessage(state, unexpectedInput, errorPos, expectedInput);
            this.State = state;
            this.UnexpectedInputElement = unexpectedInput;
            this.errorPos = errorPos;
            this.expectedElements = expectedInput;
        }

        public SyntaxParseError(int state, string unexpectedInput, Tuple<int, int> errorPos, params Terminal<T>[] expectedInput)
            : this()
        {
            this.Message = buildMessage(state, unexpectedInput, errorPos, expectedInput);
            this.State = state;
            this.UnexpectedInputElement = null;
            this.errorPos = errorPos;
            this.expectedElements = expectedInput;
        }

        private string buildMessage(int state, object unexpectedInput, Tuple<int, int> errorPos, Terminal<T>[] expectedInput)
        {
            StringBuilder b = new StringBuilder();

            b.AppendFormat("Syntax Error at state {0}.", state >= 0 ? State.ToString() : "Unkown");
            
            b.AppendFormat(" Expected {0}{{{1}}}, but found '{2}'.", expectedInput.Length > 0 ? "one of: " : " ", expectedInput.Select(a => string.Format("\'{0}\'", a)).ConcatArray(","), unexpectedInput);
            b.Append(errorPos.ToString(" Line Number: {0}, Column Number {1}"));

            return b.ToString();
        }

        public override string ToString()
        {
            return Message;
        }

        /// <summary>
        /// Gets the state that the syntax error occured at.
        /// </summary>
        public int State
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Defines a class that contains information about the parsing action.
    /// This class is thread-safe (because it is immutable).
    /// </summary>
    public struct ParseResult<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets whether the parse was a success.
        /// </summary>
        public bool Success
        {
            get;
            private set;
        }

        private ParseTree<T> parseTree;

        /// <summary>
        /// Gets the parse tree, copied over so it is immutable.
        /// This tree may or may not be complete, depending on if the parse was a success.
        /// </summary>
        /// <returns></returns>
        public ParseTree<T> GetParseTree()
        {
            if (parseTree != null)
            {
                return new ParseTree<T>(parseTree);
            }
            return null;
        }

        /// <summary>
        /// Gets the errors that occured from the parse.
        /// </summary>
        public ReadOnlyCollection<IParseError> Errors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the stack that the parse ended with.
        /// </summary>
        public ReadOnlyCollection<KeyValuePair<int, GrammarElement<T>>> Stack
        {
            get;
            private set;
        }

        public ParseResult(bool success, ParseTree<T> tree, IList<KeyValuePair<int, GrammarElement<T>>> parseStack, params IParseError[] errors)
            : this()
        {
            this.Success = success;
            this.parseTree = tree;
            if (parseStack != null)
            {
                this.Stack = new ReadOnlyCollection<KeyValuePair<int, GrammarElement<T>>>(parseStack);
            }
            if (errors != null)
            {
                this.Errors = new ReadOnlyCollection<IParseError>(errors);
            }
        }


    }
}
