namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines an interface that represents objects that can contain a position and length.
	/// </summary>
	public interface IHasSyntaxPosition
	{
		/// <summary>
		/// Gets the representation of this element's span of text.
		/// </summary>
		TextSpan Span { get; }
	}
}