using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
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

			b.AppendFormat(" Expected {0}{{{1}}}, but found '{2}'.", expectedInput.Length > 0 ? "one of: " : " ", expectedInput.Select(a => string.Format("\'{0}\'", a)).ConcatStringArray(","), unexpectedInput);
			b.Append(string.Format("Line Number: {0}, Column Number {1}", errorPos.Item1, errorPos.Item2));

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
}