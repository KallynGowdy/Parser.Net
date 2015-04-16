using KallynGowdy.SyntaxTree.Internal;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines a structure that represents a section of trivia (non-semantic content) in a syntax tree.
	/// </summary>
	public struct SyntaxTrivia : IHasSyntaxTree, IHasSyntaxPosition
	{

		public SyntaxTrivia(string v) : this()
		{
		}

		public InternalSyntaxTrivia InternalNode { get; }
		
		public TextSpan Span { get; }

		public TextSpan FullSpan { get; }

		public SyntaxTree Tree
		{
			get;
		}

		public SyntaxNode Parent { get; }
	}
}