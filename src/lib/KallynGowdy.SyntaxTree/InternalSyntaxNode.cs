using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines a class that represents a immutable syntax node.
	/// These classes allow us to create a persistent syntax tree that does not contain parent links between nodes.
	/// This allows us to reuse them on processing.
	/// Implementation detail.
	/// </summary>
	public abstract class InternalSyntaxNode
	{
		protected InternalSyntaxNode(IImmutableList<InternalSyntaxNode> children)
		{
			if (children == null) throw new ArgumentNullException("children");
			Children = children;
		}

		/// <summary>
		/// Gets the length of this node.
		/// </summary>
		public virtual long Length => Children.Sum(c => c.Length);

		/// <summary>
		/// Gets the mutable list of this syntax node's children.
		/// Be sure to check the <see cref="IList{InternalSyntaxNode}.IsReadOnly"/> property to determine whether new child nodes can be added/removed. 
		/// (Individual child nodes should always be settable though)
		/// </summary>
		public IImmutableList<InternalSyntaxNode> Children { get; }

		/// <summary>
		/// Creates a new <see cref="InternalSyntaxNode"/> from the given children.
		/// This method is used to create new nodes from the existing immutable one.
		/// </summary>
		/// <param name="children">The children that should be contained in the node.</param>
		/// <returns></returns>
		protected abstract InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children);

		/// <summary>
		/// Creates a new immutable syntax node using the given parent and tree.
		/// </summary>
		/// <param name="parent">The function that produces the parent of the new node.</param>
		/// <param name="tree">The function that produces the tree of the new node.</param>
		/// <returns></returns>
		public abstract SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree);

		public SyntaxNode CreateSyntaxNode(SyntaxNode parent, SyntaxTree tree)
		{
			return CreateSyntaxNode(n => parent, t => tree);
		}

		/// <summary>
		/// Replaces the given node with the given new node in this node's children.
		/// </summary>
		/// <param name="oldNode">The old node that should be replaced.</param>
		/// <param name="newNode">The new node that should be inserted.</param>
		/// <returns>Returns the new <see cref="InternalSyntaxNode"/> that represents the new syntax node with the changes.</returns>
		public virtual InternalSyntaxNode ReplaceNode(InternalSyntaxNode oldNode, InternalSyntaxNode newNode)
		{
			if (ReferenceEquals(oldNode, newNode))
			{
				return this;
			}
			else
			{
				return CreateNewNode(Children.Replace(oldNode, newNode));
			}
		}
	}
}