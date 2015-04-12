using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallynGowdy.SyntaxTree
{
	/// <summary>
	/// Defines a class that represents a syntax tree.
	/// If you are defining your own syntax trees, you should use <see cref="SyntaxTree{TMutable}"/>.
	/// </summary>
	public abstract class SyntaxTree : IEquatable<SyntaxTree>
	{
		private SyntaxNode root;

		/// <summary>
		/// Gets the mutable version of this tree.
		/// </summary>
		protected InternalSyntaxTree InternalTree { get; }

		/// <summary>
		/// Creates a new syntax tree with the given root.
		/// </summary>
		/// <param name="internalTree">The root that is contained in the tree.</param>
		/// <exception cref="ArgumentNullException">The value of 'InternalTree' cannot be null. </exception>
		protected SyntaxTree(InternalSyntaxTree internalTree)
		{
			if (internalTree == null) throw new ArgumentNullException("internalTree");
			this.InternalTree = internalTree;
		}

		/// <summary>
		/// Gets the root of the syntax tree.
		/// </summary>
		public SyntaxNode Root => root ?? (root = InternalTree.Root.CreateSyntaxNode(null, this));

		/// <summary>
		/// Creates a new <see cref="SyntaxTree"/> that possesses the given root.
		/// </summary>
		/// <param name="newRoot">The root node.</param>
		/// <returns></returns>
		public SyntaxTree SetRoot(SyntaxNode newRoot) => !ReferenceEquals(newRoot, Root) ? CreateNewTree(newRoot) : this;

		protected abstract SyntaxTree CreateNewTree(SyntaxNode root);

		public override string ToString()
		{
			return Root.ToString();
		}

		public bool Equals(SyntaxTree other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return other.Root.Equals(this.Root);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SyntaxTree) obj);
		}

		public override int GetHashCode()
		{
			return this.Root.GetHashCode();
		}

		public static bool operator ==(SyntaxTree left, SyntaxTree right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(SyntaxTree left, SyntaxTree right)
		{
			return !Equals(left, right);
		}
    }

	/// <summary>
	/// Defines an abstract class that 
	/// </summary>
	/// <typeparam name="TMutable"></typeparam>
	/// <typeparam name="TRoot"></typeparam>
	public abstract class SyntaxTree<TMutable, TRoot> : SyntaxTree
		where TMutable : InternalSyntaxTree
		where TRoot : SyntaxNode
	{
		protected SyntaxTree(TMutable mutable) : base(mutable)
		{
		}

		/// <summary>
		/// Gets the mutable version of this tree.
		/// </summary>
		protected new TMutable InternalTree => (TMutable)base.InternalTree;

		/// <summary>
		/// Gets the root node of the tree.
		/// </summary>
		public new TRoot Root => (TRoot)base.Root;
	}
}
