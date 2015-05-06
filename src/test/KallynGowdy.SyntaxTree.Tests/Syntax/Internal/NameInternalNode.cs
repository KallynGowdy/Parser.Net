using System;
using System.Collections.Immutable;
using KallynGowdy.SyntaxTree.Internal;

namespace KallynGowdy.SyntaxTree.Tests.Syntax.Internal
{
	public class NameInternalNode : InternalSyntaxNode, IEquatable<NameInternalNode>
	{
		public string Name { get; }

		public NameInternalNode(string name) : base(ImmutableArray.Create<InternalSyntaxNode>())
		{
			if (name == null) throw new ArgumentNullException("name");
			Name = name;
		}

		protected override InternalSyntaxNode CreateNewNode(IImmutableList<InternalSyntaxNode> children)
		{
			return new NameInternalNode(Name);
		}

		public override SyntaxNode CreateSyntaxNode(Func<SyntaxNode, SyntaxNode> parent, Func<SyntaxNode, SyntaxTree> tree)
		{
			return new NameNode(this, parent, tree);
		}

		public override int Length => Name.Length;

		public override string ToString()
		{
			return Name;
		}

		public virtual bool Equals(NameInternalNode other)
		{
			return (object)other != null &&
					(((object)this == (object)other) ||
					// ReSharper disable once PossibleNullReferenceException
					Name.Equals(other.Name));
		}

		public override bool Equals(InternalSyntaxNode other)
		{
			return (object)other != null &&
				   ((object)this == (object)other ||
					(base.Equals(other) &&
					Equals(other as NameInternalNode)));
		}

		public override bool Equals(object obj)
		{
			return obj != null &&
				   ((object)this == (object)obj ||
					Equals(obj as InternalSyntaxNode));
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