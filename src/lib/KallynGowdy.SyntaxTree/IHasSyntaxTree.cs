namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines an interface that represents an object that contains a reference to it's containing <see cref="SyntaxTree"/>.
	/// </summary>
	public interface IHasSyntaxTree
	{
		/// <summary>
		/// Gets the <see cref="SyntaxTree"/> that this element belongs to.
		/// </summary>
		SyntaxTree Tree { get; }
	}
}