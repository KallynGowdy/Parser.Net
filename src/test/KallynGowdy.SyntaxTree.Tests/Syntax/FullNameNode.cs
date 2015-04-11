using System;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax
{
	public class FullNameNode : SyntaxNode<FullNameInternalSyntaxNode, FullNameNode>
	{
		public FullNameNode(FullNameInternalSyntaxNode internalNode, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
			: base(internalNode, parent, tree)
		{
		}

		public FullNameNode(FirstNameNode firstName, LastNameNode lastName) : base(new FullNameInternalSyntaxNode(firstName.InternalNode, lastName.InternalNode), null, null)
		{
		}

		protected new FullNameInternalSyntaxNode InternalNode => (FullNameInternalSyntaxNode)base.InternalNode;

		public FirstNameNode FirstName => (FirstNameNode)Children[0];

		public LastNameNode LastName => (LastNameNode)Children[1];
	}
}