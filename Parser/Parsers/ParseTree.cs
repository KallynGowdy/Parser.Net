using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using KallynGowdy.ParserGenerator.Grammar;

namespace KallynGowdy.ParserGenerator.Parsers
{
	/// <summary>
	///     Represents a parse tree.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ParseTree<T>
	{
		public ParseTree(ParseTreebranch rootBranch)
		{
			Root = rootBranch;
			Root.ParentTree = this;
		}

		/// <summary>
		///     Creates a new parse tree from the given other tree.
		/// </summary>
		/// <param name="otherTree"></param>
		public ParseTree(ParseTree<T> otherTree)
		{
			Root = new ParseTreebranch(otherTree.Root);
		}

		/// <summary>
		///     Creates a new empty parse tree.
		/// </summary>
		public ParseTree()
		{
		}

		/// <summary>
		///     Gets the root element of this tree.
		/// </summary>
		public ParseTreebranch Root { get; }

		/// <summary>
		///     Represents a Branch in a parse tree.
		/// </summary>
		public class ParseTreebranch
		{
			private readonly List<ParseTreebranch> children;

			/// <summary>
			///     Creates an new parse tree branch copied from the original.
			/// </summary>
			/// <param name="original"></param>
			public ParseTreebranch(ParseTreebranch original)
			{
				if (original.Value != null)
					Value = original.Value.DeepCopy();
				else
					Value = null;
				if (original.children != null)
					children = original.children.Select(a => new ParseTreebranch(a)).ToList();
				else
					children = new List<ParseTreebranch>();
			}

			public ParseTreebranch(GrammarElement<T> value, ParseTreebranch parent)
			{
				Value = value;
				Parent = parent;
				ParentTree = parent.ParentTree;
				children = new List<ParseTreebranch>();
			}

			public ParseTreebranch(GrammarElement<T> value)
			{
				Value = value;
				children = new List<ParseTreebranch>();
			}

			public ParseTreebranch(List<ParseTreebranch> currentBranches)
			{
				if (currentBranches != null)
					children = currentBranches;
				else
					children = new List<ParseTreebranch>();
			}

			private ParseTreebranch(GrammarElement<T> value, ParseTree<T> tree)
			{
				Value = value;
				ParentTree = tree;
				children = new List<ParseTreebranch>();
			}

			/// <summary>
			///     Gets the list of children of this branch.
			/// </summary>
			public ReadOnlyCollection<ParseTreebranch> Children
			{
				get { return new ReadOnlyCollection<ParseTreebranch>(children); }
			}

			/// <summary>
			///     Gets the parent branch of this branch.
			/// </summary>
			public ParseTreebranch Parent { get; private set; }

			/// <summary>
			///     Gets the parent tree that contains this branch.
			/// </summary>
			public ParseTree<T> ParentTree { get; internal set; }

			/// <summary>
			///     Gets or sets the value contained by this branch.
			/// </summary>
			public GrammarElement<T> Value { get; set; }

			/// <summary>
			///     Gets the children of the branch.
			/// </summary>
			/// <returns></returns>
			public List<ParseTreebranch> GetChildren()
			{
				return children;
			}

			/// <summary>
			///     Adds the given branch as a child of this branch.
			/// </summary>
			/// <param name="branch"></param>
			public void AddChild(ParseTreebranch branch)
			{
				branch.Parent = this;
				children.Add(branch);
			}

			/// <summary>
			///     Adds the given branches as children of this branch.
			/// </summary>
			/// <param name="branches"></param>
			public void AddChildren(IEnumerable<ParseTreebranch> branches)
			{
				foreach (ParseTreebranch b in branches)
					b.Parent = this;
				children.AddRange(branches);
			}
		}
	}
}