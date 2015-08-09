using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KallynGowdy.SyntaxTree
{
    /// <summary>
    /// Defines a class that represents a set of differences/changes between an original <see cref="SyntaxTree"/> and some new string content.
    /// </summary>
    public class SyntaxTreeDifferences
    {
        /// <summary>
        /// Gets the original syntax tree that was compared for changes.
        /// </summary>
        public SyntaxTree OriginalTree { get; }

        /// <summary>
        /// Gets the new syntax tree that contains the changes in the content.
        /// </summary>
        public SyntaxTree NewTree { get; }
        
        /// <summary>
        /// Gets whether the new syntax tree contains any differences from the original tree.
        /// </summary>
        public bool HasChanges { get; }

        /// <summary>
        /// Creates a new <see cref="SyntaxTreeDifferences"/> object.
        /// </summary>
        /// <param name="originalTree"></param>
        /// <param name="newTree"></param>
        /// <exception cref="ArgumentNullException"><paramref name="originalTree"/> is <see langword="null" /> or <paramref name="newTree"/> is <see langword="null"/>.</exception>
        public SyntaxTreeDifferences(SyntaxTree originalTree, SyntaxTree newTree)
        {
            if (originalTree == null) throw new ArgumentNullException("originalTree");
            if (newTree == null) throw new ArgumentNullException("newTree");
            OriginalTree = originalTree;
            NewTree = newTree;
            HasChanges = !OriginalTree.Root.StrictEquals(NewTree.Root);
        }
    }
}
