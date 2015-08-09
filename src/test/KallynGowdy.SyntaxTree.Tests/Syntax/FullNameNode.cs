using System;
using System.Linq;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax
{
	public class FullNameNode : SyntaxNode<FullNameInternalSyntaxNode, FullNameNode>
	{
		public FullNameNode(FullNameInternalSyntaxNode internalNode, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
			: base(internalNode, parent, tree)
		{
		}

		public FullNameNode(NameNode name, NameNode lastName) : base(new FullNameInternalSyntaxNode(name.InternalNode, lastName.InternalNode), null, null)
		{
		}

		public FullNameNode(params SyntaxNode[] nodes) : base(new FullNameInternalSyntaxNode(nodes.Where(n => n != null).Select(n => n.InternalNode)), null, null)
		{
		}

		protected new FullNameInternalSyntaxNode InternalNode => (FullNameInternalSyntaxNode)base.InternalNode;

		public NameNode FirstName => (NameNode)Children.First(c => c is NameNode);

        public NameNode LastName => (NameNode)Children.Last(c => c is NameNode);

		
	}
}