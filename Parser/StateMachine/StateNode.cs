using System;
using System.Collections.Generic;
using System.Linq;

namespace KallynGowdy.ParserGenerator.StateMachine
{
	/// <summary>
	///     Defines a state that leads to other states based on "transitions" determined by "Keys".
	/// </summary>
	/// <typeparam name="T">The value that the Node should store.</typeparam>
	/// <typeparam name="TKey">Defines the type of the "Keys" which define the transitions.</typeparam>
	public class StateNode<TKey, T>
	{
		/// <summary>
		///     The transitions that lead from this node.
		/// </summary>
		private readonly Dictionary<TKey, StateNode<TKey, T>> fromTransitions;

		/// <summary>
		///     A list that contains the nodes that travel to this node.
		/// </summary>
		private readonly List<StateNode<TKey, T>> toTransitions;

		/// <summary>
		///     Creates a new empty state.
		/// </summary>
		public StateNode()
		{
			fromTransitions = new Dictionary<TKey, StateNode<TKey, T>>();
			toTransitions = new List<StateNode<TKey, T>>();
			Value = default(T);
		}

		/// <summary>
		///     Creates a new state containing the given value.
		/// </summary>
		/// <param name="value"></param>
		public StateNode(T value)
		{
			fromTransitions = new Dictionary<TKey, StateNode<TKey, T>>();
			toTransitions = new List<StateNode<TKey, T>>();
			Value = value;
		}

		/// <summary>
		///     Creates a new state contianing the given value with a reference to the given graph.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="graph"></param>
		public StateNode(T value, StateGraph<TKey, T> graph)
		{
			fromTransitions = new Dictionary<TKey, StateNode<TKey, T>>();
			toTransitions = new List<StateNode<TKey, T>>();
			Value = value;
			Graph = graph;
		}

		/// <summary>
		///     Gets the transitions that defines which node to move to when a certian input(TKey) is recieved.
		///     The transitions that lead from this node.
		/// </summary>
		public KeyValuePair<TKey, StateNode<TKey, T>>[] FromTransitions
		{
			get { return fromTransitions.ToArray(); }
		}

		/// <summary>
		///     Gets the graph that this node belongs to.
		/// </summary>
		public StateGraph<TKey, T> Graph { get; private set; }

		/// <summary>
		///     Gets an array of nodes that travel to this node.
		/// </summary>
		public StateNode<TKey, T>[] ToTransitions
		{
			get { return toTransitions.ToArray(); }
		}

		/// <summary>
		///     Gets or sets the value contianed by this node.
		/// </summary>
		public T Value { get; set; }

		/// <summary>
		///     Adds the given key and value as a transition from this node to the given node.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddTransition(TKey key, StateNode<TKey, T> value)
		{
			//add a "from" transition so that the value knows
			//that we lead to it.

			if (!value.toTransitions.Contains(this))
				value.toTransitions.Add(this);

			fromTransitions.Add(key, value);
		}

		/// <summary>
		///     Adds the given KeyValuePair as a transition.
		/// </summary>
		/// <param name="transition"></param>
		public void AddTransition(KeyValuePair<TKey, StateNode<TKey, T>> transition)
		{
			//add a "from" transition so that the value knows
			//that we lead to it.
			if (!transition.Value.toTransitions.Contains(this))
				transition.Value.toTransitions.Add(this);
			fromTransitions.Add(transition.Key, transition.Value);
		}

		/// <summary>
		///     Removes a transition from Transitions based on the given key.
		///     This will remove all of the
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
		///     Determines if the given node is contained by this node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		/// <summary>
		///     Returns whether a node is contained by this node(and linked "from" nodes).
		/// </summary>
		/// <param name="comparer">The predicate used to determine if a node is contained.</param>
		/// <returns></returns>
		/// <summary>
		///     Gets the depth first traversal of the children of this node.
		/// </summary>
		/// <exception cref="System.StackOverflowException" />
		/// <returns></returns>
		public IEnumerable<StateNode<TKey, T>> GetDepthFirstTraversal()
		{
			var stack = new Stack<StateNode<TKey, T>>();
			var traversal = new Stack<StateNode<TKey, T>>();
			traversal.Push(this);
			stack.Push(this);
			while (stack.Count != 0)
			{
				StateNode<TKey, T> node = stack.Pop();
				foreach (KeyValuePair<TKey, StateNode<TKey, T>> transition in node.FromTransitions)
				{
					stack.Push(transition.Value);
					traversal.Push(transition.Value);
				}
			}
			return traversal;
		}

		/// <summary>
		///     Gets the breadth first traversal of the graph.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<StateNode<TKey, T>> GetBreadthFirstTraversal()
		{
			var queue = new MarkerQueue<StateNode<TKey, T>>();
			var traversal = new Queue<StateNode<TKey, T>>();
			traversal.Enqueue(this);
			queue.Queue.Enqueue(new Marker<StateNode<TKey, T>>(this));
			while (queue.Queue.Count != 0)
			{
				Marker<StateNode<TKey, T>> node = queue.Queue.Dequeue();
				if (!node.Marked)
				{
					foreach (KeyValuePair<TKey, StateNode<TKey, T>> transition in node.Value.FromTransitions)
					{
						if (!traversal.Contains(transition.Value))
						{
							queue.Queue.Enqueue(new Marker<StateNode<TKey, T>>(transition.Value));
							traversal.Enqueue(transition.Value);
						}
					}
					node.Marked = true;
				}
			}
			return traversal;
		}

		/// <summary>
		///     Returns whether a node is contained by this node(and linked "from" nodes).
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="currentLoop"></param>
		/// <param name="maxLoop"></param>
		/// <returns></returns>
		/// <summary>
		///     Returns whether the given node is contained in a "from" transition.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public bool ContainsFromTransition(StateNode<TKey, T> node)
		{
			return GetBreadthFirstTraversal().Any(a => a == node);
		}

		/// <summary>
		///     Returns whether a transition is contained as a child of this node based on the given comparer.
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public bool ContainsFromTransition(Predicate<StateNode<TKey, T>> comparer)
		{
			return GetBreadthFirstTraversal().Any(a => comparer(a));
		}

		/// <summary>
		///     Gets a node from the "from" transitions that matches comparer.
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="maxLoop"></param>
		/// <returns></returns>
		/// <summary>
		///     Gets a node from the "from" transitions that matches comparer. Returns null if the transition cannot be found.
		/// </summary>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public StateNode<TKey, T> GetFromTransition(Predicate<StateNode<TKey, T>> comparer)
		{
			return GetBreadthFirstTraversal().FirstOrDefault(a => comparer(a));
		}

		/// <summary>
		///     Removes the Node at index from Transitions and returns the removed node with the stored key.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentOutOfRangeException" />
		public KeyValuePair<TKey, StateNode<TKey, T>> RemoveTransitionAt(int index)
		{
			if (index < fromTransitions.Count &&
			    index >= 0)
			{
				KeyValuePair<TKey, StateNode<TKey, T>> pair = fromTransitions.ElementAt(index);
				//remove the link
				pair.Value.toTransitions.Remove(this);
				fromTransitions.Remove(pair.Key);
				return pair;
			}
			throw new ArgumentOutOfRangeException("index", "Must be greater than or equal to 0 and less than Transitions.Count");
		}

		private class MarkerQueue<Type>
		{
			public MarkerQueue()
			{
				Queue = new Queue<Marker<Type>>();
			}

			public Queue<Marker<Type>> Queue { get; }
		}

		private struct Marker<Type>
		{
			public Marker(Type value, bool marked = false)
				: this()
			{
				Value = value;
				Marked = marked;
			}

			/// <summary>
			///     Gets or sets if this value is already marked.
			/// </summary>
			public bool Marked { get; set; }

			/// <summary>
			///     Gets or sets the value contained by the marker.
			/// </summary>
			public Type Value { get; }
		}
	}
}