using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KallynGowdy.SyntaxTree.Internal;

namespace KallynGowdy.SyntaxTree.Internal
{
    /// <summary>
    /// Defines a class that represents a <see cref="InternalSyntaxNode"/> that represents a literal change in a syntax tree.
    /// </summary>
    public class InternalSyntaxChangeNode : InternalSyntaxNode
    {
        /// <summary>
        /// Gets the content that this node represents.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Creates a new <see cref="InternalSyntaxChangeNode"/>.
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="ArgumentNullException"><paramref name="content"/> is <see langword="null" />.</exception>
        public InternalSyntaxChangeNode(string content) : base(ImmutableList<InternalSyntaxNode>.Empty)
        {
            if (content == null) throw new ArgumentNullException("content");
            this.Content = content;
        }

        protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
        {
            return new InternalSyntaxChangeNode(Content);
        }

        public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
        {
            return new SyntaxChangeNode(this, parent, tree);
        }

        public override bool Equals(InternalSyntaxNode other)
        {
            var n = other as InternalSyntaxChangeNode;
            return n != null &&
                base.Equals(other) &&
                n.Content == this.Content;
        }
    }
}
