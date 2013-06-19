using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Parser.StateMachine;

namespace Parser.Grammar
{
    /// <summary>
    /// Defines a Context Free Grammar(CFG) such that given a string of terminal elements, they will be
    /// reduced to the starting element by the rules defined in the productions.
    /// </summary>
    [Serializable]
    public class ContextFreeGrammar<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Writes this context free grammar to the given stream
        /// </summary>
        /// <exception cref="System.Runtime.Serialization.SerializationException"/>
        /// <exception cref="System.ArgumentNullException"/>
        public void WriteToStream(Stream stream)
        {

            DataContractSerializer ser = new DataContractSerializer(typeof(ContextFreeGrammar<T>));
            XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true
            });
            ser.WriteObject(writer, this);
        }

        /// <summary>
        /// Reads a context free grammar from the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ContextFreeGrammar<T> ReadFromStream(Stream stream)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof(ContextFreeGrammar<T>));
            return (ContextFreeGrammar<T>)ser.ReadObject(stream);
        }



        /// <summary>
        /// Gets or sets the list of productions used in this context free grammar.
        /// </summary>
        public List<Production<T>> Productions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the list of Non-Terminal elements used in this context free grammar.
        /// </summary>
        public IEnumerable<NonTerminal<T>> NonTerminals
        {
            get
            {
                return Productions.Select(p => p.NonTerminal).Distinct();
            }
        }

        /// <summary>
        /// Gets or sets the list of Terminal elements used in this context free grammar.
        /// </summary>
        public IEnumerable<Terminal<T>> Terminals
        {
            get
            {
                return Productions.Select(p => p.DerivedElements.Where(e => e is Terminal<T>).Cast<Terminal<T>>()).SelectMany(a => a).Distinct();
            }
        }

        private NonTerminal<T> startElement;

        /// <summary>
        /// Gets the starting element of this context free grammar. 
        /// </summary>
        public NonTerminal<T> StartElement
        {
            get
            {
                return startElement;
            }
        }

        /// <summary>
        /// Gets the End of Input element
        /// used to denote the end of the input.
        /// </summary>
        public Terminal<T> EndOfInputElement
        {
            get;
            set;
        }

        Dictionary<LRItem<T>, IEnumerable<LRItem<T>>> closures = new Dictionary<LRItem<T>, IEnumerable<LRItem<T>>>();

        /// <summary>
        /// Gets the LR(1) closure of the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<LRItem<T>> LR1Closure(LRItem<T> item)
        {
            if (closures.ContainsKey(item))
            {
                return closures[item];
            }
            else
            {
                IEnumerable<LRItem<T>> closure = lr1Closure(item, new HashSet<LRItem<T>>());
                closures.Add(item, closure);
                return closure;
            }
        }

        /// <summary>
        /// Gets the LR(1) closure of the given item, using currentItems to filter out duplicates.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="currentItems"></param>
        /// <returns></returns>
        private IEnumerable<LRItem<T>> lr1Closure(LRItem<T> item, HashSet<LRItem<T>> currentItems)
        {
            //<LRItem<T>> items;

            //items = new List<LRItem<T>>();

            //add the current items
            //if (currentItems != null)
            //{
            //    items.AddRange(currentItems);
            //}

            GrammarElement<T> nextElement;

            //if the next element is a non terminal
            if ((nextElement = item.GetNextElement()) is NonTerminal<T>)
            {
                //get all of the productions whose LHS equals the next element
                var productions = this.Productions.Where(a => a.NonTerminal.Equals(nextElement));

                //for each of the possible following elements of item
                foreach (Terminal<T> l in follow(item, currentItems))
                {
                    foreach (Production<T> p in productions)
                    {
                        //create a new item from the production
                        LRItem<T> newItem = new LRItem<T>(0, p);

                        //with a lookahead element of l from Follow(item)
                        newItem.LookaheadElement = l;

                        //if the item is not already contained
                        if (currentItems.Add(newItem))
                        {
                            //add the new item(but make sure it is not a duplicate
                            //items.Add(newItem);
                            //add the LR1 closure of the new item
                            lr1Closure(newItem, currentItems);
                        }
                    }
                }
            }
            return currentItems.Distinct();
        }

        /// <summary>
        /// Gets a collection of LR(0) items that can be derived from the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="currentItems"></param>
        /// <returns></returns>
        public IEnumerable<LRItem<T>> Closure(LRItem<T> item)
        {
            return closure(item, null);
        }

        /// <summary>
        /// Gets a collection of LR(0) items that can be derived from the given item.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="currentItems"></param>
        /// <returns></returns>
        private IEnumerable<LRItem<T>> closure(LRItem<T> item, IEnumerable<LRItem<T>> currentItems)
        {
            List<LRItem<T>> items;

            items = new List<LRItem<T>>();

            if (currentItems == null)
            {
                items.Add(item);
            }

            //if the next item is a non terminal, get all of the productions of that non terminal
            if (item.GetNextElement() is NonTerminal<T>)
            {
                var productions = this.Productions.Where(a => a.NonTerminal.Equals(item.GetNextElement()));

                //remove duplicate productions
                if (currentItems != null)
                {
                    productions = productions.Where(a => !currentItems.Any(i => i.LeftHandSide.Equals(a.NonTerminal)));
                }


                //add the productions as LRItems
                items.AddRange(productions.Select(a => new LRItem<T>(0, a)));

                //add the closure of each found production
                foreach (Production<T> p in productions)
                {
                    if (p.DerivedElements[0] is NonTerminal<T>)
                    {
                        items.AddRange(closure(new LRItem<T>(0, p), items));
                    }
                }
            }

            return items.Distinct();
        }

        /// <summary>
        /// Gets a collection of LR(0) items that be derived from the given production.
        /// </summary>
        /// <example>
        /// With Grammar:
        /// S -> A
        /// A -> A + B
        /// A -> a
        /// B -> b
        /// 
        /// Closure(0, S):
        /// S -> ⑩A
        /// A -> ⑩A + B, $ (i.e. reduce 'a' to 'A' to 'S')
        /// A -> ⑩a, $ (i.e. reduce 'a' to 'A' then to 'S')
        /// A -> ⑩A + B, + (i.e. shift)
        /// A -> ⑩a, + (i.e. shift)
        /// 
        /// Closure(1, S): I think this is right
        /// S -> A ⑩, $ (i.e. reduce 'A' to 'S')
        /// A -> A ⑩ + B, b (i.e. shift)
        /// A -> a ⑩, $ (i.e. reduce 'a' to 'A')
        /// </example>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<LRItem<T>> Closure(Production<T> production, IEnumerable<LRItem<T>> currentItems = null)
        {
            List<LRItem<T>> items;

            items = new List<LRItem<T>>();


            //if currentItems == null then add the current production with the End of input terminal
            if (currentItems == null)
            {
                items.Add(new LRItem<T>(0, production, EndOfInputElement));
            }

            //if the element in front of the dot is a non-terminal
            if (production.DerivedElements[0] is NonTerminal<T>)
            {
                //find all of the productions of the form A -> ? and where the production is not already in items
                Production<T>[] productions = this.Productions.Where(a => a.NonTerminal.Equals(production.DerivedElements[0])).ToArray();
                if (currentItems != null)
                {
                    productions = productions.Where(a => !currentItems.Any(i => i.LeftHandSide.Equals(a.NonTerminal))).ToArray();
                }


                //add all of the items to the closure
                items.AddRange(productions.Select(a => new LRItem<T>(0, a)).ToArray());

                //find the closure of each production
                foreach (Production<T> p in productions)
                {
                    if (p.DerivedElements[0] is NonTerminal<T>)
                    {
                        //items.Add(new LRItem<T>(0, p));
                        items.AddRange(Closure(p, items));
                    }
                }
                return items;
            }
            else
            {
                return new LRItem<T>[] { };
            }
        }

        /// <summary>
        /// Creates a StateGraph that matches the grammar.
        /// The graph is deterministic if the grammar is deterministic.
        /// </summary>
        /// <returns></returns>
        public StateGraph<GrammarElement<T>, LRItem<T>[]> CreateStateGraph()
        {
            //Get the first state
            IEnumerable<LRItem<T>> firstState = CreateFirstState();

            //create the graph
            StateGraph<GrammarElement<T>, LRItem<T>[]> graph = new StateGraph<GrammarElement<T>, LRItem<T>[]>();
            graph.Root = new StateNode<GrammarElement<T>, LRItem<T>[]>(firstState.ToArray(), graph);

            //create the transitions for the graph
            CreateTransitions(graph);
            return graph;
        }

        /// <summary>
        /// Creates transitions that match the DFA for this grammar from the root of the graph until no more transitions can be made. 
        /// </summary>
        /// <param name="graph"></param>
        public void CreateTransitions(StateGraph<GrammarElement<T>, LRItem<T>[]> graph)
        {
            createTransitions(graph.Root);
        }

        /// <summary>
        /// Creates transitions from the given node to other nodes based on the items contained in the given node.
        /// </summary>
        /// <param name="startingNode"></param>
        private void createTransitions(StateNode<GrammarElement<T>, LRItem<T>[]> startingNode)
        {
            //List<IEnumerable<LRItem<T>>> existingSets = new List<IEnumerable<LRItem<T>>>(startingNode.FromTransitions.Select(a => a.Value.Value));

            var set = startingNode.Value;

            var nextElements = startingNode.Value.Select(a => a.GetNextElement()).Where(a => a != null).DistinctBy(a => a.ToString());

            foreach (GrammarElement<T> next in nextElements)
            {
                List<LRItem<T>> state = new List<LRItem<T>>();
                state.AddRange(set.Where(a =>
                {
                    GrammarElement<T> e = a.GetNextElement();
                    if (e != null)
                    {
                        return e.Equals(next);
                    }
                    return false;
                }).Select(a => a.Copy()));

                state.ForEach(a => a.DotIndex++);

                //add the closure
                LRItem<T>[] closure = state.Select(a => LR1Closure(a)).SelectMany(a => a).DistinctBy(a => a.ToString()).ToArray();

                state.AddRange(closure);

                //get an already existing transition
                StateNode<GrammarElement<T>, LRItem<T>[]> node = startingNode.Graph.Root.GetFromTransition(a => a.Value.SequenceEqual(state));

                if (node == null)
                {
                    if (startingNode.Value.SequenceEqual(state))
                    {
                        node = startingNode;
                        continue;
                    }
                    else
                    {
                        node = new StateNode<GrammarElement<T>, LRItem<T>[]>(state.ToArray(), startingNode.Graph);
                    }
                }
                if (!startingNode.FromTransitions.Any(a => a.Key.Equals(next)))
                {
                    //add the state transition
                    startingNode.AddTransition(next, node);
                    createTransitions(node);
                }

            }
        }

        /// <summary>
        /// Creates the first state.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LRItem<T>> CreateFirstState()
        {
            //get the starting item, S' -> S, $
            LRItem<T> startingItem = new LRItem<T>(0, Productions[0], EndOfInputElement);

            List<LRItem<T>> firstState = new List<LRItem<T>>();

            //add S' -> S, $ to the first state
            firstState.Add(startingItem);

            //get the rest of the first state
            firstState.AddRange(lr1Closure(startingItem, new HashSet<LRItem<T>>()));
            return firstState;
        }

        /// <summary>
        /// Returns the union of the LR(0) closure of each LRItem in the given set.
        /// </summary>
        /// <param name="set"></param>
        /// <returns></returns>
        public IEnumerable<LRItem<T>> Closure(IEnumerable<LRItem<T>> set)
        {
            return set.Select(a => Closure(a)).SelectMany(a => a).Distinct();
        }


        /// <summary>
        /// Gets a collection of Terminal elements that can appear after the element that is after the dot of the item.
        /// </summary>
        /// <example>
        /// With Grammar:
        /// S -> E
        /// E -> T
        /// E -> (E)
        /// T -> n
        /// T -> + T
        /// T ->  T + n
        /// 
        /// Follow(S -> •E) : {$}
        /// Follow(T -> •+ T) : {'+', 'n'}
        /// Follow(T -> •n, ')') : {')'}
        /// </example>
        /// <param name="nonTerminal"></param>
        /// <returns></returns>
        private IEnumerable<Terminal<T>> follow(LRItem<T> item, IEnumerable<LRItem<T>> totalItems)
        {

            //Follow(item) is First(b) where item is:
            //A -> a•Eb

            GrammarElement<T> element = item.GetNextElement(1);

            if (element != null)
            {
                if (element is NonTerminal<T>)
                {
                    List<Terminal<T>> firstSet = new List<Terminal<T>>();

                    //if the element has a production with no derived elements and the element is at the end of the current item's production, then
                    //add the lookahead of the given item.

                    //if there is any production of the current element that has no derived elements
                    if (Productions.Any(a => a.NonTerminal.Equals(element) && a.DerivedElements.Count == 0))
                    {
                        //if the current element is the end of the current item's production
                        if (item.GetNextElement(2) == null)
                        {
                            firstSet.Add(item.LookaheadElement == null ? EndOfInputElement : item.LookaheadElement);
                        }
                    }

                    //select the lookahead element or end of input element for each item in the previous set
                    //List<Terminal<T>> firstSet = new List<Terminal<T>>(items.Select(a => a.LookaheadElement == null ? EndOfInputElement : a.LookaheadElement));

                    //add the rest of the first set.
                    firstSet.AddRange(First(element));
                    return firstSet;
                }
                else
                {
                    return First(element);
                }

            }
            else
            {
                if (item.LookaheadElement == null)
                {
                    return new[] { EndOfInputElement };
                }
                else
                {
                    return new[] { item.LookaheadElement };
                }
            }

        }

        /// <summary>
        /// Gets a collection of Terminal elements that can appear after the element that is after the dot of the item.
        /// </summary>
        /// <example>
        /// With Grammar:
        /// S -> E
        /// E -> T
        /// E -> (E)
        /// T -> n
        /// T -> + T
        /// T ->  T + n
        /// 
        /// Follow(S -> •E) : {$}
        /// Follow(T -> •+ T) : {'+', 'n'}
        /// Follow(T -> •n, ')') : {')'}
        /// </example>
        /// <param name="nonTerminal"></param>
        /// <returns></returns>
        public IEnumerable<Terminal<T>> Follow(LRItem<T> item)
        {

            //Follow(item) is First(b) where item is:
            //A -> a•Eb

            GrammarElement<T> element = item.GetNextElement(1);

            if (element != null)
            {
                //if(item.LookaheadElement != null && item.GetNextElement(2) == null)
                //{
                //    //if it is, then include the lookahead of item in the follow set.
                //    return First(element).Concat(new[] { item.LookaheadElement });
                //}
                ////If the element is not the last element in the production.
                //else
                //{
                return First(element);
                //}
            }
            else
            {
                if (item.LookaheadElement == null)
                {
                    return new[] { EndOfInputElement };
                }
                else
                {
                    return new[] { item.LookaheadElement };
                }
            }

        }

        /// <summary>
        /// Gets a collection of Terminal elements that can appear after the given non-terminal with the given set as context.
        /// </summary>
        /// <param name="itemSet"></param>
        /// <param name="nonTerminal"></param>
        /// <returns></returns>
        public IEnumerable<Terminal<T>> Follow(IEnumerable<LRItem<T>> itemSet, NonTerminal<T> nonTerminal)
        {
            List<Terminal<T>> terms = new List<Terminal<T>>();
            foreach (var i in itemSet)
            {
                if (i.ProductionElements.Contains(nonTerminal))
                {
                    int index = i.ProductionElements.ToList().FindIndex(a => a.Equals(nonTerminal));
                    terms.AddRange(Follow(new LRItem<T>(index, i)));
                }
            }
            //if there are no computed following elements, then $ is the only thing that could follow it.
            if (terms.Count == 0)
            {
                terms.Add(EndOfInputElement);
            }
            return terms.Distinct();
        }

        /// <summary>
        /// Gets the collection of terminal elements that can appear as the first element of the given element.
        /// If element is a Terminal, then First(element) : {element}. Otherwise if element is a NonTerminal,
        /// then First(element) : {set of first terminal elements matching the productions of element}.
        /// </summary>
        /// <example>
        /// With Grammar:
        /// E -> T
        /// E -> ( E )
        /// T -> n
        /// T -> + T
        /// T -> T + n
        /// 
        /// FIRST(E): {'n', '+', '('}
        /// 
        /// FIRST(T): {'n', '+'}
        /// </example>
        /// <param name="element"></param>
        /// <returns></returns>
        public IEnumerable<Terminal<T>> First(GrammarElement<T> element)
        {
            if (element is Terminal<T>)
            {
                return new[] { (Terminal<T>)element };
            }
            else
            {
                return First((NonTerminal<T>)element);
            }
        }

        /// <summary>
        /// Gets the collection of Terminal elements that can appear as the first element
        /// matching to the given production.
        /// </summary>
        /// <example>
        /// With Grammar:
        /// S -> A
        /// A -> A + B
        /// A -> a
        /// B -> b
        /// 
        /// First(A): {"a"}
        /// 
        /// First(B): {"b"}
        /// </example>
        /// <param name="nonTerminal"></param>
        /// <returns></returns>
        public IEnumerable<Terminal<T>> First(NonTerminal<T> nonTerminal)
        {
            //find all of the productions that start with 
            //the given production's non terminal
            //var productions = this.Productions.Where(a => a.NonTerminal.Equals(production.NonTerminal));

            List<Terminal<T>> firstElements = new List<Terminal<T>>();

            //find all of the productions that start with the given nonTerminal
            var productions = this.Productions.Where(a => a.NonTerminal.Equals(nonTerminal));

            foreach (var production in productions)
            {
                if (production.DerivedElements.Count > 0)
                {
                    //add to first elements if the first element of production is a terminal
                    if (production.DerivedElements[0] is Terminal<T>)
                    {
                        firstElements.Add((Terminal<T>)production.DerivedElements[0]);
                    }
                    //otherwise for all of the productions whose LHS == first element
                    else
                    {
                        //make sure that we are not repeating ourselves
                        if (!production.DerivedElements[0].Equals(nonTerminal))
                        {
                            //foreach (Production<T> p in this.Productions.Where(a => a.NonTerminal.Equals(production.DerivedElements[0])))
                            //{
                            //    //add First(p)
                            //    firstElements.AddRange(First(p));
                            //}
                            firstElements.AddRange(First((NonTerminal<T>)production.DerivedElements[0]));
                        }
                    }
                }
                //else
                //{
                //    firstElements.Add(EndOfInputElement);
                //}
            }
            //return a distinct set of terminals
            return firstElements.Distinct();
        }

        /// <summary>
        /// Gets the collection of terminal elements that can appear as the first element
        /// after the dot in the given LR Item. If next element, named T, is a terminal, then First(item) = {T}
        /// </summary>
        /// <example>
        /// With Grammar:
        /// S -> A
        /// A -> A + B
        /// A -> a
        /// B -> b
        /// 
        /// First(S -> •A): {'a'}
        /// 
        /// First(A -> •A + B): {'a'}
        /// 
        /// First(A -> A • + B): {'+'}
        /// 
        /// First(B -> b): {'b'}
        /// </example>
        /// <param name="item"></param>
        /// <returns></returns>
        public IEnumerable<Terminal<T>> First(LRItem<T> item)
        {
            //get the next element and evaluate if it is a terminal or non terminal
            GrammarElement<T> nextElement = item.GetNextElement();

            //if nextElement is a Terminal then return {T}
            if (nextElement is Terminal<T>)
            {
                return new[] { (Terminal<T>)nextElement };
            }
            //otherwise find all of the productions that have nextElement on the LHS.
            else
            {
                var production = this.Productions.Where(p => p.NonTerminal == item.LeftHandSide && p.DerivedElements.SequenceEqual(item.ProductionElements)).First();

                List<Terminal<T>> firstElements = new List<Terminal<T>>();

                if (production.DerivedElements.Count > 0)
                {
                    if (!production.DerivedElements[item.DotIndex].Equals(production.NonTerminal))
                    {
                        GrammarElement<T> productionItem = production.GetElement(item.DotIndex);
                        if (productionItem != null)
                        {
                            //if it is a Terminal add to first elements
                            if (productionItem is Terminal<T>)
                            {
                                firstElements.Add((Terminal<T>)productionItem);
                            }
                            //otherwise add First(new LRItem(production)) of all of the productions where the LHS == productionItem
                            else
                            {
                                foreach (LRItem<T> i in Productions.Where(p => p.NonTerminal.Equals(productionItem)).Select(p => new LRItem<T>(0, p)))
                                {
                                    firstElements.AddRange(First(i));
                                }
                            }
                        }
                    }
                }
                return firstElements.Distinct();
            }
        }

        /// <summary>
        /// Creates a new CFG given the starting non-terminal, and productions.
        /// </summary>
        /// <param name="startingElement"></param>
        /// <param name="productions"></param>
        public ContextFreeGrammar(NonTerminal<T> startingElement, IEnumerable<Production<T>> productions)
        {
            //add the production S -> startingElement
            this.startElement = new NonTerminal<T>("S'" + (new Random()).Next(int.MaxValue).ToString("X"));
            this.Productions = new List<Production<T>>();
            this.Productions.Add(new Production<T>(startElement, startingElement));

            this.Productions.AddRange(productions);

            this.EndOfInputElement = new Terminal<T>(default(T), false);
        }

        /// <summary>
        /// Creates a new CFG given the starting non-terminal, end of input terminal, and productions.
        /// </summary>
        /// <param name="startingElement"></param>
        /// <param name="endOfInput"></param>
        /// <param name="productions"></param>
        public ContextFreeGrammar(NonTerminal<T> startingElement, Terminal<T> endOfInput, IEnumerable<Production<T>> productions)
        {
            this.startElement = new NonTerminal<T>("S'" + (new Random()).Next(int.MaxValue).ToString("X"));
            this.Productions = new List<Production<T>>();
            this.Productions.Add(new Production<T>(startElement, startingElement));
            this.Productions.AddRange(productions);
            this.EndOfInputElement = endOfInput;
        }

        /// <summary>
        /// Returns a nicely formatted string representation of this CFG.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            foreach (Production<T> p in Productions)
            {
                builder.Append(p);
                builder.AppendLine();
            }
            return builder.ToString();
        }
    }
}
