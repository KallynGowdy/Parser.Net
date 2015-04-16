namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines an interface for objects that can have leading and trailing <see cref="SyntaxTrivia"/>.
	/// </summary>
	public interface IHasSyntaxTrivia
	{
		/// <summary>
		/// Gets the list of syntax trivia that leads this syntax element.
		/// </summary>
		SyntaxTrivia LeadingTrivia { get; }

		/// <summary>
		/// Gets the list of syntax trivia that trails this syntax element.
		/// </summary>
		SyntaxTrivia TrailingTrivia { get; }
	}
}