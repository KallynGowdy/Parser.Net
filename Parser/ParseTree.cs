using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Parser.Grammar;

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
            private List<ParseTreebranch> children;

            /// <summary>
            /// Gets the list of children of this branch.
            /// </summary>
            public ReadOnlyCollection<ParseTreebranch> Children
            {
                get
                {
                    return new ReadOnlyCollection<ParseTreebranch>(children);
                }
            }

            public List<ParseTreebranch> GetChildren()
            {
                return children;
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
                private set;
            }

            /// <summary>
            /// Gets or sets the value contained by this branch.
            /// </summary>
            public GrammarElement<T> Value
            {
                get;
                set;
            }

            /// <summary>
            /// Adds the given branch as a child of this branch.
            /// </summary>
            /// <param name="branch"></param>
            public void AddChild(ParseTreebranch branch)
            {
                branch.Parent = this;
                this.children.Add(branch);
            }

            /// <summary>
            /// Adds the given branches as children of this branch.
            /// </summary>
            /// <param name="branches"></param>
            public void AddChildren(IEnumerable<ParseTreebranch> branches)
            {
                foreach (ParseTreebranch b in branches)
                {
                    b.Parent = this;
                }
                this.children.AddRange(branches);
            }

            private ParseTreebranch(GrammarElement<T> value, ParseTree<T> tree)
            {
                this.Value = value;
                this.ParentTree = tree;
                this.children = new List<ParseTreebranch>();
            }

            public ParseTreebranch(GrammarElement<T> value, ParseTreebranch parent)
            {
                this.Value = value;
                this.Parent = parent;
                this.ParentTree = parent.ParentTree;
                this.children = new List<ParseTreebranch>();
            }

            public ParseTreebranch(GrammarElement<T> value)
            {
                this.Value = value;
                this.children = new List<ParseTreebranch>();
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

        public ParseTree()
        {

        }
    }
}
