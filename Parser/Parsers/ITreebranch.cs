using System.Collections.Generic;

namespace KallynGowdy.ParserGenerator.Parsers
{

    public interface ITreebranch<T>
    {
        /// <summary>
        /// Gets the parent of this tree branch.
        /// </summary>
        ITreebranch<T> Parent
        {
            get;
        }

        /// <summary>
        /// Gets whether this tree branch is the root.
        /// </summary>
        ITreebranch<T> IsRoot
        {
            get;
        }

        /// <summary>
        /// Gets the children of this branch.
        /// </summary>
        IEnumerable<ITreebranch<T>> Children
        {
            get;
        }

        /// <summary>
        /// Adds the given tree branch as a child of this branch.
        /// </summary>
        /// <param name="child"></param>
        void AddChild(ITreebranch<T> child);

        /// <summary>
        /// Adds the given branches as children of this branch.
        /// </summary>
        /// <param name="children"></param>
        void AddChildren(IEnumerable<ITreebranch<T>> children); 
    }

}
