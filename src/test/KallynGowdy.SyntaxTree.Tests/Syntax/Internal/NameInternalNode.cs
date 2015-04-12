using System;
using System.Collections.Immutable;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class NameInternalNode : InternalSyntaxNode
	{
		public string Name { get; }

		public NameInternalNode(string firstName) : base(ImmutableArray.Create<InternalSyntaxNode>())
		{
			if (firstName == null) throw new ArgumentNullException("firstName");
			Name = firstName;
		}

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			return new NameInternalNode(Name);
		}

		public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
		{
			return new NameNode(this, parent, tree);
		}

		public override long Length => Name.Length;

		public override string ToString()
		{
			return Name;
		}
	}
}