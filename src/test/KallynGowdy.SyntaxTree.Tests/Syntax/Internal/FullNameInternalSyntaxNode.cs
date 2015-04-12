using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class FullNameInternalSyntaxNode : InternalSyntaxNode
	{
		public FullNameInternalSyntaxNode(NameInternalNode name, NameInternalNode lastName) : this(name, null, lastName)
		{
		}

		public FullNameInternalSyntaxNode(NameInternalNode name, NameInternalNode middleName,  NameInternalNode lastName) 
			: base(ImmutableList.Create<InternalSyntaxNode>(name, middleName, lastName))
		{
			if (name == null) throw new ArgumentNullException("name");
			if (lastName == null) throw new ArgumentNullException("lastName");
		}

		public FullNameInternalSyntaxNode(IEnumerable<NameInternalNode> names)
			: base(ImmutableList.Create(names.Cast<InternalSyntaxNode>().ToArray()))
		{
			if (names.Count() < 3) throw new ArgumentException("Not enough required names. Min 3", "names");
		}

		public NameInternalNode FirstName => (NameInternalNode)Children[0];

		public NameInternalNode MiddleName => (NameInternalNode)Children[1];

		public NameInternalNode LastName => (NameInternalNode)Children[2];

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			if (children.Count >= 3)
			{
				InternalSyntaxNode[] c = children.Where(t => t == null || t is NameInternalNode).ToArray();
				if (c.Length == children.Count)
				{
					return new FullNameInternalSyntaxNode(c.Cast<NameInternalNode>());
				}
			}
			throw new ArgumentException("Invalid Children");
		}

		public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
		{
			return new FullNameNode(this, parent, tree);
		}

		public override string ToString()
		{
			return string.Format("{{{0}}}", string.Join(" ", Children.Where(c => c != null)));
		}
	}
}