using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KallynGowdy.SyntaxTree.Internal;
using KallynGowdy.SyntaxTree.Tests.Syntax;
using KallynGowdy.SyntaxTree.Tests.Syntax.Internal;
using Xunit;

namespace KallynGowdy.SyntaxTree.Tests.Example
{
    /// <summary>
    /// Defines a class that contains examples on how a syntax tree should be built.
    /// </summary>
    public class BuildSyntaxTreeExample
    {
        /// <summary>
        /// An example of how a syntax tree would be built from the bottom up.
        /// </summary>
        /// <remarks>
        /// As you look through the example, notice that the child nodes are generated first. 
        /// This is the very definition of bottom-up parsing, and it is what the API is optimized for. 
        /// If you were to generate a syntax tree top-down (which is very possible by the way), performance
        /// might be hampered by the fact that new nodes would have to be re-created for every addition and insertion to the tree.
        /// </remarks>
        [Fact]
        public void Test_BuildSyntaxTree()
        {
            // The input string into the parser
            // In our case, the parser is a very simple lastname-firstname parser, where the first token in
            // the input is the first name and the rest are just added on.
            string[] input = {"First", "Last"};

            NameNode[] names = new NameNode[input.Length];

            // In our case, the first grammar rule would be to reduce all of the names in the input to a list of NameInternalNode objects
            // e.g. 
            // FullName -> { Name }
            // Name -> Word
            for (int i = 0; i < input.Length; i++)
            {
                // The parser identifies a node and creates the internal representation of it
                string name = input[i];
                NameNode nameNode = new NameNode(name);
                names[i] = nameNode;
            }

            // Then the parser checks to see if a reduction to a full name node can be made and reduces if it can
            FullNameNode fullName = new FullNameNode(names);

            // Then a full syntax tree is produced and returned
            MockSyntaxTree tree = new MockSyntaxTree(fullName);

            Assert.Collection(
                tree.Root.Children,
                node => Assert.Equal("First", node.ToString()),
                node => Assert.Equal("Last", node.ToString())
            );
        }

        [Fact]
        public void Test_FindChangesInSyntaxTree()
        {
            // "First Last"
            NameNode firstName = new NameNode("First", new SyntaxTrivia(), new SyntaxTrivia(" "));
            NameNode lastName = new NameNode("Last");
            FullNameNode fullName = new FullNameNode(
                firstName,
                lastName
            );

            MockSyntaxTree tree = new MockSyntaxTree(fullName);

            SyntaxTreeDifferences changes = tree.FindChanges("\nFirst Middle Last ");

            Assert.Same(tree, changes.OriginalTree);
            Assert.True(changes.HasChanges);
            Assert.Equal(
                new MockSyntaxTree(
                    new FullNameNode(
                        new SyntaxChangeNode("\n"),
                        firstName,
                        new SyntaxChangeNode("Middle "),
                        lastName,
                        new SyntaxChangeNode(" ")
                    )
                ),
                changes.NewTree);
        }

    }
}
