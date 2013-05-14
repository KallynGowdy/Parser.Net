using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser
{
    /// <summary>
    /// Defines a state that leads to other states based on "transitions" determined by "Keys".
    /// </summary>
    /// <typeparam name="T">The value that the Node should store.</typeparam>
    /// <typeparam name="TKey">Defines the type of the "Keys" which define the transitions.</typeparam>
    public class StateNode<TKey, T>
    {
        /// <summary>
        /// Gets or sets the value contianed by this node.
        /// </summary>
        public T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the graph that this node belongs to.
        /// </summary>
        public StateGraph<TKey, T> Graph
        {
            get;
            private set;
        }

        /// <summary>
        /// A list that contains the nodes that travel to this node.
        /// </summary>
        private List<StateNode<TKey, T>> toTransitions;

        /// <summary>
        /// Gets an array of nodes that travel to this node.
        /// </summary>
        public StateNode<TKey, T>[] ToTransitions
        {
            get
            {
                return toTransitions.ToArray();
            }
        }

        /// <summary>
        /// Gets the transitions that defines which node to move to when a certian input(TKey) is recieved.
        /// The transitions that lead from this node.
        /// </summary>
        public KeyValuePair<TKey, StateNode<TKey, T>>[] FromTransitions
        {
            get
            {
                return fromTransitions.ToArray();
            }
        }

        /// <summary>
        /// The transitions that lead from this node.
        /// </summary>
        private Dictionary<TKey, StateNode<TKey, T>> fromTransitions;

        /// <summary>
        /// Adds the given key and value as a transition from this node to the given node.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddTransition(TKey key, StateNode<TKey, T> value)
        {
            //add a "from" transition so that the value knows
            //that we lead to it.
            
            
            if(!value.toTransitions.Contains(this))
            {
                value.toTransitions.Add(this);
            }
            
            fromTransitions.Add(key, value);
        }

        /// <summary>
        /// Adds the given KeyValuePair as a transition.
        /// </summary>
        /// <param name="transition"></param>
        public void AddTransition(KeyValuePair<TKey, StateNode<TKey, T>> transition)
        {
            //add a "from" transition so that the value knows
            //that we lead to it.
            if(!transition.Value.toTransitions.Contains(this))
            {
                transition.Value.toTransitions.Add(this);
            }
            fromTransitions.Add(transition.Key, transition.Value);
        }

        /// <summary>
        /// Removes a transition from Transitions based on the given key.
        /// This will remove all of the 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveTransition(TKey key)
        {
            //remove the link
            fromTransitions[key].toTransitions.Remove(this);
            return fromTransitions.Remove(key);
        }

        /// <summary>
        /// Determines if the given node is contained by this node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool ContainsFromTransition(StateNode<TKey, T> node)
        {
            if (!fromTransitions.Values.Contains(node))
            {
                foreach (var transition in fromTransitions)
                {
                    if (transition.Value.ContainsFromTransition(node))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns whether a node is contained by this node(and linked "from" nodes).
        /// </summary>
        /// <param name="comparer">The predicate used to determine if a node is contained.</param>
        /// <param name="maxLoop">The maximum distance to travel through the graph, this stops infinate loops.</param>
        /// <returns></returns>
        public bool ContainsFromTransition(Predicate<StateNode<TKey, T>> comparer, int maxLoop = 1000)
        {
            //if it is not contained
            if (fromTransitions.All(a => !comparer(a.Value)))
            {
                int looper = 0;
                //loop through transitions and find it
                foreach (var transition in fromTransitions)
                {
                    if (transition.Value.containsFromTransition(comparer, ref looper, maxLoop))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        private bool containsFromTransition(Predicate<StateNode<TKey, T>> comparer, ref int currentLoop, int maxLoop)
        {
            //if it is not contained
            if (fromTransitions.All(a => !comparer(a.Value)))
            {

                //loop through transitions and find it
                foreach (var transition in fromTransitions)
                {

                    currentLoop++;
                    if (currentLoop >= maxLoop)
                    {
                        return false;
                    }
                    if (transition.Value.containsFromTransition(comparer, ref currentLoop, maxLoop))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a node from the "from" transitions that matches comparer.
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="maxLoop"></param>
        /// <returns></returns>
        public StateNode<TKey, T> GetFromTransition(Predicate<StateNode<TKey, T>> comparer, int maxLoop = 1000)
        {
            StateNode<TKey, T> node = null;
            ContainsFromTransition(a =>
            {
                if (comparer(a))
                {
                    node = a;
                    return true;
                }
                return false;
            }, maxLoop);
            return node;
        }

        /// <summary>
        /// Removes the Node at index from Transitions and returns the removed node with the stored key.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"/>
        public KeyValuePair<TKey, StateNode<TKey, T>> RemoveTransitionAt(int index)
        {
            if (index < fromTransitions.Count && index >= 0)
            {
                KeyValuePair<TKey, StateNode<TKey, T>> pair = fromTransitions.ElementAt(index);
                //remove the link
                pair.Value.toTransitions.Remove(this);
                fromTransitions.Remove(pair.Key);
                return pair;
            }
            else
            {
                throw new ArgumentOutOfRangeException("index", "Must be greater than or equal to 0 and less than Transitions.Count");
            }
        }

        public StateNode()
        {
            fromTransitions = new Dictionary<TKey, StateNode<TKey, T>>();
            toTransitions = new List<StateNode<TKey,T>>();
            Value = default(T);
        }

        public StateNode(T value)
        {
            fromTransitions = new Dictionary<TKey, StateNode<TKey, T>>();
            toTransitions = new List<StateNode<TKey,T>>();
            this.Value = value;
        }

        public StateNode(T value, StateGraph<TKey, T> graph)
        {
            fromTransitions = new Dictionary<TKey, StateNode<TKey, T>>();
            toTransitions = new List<StateNode<TKey,T>>();
            this.Value = value;
            this.Graph = graph;
        }
    }
}
