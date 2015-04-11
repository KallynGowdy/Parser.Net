using System;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines a syntax tree that is immutable.
	/// </summary>
	public abstract class InternalSyntaxTree
	{
		protected InternalSyntaxTree(InternalSyntaxNode root)
		{
			if (root == null) throw new ArgumentNullException("root");
			Root = root;
		}

		/// <summary>
		/// Gets the mutable root of the syntax tree.
		/// </summary>
		public InternalSyntaxNode Root { get; }
		
	}
}