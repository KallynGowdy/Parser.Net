using System;
using KallynGowdy.SyntaxTree.Internal;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax
{
	public class NameNode : SyntaxNode<NameInternalNode, NameNode>
	{
		public NameNode(string firstName) : this(firstName, n => null, t => null)
		{
		}

		public NameNode(string firstName, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree) : this(new NameInternalNode(firstName), parent, tree)
		{
		}

		public NameNode(InternalSyntaxNode internalNode, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree) : base(internalNode, parent, tree)
		{
		}

		public string Name => InternalNode.Name;

		public NameNode SetFirstName(string firstName)
		{
			return new NameNode(firstName);
		}
	}
}