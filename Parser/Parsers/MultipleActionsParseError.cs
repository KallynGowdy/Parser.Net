using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
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
}