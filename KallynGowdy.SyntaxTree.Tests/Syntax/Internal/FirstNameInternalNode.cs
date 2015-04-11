using System;
using System.Collections.Immutable;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class FirstNameInternalNode : InternalSyntaxNode
	{
		public string FirstName { get; }

		public FirstNameInternalNode(string firstName) : base(ImmutableArray.Create<InternalSyntaxNode>())
		{
			if (firstName == null) throw new ArgumentNullException("firstName");
			FirstName = firstName;
		}

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			return new FirstNameInternalNode(FirstName);
		}

		public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
		{
			return new FirstNameNode(this, parent, tree);
		}

		public override long Length => FirstName.Length;

		public override string ToString()
		{
			return FirstName;
		}
	}
}