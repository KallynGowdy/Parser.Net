using System;
using System.Collections.Immutable;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class NameInternalNode : InternalSyntaxNode, IEquatable<NameInternalNode>
	{
		public string Name { get; }

		public NameInternalNode(string firstName) : base(ImmutableArray.Create<InternalSyntaxNode>())
		{
			if (firstName == null) throw new ArgumentNullException("firstName");
			Name = firstName;
		}

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			return new NameInternalNode(Name);
		}

		public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
		{
			return new NameNode(this, parent, tree);
		}

		public override long Length => Name.Length;

		public override string ToString()
		{
			return Name;
		}

		public virtual bool Equals(NameInternalNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return this.Name.Equals(other.Name);
		}

		public override bool Equals(InternalSyntaxNode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			if (other.GetType() != this.GetType()) return false;
			return base.Equals(other) &&
				Equals((NameInternalNode)other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((NameInternalNode)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ Name.GetHashCode();
			}
		}

		public static bool operator ==(NameInternalNode left, NameInternalNode right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(NameInternalNode left, NameInternalNode right)
		{
			return !Equals(left, right);
		}
	}
}