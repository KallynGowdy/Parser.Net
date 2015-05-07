using System;
using System.Diagnostics.CodeAnalysis;
using KallynGowdy.SyntaxTree.Tests.Syntax;
using Xunit;

namespace KallynGowdy.SyntaxTree.Tests
{
    /// <summary>
    /// Tests for <see cref="SyntaxTree"/>.
    /// </summary>
    [SuppressMessage("ReSharper", "ExceptionNotDocumented")]
    public class SyntaxNodeTests
    {
        [Fact]
        public void Test_TreeIsProperlyCreatedWithReferences()
        {
            NameNode firstName = new NameNode("Kallyn");

            Assert.Equal("Kallyn", firstName.Name);
            Assert.Null(firstName.Parent);
            Assert.Null(firstName.Tree);
            Assert.Equal(0, firstName.Position);
            Assert.NotNull(firstName.Children);
            Assert.Equal(0, firstName.Children.Count);
            Assert.Equal(6, firstName.Width);

            NameNode lastName = new NameNode("Gowdy");

            Assert.Equal("Gowdy", lastName.Name);
            Assert.Null(lastName.Parent);
            Assert.Null(lastName.Tree);
            Assert.NotNull(lastName.Children);
            Assert.Equal(0, lastName.Position);
            Assert.Equal(0, lastName.Children.Count);
            Assert.Equal(5, lastName.Width);


            FullNameNode fullName = new FullNameNode(
                firstName,
                lastName
                );

            Assert.Equal(new FullNameNode(new NameNode("Kallyn"), new NameNode("Gowdy")), fullName);
            Assert.Null(fullName.Parent);
            Assert.Null(fullName.Tree);
            Assert.NotNull(fullName.Children);
            Assert.Equal(0, fullName.Position);

            Assert.Equal(2, fullName.Children.Count);
            Assert.Same(fullName.FirstName, fullName.Children[0]);
            Assert.Same(fullName.LastName, fullName.Children[1]);

            Assert.Equal(11, fullName.Width);

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

            SyntaxTree newTree = tree.SetRoot(newFullName);

            Assert.NotNull(newTree);
            Assert.IsType<MockSyntaxTree>(newTree);
            Assert.NotSame(tree, newTree);
        }

        [Fact]
        public void Test_InternalNodeCanBeSharedBetweenDifferentTrees()
        {
            NameNode firstName = new NameNode("Kallyn");

            MockSyntaxTree tree = new MockSyntaxTree(
                new FullNameNode(
                    firstName,
                    new NameNode("Gowdy")
                    )
                );

            Assert.Equal("Kallyn", tree.FullName.FirstName.ToString());

            MockSyntaxTree newTree = tree.SetRoot(
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

            FullNameNode root = tree.Root as FullNameNode;

            Assert.Null(root.Parent);
            Assert.NotNull(root.Tree);
            Assert.Same(tree, root.Tree);
            Assert.Equal(0, root.Position);
            Assert.Equal(11, root.Width);
            Assert.Equal("{Kallyn Gowdy}", root.ToString());

            NameNode firstName = root.FirstName;

            Assert.NotNull(firstName.Parent);
            Assert.Same(root, firstName.Parent);
            Assert.Same(tree, firstName.Tree);
            Assert.Equal(0, firstName.Position);

            NameNode lastName = root.LastName;

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

            FullNameNode newRoot = tree.Root.ReplaceNode(tree.Root.FirstName, newName);

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

            NameNode lastName = tree.FullName.LastName;
            NameNode newLastName = new NameNode("G");

            FullNameNode newFullName = tree.FullName.ReplaceNode(lastName, newLastName);

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

            NameNode middleName = new NameNode("G.");

            FullNameNode fullName = (FullNameNode)tree.FullName.InsertNode(1, middleName);

            Assert.Equal(13, fullName.Width);
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
                n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode)
            );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.NotSame(n, fullName.FirstName),
                n => Assert.NotSame(n, fullName.LastName)
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

            Assert.Equal(2, tree.FullName.Children.Count);

            NameNode otherLastName = new NameNode("Other");

            FullNameNode fullName = (FullNameNode)tree.FullName.AddNode(otherLastName);

            Assert.Collection(
                fullName.Children,
                n => Assert.Equal(new NameNode("Kallyn"), n),
                n => Assert.Equal(new NameNode("Gowdy"), n),
                n => Assert.Equal(new NameNode("Other"), n)
            );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
                n => Assert.Same(n.InternalNode, fullName.Children[1].InternalNode) // Last name gets changed by AddNode
            );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.NotSame(n, fullName.FirstName),
                n => Assert.NotSame(n, fullName.Children[1])
            );

            Assert.Equal(16, fullName.Width);
            Assert.Equal("{Kallyn Gowdy Other}", fullName.ToString());
        }

        [Fact]
        public void Test_RemoveNode()
        {
            MockSyntaxTree tree = new MockSyntaxTree(
                new FullNameNode(
                    new NameNode("Kallyn"),
                    new NameNode("G."),
                    new NameNode("Gowdy"),
                    new NameNode("Other")
                    )
                );

            FullNameNode fullName = (FullNameNode)tree.FullName.RemoveNode(tree.FullName.Children[3]);


            Assert.Collection(
                fullName.Children,
                n => Assert.Equal(new NameNode("Kallyn"), n),
                n => Assert.Equal(new NameNode("G."), n),
                n => Assert.Equal(new NameNode("Gowdy"), n)
            );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
                n => Assert.Same(n.InternalNode, fullName.Children[1].InternalNode),
                n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode),
                Assert.NotNull
            );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.NotSame(n, fullName.FirstName),
                n => Assert.NotSame(n, fullName.Children[1]),
                n => Assert.NotSame(n, fullName.LastName),
                Assert.NotNull
            );

            Assert.Equal(13, fullName.Width);
            Assert.Equal("{Kallyn G. Gowdy}", fullName.ToString());

        }

        [Fact]
        public void Test_RemoveNodeShiftsChildren()
        {
            MockSyntaxTree tree = new MockSyntaxTree(
                new FullNameNode(
                    new NameNode("Kallyn"),
                    new NameNode("G."),
                    new NameNode("Gowdy"),
                    new NameNode("Other")
                    )
                );

            var fullName = (FullNameNode)tree.FullName.RemoveNode(tree.FullName.Children[1]);

            Assert.Collection(
                fullName.Children,
                n => Assert.Equal(new NameNode("Kallyn"), n),
                n => Assert.Equal(new NameNode("Gowdy"), n),
                n => Assert.Equal(new NameNode("Other"), n)
                );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
                Assert.NotNull,
                n => Assert.Same(n.InternalNode, fullName.Children[1].InternalNode),
                n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode)
                );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.NotSame(n, fullName.FirstName),
                Assert.NotNull,
                n => Assert.NotSame(n, fullName.Children[1]),
                n => Assert.NotSame(n, fullName.LastName)
                );

            Assert.Equal(16, fullName.Width);
            Assert.Equal("{Kallyn Gowdy Other}", fullName.ToString());
        }

        [Fact]
        public void Test_RemoveNodeAt()
        {
            MockSyntaxTree tree = new MockSyntaxTree(
                new FullNameNode(
                    new NameNode("Kallyn"),
                    new NameNode("G."),
                    new NameNode("Gowdy"),
                    new NameNode("Other")
                    )
                );

            FullNameNode fullName = (FullNameNode)tree.FullName.RemoveNodeAt(3);

            Assert.Collection(
                fullName.Children,
                n => Assert.Equal(new NameNode("Kallyn"), n),
                n => Assert.Equal(new NameNode("G."), n),
                n => Assert.Equal(new NameNode("Gowdy"), n)
                );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
                n => Assert.Same(n.InternalNode, fullName.Children[1].InternalNode),
                n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode),
                Assert.NotNull
                );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.NotSame(n, fullName.FirstName),
                n => Assert.NotSame(n, fullName.Children[1]),
                n => Assert.NotSame(n, fullName.LastName),
                Assert.NotNull
                );

            Assert.Equal(13, fullName.Width);
            Assert.Equal("{Kallyn G. Gowdy}", fullName.ToString());

        }

        [Fact]
        public void Test_RemoveNodeAtShiftsChildren()
        {
            MockSyntaxTree tree = new MockSyntaxTree(
                new FullNameNode(
                    new NameNode("Kallyn"),
                    new NameNode("G."),
                    new NameNode("Gowdy"),
                    new NameNode("Other")
                    )
                );

            var fullName = (FullNameNode)tree.FullName.RemoveNodeAt(1);

            Assert.Collection(
                fullName.Children,
                n => Assert.Equal(new NameNode("Kallyn"), n),
                n => Assert.Equal(new NameNode("Gowdy"), n),
                n => Assert.Equal(new NameNode("Other"), n)
                );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.Same(n.InternalNode, fullName.FirstName.InternalNode),
                Assert.NotNull,
                n => Assert.Same(n.InternalNode, fullName.Children[1].InternalNode),
                n => Assert.Same(n.InternalNode, fullName.LastName.InternalNode)
            );

            Assert.Collection(
                tree.FullName.Children,
                n => Assert.NotSame(n, fullName.FirstName),
                Assert.NotNull,
                n => Assert.NotSame(n, fullName.Children[1]),
                n => Assert.NotSame(n, fullName.LastName)
                );

            Assert.Equal(16, fullName.Width);
            Assert.Equal("{Kallyn Gowdy Other}", fullName.ToString());
        }

        [Fact]
        public void Test_CreateNodeWithTrivia()
        {
            NameNode name = new NameNode("Name", new SyntaxTrivia("\n\n "), new SyntaxTrivia("\n"));

            Assert.Equal(new SyntaxTrivia("\n\n "), name.LeadingTrivia);
            Assert.Equal(new SyntaxTrivia("\n"), name.TrailingTrivia);
            Assert.Equal("\n\n Name\n", name.ToString());
        }

        [Fact]
        public void Test_TriviaDoesNotFactorIntoEquality()
        {
            NameNode name = new NameNode("Name", new SyntaxTrivia("\n\n "), new SyntaxTrivia("\n"));
            NameNode otherName = new NameNode("Name");

            Assert.NotEqual(name.LeadingTrivia, otherName.LeadingTrivia);
            Assert.NotEqual(name.TrailingTrivia, otherName.TrailingTrivia);
            Assert.NotEqual(name.ToString(), otherName.ToString());
            Assert.Equal(name, otherName);
        }

        [Fact]
        public void Test_StrictEqualsCoversTrivia()
        {
            NameNode name = new NameNode("Name", new SyntaxTrivia("\n\n "), new SyntaxTrivia("\n"));
            NameNode otherName = new NameNode("Name");

            Assert.NotEqual(name.LeadingTrivia, otherName.LeadingTrivia);
            Assert.NotEqual(name.TrailingTrivia, otherName.TrailingTrivia);
            Assert.NotEqual(name.ToString(), otherName.ToString());
            Assert.False(name.StrictEquals(otherName));
        }

        [Fact]
        public void Test_StrictEqualsCoversChildren()
        {
            NameNode firstName = new NameNode("First", new SyntaxTrivia("\n\n "), new SyntaxTrivia("\n"));
            NameNode lastName= new NameNode("Last", new SyntaxTrivia(" "), new SyntaxTrivia("\t\t"));
            FullNameNode fullName = new FullNameNode(firstName, lastName);

            NameNode otherFirstName = new NameNode("First", new SyntaxTrivia("\n "), new SyntaxTrivia("\n"));
            FullNameNode otherFullName = new FullNameNode(otherFirstName, lastName);

            Assert.Equal(fullName, otherFullName);
            Assert.False(fullName.StrictEquals(otherFullName));
        }

        [Theory]
        [MemberData("Test_NodeCalculatesSpanCorrectly_Data")]
        public void Test_NodeCalculatesSpanCorrectly(SyntaxNode node, TextSpan expectedSpan)
        {
            TextSpan span = node.Span;
            Assert.Equal(expectedSpan, span);
            Assert.Equal(node.Position, span.Start);
            Assert.Equal(node.Width, span.Length);
        }

        public static object[] Test_NodeCalculatesSpanCorrectly_Data => new object[]
        {
            new object[]
            {
                new NameNode("Name"),
                new TextSpan(0, 4)
            },
            new object[]
            {
                new FullNameNode(
                    new NameNode("First"),
                    new NameNode("Last")
                ),
                new TextSpan(0, 9)
            }
        };

        [Theory]
        [MemberData("Test_NodeOperatorEquality_Data")]
        public void Test_NodeOperatorEquality(SyntaxNode first, SyntaxNode second)
        {
            Assert.Equal(first, second);
            Assert.Equal((object)first, (object)second);
            Assert.Equal(first.InternalNode, second.InternalNode);
            Assert.Equal((object)first.InternalNode, (object)second.InternalNode);
            Assert.True(first == second, "first == second");
            Assert.False(first != second, "first != second");
            Assert.True(first.InternalNode == second.InternalNode, "first.InternalNode == second.InternalNode");
            Assert.False(first.InternalNode != second.InternalNode, "first.InternalNode != second.InternalNode");
            Assert.NotSame(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
            Assert.True(first.Equals((object)first), "first.Equals(first)");
            Assert.True(second.Equals((object)second), "second.Equals(second)");
        }

        [Theory]
        [MemberData("Test_NodeOperatorNotEqual_Data")]
        public void Test_NodeOperatorNotEqual(SyntaxNode first, SyntaxNode second)
        {
            Assert.NotEqual(first, second);
            Assert.NotEqual((object)first, (object)second);
            Assert.NotEqual(first.InternalNode, second.InternalNode);
            Assert.NotEqual((object)first.InternalNode, (object)second.InternalNode);
            Assert.True(first != second, "first != second");
            Assert.False(first == second, "first == second");
            Assert.True(first.InternalNode != second.InternalNode, "first.InternalNode != second.InternalNode");
            Assert.False(first.InternalNode == second.InternalNode, "first.InternalNode == second.InternalNode");
            Assert.NotSame(first, second);
            Assert.NotEqual(first.GetHashCode(), second.GetHashCode());
            Assert.False(first.Equals(null));
            Assert.False(second.Equals(null));
            Assert.False(first.Equals((object)null));
            Assert.False(second.Equals((object)null));
        }

        public static object[] Test_NodeOperatorEquality_Data => new object[]
        {
            new object[]
            {
                new NameNode("Kallyn"),
                new NameNode("Kallyn")
            },
            new object[]
            {
                new FullNameNode(new NameNode("Kal"), new NameNode("Gowdy")),
                new FullNameNode(new NameNode("Kal"), new NameNode("Gowdy"))
            }
        };

        public static object[] Test_NodeOperatorNotEqual_Data => new object[]
        {
            new object[]
            {
                new NameNode("Gowdy"),
                new NameNode("Kallyn")
            },
            new object[]
            {
                new FullNameNode(new NameNode("Kal"), new NameNode("Gowdy")),
                new FullNameNode(new NameNode("NotKal"), new NameNode("Gowdy"))
            }

        };
    }
}
