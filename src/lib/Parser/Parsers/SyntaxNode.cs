using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Represents a Branch in a parse tree.
	/// </summary>
	public class SyntaxNode<T>
	{
		private readonly List<SyntaxNode<T>> children;
		
		public SyntaxNode(GrammarElement<T> value, SyntaxNode<T> parent)
		{
			Value = value;
			Parent = parent;
			ParentTree = parent.ParentTree;
			children = new List<SyntaxNode<T>>();
		}

		/// <summary>
		///     Gets the list of children of this branch.
		/// </summary>
		public ReadOnlyCollection<SyntaxNode<T>> Children
		{
			get { return new ReadOnlyCollection<SyntaxNode<T>>(children); }
		}

		/// <summary>
		///     Gets the parent branch of this branch.
		/// </summary>
		public SyntaxNode<T> Parent { get; private set; }

		/// <summary>
		///     Gets the parent tree that contains this branch.
		/// </summary>
		public SyntaxTree<T> ParentTree { get; internal set; }

		/// <summary>
		///     Gets or sets the value contained by this branch.
		/// </summary>
		public GrammarElement<T> Value { get; private set; }

		/// <summary>
		///     Gets the children of the branch.
		/// </summary>
		/// <returns></returns>
		public List<SyntaxNode<T>> GetChildren()
		{
			return children;
		}

		/// <summary>
		///     Adds the given branch as a child of this branch.
		/// </summary>
		/// <param name="branch"></param>
		public void AddChild(SyntaxNode<T> branch)
		{
			branch.Parent = this;
			children.Add(branch);
		}

		/// <summary>
		///     Adds the given branches as children of this branch.
		/// </summary>
		/// <param name="branches"></param>
		public void AddChildren(IEnumerable<SyntaxNode<T>> branches)
		{
			foreach (SyntaxNode<T> b in branches)
				b.Parent = this;
			children.AddRange(branches);
		}
	}

}