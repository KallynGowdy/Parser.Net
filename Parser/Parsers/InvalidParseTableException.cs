using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Parser.Grammar;
using Parser.StateMachine;

namespace Parser.Parsers
{
    /// <summary>
    /// Defines a set of values that determine the possible errors(shift-reduce/reduce-reduce) that can occur with a parse table.
    /// </summary>
    [Flags]
    public enum ParseTableExceptionType
    {
        SHIFT_REDUCE = 1,
        REDUCE_REDUCE = 2,
        FIRST_SET = 3
    };

    /// <summary>
    /// Defines an exception that is thrown when given a parse table cannot be used with a certian parser.
    /// </summary>
    [Serializable]
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public class InvalidParseTableException<T> : Exception where T : IEquatable<T>
    {
        private ParseTableConflict<T>[] conflicts;

        /// <summary>
        /// Gets the conflicts contained in the parse table.
        /// </summary>
        public ReadOnlyCollection<ParseTableConflict<T>> Conflicts
        {
            get
            {
                return new ReadOnlyCollection<ParseTableConflict<T>>(conflicts);
            }
        }

        /// <summary>
        /// Gets the invalid parse table that was provided.
        /// </summary>
        public IParseTable<T> ParseTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the state that the exception occurs at.
        /// </summary>
        public int State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the input/column that the exception occurs at.
        /// </summary>
        public GrammarElement<T> NextInput
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the State node that the conflict occured at.
        /// </summary>
        public StateNode<GrammarElement<T>, LRItem<T>[]> InvalidNode
        {
            get;
            private set;
        }

        public InvalidParseTableException(string message)
            : base(message)
        {
        }

        public InvalidParseTableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidParseTableException(string message, IParseTable<T> invalidTable)
            : base(message)
        {
            this.ParseTable = invalidTable;
        }

        public InvalidParseTableException(ParseTableExceptionType exceptions, IParseTable<T> invalidTable, int state = -1, GrammarElement<T> nextInput = null)
            : base(createExceptionMsg(exceptions))
        {
            this.ParseTable = invalidTable;
            this.State = state;
            this.NextInput = nextInput;
        }

        public InvalidParseTableException(IParseTable<T> invalidTable, params ParseTableConflict<T>[] conflicts)
            : base(createExceptionMsg(conflicts))
        {
            this.conflicts = conflicts;
            this.ParseTable = invalidTable;
        }

        public InvalidParseTableException(StateMachine.ParseTable<T> invalidTable,  StateMachine.StateNode<GrammarElement<T>, LRItem<T>[]> node, params ParseTableConflict<T>[] conflicts)
        {
            this.conflicts = conflicts;
            this.ParseTable = invalidTable;
            this.InvalidNode = node;
        }

        private static string createExceptionMsg(ParseTableConflict<T>[] conflicts)
        {
            StringBuilder b = new StringBuilder();

            b.AppendFormat("There {0} {2} conflict{1} contained in the parse table:", (conflicts.Length > 1 ? "are" : "is"), (conflicts.Length > 1 ? "s" : string.Empty), conflicts.Length);
            b.AppendLine();

            for(int i = 0; i < conflicts.Length; i++)
            {
                var conflict = conflicts[i];

                b.AppendFormat("{0}). {1} conflict at (State: {2}, Column {3})", i + 1, conflict.ConflictType, conflict.TableLocation.Row, conflict.TableLocation.Column);
            }

            return b.ToString();
        }

        static string createExceptionMsg(ParseTableExceptionType exceptions)
        {
            StringBuilder b = new StringBuilder();

            b.Append("The parse table contains the following conflicts: {");

            //shift-reduce conflict
            if ((exceptions & ParseTableExceptionType.SHIFT_REDUCE) == ParseTableExceptionType.SHIFT_REDUCE)
            {
                b.Append("SHIFT_REDUCE,");
            }
            //reduce-reduce conflict
            if ((exceptions & ParseTableExceptionType.REDUCE_REDUCE) == ParseTableExceptionType.REDUCE_REDUCE)
            {
                b.Append(" REDUCE_REDUCE,");
            }
            //first-set clash
            if ((exceptions & ParseTableExceptionType.FIRST_SET) == ParseTableExceptionType.FIRST_SET)
            {
                b.Append(" FIRST_SET_CLASH");
            }

            string s = b.ToString();

            //remove possible trailing ','
            if (s.Last() == ',')
            {
                s = s.Remove(s.Length - 1);
            }

            s += " }";

            return s;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("conflicts", conflicts);
            info.AddValue("ParseTable", ParseTable);
            info.AddValue("State", State);
            info.AddValue("NextInput", NextInput);
            base.GetObjectData(info, context);
        }
    }
}
