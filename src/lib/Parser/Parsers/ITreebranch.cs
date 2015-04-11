using System.Collections.Generic;

namespace KallynGowdy.ParserGenerator.Parsers
{
	public interface ITreeBranch<T>
	{
		/// <summary>
		///     Gets the children of this branch.
		/// </summary>
		IEnumerable<ITreeBranch<T>> Children { get; }

		/// <summary>
		///     Gets whether this tree branch is the root.
		/// </summary>
		ITreeBranch<T> IsRoot { get; }

		/// <summary>
		///     Gets the parent of this tree branch.
		/// </summary>
		ITreeBranch<T> Parent { get; }

		/// <summary>
		///     Adds the given tree branch as a child of this branch.
		/// </summary>
		/// <param name="child"></param>
		void AddChild(ITreeBranch<T> child);

		/// <summary>
		///     Adds the given branches as children of this branch.
		/// </summary>
		/// <param name="children"></param>
		void AddChildren(IEnumerable<ITreeBranch<T>> children);
	}
}