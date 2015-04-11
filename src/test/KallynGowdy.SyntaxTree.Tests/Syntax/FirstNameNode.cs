using System;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax
{
	public class FirstNameNode : SyntaxNode<FirstNameInternalNode, FirstNameNode>
	{
		public FirstNameNode(string firstName) : this(firstName, n => null, t => null)
		{
		}

		public FirstNameNode(string firstName, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree) : this(new FirstNameInternalNode(firstName), parent, tree)
		{
		}

		public FirstNameNode(InternalSyntaxNode internalNode, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree) : base(internalNode, parent, tree)
		{
		}

		public string FirstName => InternalNode.FirstName;

		public FirstNameNode SetFirstName(string firstName)
		{
			return new FirstNameNode(firstName);
		}

		public override bool Equals(SyntaxNode other)
		{
			return base.Equals(other) && Equals((FirstNameNode)other);
		}

		private bool Equals(FirstNameNode other)
		{
			return other.FirstName == this.FirstName;
		}
	}
}