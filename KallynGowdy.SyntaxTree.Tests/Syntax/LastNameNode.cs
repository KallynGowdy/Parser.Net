using System;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax
{
	public class LastNameNode : SyntaxNode<LastNameInternalNode, LastNameNode>
	{
		public LastNameNode(LastNameInternalNode lastNameInternalNode, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
			: base(lastNameInternalNode, parent, tree)
		{
		}

		public LastNameNode(string lastName) : this(lastName, n => null, t => null)
		{
		}

		public LastNameNode(string lastName, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
			: base(new LastNameInternalNode(lastName), parent, tree)
		{
		}

		public string LastName => InternalNode.LastName;

		public LastNameNode SetLastName(string lastName)
		{
			return new LastNameNode(lastName);
		}

		public override bool Equals(SyntaxNode other)
		{
			return base.Equals(other) && Equals((LastNameNode)other);
		}

		private bool Equals(LastNameNode lastNameNode)
		{
			return lastNameNode.LastName == LastName;
		}
	}
}