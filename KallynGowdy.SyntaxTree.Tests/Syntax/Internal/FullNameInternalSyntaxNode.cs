using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class FullNameInternalSyntaxNode : InternalSyntaxNode
	{
		public FullNameInternalSyntaxNode(FirstNameInternalNode firstName, LastNameInternalNode lastName) : base(ImmutableArray.Create<InternalSyntaxNode>(firstName, lastName))
		{
			if (firstName == null) throw new ArgumentNullException("firstName");
			if (lastName == null) throw new ArgumentNullException("lastName");
		}

		public FirstNameInternalNode FirstName => (FirstNameInternalNode)Children[0];

		public LastNameInternalNode LastName => (LastNameInternalNode)Children[1];

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			if (children.Count == 2)
			{
				var firstName = children[0] as FirstNameInternalNode;
				var lastName = children[1] as LastNameInternalNode;
				if (firstName != null &&
					lastName != null)
				{
					return new FullNameInternalSyntaxNode(firstName, lastName);
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
			return string.Format("{{{0} {1}}}", FirstName, LastName);
		}
	}
}