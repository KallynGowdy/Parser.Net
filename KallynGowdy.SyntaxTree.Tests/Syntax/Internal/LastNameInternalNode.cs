using System;
using System.Collections.Immutable;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class LastNameInternalNode : InternalSyntaxNode
	{
		public string LastName { get; }

		public LastNameInternalNode(string lastName) : base(ImmutableArray.Create<InternalSyntaxNode>())
		{
			if (lastName == null) throw new ArgumentNullException("lastName");
			LastName = lastName;
		}

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			return new LastNameInternalNode(LastName);
		}

		public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
		{
			return new LastNameNode(this, parent, tree);
		}

		public override long Length => LastName.Length;

		public override string ToString()
		{
			return LastName;
		}
	}
}