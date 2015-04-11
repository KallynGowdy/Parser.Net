namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class MockInternalSyntaxTree : InternalSyntaxTree
	{
		public new FullNameInternalSyntaxNode Root => (FullNameInternalSyntaxNode)base.Root;

		public MockInternalSyntaxTree(FullNameInternalSyntaxNode root) : base(root)
		{
		}
	}
}