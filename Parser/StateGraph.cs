using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.StateMachine;

namespace Parser
{
    /// <summary>
    /// Provides a graph that relates states to other states based on transitions.
    /// </summary>
    public class StateGraph<TKey, T>
    {
        /// <summary>
        /// Gets or sets the root node of the graph.
        /// </summary>
        public StateNode<TKey, T> Root
        {
            get;
            set;
        }

        public StateGraph()
        {
            this.Root = null;
        }

        public StateGraph(StateNode<TKey, T> rootNode)
        {
            this.Root = rootNode;
        }

        public bool Contains(StateNode<TKey, T> node)
        {
            return Root.ContainsFromTransition(node);
        }

        public bool Contains(Predicate<StateNode<TKey, T>> condition, int maxLoop = 1000)
        {
            return Root.ContainsFromTransition(condition, maxLoop);
        }

        public StateNode<TKey, T> GetExistingNode(Predicate<StateNode<TKey, T>> condition, int maxLoop = 1000)
        {
            return Root.GetFromTransition(condition, maxLoop);
        }
    }
}
