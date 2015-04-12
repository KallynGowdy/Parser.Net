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
			var firstName = new NameNode("Kallyn");

			Assert.Equal("Kallyn", firstName.Name);
			Assert.Null(firstName.Parent);
			Assert.Null(firstName.Tree);
			Assert.Equal(0, firstName.Position);
			Assert.NotNull(firstName.Children);
			Assert.Equal(0, firstName.Children.Count);
			Assert.Equal(6, firstName.Length);

			var lastName = new NameNode("Gowdy");

			Assert.Equal("Gowdy", lastName.Name);
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

			Assert.Equal(new FullNameNode(new NameNode("Kallyn"), new NameNode("Gowdy")), fullName);
			Assert.Null(fullName.Parent);
			Assert.Null(fullName.Tree);
			Assert.NotNull(fullName.Children);
			Assert.Equal(0, fullName.Position);

			Assert.Equal(3, fullName.Children.Count);
			Assert.Same(fullName.FirstName, fullName.Children[0]);
			Assert.Same(fullName.LastName, fullName.Children[2]);

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
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
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
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
				)
			);

			FullNameNode newFullName = new FullNameNode(
				new NameNode("K"),
				new NameNode("G")
			);

			var newTree = tree.SetRoot(newFullName);

			Assert.NotNull(newTree);
			Assert.IsType<MockSyntaxTree>(newTree);
			Assert.NotSame(tree, newTree);
		}

		[Fact]
		public void Test_InternalNodeCanBeSharedBetweenDifferentTrees()
		{
			var firstName = new NameNode("Kallyn");

			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					firstName,
					new NameNode("Gowdy")
				)
			);

			Assert.Equal("Kallyn", tree.FullName.FirstName.ToString());

			var newTree = tree.SetRoot(
				new FullNameNode(
					firstName,
					new NameNode("G")
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
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
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
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
				)
			);

			NameNode newName = tree.Root.FirstName.SetFirstName("Kal");

			Assert.Null(newName.Parent);
			Assert.Null(newName.Parent);
			Assert.NotNull(newName);

			Assert.NotSame(tree.Root.FirstName, newName);
			Assert.NotSame(tree.Root, newName.Parent);

			Assert.Equal("Kal", newName.Name);

			var newRoot = tree.Root.ReplaceNode(tree.Root.FirstName, newName);

			Assert.Null(newRoot.Parent);
			Assert.NotNull(newRoot.Tree);

			Assert.Same(newRoot.InternalNode.FirstName, newName.InternalNode);

			Assert.NotSame(tree.Root, newRoot);
			Assert.NotSame(tree, newRoot.Tree);
			Assert.NotSame(newName, newRoot.FirstName);
		}

		[Fact]
		public void Test_ReplaceNodeCreatesNewFacadeTree()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
				)
			);

			var lastName = tree.FullName.LastName;
			var newLastName = new NameNode("G");

			var newFullName = tree.FullName.ReplaceNode(lastName, newLastName);

			Assert.NotSame(tree.FullName, newFullName);
			Assert.NotEqual(tree.FullName, newFullName);

			Assert.NotSame(tree, newFullName.Tree);
			Assert.NotEqual(tree, newFullName.Tree);
		}

		[Fact]
		public void Test_InsertNodeCreatesNewFacadeTree()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
				)
			);

			Assert.Null(tree.FullName.MiddleName);

			NameNode middleName = new NameNode("G.");

			var fullName = (FullNameNode)tree.FullName.InsertNode(1, middleName);

			Assert.NotNull(fullName.MiddleName);
			Assert.Equal(middleName, fullName.MiddleName);
			Assert.Equal(13, fullName.Length);
			Assert.Equal("{Kallyn G. Gowdy}", fullName.ToString());
			Assert.NotSame(tree, fullName.Tree);

			Assert.Collection(
				fullName.Children,
				n => Assert.Equal(new NameNode("Kallyn"), n),
				n => Assert.Equal(new NameNode("G."), n),
				n => Assert.Equal(new NameNode("Gowdy"), n)
			);

			Assert.Collection(
				tree.FullName.Children,
				n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
				n => Assert.Null(n),
				n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode)
			);

			Assert.Collection(
				fullName.Children,
				n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
				n => Assert.Same(n.InternalNode, middleName.InternalNode),
				n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode)
			);
		}

		[Fact]
		public void Test_AddNodeCreatesNewFacadeTree()
		{
			MockSyntaxTree tree = new MockSyntaxTree(
				new FullNameNode(
					new NameNode("Kallyn"),
					new NameNode("Gowdy")
				)
			);

			Assert.Null(tree.FullName.MiddleName);
			Assert.Equal(3, tree.FullName.Children.Count);

			var otherLastName = new NameNode("Other");

			var fullName = (FullNameNode)tree.FullName.AddNode(otherLastName);

			Assert.Collection(
				fullName.Children,
				n => Assert.Equal(new NameNode("Kallyn"), n),
				n => Assert.Null(n),
				n => Assert.Equal(new NameNode("Gowdy"), n),
				n => Assert.Equal(new NameNode("Other"), n)
            );

			Assert.Collection(
				tree.FullName.Children,
				n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
				n => Assert.Null(n),
				n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode)
			);

			Assert.Equal(16, fullName.Length);
			Assert.Equal("{Kallyn Gowdy Other}", fullName.ToString());
		}
	}
}
