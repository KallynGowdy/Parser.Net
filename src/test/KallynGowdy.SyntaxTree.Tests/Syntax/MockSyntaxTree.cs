using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax
{

	public class MockSyntaxTree : SyntaxTree<MockInternalSyntaxTree, FullNameNode>
	{
		public MockSyntaxTree(FullNameNode fullNameNode) : base(new MockInternalSyntaxTree(fullNameNode.InternalNode))
		{
		}

		public MockSyntaxTree SetFullName(FullNameNode fullName)
		{
			return (MockSyntaxTree) SetRoot(fullName);
		}

		public FullNameNode FullName => Root;
		protected override SyntaxTree CreateNewTree(SyntaxNode root)
		{
			return new MockSyntaxTree((FullNameNode)root);
		}
	}
}