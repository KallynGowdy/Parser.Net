namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Represents a parse tree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class SyntaxTree<T>
	{
		public SyntaxTree(SyntaxNode<T> rootBranch)
		{
			Root = rootBranch;
			Root.ParentTree = this;
		}

		/// <summary>
		///     Creates a new parse tree from the given other tree.
		/// </summary>
		/// <param name="otherTree"></param>
		public SyntaxTree(SyntaxTree<T> otherTree)
		{
			Root = new SyntaxNode<T>(otherTree.Root);
		}

		/// <summary>
		///     Creates a new empty parse tree.
		/// </summary>
		public SyntaxTree()
		{
		}

		/// <summary>
		///     Gets the root element of this tree.
		/// </summary>
		public SyntaxNode<T> Root { get; }
	}
}