using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Represents a parse tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParseTree<T>
    {
        /// <summary>
        /// Represents a Branch in a parse tree.
        /// </summary>
        public class ParseTreebranch
        {
            /// <summary>
            /// Gets the list of children of this branch.
            /// </summary>
            public List<ParseTreebranch> Children
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the parent tree that contains this branch.
            /// </summary>
            public ParseTree<T> ParentTree
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets the parent branch of this branch.
            /// </summary>
            public ParseTreebranch Parent
            {
                get;
                internal set;
            }

            /// <summary>
            /// Gets or sets the value contained by this branch.
            /// </summary>
            public T Value
            {
                get;
                set;
            }

            private ParseTreebranch(T value, ParseTree<T> tree)
            {
                this.Value = value;
                this.ParentTree = tree;
            }

            public ParseTreebranch(T value, ParseTreebranch parent)
            {
                this.Value = value;
                this.Parent = parent;
                this.ParentTree = parent.ParentTree;
            }

            public ParseTreebranch(T value)
            {
                this.Value = value;
            }
        }

        /// <summary>
        /// Gets the root element of this tree.
        /// </summary>
        public ParseTreebranch Root
        {
            get;
            private set;
        }   

        public ParseTree(ParseTreebranch rootBranch)
        {
            this.Root = rootBranch;
            this.Root.ParentTree = this;
        }
    }
}
