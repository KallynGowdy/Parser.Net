using System;
using System.Collections.ObjectModel;
using System.Linq;
using KallynGowdy.ParserGenerator.Collections;
using KallynGowdy.ParserGenerator.Grammar;
using KallynGowdy.ParserGenerator.StateMachine;

namespace KallynGowdy.ParserGenerator.Parsers
{
    /// <summary>
    /// Defines a conflict for a LR parser as either a ShiftReduce or ReduceReduce conflict.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ParseTableConflict<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the location of the conflict in the parse table.
        /// </summary>
        public ColumnRowPair<int, Terminal<T>> TableLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the StateNode where the conflict takes place in the graph.
        /// </summary>
        public StateNode<GrammarElement<T>, LRItem<T>[]> GraphLocation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the conflict(either Shift_Reduce or Reduce_Reduce)
        /// </summary>
        public ParseTableExceptionType ConflictType
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets the list of conflicting LRItems that caused the conflict.
        /// </summary>
        public ReadOnlyCollection<LRItem<T>> ConflictingItems
        {
            get;
            private set;
        }

        public ParseTableConflict(ParseTableExceptionType conflictType, ColumnRowPair<int, Terminal<T>> tableLocation, StateNode<GrammarElement<T>, LRItem<T>[]> graphLocation, params LRItem<T>[] conflictingItems)
        {
            this.TableLocation = tableLocation;
            this.GraphLocation = graphLocation;
            this.ConflictType = conflictType;
            this.ConflictingItems = new ReadOnlyCollection<LRItem<T>>(conflictingItems.ToList());
        }
    }
}
