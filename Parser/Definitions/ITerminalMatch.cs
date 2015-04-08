using System;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Definitions
{
	/// <summary>
	///     Defines an interface for objects that support matching themselves to terminals
	/// </summary>
	public interface ITerminalMatch<T>
		where T : IEquatable<T>
	{
		bool TerminalMatch(Terminal<T> terminal);
	}
}