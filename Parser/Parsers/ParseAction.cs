using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Provides an abstract class that defines an action that the parser should take when input is read based on the current state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public abstract class ParserAction<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the parse table that the Action uses to determine what to perform.
        /// </summary>
        [DataMember]
        public ParseTable<T> ParseTable
        {
            get;
            private set;
        }

        protected ParserAction(ParseTable<T> table)
        {
            this.ParseTable = table;
        }
    }

    /// <summary>
    /// Provides a class that defines that a "shift" should occur at the location that the action is stored in the table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public sealed class ShiftAction<T> : ParserAction<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the next state that the parser should move to.
        /// </summary>
        [DataMember]
        public int NextState
        {
            get;
            private set;
        }

        public ShiftAction(ParseTable<T> table, int nextState)
            : base(table)
        {
            if (nextState < 0)
            {
                throw new ArgumentOutOfRangeException("nextState", "Must be greater than or equal to 0");
            }
            this.NextState = nextState;
        }

        public override string ToString()
        {
            return string.Format("Shift({0})", NextState);
        }
    }

    /// <summary>
    /// Provides a class that defines that a "reduce" should occur at the location that the action is stored in the table.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract]
    public sealed class ReduceAction<T> : ParserAction<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the item that defines the reduction to perform.
        /// </summary>
        [DataMember]
        public LRItem<T> ReduceItem
        {
            get;
            private set;
        }

        public ReduceAction(ParseTable<T> table, LRItem<T> reduceItem)
            : base(table)
        {
            if (reduceItem != null)
            {
                this.ReduceItem = reduceItem;
            }
            else
            {
                throw new ArgumentNullException("reduceItem", "Must be non-null");
            }
        }

        public override string ToString()
        {
            return string.Format("Reduce({0})", ReduceItem.ToString());
        }
    }

    /// <summary>
    /// Provides a class that defines that the parse should be accepted(and therefore successful).
    /// </summary>
    [DataContract]
    public sealed class AcceptAction<T> : ParserAction<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Gets the item that defines that the parse should be accepted.
        /// </summary>
        [DataMember]
        public LRItem<T> AcceptItem
        {
            get;
            private set;
        }

        public AcceptAction(ParseTable<T> table, LRItem<T> acceptItem)
            : base(table)
        {
            if (acceptItem != null)
            {
                this.AcceptItem = acceptItem;
            }
            else
            {
                throw new ArgumentNullException("acceptItem", "Must be non-null");
            }
        }

        public override string ToString()
        {
            return "Accept";
        }
    }
}
