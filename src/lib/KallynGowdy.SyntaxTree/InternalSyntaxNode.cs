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
	/// <remarks>
	/// Because both <see cref="SyntaxNode"/> and <see cref="InternalSyntaxNode"/> objects are immutable,
	/// new nodes are created whenever an operation occurs (Adding, Replacing, Removing nodes/values). Some <see cref="InternalSyntaxNode"/> objects are able to be reused because
	/// they don't contain parent links, but <see cref="SyntaxNode"/> objects cannot be reused, so they're rebuilt on every change.
	/// </remarks>
	public abstract class InternalSyntaxNode : IEquatable<InternalSyntaxNode>
	{
		protected InternalSyntaxNode(IImmutableList<InternalSyntaxNode> children)
		{
			if (children == null) throw new ArgumentNullException("children");
			Children = children;
		}

		/// <summary>
		/// Gets the length of this node.
		/// </summary>
		public virtual long Length => Children.Where(c => c != null).Sum(c => c.Length);

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

		/// <summary>
		/// Inserts the given node at the given index. 
		/// If the child node at the given index is null, it is filled with the given node.
		/// If the given index is equal to Children.Count, then the node is inserted at the end.
		/// </summary>
		/// <param name="index">The index that the node should be inserted at.</param>
		/// <param name="newNode">The node that should be inserted at the given location.</param>
		/// <returns>Returns a new <see cref="SyntaxNode"/> that represents this internal node after the manipulation.</returns>
		/// <exception cref="ArgumentOutOfRangeException">index is less than 0 or greater than Children.Count</exception>
		/// <exception cref="ArgumentNullException">The value of 'newNode' cannot be null. </exception>
		public virtual InternalSyntaxNode InsertNode(int index, InternalSyntaxNode newNode)
		{
			if (index < 0 || index > Children.Count) throw new ArgumentOutOfRangeException("index", "Must be greater than or equal to 0 and less than Children.Count");
			if (newNode == null) throw new ArgumentNullException("newNode");

			if(index == Children.Count)
			{
				return CreateNewNode(Children.Add(newNode));
			}
			else if (Children[index] == null)
			{
				return CreateNewNode(Children.SetItem(index, newNode));
			}
			else
			{
				return CreateNewNode(Children.Insert(index, newNode));
			}
		}

		/// <summary>
		/// Removes the given node from this node's children and returns a new instance of this.
		/// </summary>
		/// <param name="internalNode">The node that should be removed.</param>
		/// <returns></returns>
		public InternalSyntaxNode RemoveNode(InternalSyntaxNode internalNode)
		{
			return CreateNewNode(Children.Remove(internalNode));
		}

		/// <summary>
		/// Removes the node at the given index and returns a new <see cref="SyntaxNode"/> that represents the changes.
		/// </summary>
		/// <param name="index">The index of the node that should be removed.</param>
		/// <returns>Returns a new <see cref="SyntaxNode"/> that contains the changes.</returns>
		/// <exception cref="ArgumentOutOfRangeException">'index' must be between 0 and Children.Count.</exception>
		public InternalSyntaxNode RemoveNodeAt(int index)
		{
			if (index < 0 || index >= Children.Count) throw new ArgumentOutOfRangeException("index", "Must be between 0 and Children.Count");
			return CreateNewNode(Children.RemoveAt(index));
		}

		public virtual bool Equals(InternalSyntaxNode other)
		{
			return other != null &&
				Children.SequenceEqual(other.Children);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InternalSyntaxNode) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 1209233;
				for (int i = 0; i < Children.Count; i++)
				{
					hashCode = (hashCode * 575557) ^ i.GetHashCode();
					hashCode = (hashCode * 575557) ^ (Children[i]?.GetHashCode() ?? 3);
				}
				return hashCode;
            }
		}

		public static bool operator ==(InternalSyntaxNode left, InternalSyntaxNode right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(InternalSyntaxNode left, InternalSyntaxNode right)
		{
			return !Equals(left, right);
		}
	}
}