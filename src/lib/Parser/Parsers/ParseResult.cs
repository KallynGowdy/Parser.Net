using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Defines a class that contains information about the parsing action.
	///     This class is thread-safe (because it is immutable).
	/// </summary>
	public struct ParseResult<T>
		where T : IEquatable<T>
	{
		private readonly SyntaxTree<T> syntaxTree;

		public ParseResult(bool success, SyntaxTree<T> tree, IList<KeyValuePair<int, GrammarElement<T>>> parseStack, params IParseError[] errors)
			: this()
		{
			Success = success;
			syntaxTree = tree;
			if (parseStack != null)
				Stack = new ReadOnlyCollection<KeyValuePair<int, GrammarElement<T>>>(parseStack);
			if (errors != null)
				Errors = new ReadOnlyCollection<IParseError>(errors);
		}

		/// <summary>
		///     Gets the errors that occured from the parse.
		/// </summary>
		public ReadOnlyCollection<IParseError> Errors { get; private set; }

		/// <summary>
		///     Gets the stack that the parse ended with.
		/// </summary>
		public ReadOnlyCollection<KeyValuePair<int, GrammarElement<T>>> Stack { get; private set; }

		/// <summary>
		///     Gets whether the parse was a success.
		/// </summary>
		public bool Success { get; private set; }

		/// <summary>
		///     Gets the parse tree, copied over so it is immutable.
		///     This tree may or may not be complete, depending on if the parse was a success.
		/// </summary>
		/// <returns></returns>
		public SyntaxTree<T> GetParseTree()
		{
			if (syntaxTree != null)
				return new SyntaxTree<T>(syntaxTree);
			return null;
		}
	}
}