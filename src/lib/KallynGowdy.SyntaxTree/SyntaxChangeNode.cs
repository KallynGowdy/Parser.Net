using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KallynGowdy.SyntaxTree.Internal;

namespace KallynGowdy.SyntaxTree
{
    /// <summary>
    /// Defines a class that represents a <see cref="SyntaxNode"/> that represents a literal change in a syntax tree.
    /// </summary>
    public class SyntaxChangeNode : SyntaxNode<InternalSyntaxChangeNode, SyntaxChangeNode>
    {
        public SyntaxChangeNode(InternalSyntaxNode internalNode, Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree) : base(internalNode, parent, tree)
        {
        }

        public SyntaxChangeNode(string content) : this(new InternalSyntaxChangeNode(content), p => null, t => null)
        {
        }

        /// <summary>
        /// Gets the content that this node represents.
        /// </summary>
        public string Content => InternalNode.Content;
    }
}
