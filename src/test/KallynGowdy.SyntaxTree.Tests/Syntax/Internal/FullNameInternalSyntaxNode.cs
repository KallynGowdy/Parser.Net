using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using KallynGowdy.SyntaxTree.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
    public class FullNameInternalSyntaxNode : InternalSyntaxNode
    {
        public FullNameInternalSyntaxNode(NameInternalNode name, NameInternalNode lastName)
            : base(ImmutableList.Create<InternalSyntaxNode>(name, lastName))
        {
            if (name == null) throw new ArgumentNullException("name");
            if (lastName == null) throw new ArgumentNullException("lastName");
        }

        public FullNameInternalSyntaxNode(IEnumerable<InternalSyntaxNode> nodes)
            : base(ImmutableList.Create(nodes.ToArray()))
        {
        }

        public NameInternalNode FirstName => (NameInternalNode)Children.First(c => c is NameInternalNode);

        public NameInternalNode LastName => (NameInternalNode)Children.Last(c => c is NameInternalNode);

        protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
        {
            if (children.Count >= 2)
            {
                InternalSyntaxNode[] c = children.Where(t => t != null).ToArray();
                if (c.Length == children.Count)
                {
                    return new FullNameInternalSyntaxNode(c);
                }
            }
            throw new ArgumentException("Invalid Children");
        }

        public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
        {
            return new FullNameNode(this, parent, tree);
        }

        protected override string ToStringInternal()
        {
            return string.Format("{{{0}}}", string.Join(" ", Children.Where(c => c != null)));
        }
    }
}