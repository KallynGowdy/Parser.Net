using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KallynGowdy.SyntaxTree.Tests.Syntax;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;
using Xunit;

namespace KallynGowdy.SyntaxTree.Tests
{
	/// <summary>
	/// Tests for <see cref="SyntaxTree"/>.
	/// </summary>
	public class SyntaxTreeTests
	{
		[Fact]
		public void Test_TreeIsProperlyCreatedWithReferences()
		{
			var firstName = new FirstNameNode("Kallyn");

			Assert.Equal("Kallyn", firstName.FirstName);
			Assert.Null(firstName.Parent);
			Assert.Null(firstName.Tree);
			Assert.Equal(0, firstName.Position);
			Assert.NotNull(firstName.Children);
			Assert.Equal(0, firstName.Children.Count);
			Assert.Equal(6, firstName.Length);

			var lastName = new LastNameNode("Gowdy");

			Assert.Equal("Gowdy", lastName.LastName);
			Assert.Null(lastName.Parent);
			Assert.Null(lastName.Tree);
			Assert.NotNull(lastName.Children);
			Assert.Equal(0, lastName.Position);
			Assert.Equal(0, lastName.Children.Count);
			Assert.Equal(5, lastName.Length);


			var fullName = new FullNameNode(
					firstName,
					lastName
			);

			Assert.Equal(new FullNameNode(new FirstNameNode("Kallyn"), new LastNameNode("Gowdy")), fullName);
			Assert.Null(fullName.Parent);
			Assert.Null(fullName.Tree);
			Assert.NotNull(fullName.Children);
			Assert.Equal(0, fullName.Position);

			Assert.Equal(2, fullName.Children.Count);
			Assert.Same(fullName.FirstName, fullName.Children[0]);
			Assert.Same(fullName.LastName, fullName.Children[1]);

			//Assert.Collection(fullName.Children, 
			//	n => Assert.Same(fullName.FirstName, n),
			//	n => Assert.Same(fullName.LastName, n)
			//);

			Assert.Equal(11, fullName.Length);

			Assert.Equal(firstName, fullName.FirstName);
			Assert.Equal(lastName, fullName.LastName);
			Assert.Equal(firstName.InternalNode, fullName.FirstName.InternalNode);
			Assert.Equal(lastName.InternalNode, fullName.LastName.InternalNode);

			Assert.Equal(0, fullName.FirstName.Position);
			Assert.Equal(6, fullName.LastName.Position);

			MockSyntaxTree tree = new MockSyntaxTree(
				fullName
			);

			Assert.Equal(new MockSyntaxTree(
				new FullNameNode(
					new FirstNameNode("Kallyn"),
					new LastNameNode("Gowdy")
				)
			), tree);
			Assert.Equal(fullName, tree.Root);
			Assert.Same(tree.Root, tree.FullName);
		}

		[Fact]
		public void Test_SetRootCreatesNewImmutableTree()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new FirstNameNode("Kallyn"),
					new LastNameNode("Gowdy")
				)
			);

			FullNameNode newFullName = new FullNameNode(
				new FirstNameNode("K"),
				new LastNameNode("G")
			);

			var newTree = tree.SetRoot(newFullName);

			Assert.NotNull(newTree);
			Assert.IsType<MockSyntaxTree>(newTree);
			Assert.NotSame(tree, newTree);
		}

		[Fact]
		public void Test_InternalNodeCanBeSharedBetweenDifferentTrees()
		{
			var firstName = new FirstNameNode("Kallyn");

			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					firstName,
					new LastNameNode("Gowdy")
				)
			);

			Assert.Equal("Kallyn", tree.FullName.FirstName.ToString());

			var newTree = tree.SetRoot(
				new FullNameNode(
					firstName,
					new LastNameNode("G")
				)
			) as MockSyntaxTree;

			Assert.NotNull(newTree);

			Assert.NotSame(tree, newTree);
			Assert.NotSame(tree.Root, newTree.Root);
			Assert.NotSame(tree.Root.FirstName, firstName);
			Assert.NotSame(newTree.Root.FirstName, firstName);
			Assert.NotSame(tree.Root.FirstName, newTree.Root.FirstName);

			Assert.NotEqual(tree, newTree);
			Assert.NotEqual(tree.Root, newTree.Root);

			Assert.Equal(tree.Root.FirstName, firstName);
			Assert.Equal(newTree.Root.FirstName, firstName);
			Assert.Equal(tree.Root.FirstName, newTree.Root.FirstName);

			Assert.Same(tree.Root.FirstName.InternalNode, firstName.InternalNode);
		}

		[Fact]
		public void Test_NewNodesAreGeneratedAsTreeIsTraversed()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new FirstNameNode("Kallyn"),
					new LastNameNode("Gowdy")
				)
			);

			Assert.NotNull(tree);
			Assert.NotNull(tree.Root);

			var root = tree.Root as FullNameNode;

			Assert.Null(root.Parent);
			Assert.NotNull(root.Tree);
			Assert.Same(tree, root.Tree);
			Assert.Equal(0, root.Position);
			Assert.Equal(11, root.Length);
			Assert.Equal("{Kallyn Gowdy}", root.ToString());

			var firstName = root.FirstName;

			Assert.NotNull(firstName.Parent);
			Assert.Same(root, firstName.Parent);
			Assert.Same(tree, firstName.Tree);
			Assert.Equal(0, firstName.Position);

			var lastName = root.LastName;

			Assert.NotNull(lastName);
			Assert.Same(root, lastName.Parent);
			Assert.Same(tree, firstName.Tree);
			Assert.Equal(6, lastName.Position);
		}

		[Fact]
		public void Test_NodesAreImmutable()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new FirstNameNode("Kallyn"),
					new LastNameNode("Gowdy")
				)
			);

			FirstNameNode newFirstName = tree.Root.FirstName.SetFirstName("Kal");

			Assert.Null(newFirstName.Parent);
			Assert.Null(newFirstName.Parent);
			Assert.NotNull(newFirstName);

			Assert.NotSame(tree.Root.FirstName, newFirstName);
			Assert.NotSame(tree.Root, newFirstName.Parent);

			Assert.Equal("Kal", newFirstName.FirstName);

			var newRoot = tree.Root.ReplaceNode(tree.Root.FirstName, newFirstName);

			Assert.Null(newRoot.Parent);
			Assert.NotNull(newRoot.Tree);

			Assert.Same(newRoot.InternalNode.FirstName, newFirstName.InternalNode);

			Assert.NotSame(tree.Root, newRoot);
			Assert.NotSame(tree, newRoot.Tree);
			Assert.NotSame(newFirstName, newRoot.FirstName);
		}

		[Fact]
		public void Test_ReplaceNodeCreatesNewFacadeTree()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new FirstNameNode("Kallyn"),
					new LastNameNode("Gowdy")
				)
			);

			var lastName = tree.FullName.LastName;
			var newLastName = new LastNameNode("G");

			var newFullName = tree.FullName.ReplaceNode(lastName, newLastName);

			Assert.NotSame(tree.FullName, newFullName);
			Assert.NotEqual(tree.FullName, newFullName);

			Assert.NotSame(tree, newFullName.Tree);
			Assert.NotEqual(tree, newFullName.Tree);
		}
	}
}
